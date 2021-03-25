using Blundergat.Common.Model.Io;
using Blundergat.Io.Ply.Constants;
using Blundergat.Io.Ply.Models;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public sealed class AsciiReaderWriter : IPlyReaderWriter
	{
		private FileStream _fileStream;
		private StreamReader _streamReader;
		private StreamWriter _streamWriter; 

		public AsciiReaderWriter(PlyFile plyFile, OperationType operationType) 
		{
			switch (operationType)
			{
				case OperationType.Import:
					InitializeRead(plyFile);
					break;
				case OperationType.Export:
					InitializeWrite(plyFile);
					break;
			}
		}

		private void InitializeRead(PlyFile plyFile)
		{
			_streamReader = new StreamReader(plyFile.FilePath);
			while (!_streamReader.ReadLine().Contains(PlyKeywords.EndHeader))
			{
			}
		}

		private void InitializeWrite(PlyFile plyFile)
		{
			_fileStream = File.OpenWrite(plyFile.FilePath);
			_streamWriter = new StreamWriter(_fileStream);

			var headerAsText = plyFile.Header.HeaderLines.Aggregate((a, b) => $"{a}\n{b}") + "\n";
			var headerAsBytes = Encoding.ASCII.GetBytes(headerAsText);

			_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
		}

		public string[] ReadElementTokens(PlyElement element)
		{
			if (_streamReader.EndOfStream)
				return null;

			var line = _streamReader.ReadLine();
			if (line == null)
				return null;

			return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public void WriteElements(Mesh mesh)
		{
			if (mesh.ContainsNormals && mesh.ContainsColors)
			{
				for (int i = 0; i < mesh.Points.Count; ++i)
				{
					WriteElement(mesh.Points[i], mesh.Normals[i], mesh.Colors[i]);
				}
			}
			else if (mesh.ContainsNormals)
			{
				for (int i = 0; i < mesh.Points.Count; ++i)
				{
					WriteElement(mesh.Points[i], mesh.Normals[i]);
				}
			}
			else
			{
				for (int i = 0; i < mesh.Points.Count; ++i)
				{
					WriteElement(mesh.Points[i]);
				}
			}

			if (mesh.ContainsEdges)
			{
				for (int i = 0; i < mesh.Edges.Count; ++i)
				{
					WriteElement(mesh.Edges[i]);
				}
			}
		}

		private void WriteElement(Vector3 point, Vector3 normal, Color color)
		{
			_streamWriter.WriteLine(
				$"{point.X} {point.Y} {point.Z} " +
				$"{normal.X} {normal.Y} {normal.Z} " +
				$"{color.Red} {color.Green} {color.Blue} {color.Alpha}");
		}

		private void WriteElement(Vector3 point, Vector3 normal)
		{
			_streamWriter.WriteLine($"{point.X} {point.Y} {point.Z} {normal.X} {normal.Y} {normal.Z}");
		}

		private void WriteElement(Vector3 point)
		{
			_streamWriter.WriteLine($"{point.X} {point.Y} {point.Z}");
		}

		private void WriteElement(Edge edge)
		{
			_streamWriter.WriteLine($"{edge.StartIndex} {edge.EndIndex} {edge.Color.Red} {edge.Color.Green} {edge.Color.Blue}");
		}

		#region IDisposable Implementation.
		private bool disposedValue;

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_streamReader?.Dispose();
					_streamWriter?.Dispose();
					_fileStream?.Dispose();
				}
				disposedValue = true;
			}
		}
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion // IDisposable Implementation.
	}
}
