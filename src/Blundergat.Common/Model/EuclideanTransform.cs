using System.Numerics;
using System.Runtime.CompilerServices;

namespace Blundergat.Common.Model
{
	public struct EuclideanTransform
	{
		public Quaternion Rotation;
		public Vector3 Translation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 Apply(Vector3 v)
		{
			return Vector3.Transform(v, Rotation) + Translation;
		}

		public EuclideanTransform Inverse()
		{
			EuclideanTransform result;

			// The rotation is the opposite of the applied rotation.
			result.Rotation = Quaternion.Conjugate(Rotation);
			result.Translation = Vector3.Transform(-1 * Translation, result.Rotation);
			return result;
		}

		/// p2 = r * p + t
		/// p = (p2 - t0) * r^-1
		/// p = r^-1 * p2 - t * r^-1

		/// <summary>
		/// Computes a transform that represents applying e2 then e1
		/// </summary>
		public static EuclideanTransform operator *(EuclideanTransform e1, EuclideanTransform e2)
		{
			EuclideanTransform result;
			result.Rotation = e1.Rotation * e2.Rotation;
			result.Translation = Vector3.Transform(e2.Translation, e1.Rotation) + e1.Translation;
			return result;
		}

		public static EuclideanTransform Identity
		{
			get
			{
				return new EuclideanTransform
				{
					Translation = new Vector3(),
					Rotation = Quaternion.Identity
				};
			}
		}

		public override string ToString()
		{
			return $"Translation = {Translation.ToString()}, Rotation = {Rotation.ToString()}";
		}
	}
}