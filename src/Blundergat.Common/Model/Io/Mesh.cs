using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Blundergat.Common.Model.Io
{
	public class Mesh
	{
		public Mesh() 
			: this(String.Empty, new List<Vector3>(), new List<Vector3>(), new List<Color>()) 
		{ 
		}

		public Mesh(List<Vector3> points) 
			: this(String.Empty, points, new List<Vector3>(), new List<Color>()) 
		{ 
		}

		public Mesh(List<Vector3> points, IEnumerable<Vector3> normals) 
			: this(String.Empty, points, normals, new List<Color>())
		{
		}

		public Mesh(string name, IEnumerable<Vector3> points, IEnumerable<Vector3> normals)
			: this(name, points, normals, new List<Color>())
		{
		}

		public Mesh(string name, IEnumerable<Vector3> points, IEnumerable<Vector3> normals, IEnumerable<Color> colors)
		{
			Name = name;
			Points = points.ToList();
			Normals = normals.ToList();
			Colors = colors.ToList();
			Indices = new List<int>();
			TextureCoordinates = new List<Vector3>();
			Transform = new Tuple<Vector3, Quaternion>(Vector3.Zero, Quaternion.Identity);
			Edges = new List<Edge>();
		}

		public string Name { get; set; }

		public List<Vector3> Points { get; private set; }

		public List<Vector3> Normals { get; private set; }

		public List<Color> Colors { get; private set; }

		public List<Vector3> TextureCoordinates { get; private set; }

		public MaterialBase Material { get; set; }

		public List<Edge> Edges { get; set; }

		public List<int> Indices { get; private set; }

		public Tuple<Vector3, Quaternion> Transform { get; set; }

		public bool ContainsNormals => Normals.Any(n => n.X != 0.0f || n.Y != 0.0f || n.Z != 0.0f);

		public bool ContainsColors => Colors.Any(c => c.Red != 0 || c.Green != 0 || c.Blue != 0 || c.Alpha != 0);

		public bool ContainsEdges => Edges.Any();

		public string GetMetrics()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append($"Name = {Name}, ");
			builder.Append($"Point Count = {Points.Count:N0}, ");
			builder.Append($"Non-Zero Normals = {Normals.Count(n => n.X != 0.0f && n.Y != 0.0f && n.Z != 0.0f):N0}");
			return builder.ToString();
		}
	}
}