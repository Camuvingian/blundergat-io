using Blundergat.Common.Helpers;
using Blundergat.Common.Model;
using Blundergat.Common.Model.Io;
using Blundergat.Common.Settings.Impl;
using Blundergat.Common.Settings.Interfaces;
using Blundergat.Common.Types;
using Blundergat.Common.Adapters;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Blundergat.Io.Test.Helpers;

namespace Blundergat.Io.Ply.Test
{
	[TestFixture]
	public class AssetImporterExporterIntegrationTests
	{
		private AssetImporterExporterBase _importerExporter;
		private string _basicPlyFilePath = String.Empty;

		private Mock<ISettingsProvider> _mockSettingsProvider;

		[SetUp]
		public void Setup()
		{
			_mockSettingsProvider = new Mock<ISettingsProvider>();
			_mockSettingsProvider.Setup(m => m.DataSettings).Returns(new DataSettings());
			_mockSettingsProvider.Object.DataSettings.DefaultPlyFileFormat.Value = Common.Types.PlyFileFormat.BinaryLittleEndian;

			_importerExporter = new PlyAssetImporterExporter(_mockSettingsProvider.Object, null);

			var assembly = Assembly.GetExecutingAssembly();
			_basicPlyFilePath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Resources", "basic.ply");
		}

		[Test]
		public async Task ReadInSmallPlyEnsureStructureIsCorrect()
		{
			Scene scene = await _importerExporter.ImportFileAsync(_basicPlyFilePath);
			var mesh = scene.Meshes[0];

			Assert.AreEqual(8, mesh.Points.Count);
			Assert.AreEqual(false, mesh.ContainsNormals);
		}

		[Test]
		public async Task ReadWriteAndEnsureStructureIsCorrect()
		{
			Scene correctScene = await _importerExporter.ImportFileAsync(_basicPlyFilePath);
			string tmpPath = Path.Combine(Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "tmp.ply"));

			try
			{
				await _importerExporter.ExportFileAsync(correctScene, tmpPath, true);

				Scene tmpScene = await _importerExporter.ImportFileAsync(tmpPath);
				Assert.AreEqual(correctScene.Meshes.Count, tmpScene.Meshes.Count);

				Mesh correctMesh = correctScene.Meshes[0];
				Mesh tmpMesh = tmpScene.Meshes[0];
				Assert.AreEqual(correctMesh.Points.Count, tmpMesh.Points.Count);
			}
			finally
			{
				File.Delete(tmpPath);
			}
		}

		[Test]
		public async Task CanExportEdges()
		{
			var vertices = new List<Vector3>
			{
				new Vector3(0, 0, 0),
				new Vector3(1, 1, 1)
			};

			var pointCloud = new PointCloud(vertices.Select(p => new Vertex(p)).ToArray());
			var scene = PointCloudToSceneAdapter.GenerateSceneFromPointCloud(pointCloud);

			var mesh = scene.Meshes.First();
			mesh.Edges.Add(new Edge(0, 1, System.Drawing.Color.Red.ToCustomColor()));

			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, "tmp.ply");

