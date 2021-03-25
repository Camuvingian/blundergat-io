using Blundergat.Common.Model.Io;
using Blundergat.Io.Ply.Models;
using System;

namespace Blundergat.Io.Ply.ReaderWriters
{
	public interface IPlyReaderWriter : IDisposable
	{
		string[] ReadElementTokens(PlyElement element);

		void WriteElements(Mesh mesh);
	}
}
