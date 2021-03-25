using System.Collections.Generic;
using System.Text;

namespace Blundergat.Common.Model.Io
{
	public class Scene
	{
		public Scene()
		{
			Meshes = new List<Mesh>();
			Materials = new List<MaterialBase>();
		}

		public Scene(List<Mesh> meshes, string filePath) : this(meshes)
		{
			FilePath = filePath;
		}

		public Scene(List<Mesh> meshes)
		{
			Meshes = meshes;
			Materials = new List<MaterialBase>();
		}

		public string FilePath { get; set; }

		public List<Mesh> Meshes { get; private set; }

		public List<MaterialBase> Materials { get; private set; }

		public string GetMetrics()
		{
			var builder = new StringBuilder();
			builder.Append($"Mesh Count = {Meshes.Count:N0}");

			foreach (var mesh in Meshes)
				builder.AppendLine(mesh.GetMetrics());

			return builder.ToString();
		}
	}
}