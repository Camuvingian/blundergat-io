using Blundergat.Common.Adapters;
using Blundergat.Common.Helpers;
using Blundergat.Common.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blundergat.Io
{
	public class AssetImportExportUtilities
	{
		public virtual Task WritePointCloudToFileAsync(IAssetImporterExporter assetImporterExporter, IPointCloud pointCloud, string filePath)
		{
			var scene = PointCloudToSceneAdapter.GenerateSceneFromPointCloud(pointCloud);
			return assetImporterExporter.ExportFileAsync(scene, filePath, true);
		}

		public virtual async Task<IPointCloud> ReadPointCloudFromFileAsync(IAssetImporterExporter assetImporterExporter, string filePath)
		{
			var scene = await assetImporterExporter.ImportFileAsync(filePath);

			var mesh = scene.Meshes.First();
			var vertices = new List<Vertex>();

			for (int i = 0; i < scene.Meshes.First().Points.Count; ++i)
			{
				var vertex = new Vertex(mesh.Points[i], mesh.Normals[i], mesh.Colors[i].ToSystemColor());
				vertices.Add(vertex);
			}

			return new PointCloud(vertices.ToArray(), null, filePath);
		}
	}
}