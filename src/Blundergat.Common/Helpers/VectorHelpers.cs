using Blundergat.Common.Model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MNLA = MathNet.Numerics.LinearAlgebra;

namespace Blundergat.Common.Helpers
{
	public static class VectorHelpers
	{
		public static Vector3 Mean(Vertex[] points)
		{
			return Sum(points) / points.Length;
		}

		public static Vector3 Sum(IEnumerable<Vertex> points)
		{
			return Sum(points.Select(p => p.Point));
		}

		public static Vector3 Sum(IEnumerable<Vector3> vectors)
		{
			var sum = Vector3.Zero;
			foreach (var v in vectors)
				sum += v;

			return sum;
		}

		/// <summary>
		/// Gets the length of the vector.
		/// </summary>
		public static float Magnitude(this Vector3 vector)
		{
			return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		}

		/// <summary>
		/// Gets the squared length of the vector.
		/// </summary>
		public static float SquaredMagnitude(this Vector3 vector)
		{
			return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
		}

		public static Vector3 ComputeNormal(Matrix<float> eigenVectors, MNLA.Vector<Complex> eigenValue)
		{
			// Keep the smallest eigenvector as surface normal.
			int smallestIndex = 0;
			float smallestValue = float.MaxValue;
			for (int j = 0; j < eigenVectors.ColumnCount; j++)
			{
				float lambda = (float)eigenValue[j].Real;
				if (lambda < smallestValue)
				{
					smallestIndex = j;
					smallestValue = lambda;
				}
			}

			var normalVector = eigenVectors.Column(smallestIndex);
			return normalVector.Normalize(2).ToVector3();
		}

		public static Vector3 FlipNormalTowardViewPoint(Vector3 normal, Vector3 point, Vector3 viewPoint)
		{
			var normalVector = normal.ToVector();
			var viewPointVector = (viewPoint - point).ToVector();

			if (viewPointVector.DotProduct(normalVector) < 0)
				normalVector *= -1;

			return normalVector.ToVector3();
		}

		public static MNLA.Vector<float> CrossProduct(this MNLA.Vector<float> left, MNLA.Vector<float> right, bool normalize = true)
		{
			if ((left.Count != 3 || right.Count != 3))
				throw new Exception("Vectors must have a length of 3.");

			MNLA.Vector<float> result = MNLA.Vector<float>.Build.Dense(3);

			result[0] = left[1] * right[2] - left[2] * right[1];
			result[1] = -left[0] * right[2] + left[2] * right[0];
			result[2] = left[0] * right[1] - left[1] * right[0];

			if (normalize)
				result = result.Normalize(2);

			return result;
		}

		public static float Norm(this Vector3 v)
		{
			return (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z));
		}

		public static float SquaredNorm(this Vector3 v)
		{
			return (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z);
		}

		public static float Norm(this Vector4 v)
		{
			return (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z) + (v.W * v.W));
		}

		public static float SquaredNorm(this Vector4 v)
		{
			return (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z) + (v.W * v.W);
		}

		public static Vector3 Normalize(this Vector3 v)
		{
			return Vector3.Multiply(v, 1 / v.Norm());
		}

		public static Vector3 ToVector3(this MNLA.Vector<float> v)
		{
			return new Vector3(v[0], v[1], v[2]);
		}

		public static MNLA.Vector<float> ToVector(this Vector3 v)
		{
			return new MNLA.Single.DenseVector(new[] { v.X, v.Y, v.Z });
		}

		public static float AngularDistance(Quaternion q1, Quaternion q2)
		{
			float dot = Quaternion.Dot(q1, q2);
			dot = Math.Min(dot, 1);
			dot = Math.Max(dot, -1);
			return (float)(2 * Math.Acos(dot));
		}

		public static Vector3 MinimumBound(IEnumerable<Vector3> vectors)
		{
			var result = new Vector3(float.MaxValue);
			foreach (var v in vectors)
				result = Vector3.Min(result, v);

			return result;
		}

		public static Vector3 MaximumBound(IEnumerable<Vector3> vectors)
		{
			var result = new Vector3(float.MinValue);
			foreach (var v in vectors)
				result = Vector3.Max(result, v);

			return result;
		}

		public static float GetAt(Vector3 vector, int index)
		{
			return index switch
			{
				0 => vector.X,
				1 => vector.Y,
				2 => vector.Z,
				_ => throw new ArgumentException("index is invalid", "index"),
			};
		}

		public static Vector3 SetAt(Vector3 vector, int index, float v)
		{
			switch (index)
			{
				case 0:
					vector.X = v;
					break;
				case 1:
					vector.Y = v;
					break;
				case 2:
					vector.Z = v;
					break;
			}
			return vector;
		}

		public static int MaxDimension(Vector3 v)
		{
			if (v.X > v.Y && v.X > v.Z)
				return 0;
			else if (v.Y > v.Z)
				return 1;
			else
				return 2;
		}

		public static double[] ToDoubleArray(this Vector3 v)
		{
			return new double[]
				{
					v.X, 
					v.Y, 
					v.Z
				};
		}

		public static Vector3 Copy(this Vector3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static Vector3 ToVector(this double[] array)
		{
			return ToVector(array.Select(x => (float)x).ToArray());
		}

		public static Vector3 ToVector(this float[] array)
		{
			return new Vector3(array[0], array[1], array[2]);
		}

		public static double[][] ToDoubleArray(this IEnumerable<Vector3> collection)
		{
			List<double[]> l = new List<double[]>();
			foreach (var v in collection)
			{
				l.Add(new double[] { v.X, v.Y, v.Z });
			}
			return l.ToArray();
		}

		public static void MakePointNan(ref Vector3 point)
		{
			point.X = point.Y = point.Z = float.NaN;
		}

		public static bool IsNan(this Vector3 point)
		{
			return float.IsNaN(point.X) && float.IsNaN(point.Y) && float.IsNaN(point.Z);
		}

		public static float AverageSqDistance(PointCloud pointCloud, PointCloud pointCloud2)
		{
			float sum = 0;
			for (int i = 0; i < pointCloud.Vertices.Length; i++)
				sum += (pointCloud.Vertices[i].Point - pointCloud2.Vertices[i].Point).LengthSquared();

			return sum / pointCloud.Vertices.Length;
		}

		public static bool IsEqual(this Vector3 left, Vector3 right)
		{
			return left.X.IsEqual(right.X) && left.Y.IsEqual(right.Y) && left.Z.IsEqual(right.Z);
		}

		public static Vector4 ToVector4(this Vector3 vector, float initializer = 0)
		{
			return new Vector4(vector, initializer);
		}
	}
}
