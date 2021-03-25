using Blundergat.Common.Helpers;
using Blundergat.Common.Model;
using Blundergat.Common.Model.Io;
using System.Collections.Generic;
using System.Linq;

namespace Blundergat.Common.Adapters
{
	public static class SceneToPointCloudAdapter
	{
		public static IPointCloud ExtractPointCloudFromScene(Scene scene)
		{
			if (scene == null || scene.Meshes == null || scene.Meshes.Count == 0)
				throw new System.Exception("Empty scene");

			Mesh mesh = scene.Meshes.First();
			var verticies = new List<Vertex>();

			if (mesh.ContainsNormals)
			{
				if (mesh.ContainsColors)
				{
					for (int i = 0; i < mesh.Points.Count; ++i)
						verticies.Add(new Vertex(mesh.Points[i], mesh.Normals[i], mesh.Colors[i].ToSystemColor()));
				}
				else
				{
					for (int i = 0; i < mesh.Points.Count; ++i)
						verticies.Add(new Vertex(mesh.Points[i], mesh.Normals[i]));
				}
			}
			else
			{
				if (mesh.ContainsColors)
				{
					for (int i = 0; i < mesh.Points.Count; ++i)
						verticies.Add(new Vertex(mesh.Points[i], mesh.Colors[i].ToSystemColor()));
				}
				else
				{
					for (int i = 0; i < mesh.Points.Count; ++i)
						verticies.Add(new Vertex(mesh.Points[i]));
				}
			}
			return new PointCloud(verticies.ToArray(), null, scene.FilePath);
		}
	}
}
