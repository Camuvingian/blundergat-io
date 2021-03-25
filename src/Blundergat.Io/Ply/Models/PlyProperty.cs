
using Blundergat.Io.Ply.Helpers;

namespace Blundergat.Io.Ply.Models
{
	public class PlyProperty
	{
		public PlyProperty()
		{
		}

		public PlyProperty(string name, int index, PlyPropertyType type)
		{
			Name = name;
			Index = index;
			Type = TypeMapper.GetPlyType(type);
			IsList = false;
		}

		public PlyProperty(string name, int index, PlyPropertyType type, PlyPropertyType listType) 
		{
			Name = name;
			Index = index;
			Type = TypeMapper.GetPlyType(type);
			ListType = TypeMapper.GetPlyType(listType);
			IsList = true;
		}

		/// <summary>
		/// Property name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The index of this property. 
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Property's data type.
		/// </summary>
		public PlyType Type { get; }

		/// <summary>
		/// The internal list type. 
		/// </summary>
		public PlyType ListType { get; }

		/// <summary>
		/// True for list, false for scalar.
		/// </summary>
		public bool IsList { get; }

		public override string ToString()
		{
			return $"property {Type.PlyPropertyType.ToString().ToLower()} {Name}";
		}
	}
}