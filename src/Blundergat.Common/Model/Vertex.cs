using Blundergat.Common.Helpers;
using System;
using System.Drawing;
using System.Numerics;

namespace Blundergat.Common.Model
{
	public class Vertex : IEquatable<Vertex>, ICloneable
	{
		public Vertex() 
		{ 
		}

		public Vertex(Vertex vertex) 
			: this(vertex.Point, vertex.Normal, vertex.Color) 
		{ 
		}

		public Vertex(Vector3 point) 
			: this(point, new Vector3(), new Color()) 
		{ 
		}

		public Vertex(Vector3 point, Color color) 
			: this(point, new Vector3(), color) 
		{ 
		}

		public Vertex(Vector3 point, Vector3 normal) 
			: this(point, normal, new Color())
		{
		}

		public Vertex(Vector3 point, Vector3 normal, Color? color)
		{
			Point = point;
			Normal = normal;
			Color = color;
		}

		#region IClonable.
		public object Clone()
		{
			return (Vertex)MemberwiseClone();
		}
		#endregion // IClonable.

		#region IEquatable<Vertex>.
		public override bool Equals(object obj)
		{
			return this.Equals(obj as Vertex);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 29 + Point.GetHashCode();
				hash = hash * 29 + Normal.GetHashCode();
				return hash;
			}
		}
		
		public bool Equals(Vertex other)
		{
			if (other is null)
				return false;

			if (ReferenceEquals(other, this))
				return true;

			return Point.IsEqual(other.Point) &&
				   Normal.IsEqual(other.Normal);
		}
		#endregion // IEquatable<Vertex>.

		public Vector3 Point;

		public Vector3 Normal;

		public Color? Color;
	}
}