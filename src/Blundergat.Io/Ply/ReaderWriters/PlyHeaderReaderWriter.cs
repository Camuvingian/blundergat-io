using Blundergat.Common.Helpers;
using Blundergat.Common.Types;
using Blundergat.Io.Ply.Constants;
using Blundergat.Io.Ply.Helpers;
using Blundergat.Io.Ply.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public sealed class PlyHeaderReaderWriter : IPlyHeaderReaderWriter
	{
		public PlyFile ReadFileStructure(string filePath)
		{
			var plyFile = new PlyFile(filePath);
			var header = new PlyHeader(File.ReadLines(filePath).TakeUntilIncluding(x => x == PlyKeywords.EndHeader).ToList());
			plyFile.Header = header;

			foreach (var line in header.HeaderLines)
			{
				var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				switch (tokens[0])
				{
					case PlyKeywords.Format:
					{
						if (tokens.Length != 3)
							throw new Exception("Invalid header - format is invalid");

						plyFile.Header.PlyFileFormat = (tokens[1]) switch
						{
							PlyKeywords.Ascii => PlyFileFormat.Ascii,
							PlyKeywords.BinaryLittleEndian => PlyFileFormat.BinaryLittleEndian,
							PlyKeywords.BinaryBigEndian => PlyFileFormat.BinaryBigEndian,
							_ => throw new Exception("Invalid header - format is invalid"),
						};
						plyFile.Header.Version = Convert.ToSingle(tokens[2]);
						break;
					}
					case PlyKeywords.Element:
					{
						AddElement(plyFile, tokens);
						break;
					}
					case PlyKeywords.Property:
					{
						AddProperty(plyFile, tokens);
						break;
					}
					case PlyKeywords.Comment:
					{
						AddComment(plyFile, line);
						break;
					}
					case PlyKeywords.ObjectInfo:
					{
						AddObjectInfo(plyFile, line);
						break;
					}
					case PlyKeywords.EndHeader:
					{
						return plyFile;
					}
				}
			}
			return plyFile;
		}

		public void WriteFileStructure(PlyFile plyFile)
		{
			var headerLines = new List<string>
			{
				$"{PlyKeywords.Ply}",
				$"{PlyKeywords.Format} {TypeMapper.FormatMapping[plyFile.Header.PlyFileFormat]} {plyFile.Header.Version:F1}"
			};

			// Comments.
			//foreach (var comment in plyFile.Comments)
			//{
			//	headerLines.Add($"{PlyKeywords.Comment} {comment}");
			//}

			// Elements.
			foreach (var element in plyFile.Elements)
			{
				headerLines.Add(element.ToString());
				foreach (var property in element.Properties)
				{
					headerLines.Add(property.Value.ToString());
				}
			}
			headerLines.Add(PlyKeywords.EndHeader);

			using var fileStream = File.OpenWrite(plyFile.FilePath);
			using var streamWriter = new StreamWriter(fileStream);

			plyFile.Header.HeaderLines = headerLines;
			streamWriter.Write(plyFile.Header.RawHeader);			
		}

		private void AddElement(PlyFile plyFile, string[] tokens)
		{
			plyFile.Elements.Add(new PlyElement(tokens[1], Convert.ToInt32(tokens[2])));
		}

		private void AddProperty(PlyFile plyFile, string[] tokens)
		{
			PlyProperty property;
			if (tokens[1] == PlyKeywords.List)
			{
				var name = tokens[4];
				var element = plyFile.Elements.Last();
				property = new PlyProperty(name, element.Properties.Count, TypeMapper.GetPropertyType(tokens[3]), TypeMapper.GetPropertyType(tokens[2]));
				element.Properties.Add(tokens[4], property);
			}
			else
			{
				var element = plyFile.Elements.Last();
				property = new PlyProperty(tokens[2], element.Properties.Count, TypeMapper.GetPropertyType(tokens[1]));
				element.Properties.Add(tokens[2], property);
			}
		}

		private void AddComment(PlyFile plyFile, string line)
		{
			// Skip over 'comment' and leading spaces and tabs.
			int i = 7;
			while (line[i] == ' ' || line[i] == '\t')
				i++;

			plyFile.Comments.Add(line.Substring(i));
		}

		private void AddObjectInfo(PlyFile plyFile, string line)
		{
			// Skip over 'obj_info' and leading spaces and tabs.
			int i = 8;
			while (line[i] == ' ' || line[i] == '\t')
				i++;

			plyFile.ObjectInformationItems.Add(line.Substring(i));
		}
	}
}