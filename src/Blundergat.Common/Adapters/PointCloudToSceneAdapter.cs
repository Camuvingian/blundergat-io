using Blundergat.Common.Helpers;
using Blundergat.Common.Model;
using Blundergat.Common.Model.Io;
using System.Collections.Generic;
using System.Linq;

namespace Blundergat.Common.Adapters
{
	public static class PointCloudToSceneAdapter
	{
		public static Scene GenerateSceneFromPointCloud(IPointCloud pointCloud)
		{
			return new Scene(new List<Mesh>() { GenerateMeshFromPointCloud(pointCloud) }, pointCloud.Source);
		}

		private static Mesh GenerateMeshFromPointCloud(IPointCloud pointCloud)
		{
			return new Mesh(
				pointCloud.Source,
				pointCloud.Vertices.Select(v => v.Point).ToList(),
				pointCloud.Vertices.Select(v => v.Normal).ToList(), 
				pointCloud.Vertices.Select(v => v.Color.ToCustomColor()).ToList());
		}
	}
}