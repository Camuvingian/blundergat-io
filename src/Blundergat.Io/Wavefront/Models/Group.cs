using Blundergat.Common.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Blundergat.Io.Wavefront.Models
{
	public class Group
	{
		public Group() : this("default") { }

		public Group(string name)
		{
			Name = name;

			Faces = new List<Face>();
			Indices = new List<int>();

			Vertices = new List<Vector3>();
			Normals = new List<Vector3>();

			TextureCoordinates = new List<TextureCoordinate>();
			IndexCount = 0;
		}

		public void AddFace(Face face)
		{
			Faces.Add(face);
		}

		public void Pack()
		{
			float minX = 0.0f;
			float minY = 0.0f;
			float minZ = 0.0f;
			foreach (Face face in Faces)
			{
				foreach (Vector3 vertex in face.Vertices)
				{
					if (Math.Abs(vertex.X) > minX)
						minX = Math.Abs(vertex.X);
					if (Math.Abs(vertex.Y) > minY)
						minY = Math.Abs(vertex.Y);
					if (Math.Abs(vertex.Z) > minZ)
						minZ = Math.Abs(vertex.Z);
				}
			}
			Min = new Vertex(new Vector3(minX, minY, minZ));
		}

		public string Name { get; private set; }

		public Material Material { get; set; }

		public List<Face> Faces { get; set; }

		public List<int> Indices { get;  set; }

		public List<Vector3> Vertices { get; set; }

		public List<Vector3> Normals { get; set; }

		public List<TextureCoordinate> TextureCoordinates { get; set; }

		public Vertex Min { get; private set; }

		public int IndexCount { get; set; }

		
	}
}