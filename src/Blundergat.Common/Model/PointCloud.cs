using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using Blundergat.Common.Helpers;

namespace Blundergat.Common.Model
{
	public class PointCloud : IEquatable<PointCloud>, IPointCloud
	{
		public PointCloud()
		{
			Id = new TId();
		}

		public PointCloud(IPointCloud pointCloud) 
			: this(pointCloud.Vertices, pointCloud.Index, pointCloud.Source)
		{
		}

		public PointCloud(Vertex[] vertices) : this()
		{
			Vertices = vertices;
		}

		public PointCloud(Vertex[] vertices, int? index, string source = default)
			: this(vertices)
		{
			Index = index;
			Source = source;
		}

		#region Methods.
		public Vector3 GetCentroid()
		{
			var centroid = new Vector3();
			if (IsEmpty)
				return centroid;

			foreach (var vertex in Vertices)
				centroid += vertex.Point;

			return centroid / Vertices.Length;
		}

		public int ReorientateNormals()
		{
			if (!ContainsNormals)
				return 0;

			int counter = 0;
			var centroid = GetCentroid();
			for (int i = 0; i < Vertices.Length; ++i)
			{
				var normal = Vertices[i].Normal;

				// #TODO We must change the direct access and setting of struct types. 
				//Vertices[i] = new Vertex(Vertices[i].Point, FlipNormalTowardCentroid(Vertices[i], centroid), Vertices[i].Color);
				Vertices[i].Normal = FlipNormalTowardCentroid(Vertices[i], centroid);

				if (normal != Vertices[i].Normal)
					counter++;
			}
			return counter;
		}

		private Vector3 FlipNormalTowardCentroid(Vertex vertex, Vector3 centroid)
		{
			var normal = vertex.Normal.ToVector();
			var viewPointVector = (centroid - vertex.Point).ToVector();

			if (viewPointVector.DotProduct(normal) < 0)
				normal *= -1;

			return normal.ToVector3();
		}

		public virtual void RemovePoints(IPointCloud pointCloudToRemove)
		{
			if (pointCloudToRemove.Vertices == null || pointCloudToRemove.Vertices.Length == 0)
				return;

			var verticesToKeep = new List<Vertex>();
			var verticesToRemove = pointCloudToRemove.Vertices.ToList();

			foreach(var vertex in Vertices)
			{
				if (!verticesToRemove.Contains(vertex))
					verticesToKeep.Add(vertex);
			}

			Array.Clear(Vertices, 0, Vertices.Length);
			Vertices = verticesToKeep.ToArray();
		}

		public IPointCloud Merge(IPointCloud pointCloud)
		{
			if (pointCloud == null || pointCloud.Vertices.Length == 0)
				return this;

			List<Vertex> vertices = pointCloud.Vertices.ToList();
			if (Vertices == null || Vertices.Length == 0)
				return new PointCloud(vertices.ToArray(), pointCloud.Index, pointCloud.Source);

			vertices.AddRange(Vertices);
			return new PointCloud(vertices.ToArray(), Index, Source);
		}

		public (Vector3 MinimumBound, Vector3 MaximumBound) GetBoundingBoxAsPoints()
		{
			float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
			float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

			if (Vertices.Length > 0)
			{
				foreach (var point in Vertices.Select(v => v.Point))
				{
					if (minX > point.X)
						minX = point.X;

					if (minY > point.Y)
						minY = point.Y;

					if (minZ > point.Z)
						minZ = point.Z;

					if (maxX < point.X)
						maxX = point.X;

					if (maxY < point.Y)
						maxY = point.Y;

					if (maxZ < point.Z)
						maxZ = point.Z;
				}
			}
			return (new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
		}

		public BoundingBox GetBoundingBox()
		{
			var (MinimumBound, MaximumBound) = GetBoundingBoxAsPoints();
			return new BoundingBox(
				new Range(MinimumBound.X, MaximumBound.X),
				new Range(MinimumBound.Y, MaximumBound.Y),
				new Range(MinimumBound.Z, MaximumBound.Z));
		}

		public BoundingBox GetPaddedBoundingBox(float paddingPercentage)
		{
			var (MinimumBound, MaximumBound) = GetBoundingBoxAsPoints();

			var xLength = Math.Abs(MaximumBound.X - MinimumBound.X);
			var yLength = Math.Abs(MaximumBound.Y - MinimumBound.Y);
			var zLength = Math.Abs(MaximumBound.Z - MinimumBound.Z);

			return new BoundingBox(
				new Range(MinimumBound.X - paddingPercentage * xLength, MaximumBound.X + paddingPercentage * xLength),
				new Range(MinimumBound.Y - paddingPercentage * yLength, MaximumBound.Y + paddingPercentage * yLength),
				new Range(MinimumBound.Z - paddingPercentage * zLength, MaximumBound.Z + paddingPercentage * zLength));
		}

		public double GetPointDensity()
		{
			var boundingBox = GetBoundingBox();
			var volume = boundingBox.XRange.Length * boundingBox.YRange.Length * boundingBox.ZRange.Length;
			return Vertices.Length / volume;
		}
		#endregion // Methods.

		#region Operator Overrides.
		public bool Equals([AllowNull] PointCloud other)
		{
			return other.Id == Id;
		}

		public static bool operator ==(PointCloud left, PointCloud right)
		{
			if (ReferenceEquals(left, null))
				return ReferenceEquals(right, null);

			return left.Equals(right);
		}

		public static bool operator !=(PointCloud left, PointCloud right)
		{
			if (ReferenceEquals(left, null))
				return !ReferenceEquals(right, null);

			return !left.Equals(right);
		}
		#endregion // Operator Overrides.

		#region Object Overrides.
		/// <summary>
		/// Determines whether the specified <see cref="Range"/> is equal to the current one.
		/// </summary>
		/// <param name="obj">The <see cref="Range"/> object to compare with the current one.</param>
		/// <returns><c>true</c> if the specified <see cref="Range"/> is equal to the current one;
		/// otherwise <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as PointCloud);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Range"/>.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 29 + Id.GetHashCode();
				return hash;
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder($"Point Cloud {{{Id}}}, Index {Index ?? -1:N0} (\"{Source}\"), ");
			builder.Append($"\tVertex count {Vertices.Length:N0}, Contains Normals = {ContainsNormals.ToString()}");
			return builder.ToString();
		}
		#endregion // Object Overrides.

		public TId Id { get; private set; }

		public Vertex[] Vertices { get; set; }

		public int? Index { get; set; }

		public string Source { get; set; }

		public bool IsEmpty => Vertices == null || Vertices.Length == 0;

		public bool ContainsNormals => Vertices == null ? false : Vertices.Any(v => v.Normal != Vector3.Zero);

		public Vector3 GlobalShiftVector { get; private set; }

		public IList<EuclideanTransform> EclidianTransformStore { get; set; }
	}
}