using Blundergat.Common.Types;
using Blundergat.Io.Ply;
using Blundergat.Io.Settings;
using Blundergat.Io.Wavefront;
using Moq;
using NUnit.Framework;
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
			var assetImporterExporter = _assetImporterExporterFactory.GetImporterExporter(AssetImporterExporterType.Ply);
		}
	}
}
