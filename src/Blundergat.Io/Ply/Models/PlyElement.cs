using System.Collections.Generic;

namespace Blundergat.Io.Ply.Models
{
	public class PlyElement
	{
		public PlyElement()
		{
			Properties = new Dictionary<string, PlyProperty>();
			ElementValues = new List<PlyElementValue>();
		}

		public PlyElement(string name, int objectCount) : this()
		{
			Name = name;
			ObjectCount = objectCount;
		}

		/// <summary>
		/// Element name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Size of element (bytes) or -1 if variable.
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Number of elements in this object.
		/// </summary>
		public int ObjectCount { get; set; }

		/// <summary>
		/// List of properties in the file.
		/// </summary>
		//public List<PlyProperty> Properties { get; set; }

		public Dictionary<string, PlyProperty> Properties { get; set; }

		/// <summary>
		/// List of complex element values. 
		/// </summary>
		public List<PlyElementValue> ElementValues { get; set; }

		public override string ToString()
		{
			return $"element {Name} {ObjectCount}";
		}
	}
}