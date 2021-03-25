using Blundergat.Io.Ply.Models;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public interface IPlyHeaderReaderWriter
	{
		PlyFile ReadFileStructure(string filePath);

		void WriteFileStructure(PlyFile plyFile);
	}
}