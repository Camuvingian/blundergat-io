using Blundergat.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blundergat.Io.Ply.Models
{
	public class PlyHeader
	{
		public PlyHeader()
		{
			Version = 1.0f;
			HeaderLines = new List<string>();
		}

		public PlyHeader(IEnumerable<string> headerLines)
		{
			Version = 1.0f;
			HeaderLines = headerLines.ToList();
		}

		public float Version { get; set; }

		public PlyFileFormat PlyFileFormat { get; set; }

		public List<string> HeaderLines { get; set; }

		public string RawHeader
		{
			get
			{
				if (HeaderLines == null || HeaderLines.Count == 0)
					return null;

				return String.Join("\n", HeaderLines) + "\n";
			}
		}
	}
}
