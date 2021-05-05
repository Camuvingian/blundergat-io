using Blundergat.Io.Ply;
using NUnit.Framework;
using System.IO;

namespace Blundergat.Io.Test.Ply
{
	[TestFixture]
	public class AssetImporterExporterBaseTests
	{
		private AssetImporterExporterBase _exporter;

		[SetUp]
		public void Setup()
		{
			_exporter = new PlyAssetImporterExporter(null, null);
		}

		[Test]
		public void EnsureIncorrectFilePathExtensionGetsCorrected()
		{
			string filePath = "C:\\data\\some_file_name.obj";
			string correctedFilePath = _exporter.CorrectFilePath(filePath);

			Assert.AreEqual(".ply", Path.GetExtension(correctedFilePath));
		}

		[Test]
		public void EnsureIncorrectFilePathExtensionGetsCorrectedWithNumericSubExtension()
		{
			string filePath = "C:\\data\\some_file_name.0100.obj";
			string correctedFilePath = _exporter.CorrectFilePath(filePath);

			Assert.IsTrue(correctedFilePath.Contains("0100"));
			Assert.AreEqual(".ply", Path.GetExtension(correctedFilePath));
		}

		[Test]
		public void EnsureFilePathWithoutExtensionGetsCorrectedExtension()
		{
			string filePath = "C:\\data\\some_file_name";
			string correctedFilePath = _exporter.CorrectFilePath(filePath);

			Assert.AreEqual(".ply", Path.GetExtension(correctedFilePath));
		}

		[Test]
		public void EnsureInvalidDirectoryThrowsIOException()
		{
			string filePath = "C:\\null\\some_file_name.ply";
			var ex = Assert.Throws<IOException>(() => _exporter.CorrectFilePath(filePath));

			string directory = Path.GetDirectoryName(filePath);
			Assert.That(ex.Message, Is.EqualTo($"Directory \"{directory}\" does not exists"));
		}
	}
}