			try
			{
				await _importerExporter.ExportFileAsync(scene, filePath, true);

				var importedScene = await _importerExporter.ImportFileAsync(filePath);
				var importedMesh = importedScene.Meshes.First();

				Assert.AreEqual(mesh.Edges.Count, importedMesh.Edges.Count);
				Assert.AreEqual(mesh.Edges[0].StartIndex, importedMesh.Edges[0].StartIndex);
				Assert.AreEqual(mesh.Edges[0].EndIndex, importedMesh.Edges[0].EndIndex);

				var expectedColor = System.Drawing.Color.Red.ToCustomColor();

				Assert.AreEqual(expectedColor.Red, importedMesh.Edges[0].Color.Red);
				Assert.AreEqual(expectedColor.Green, importedMesh.Edges[0].Color.Green);
				Assert.AreEqual(expectedColor.Blue, importedMesh.Edges[0].Color.Blue);
				Assert.AreEqual(expectedColor.Alpha, importedMesh.Edges[0].Color.Alpha);
			}
			finally
			{
				if (File.Exists(filePath))
					File.Delete(filePath);
			}
		}

		[Test]
		public async Task ReadFromBinaryLittleEndianAndAsciiShouldReturnTheSameScene()
		{
			const string fileNameAscii = "small_point_cloud_with_color.ply";
			const string fileNameBinary = "small_point_cloud_with_color_binary.ply";

			var importerExporter = new PlyAssetImporterExporter(_mockSettingsProvider.Object, null);

			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, fileNameAscii);
			var asciiScene = await importerExporter.ImportFileAsync(filePath);

			filePath = Path.Combine(TestConstants.ResourcesFolderPath, fileNameBinary);
			var binaryScene = await importerExporter.ImportFileAsync(filePath);

			Assert.AreEqual(fileNameAscii, Path.GetFileName(asciiScene.FilePath));
			Assert.AreEqual(fileNameBinary, Path.GetFileName(binaryScene.FilePath));
			Assert.AreEqual(asciiScene.Meshes.Count, binaryScene.Meshes.Count);

			Assert.AreEqual(asciiScene.Meshes.First().Points.Count, binaryScene.Meshes.First().Points.Count);
			Assert.AreEqual(asciiScene.Meshes.First().Normals.Count, binaryScene.Meshes.First().Normals.Count);
			Assert.AreEqual(asciiScene.Meshes.First().Colors.Count, binaryScene.Meshes.First().Colors.Count);

			for (int i = 0; i < asciiScene.Meshes.First().Points.Count; ++i)
			{
				Assert.AreEqual(asciiScene.Meshes.First().Points[i], binaryScene.Meshes.First().Points[i]);
			}

			for (int i = 0; i < asciiScene.Meshes.First().Normals.Count; ++i)
			{
				Assert.AreEqual(asciiScene.Meshes.First().Normals[i], binaryScene.Meshes.First().Normals[i]);
			}

			for (int i = 0; i < asciiScene.Meshes.First().Colors.Count; ++i)
			{
				Assert.AreEqual(asciiScene.Meshes.First().Colors[i].Alpha, binaryScene.Meshes.First().Colors[i].Alpha);
				Assert.AreEqual(asciiScene.Meshes.First().Colors[i].Red, binaryScene.Meshes.First().Colors[i].Red);
				Assert.AreEqual(asciiScene.Meshes.First().Colors[i].Green, binaryScene.Meshes.First().Colors[i].Green);
				Assert.AreEqual(asciiScene.Meshes.First().Colors[i].Blue, binaryScene.Meshes.First().Colors[i].Blue);
			}
		}

		[Test]
		public async Task ReadFromAsciiAndWriteToBinaryShouldMatch()
		{
			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, "small_point_cloud_with_color.ply");
			var scene = await _importerExporter.ImportFileAsync(filePath);

			string tmp = Path.Combine(TestConstants.ResourcesFolderPath, "tmp.ply");
			try
			{
				await _importerExporter.ExportFileAsync(scene, tmp, true);
				var binaryScene = await _importerExporter.ImportFileAsync(tmp);

				Assert.AreEqual(scene.Meshes[0].Points.Count, binaryScene.Meshes[0].Points.Count);
				Assert.AreEqual(scene.Meshes[0].Normals.Count, binaryScene.Meshes[0].Normals.Count);
				Assert.AreEqual(scene.Meshes[0].Colors.Count, binaryScene.Meshes[0].Colors.Count);
			}
			finally
			{
				if (File.Exists(tmp))
					File.Delete(tmp);
			}
		}

		[Test]
		public async Task DoesReadColorFromPlyFile()
		{
			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, "small_point_cloud_with_color.ply");
			var scene = await _importerExporter.ImportFileAsync(filePath);

			var mesh = scene.Meshes.First();
			var vertices = new List<Vertex>();

			for (int i = 0; i < scene.Meshes.First().Points.Count; ++i)
			{
				var vertex = new Vertex(mesh.Points[i], mesh.Normals[i], mesh.Colors[i].ToSystemColor());
				vertices.Add(vertex);
			}

			var outputFilePath = Path.ChangeExtension(filePath, ".new_color.ply");
			try
			{
				var pointCloud = new PointCloud(vertices.ToArray());
				var transformedScene = PointCloudToSceneAdapter.GenerateSceneFromPointCloud(pointCloud);
				await _importerExporter.ExportFileAsync(transformedScene, outputFilePath, true);

				Assert.IsTrue(File.Exists(outputFilePath));
			}
			finally
			{
				if (File.Exists(outputFilePath))
					File.Delete(outputFilePath);
			}
		}

		[TestCase(PlyFileFormat.Ascii)]
		[TestCase(PlyFileFormat.BinaryLittleEndian)]
		public async Task CanWriteAndReadSimplisticPointCloud(PlyFileFormat plyFileFormat)
		{
			var point = new List<Vector3>
			{
				new Vector3(0, 0, 0),
				new Vector3(1, 1, 1)
			};

			var pointCloud = new PointCloud(point.Select(p => new Vertex(p)).ToArray());
			var scene = PointCloudToSceneAdapter.GenerateSceneFromPointCloud(pointCloud);
			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, "tmp.ply");

			try
			{
				_mockSettingsProvider.Object.DataSettings.DefaultPlyFileFormat.Value = plyFileFormat;

				_importerExporter = new PlyAssetImporterExporter(_mockSettingsProvider.Object, null);
				await _importerExporter.ExportFileAsync(scene, filePath, true);

				var importedScene = await _importerExporter.ImportFileAsync(filePath);
				var importedMesh = importedScene.Meshes.First();

				Assert.AreEqual(point.Count, importedMesh.Points.Count);
				Assert.AreEqual(point[0], importedMesh.Points[0]);
				Assert.AreEqual(point[1], importedMesh.Points[1]);
			}
			finally
			{
				if (File.Exists(filePath))
					File.Delete(filePath);
			}
		}

		[Test]
		public async Task CanParseFacesEdgesAndLists()
		{
			string filePath = Path.Combine(TestConstants.ResourcesFolderPath, "pure_ply.ply");
			
			var scene = await _importerExporter.ImportFileAsync(filePath);
			var mesh = scene.Meshes.First();

			Assert.AreEqual(8, mesh.Points.Count);
			Assert.AreEqual(5, mesh.Edges.Count);
		}
	}
}