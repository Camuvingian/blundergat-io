using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Blundergat.Common.Helpers
{
	public static class DoubleExtensions
	{
		public static double RoundToNearest(this double num)
		{
			return (num > 0.0) ? Math.Floor(num + 0.5) : Math.Ceiling(num - 0.5);
		}

		public static bool IsEqual(this double left, double right)
		{
			return left.AlmostEqual(right, double.Epsilon);
		}

		public static bool IsLess(this double left, double right)
		{
			return left.IsSmaller(right, double.Epsilon);
		}

		public static bool IsLessOrEqual(this double left, double right)
		{
			return left.IsSmaller(right, double.Epsilon) || left.AlmostEqual(right, double.Epsilon);
		}

		public static bool IsGreater(this double left, double right)
		{
			return left.IsLarger(right, double.Epsilon);
		}

		public static bool IsGreaterOrEqual(this double left, double right)
		{
			return left.IsLarger(right, double.Epsilon) || left.AlmostEqual(right, double.Epsilon);
		}

		public static String ToStringInvariant(this double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}

	public sealed class DoubleExtensionComparer : IComparer<double>
	{
		public int Compare(double x, double y)
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
