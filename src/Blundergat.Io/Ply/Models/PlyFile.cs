using System.Collections.Generic;

namespace Blundergat.Io.Ply.Models
{
	public class PlyFile
	{
		public PlyFile(string filePath)
		{
			FilePath = filePath;
			Header = new PlyHeader();
			Comments = new List<string>();
			Elements = new List<PlyElement>();
			ObjectInformationItems = new List<string>();
		}

		public string FilePath { get; }

		public PlyHeader Header { get; set; }

		public List<string> Comments { get; set; }

		public List<PlyElement> Elements { get; set; }

		public List<string> ObjectInformationItems { get; set; }
	}
}