using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Blundergat.Common.Model
{
	public interface IPointCloud
	{
		IPointCloud Merge(IPointCloud pointCloud);

		Vector3 GetCentroid();

		int ReorientateNormals();

		void RemovePoints(IPointCloud pointCloudToRemove);

		(Vector3 MinimumBound, Vector3 MaximumBound) GetBoundingBoxAsPoints();

		BoundingBox GetBoundingBox();

		BoundingBox GetPaddedBoundingBox(float paddingPercentage);

		double GetPointDensity();

		TId Id { get; }

		Vertex[] Vertices { get; set; }

		int? Index { get; set; }

		string Source { get; set; }

		bool IsEmpty { get; }

		bool ContainsNormals { get; }

		Vector3 GlobalShiftVector { get; }
	}
}