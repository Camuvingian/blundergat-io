using Blundergat.Io.Ply.Models;
using Blundergat.Io.Ply.ReaderWriters;
using Blundergat.Io.Test.Helpers;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Blundergat.Io.Test.Ply
{
	[TestFixture]
	public class PlyElementReaderWriterTests
	{
		private PlyFile _asciiPlyFile;
		private PlyFile _binaryPlyFile;

		[SetUp]
		public void SetUp()
		{
			var headerParser = new PlyHeaderReaderWriter();

			string asciiFilePath = Path.Combine(TestConstants.ResourcesFolderPath, "small_point_cloud_with_color.ply");
			_asciiPlyFile = headerParser.ReadFileStructure(asciiFilePath);

			string binaryFilePath = Path.Combine(TestConstants.ResourcesFolderPath, "small_point_cloud_with_color_binary.ply");
			_binaryPlyFile = headerParser.ReadFileStructure(binaryFilePath);
		}

		[Test]
		public void ReadShouldReturnTheSameData()
		{
			var asciiReaderWriter = new AsciiReaderWriter(_asciiPlyFile, OperationType.Import);
			var asciiFirstTokenSet = asciiReaderWriter.ReadElementTokens(_asciiPlyFile.Elements.First());

			var binaryReaderWriter = new BinaryLittleEndianReaderWriter(_binaryPlyFile, OperationType.Import);
			var binaryFirstTokenSet = binaryReaderWriter.ReadElementTokens(_binaryPlyFile.Elements.First());

			for (int i = 0; i < asciiFirstTokenSet.Length; ++i)
			{
				Assert.AreEqual(asciiFirstTokenSet[i], binaryFirstTokenSet[i]);
			}
		}
	}
}
