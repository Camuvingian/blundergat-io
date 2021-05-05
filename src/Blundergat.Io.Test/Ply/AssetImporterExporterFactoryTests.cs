using Blundergat.Common.Types;
using Blundergat.Io.Ply;
using Blundergat.Io.Settings;
using Blundergat.Io.Wavefront;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Blundergat.Io.Test.Ply
{
	[TestFixture]
	public class AssetImporterExporterFactoryTests
	{
		protected IAssetImporterExporterFactory _assetImporterExporterFactory;
		private Mock<IIoSettings> _mockSettings;

		[SetUp]
		public void SetUp()
		{
			_mockSettings = new Mock<IIoSettings>();
			_mockSettings.Object.DefaultPlyFileFormat = PlyFileFormat.BinaryLittleEndian;

			_assetImporterExporterFactory = AssetImporterExporterFactory.Instance(
				new List<(AssetImporterExporterType, IAssetImporterExporter)>()
					{
						(AssetImporterExporterType.Ply, new PlyAssetImporterExporter(_mockSettings.Object, null)),
						(AssetImporterExporterType.Obj, new ObjAssetImporterExporter(_mockSettings.Object, null))
					});
		}

		[TestCase(AssetImporterExporterType.Obj, ".obj")]
		[TestCase(AssetImporterExporterType.Ply, ".ply")]
		public void DoesResolveCorrectExporterUsingEnum(AssetImporterExporterType type, string extension)
		{
			var assetImporterExporter = _assetImporterExporterFactory.GetImporterExporter(type);
			Assert.AreEqual(extension, assetImporterExporter.Extension);
		}

		[TestCase("pointCloudFileName.obj", typeof(ObjAssetImporterExporter))]
		[TestCase("pointCloudFileName.ply", typeof(PlyAssetImporterExporter))]
		[TestCase("C:\\SomeDir\\SomeOtherDir\\pointCloudFileName.obj", typeof(ObjAssetImporterExporter))]
		[TestCase("C:\\SomeDir\\SomeOtherDir\\pointCloudFileName.ply", typeof(PlyAssetImporterExporter))]
		public void DoesResolveCorrectExporterUsingFilePath(string filePath, Type type)
		{
			var assetImporterExporter = _assetImporterExporterFactory.GetImporterExporter(filePath);
			Assert.IsAssignableFrom(type, assetImporterExporter);
		}
	}
}
