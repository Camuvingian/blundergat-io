using Blundergat.Common.Model.Io;
using Blundergat.Common.Types;
using Blundergat.Io.Ply.Constants;
using Blundergat.Io.Ply.Helpers;
using Blundergat.Io.Ply.Models;
using Blundergat.Io.Ply.ReaderWriters;
using Blundergat.Io.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace Blundergat.Io.Ply
{
	[AssetImporterExporter(".ply", "Polygon File Format")]
	public class PlyAssetImporterExporter : AssetImporterExporterBase, IPlyAssetImporterExporter
	{
		private readonly IIoSettings _settings;
		private readonly ILogger<PlyAssetImporterExporter> _logger;

		public PlyAssetImporterExporter(
			IIoSettings settings, 
			ILogger<PlyAssetImporterExporter> logger) : base(settings, logger)
		{
			_settings = settings;
			_logger = logger;
		}

		#region IAssetImporterExporter Implementation.
		public override Task<Scene> ImportFileImpl(string filePath)
		{
			return Task.Run(() =>
			{
				var mesh = new Mesh();
				var scene = new Scene();
				scene.Meshes.Add(mesh);

				var headerParser = new PlyHeaderReaderWriter();
				PlyFile plyFile = headerParser.ReadFileStructure(filePath);

				if (plyFile.Header.PlyFileFormat == PlyFileFormat.BinaryBigEndian)
					throw new Exception($"{nameof(PlyFileFormat.BinaryBigEndian)} format is not currently supported");

				using var elementReadWriter = PlyReaderWriterFactory.GetReaderWriter(plyFile, OperationType.Import);

				BuildElements(elementReadWriter, plyFile);
				BuildMeshStructure(mesh, plyFile);

				return scene;
			});
		}

		protected override Task ExportFileImpl(Scene scene, string filePath)
		{
			if (scene.Meshes == null || scene.Meshes.Count != 1)
				throw new ArgumentException("Can only deal with scenes containing a single mesh");

			return Task.Run(() =>
			{
				var mesh = scene.Meshes[0];
				var plyFile = ConstructPlyFile(mesh, filePath);

				var headerParser = new PlyHeaderReaderWriter();
				headerParser.WriteFileStructure(plyFile);

				using (var elementReadWriter = PlyReaderWriterFactory.GetReaderWriter(plyFile, OperationType.Export))
				{
					elementReadWriter.WriteElements(mesh);
				}

				File.SetCreationTime(filePath, DateTime.Now);
			});
		}

		public override string Extension => ".ply";
		#endregion // IAssetImporterExporter Implementation.

		#region Import Methods.
		private void BuildElements(IPlyReaderWriter elementReadWriter, PlyFile plyFile)
		{
			foreach (var element in plyFile.Elements)
			{
				for (int i = 0; i < element.ObjectCount; i++)
				{
					var tokens = elementReadWriter.ReadElementTokens(element);
					if (tokens == null)
						throw new Exception("Unexpected end of file");

					int whichToken = 0;
					var elementValue = new PlyElementValue();

					foreach (var property in element.Properties)
					{
						int intValue;
						uint uintValue;
						double doubleValue;
						if (property.Value.IsList)
						{
							TypeMapper.GetPropertyValues(tokens[whichToken++], 
								property.Value.Type, out intValue, out uintValue, out doubleValue);
							
							var listCount = intValue;
							for (int j = 0; j < listCount; j++)
							{
								var propertyValue = TypeMapper.GetPropertyValues(tokens[whichToken++],
									property.Value.Type, out intValue, out uintValue, out doubleValue);
								elementValue.PropertyValues.Add(propertyValue);
							}
						}
						else
						{
							var propertyValue = TypeMapper.GetPropertyValues(tokens[whichToken++],
								property.Value.Type, out intValue, out uintValue, out doubleValue);
							elementValue.PropertyValues.Add(propertyValue);
						}
					}
					element.ElementValues.Add(elementValue);
				}
			}
		}

		private void BuildMeshStructure(Mesh mesh, PlyFile plyFile)
		{
			foreach (var element in plyFile.Elements)
			{
				var indexes = PropertyIndexRetreiver.GetPropertyIndexes(element.Properties);

				switch (element.Name)
				{
					case PlyKeywords.Vertex:
					{
						foreach (var elementValue in element.ElementValues)
						{
							if (indexes.X != -1 && indexes.Y != -1 && indexes.Z != -1)
							{
								mesh.Points.Add(new Vector3(
									Convert.ToSingle(elementValue.PropertyValues[indexes.X]),
									Convert.ToSingle(elementValue.PropertyValues[indexes.Y]),
									Convert.ToSingle(elementValue.PropertyValues[indexes.Z])));
							}

							if (indexes.Nx != -1 && indexes.Ny != -1 && indexes.Nz != -1)
							{
								mesh.Normals.Add(new Vector3(
									Convert.ToSingle(elementValue.PropertyValues[indexes.Nx]),
									Convert.ToSingle(elementValue.PropertyValues[indexes.Ny]),
									Convert.ToSingle(elementValue.PropertyValues[indexes.Nz])));
							}

							if (indexes.Red != -1 && indexes.Green != -1 && indexes.Blue != -1)
							{
								mesh.Colors.Add(new Color(
									Convert.ToByte(elementValue.PropertyValues[indexes.Red]),
									Convert.ToByte(elementValue.PropertyValues[indexes.Green]),
									Convert.ToByte(elementValue.PropertyValues[indexes.Blue]),
									indexes.Alpha != -1 ? 
										Convert.ToByte(elementValue.PropertyValues[indexes.Alpha]) : 
										byte.MaxValue));
							}

							if (indexes.S != -1 && indexes.T != -1)
							{
								mesh.TextureCoordinates.Add(new Vector3(
									Convert.ToSingle(elementValue.PropertyValues[indexes.S]),
									Convert.ToSingle(elementValue.PropertyValues[indexes.T]),
									0.0f));
							}
						}
						break;
					}
					case PlyKeywords.Face:
					{
						foreach (var elementValue in element.ElementValues)
						{
							if (elementValue.PropertyValues.Count != 3)
								throw new Exception("Only triangle faces are currently supported");

							mesh.Indices.Add(Convert.ToInt32(elementValue.PropertyValues[0]));
							mesh.Indices.Add(Convert.ToInt32(elementValue.PropertyValues[1]));
							mesh.Indices.Add(Convert.ToInt32(elementValue.PropertyValues[2]));
						}
						break;
					}
					case PlyKeywords.Edge:
					{
						foreach (var elementValue in element.ElementValues)
						{
							if (indexes.Vertex1 != -1 && indexes.Vertex2 != -1 && 
								indexes.Red != -1 && indexes.Green != -1 && indexes.Blue != -1)
							{
								mesh.Edges.Add(new Edge(
									Convert.ToInt32(elementValue.PropertyValues[indexes.Vertex1]),
									Convert.ToInt32(elementValue.PropertyValues[indexes.Vertex2]),
									new Color(
										Convert.ToByte(elementValue.PropertyValues[indexes.Red]),
										Convert.ToByte(elementValue.PropertyValues[indexes.Green]),
										Convert.ToByte(elementValue.PropertyValues[indexes.Blue]),
										byte.MaxValue))
								);
							}
						}
						break;
					}
				}
			}
		}
		#endregion // Import Methods.

		#region Build PlyFile Object.
		public PlyFile ConstructPlyFile(Mesh mesh, string filePath)
		{
			var plyFile = new PlyFile(filePath);

			plyFile.Header.PlyFileFormat = _settings != null ? 
				_settings.DefaultPlyFileFormat : 
				PlyFileFormat.BinaryLittleEndian;

			AddComments(plyFile, mesh);
			AddVertexElement(plyFile, mesh);
			AddEdgeElement(plyFile, mesh);

			return plyFile;
		}

		private void AddComments(PlyFile plyFile, Mesh mesh)
		{
			plyFile.Comments = GetBanner(mesh);
		}

		private void AddVertexElement(PlyFile plyFile, Mesh mesh)
		{
			int index = 0;
			var vertexElements = new PlyElement(PlyKeywords.Vertex, mesh.Points.Count);

			foreach (var name in new List<string>() { PlyKeywords.X, PlyKeywords.Y, PlyKeywords.Z })
				AddVertexProperty(vertexElements, name, index++);

			if (mesh.ContainsNormals)
			{
				foreach (var name in new List<string>() { PlyKeywords.NormalX, PlyKeywords.NormalY, PlyKeywords.NormalZ })
					AddVertexProperty(vertexElements, name, index++);
			}

			if (mesh.ContainsColors)
			{
				foreach (var name in new List<string>() { PlyKeywords.Red, PlyKeywords.Green, PlyKeywords.Blue, PlyKeywords.Alpha })
					AddColorProperty(vertexElements, name, index++);
			}

			plyFile.Elements.Add(vertexElements);
		}

		private void AddVertexProperty(PlyElement element, string name, int index)
		{
			element.Properties.Add(name, new PlyProperty(name, index, PlyPropertyType.Float));
		}

		private void AddColorProperty(PlyElement element, string name, int index)
		{
			element.Properties.Add(name, new PlyProperty(name, index, PlyPropertyType.UChar));
		}

		private void AddEdgeElement(PlyFile plyFile, Mesh mesh)
		{
			if (mesh.ContainsEdges)
			{
				int index = 0;
				var edgeElements = new PlyElement(PlyKeywords.Edge, mesh.Edges.Count);

				AddEdgeProperty(edgeElements, PlyKeywords.Vertex1, index++);
				AddEdgeProperty(edgeElements, PlyKeywords.Vertex2, index++);
				AddColorProperties(edgeElements, index++);

				plyFile.Elements.Add(edgeElements);
			}
		}

		private void AddEdgeProperty(PlyElement edgeElement, string name, int index)
		{
			edgeElement.Properties.Add(name, new PlyProperty(name, index, PlyPropertyType.Int));
		}

		private void AddColorProperties(PlyElement edgeElement, int index)
		{
			foreach (var color in new[] { PlyKeywords.Red, PlyKeywords.Green, PlyKeywords.Blue })
			{
				edgeElement.Properties.Add(color, new PlyProperty(color, index, PlyPropertyType.UChar));
			}
		}
		#endregion // Build PlyFile Object.
	}
}