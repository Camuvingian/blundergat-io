using Blundergat.Common.Model.Io;
using Blundergat.Io.Ply.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public class BinaryLittleEndianReaderWriter : IPlyReaderWriter
	{
		private FileStream _fileStream;
		private BinaryReader _binaryReader;
		private BinaryWriter _binaryWriter;

		public BinaryLittleEndianReaderWriter(PlyFile plyFile, OperationType operationType)
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
			var headerAsText = plyFile.Header.HeaderLines.Aggregate((a, b) => $"{a}\n{b}") + "\n";

			_fileStream = new FileStream(plyFile.FilePath, FileMode.Open);
			_binaryReader = new BinaryReader(_fileStream);

			_binaryReader.BaseStream.Seek(headerAsText.Length, SeekOrigin.Begin);
		}

		private void InitializeWrite(PlyFile plyFile)
		{
			_fileStream = new FileStream(plyFile.FilePath, FileMode.OpenOrCreate);
			_binaryWriter = new BinaryWriter(_fileStream);

			_binaryWriter.BaseStream.Seek(0, SeekOrigin.End);
		}

		public string[] ReadElementTokens(PlyElement element)
		{
			var tokens = new List<string>();
			foreach (var property in element.Properties)
			{
				var bytes = _binaryReader.ReadBytes(property.Value.Type.ByteCount);
				var o = property.Value.Type.ByteConverter(bytes, 0);
				tokens.Add(o.ToString());
			}
			return tokens.ToArray();
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
			else if (mesh.ContainsColors)
			{
				for (int i = 0; i < mesh.Points.Count; ++i)
				{
					WriteElement(mesh.Points[i], mesh.Colors[i]);
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
			WriteElement(point, normal);

			_binaryWriter.Write(color.Red);
			_binaryWriter.Write(color.Green);
			_binaryWriter.Write(color.Blue);
			_binaryWriter.Write(color.Alpha);
		}

		private void WriteElement(Vector3 point, Vector3 normal)
		{
			WriteElement(point);

			_binaryWriter.Write(BitConverter.GetBytes(normal.X));
			_binaryWriter.Write(BitConverter.GetBytes(normal.Y));
			_binaryWriter.Write(BitConverter.GetBytes(normal.Z));
		}

		private void WriteElement(Vector3 point, Color color)
		{
			WriteElement(point);

			_binaryWriter.Write(color.Red);
			_binaryWriter.Write(color.Green);
			_binaryWriter.Write(color.Blue);
			_binaryWriter.Write(color.Alpha);
		}

		private void WriteElement(Vector3 point)
		{
			_binaryWriter.Write(BitConverter.GetBytes(point.X));
			_binaryWriter.Write(BitConverter.GetBytes(point.Y));
			_binaryWriter.Write(BitConverter.GetBytes(point.Z));
		}

		private void WriteElement(Edge edge)
		{
			_binaryWriter.Write(BitConverter.GetBytes(edge.StartIndex));
			_binaryWriter.Write(BitConverter.GetBytes(edge.EndIndex));

			_binaryWriter.Write(edge.Color.Red);
			_binaryWriter.Write(edge.Color.Green);
			_binaryWriter.Write(edge.Color.Blue);
		}

		#region IDisposable Implementation.
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_binaryReader?.Dispose();
					_binaryWriter?.Dispose();
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
