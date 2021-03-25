using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Blundergat.Common.Helpers
{
	public static class FloatExtensions
	{
		public static float RoundToNearest(this float num)
		{
			return (float)DoubleExtensions.RoundToNearest(num);
		}

		public static bool IsEqual(this float left, float right)
		{
			return left.AlmostEqual(right, float.Epsilon);
		}

		public static bool IsLess(this float left, float right)
		{
			return left.IsSmaller(right, float.Epsilon);
		}

		public static bool IsLessOrEqual(this float left, float right)
		{
			return left.IsSmaller(right, float.Epsilon) || left.AlmostEqual(right, float.Epsilon);
		}

		public static bool IsGreater(this float left, float right)
		{
			return left.IsLarger(right, float.Epsilon);
		}

		public static bool IsGreaterOrEqual(this float left, float right)
		{
			return left.IsLarger(right, float.Epsilon) || left.AlmostEqual(right, float.Epsilon);
		}

		public static String ToStringInvariant(this float value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}

	public sealed class FloatExtensionComparer : IComparer<float>
	{
		public int Compare(float x, float y)
		{
			if (x.IsLess(y))
			{
				return -1;
			}
			else if (x.IsGreater(y))
			{
				return 1;
			}
			return 0;
		}
	}
}
