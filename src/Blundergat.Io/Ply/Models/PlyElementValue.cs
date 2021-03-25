using System.Collections.Generic;

namespace Blundergat.Io.Ply.Models
{
	public class PlyElementValue
	{
		public PlyElementValue()
		{
			PropertyValues = new List<object>();
		}

		public List<object> PropertyValues { get; set; }
	}
}