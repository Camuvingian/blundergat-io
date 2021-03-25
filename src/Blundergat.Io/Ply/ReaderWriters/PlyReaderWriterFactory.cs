using Blundergat.Common.Types;
using Blundergat.Io.Ply.Models;
using System;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public static class PlyReaderWriterFactory
	{
		public static IPlyReaderWriter GetReaderWriter(PlyFile plyFile, OperationType operationType)
		{
			return plyFile.Header.PlyFileFormat switch
			{
				PlyFileFormat.Ascii => new AsciiReaderWriter(plyFile, operationType),
				PlyFileFormat.BinaryLittleEndian => new BinaryLittleEndianReaderWriter(plyFile, operationType),
				_ => throw new Exception(),
			};
		}
	}
}
