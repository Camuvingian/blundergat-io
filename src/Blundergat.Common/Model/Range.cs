using Blundergat.Common.Helpers;
using System;
using System.Globalization;

namespace Blundergat.Common.Model
{
	/// <summary>
	/// Represent an immutable range with open / closed boundaries. 
	/// </summary>
	public sealed class Range : IEquatable<Range>, IFormattable
	{
		private readonly string _cachedStringFormat;

		public Range(double minimum, bool isMinimumOpen, double maximum, bool isMaximumOpen)
		{
			ValidateBoundaries(minimum, maximum);

			Minimum = minimum;
			Maximum = maximum;
			Length = Math.Abs(Maximum - Minimum);

			IsMinimumOpen = double.IsNegativeInfinity(Minimum) ? true : isMinimumOpen;
			IsMaximumOpen = double.IsPositiveInfinity(Maximum) ? true : isMaximumOpen;
			IsOpen = IsMinimumOpen && IsMaximumOpen;

			_cachedStringFormat = (IsMinimumOpen ? "(" : "[") + "{0}, {1}" + (IsMaximumOpen ? ")" : "]");
		}

		public Range(double minimum, double maximum) : this(minimum, false, maximum, false) { }

		#region Factory Methods.
		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationValue"/>; ends are closed (inclusive).
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationValue">Desired <see cref="Range.Length"/> / 2.</param>
		/// <returns>New instance of <see cref="Range"/>. Its minimum is
		/// <paramref name="mean"/> - <paramref name="deviationValue"/> and its
		/// maximum is <paramref name="mean"/> + <paramref name="deviationValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> 
		/// or <paramref name="deviationValue"/> is <see cref="double.NaN"/>. If 
		/// <paramref name="mean"/> or <paramref name="deviationValue"/> is 
		/// <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/>.
		/// If <paramref name="deviationValue"/> is less than zero.</exception>
		public static Range Create(double mean, double deviationValue)
		{
			return Range.Create(mean, deviationValue, false, false);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationValue"/>.
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationValue">Desired <see cref="Range.Length"/> / 2.</param>
		/// <param name="isMinimumOpen">Whether lower boundary is open (exclusive) or not.</param>
		/// <param name="isMaximumOpen">Whether upper boundary is open (exclusive) or not.</param>
		/// <returns>New instance of <see cref="Range"/>. Its minimum is
		/// <paramref name="mean"/> - <paramref name="deviationValue"/> and its
		/// maximum is <paramref name="mean"/> + <paramref name="deviationValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> or 
		/// <paramref name="deviationValue"/> is <see cref="double.NaN"/>. If 
		/// <paramref name="mean"/> or <paramref name="deviationValue"/> is 
		/// <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/>.
		/// If <paramref name="deviationValue"/> is less than zero.</exception>
		public static Range Create(double mean, double deviationValue, bool isMinimumOpen, bool isMaximumOpen)
		{
			ValidateMean(mean);
			ValidateDeviationValue(deviationValue);

			double min = mean - deviationValue;
			double max = mean + deviationValue;

			return new Range(min, isMinimumOpen, max, isMaximumOpen);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationPercent"/>; ends are closed (inclusive).
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationPercent">Desired distance between <paramref name="mean"/>
		/// and resulting <see cref="Range.Minimum"/> (or <see cref="Range.Minimum"/>)
		/// expressed in percents of <paramref name="mean"/>.</param>
		/// <returns>New instance of <see cref="Range"/> which is <paramref name="mean"/>
		/// +- <paramref name="deviationPercent"/> * <paramref name="mean"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> is 
		/// <see cref="double.NaN"/>. If <paramref name="mean"/> is <see cref="double.NegativeInfinity"/>
		/// or <see cref="double.PositiveInfinity"/>. If <paramref name="deviationPercent"/> is less than zero.
		/// </exception>
		public static Range Create(double mean, int deviationPercent)
		{
			return Create(mean, deviationPercent, false, false);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationPercent"/>.
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationPercent">Desired distance between <paramref name="mean"/>
		/// and resulting <see cref="Range.Minimum"/> (or <see cref="Range.Minimum"/>)
		/// expressed in percents of <paramref name="mean"/>.</param>
		/// <param name="isMinimumOpen">Whether lower boundary is open (exclusive) or not.</param>
		/// <param name="isMaximumOpen">Whether upper boundary is open (exclusive) or not.</param>
		/// <returns>New instance of <see cref="Range"/> which is <paramref name="mean"/>
		/// +- <paramref name="deviationPercent"/> * <paramref name="mean"/>.</returns>
		/// <exception cref="ArgumentException">If <paramref name="mean"/> is <see cref="double.NaN"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> is
		/// <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/>.
		/// If <paramref name="deviationPercent"/> is less than zero.</exception>
		public static Range Create(double mean, int deviationPercent, bool isMinimumOpen, bool isMaximumOpen)
		{
			ValidateMean(mean);
			ValidateDeviationPercent(deviationPercent);

			double max;
			double min;
			double deviationValue = deviationPercent / 100.0;

			if (mean.IsGreaterOrEqual(0.0))
			{
				min = mean - deviationValue * mean;
				max = mean + deviationValue * mean;
			}
			else
			{
				min = mean + deviationValue * mean;
				max = mean - deviationValue * mean;
			}

			return new Range(min, isMinimumOpen, max, isMaximumOpen);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationValue"/> but never exceed
		/// the restrictions; ends are closed (inclusive).
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationValue">Desired <see cref="Range.Length"/> / 2.</param>
		/// <param name="minRestriction">Lower boundary restriction. <see cref="Range.Minimum"/>
		/// of the resulting <see cref="Range"/> instance will not be less than this value.</param>
		/// <param name="maxRestriction">Upper boundary restriction. <see cref="Range.Maximum"/>
		/// of the resulting <see cref="Range"/> instance will not be greater than this value.</param>
		/// <returns>New instance of <see cref="Range"/>. Its minimum is
		/// MAX(<paramref name="mean"/> - <paramref name="deviationValue"/>; <paramref name="minRestriction"/>)
		/// and its maximum is MIN(<paramref name="mean"/> + <paramref name="deviationValue"/>;
		/// <paramref name="maxRestriction"/>).</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> or 
		/// <paramref name="deviationValue"/> or <paramref name="minRestriction"/> or 
		/// <paramref name="maxRestriction"/> is <see cref="double.NaN"/>. If 
		/// <paramref name="mean"/> or <paramref name="deviationValue"/> is 
		/// <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/>.
		/// If <paramref name="deviationValue"/> is less than zero.
		/// If <paramref name="minRestriction"/> is greater than <paramref name="maxRestriction"/>.
		/// </exception>
		public static Range CreateWithRestrictions(double mean, double deviationValue, double minRestriction, double maxRestriction)
		{
			return CreateWithRestrictions(mean, deviationValue, minRestriction, maxRestriction, false, false);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationValue"/> but never exceed
		/// the restrictions.
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationValue">Desired <see cref="Range.Length"/> / 2.</param>
		/// <param name="minRestriction">Lower boundary restriction. <see cref="Range.Minimum"/>
		/// of the resulting <see cref="Range"/> instance will not be less than this value.</param>
		/// <param name="maxRestriction">Upper boundary restriction. <see cref="Range.Maximum"/>
		/// of the resulting <see cref="Range"/> instance will not be greater than this value.</param>
		/// <param name="isMinimumOpen">Whether lower boundary is open (exclusive) or not.</param>
		/// <param name="isMaximumOpen">Whether upper boundary is open (exclusive) or not.</param>
		/// <returns>New instance of <see cref="Range"/>. Its minimum is
		/// MAX(<paramref name="mean"/> - <paramref name="deviationValue"/>; <paramref name="minRestriction"/>)
		/// and its maximum is MIN(<paramref name="mean"/> + <paramref name="deviationValue"/>;
		/// <paramref name="maxRestriction"/>).</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> or 
		/// <paramref name="deviationValue"/> or <paramref name="minRestriction"/> or 
		/// <paramref name="maxRestriction"/> is <see cref="double.NaN"/>. If <paramref name="mean"/>
		/// or <paramref name="deviationValue"/> is <see cref="double.NegativeInfinity"/> or
		/// <see cref="double.PositiveInfinity"/>. If <paramref name="deviationValue"/> is
		/// less than zero. If <paramref name="minRestriction"/> is greater than
		/// <paramref name="maxRestriction"/>.</exception>
		public static Range CreateWithRestrictions(
			double mean,
			double deviationValue,
			double minRestriction,
			double maxRestriction,
			bool isMinimumOpen,
			bool isMaximumOpen)
		{
			ValidateMean(mean);
			ValidateDeviationValue(deviationValue);
			ValidateBoundaries(minRestriction, maxRestriction);

			double min = mean - deviationValue;
			if (min.IsLess(minRestriction))
				min = minRestriction;

			double max = mean + deviationValue;
			if (max.IsGreater(maxRestriction))
				max = maxRestriction;

			return new Range(min, isMinimumOpen, max, isMaximumOpen);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationPercent"/> but never exceed
		/// the restrictions.
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationPercent">Desired distance between <paramref name="mean"/>
		/// and resulting <see cref="Range.Minimum"/> (or <see cref="Range.Minimum"/>)
		/// expressed in percents of <paramref name="mean"/>.</param>
		/// <param name="minRestriction">Lower boundary restriction. <see cref="Range.Minimum"/>
		/// of the resulting <see cref="Range"/> instance will not be less than this value.</param>
		/// <param name="maxRestriction">Upper boundary restriction. <see cref="Range.Maximum"/>
		/// of the resulting <see cref="Range"/> instance will not be greater than this value.</param>
		/// <returns>New instance of <see cref="Range"/> which is <paramref name="mean"/>
		/// +- <paramref name="deviationPercent"/> * <paramref name="mean"/>. Restrictions
		/// are applied and can replace <see cref="Range.Minimum"/> and <see cref="Range.Maximum"/>
		/// in the result.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> or 
		/// <paramref name="minRestriction"/> or <paramref name="maxRestriction"/> is 
		/// <see cref="double.NaN"/>. If <paramref name="mean"/> is <see cref="double.NegativeInfinity"/>
		/// or <see cref="double.PositiveInfinity"/>. If <paramref name="deviationPercent"/> is less than 
		/// zero. If <paramref name="minRestriction"/> is greater than <paramref name="maxRestriction"/>.
		/// </exception>
		public static Range CreateWithRestrictions(
			double mean,
			int deviationPercent,
			double minRestriction,
			double maxRestriction)
		{
			return CreateWithRestrictions(mean, deviationPercent, minRestriction, maxRestriction, false, false);
		}

		/// <summary>
		/// Creates new instance of <see cref="Range"/>. Boundaries are calculated from
		/// <paramref name="mean"/> and <paramref name="deviationPercent"/> but never exceed
		/// the restrictions.
		/// </summary>
		/// <param name="mean">Mean (middle) of the interval.</param>
		/// <param name="deviationPercent">Desired distance between <paramref name="mean"/>
		/// and resulting <see cref="Range.Minimum"/> (or <see cref="Range.Minimum"/>)
		/// expressed in percents of <paramref name="mean"/>.</param>
		/// <param name="minRestriction">Lower boundary restriction. <see cref="Range.Minimum"/>
		/// of the resulting <see cref="Range"/> instance will not be less than this value.</param>
		/// <param name="maxRestriction">Upper boundary restriction. <see cref="Range.Maximum"/>
		/// of the resulting <see cref="Range"/> instance will not be greater than this value.</param>
		/// <param name="isMinimumOpen">Whether lower boundary is open (exclusive) or not.</param>
		/// <param name="isMaximumOpen">Whether upper boundary is open (exclusive) or not.</param>
		/// <returns>New instance of <see cref="Range"/> which is <paramref name="mean"/>
		/// +- <paramref name="deviationPercent"/> * <paramref name="mean"/>. Restrictions
		/// are applied and can replace <see cref="Range.Minimum"/> and <see cref="Range.Maximum"/>
		/// in the result.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="mean"/> or 
		/// <paramref name="minRestriction"/> or <paramref name="maxRestriction"/> is 
		/// <see cref="double.NaN"/>. If <paramref name="mean"/> is <see cref="double.NegativeInfinity"/>
		/// or <see cref="double.PositiveInfinity"/>. If <paramref name="deviationPercent"/> 
		/// is less than zero. If <paramref name="minRestriction"/> is greater than <paramref name="maxRestriction"/>.
		/// </exception>
		public static Range CreateWithRestrictions(
			double mean,
			int deviationPercent,
			double minRestriction,
			double maxRestriction,
			bool isMinimumOpen,
			bool isMaximumOpen)
		{
			ValidateMean(mean);
			ValidateDeviationPercent(deviationPercent);
			ValidateBoundaries(minRestriction, maxRestriction);

			double min;
			double max;
			double deviationValue = deviationPercent / 100.0;

			if (mean.IsGreaterOrEqual(0.0))
			{
				min = mean - deviationValue * mean;
				max = mean + deviationValue * mean;
			}
			else
			{
				min = mean + deviationValue * mean;
				max = mean - deviationValue * mean;
			}

			if (min.IsLess(minRestriction))
				min = minRestriction;

			if (max.IsGreater(maxRestriction))
				max = maxRestriction;

			return new Range(min, isMinimumOpen, max, isMaximumOpen);
		}

		public static Range Zero { get { return new Range(0.0, 0.0); } }
		#endregion // Factory Methods.

		#region Generic Checking Methods.
		public bool IsInRange(double value)
		{
			if (value.IsLess(Minimum) || value.IsGreater(Maximum))
				return false;

			if (value.IsGreater(Minimum) && value.IsLess(Maximum))
				return true;

			bool isMinEdge = value.IsEqual(Minimum);
			if (IsMinimumOpen && isMinEdge)
				return false;

			bool isMaxEdge = value.IsEqual(Maximum);
			if (IsMaximumOpen && isMaxEdge)
				return false;

			return true;
		}

		public bool IsInRange(Range otherRange)
		{
			return IsInRange(otherRange.Minimum) && IsInRange(otherRange.Maximum);
		}

		private static void ValidateMean(double mean)
		{
			if (double.IsNaN(mean) || double.IsInfinity(mean))
				throw new ArgumentOutOfRangeException(nameof(mean));
		}

		private static void ValidateDeviationValue(double deviationValue)
		{
			if (double.IsNaN(deviationValue) ||
				double.IsInfinity(deviationValue) ||
				deviationValue.IsLess(0.0))
			{
				throw new ArgumentOutOfRangeException(nameof(deviationValue));
			}
		}

		private static void ValidateDeviationPercent(int deviationPercent)
		{
			if (deviationPercent < 0)
				throw new ArgumentOutOfRangeException(nameof(deviationPercent));
		}

		private static void ValidateBoundaries(double minimum, double maximum)
		{
			if (double.IsNaN(minimum))
				throw new ArgumentOutOfRangeException(nameof(minimum));

			if (double.IsNaN(maximum))
				throw new ArgumentOutOfRangeException(nameof(maximum));

			if (minimum.IsGreater(maximum))
				throw new ArgumentOutOfRangeException(nameof(minimum));
		}
		#endregion // Generic Checking Methods.

		#region IFormattable.
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return String.Format(_cachedStringFormat,
				Minimum.ToString(format, formatProvider),
				Maximum.ToString(format, formatProvider));
		}

		public string ToStringInvariant()
		{
			return ToStringInvariant(null);
		}

		public string ToStringInvariant(string format)
		{
			return String.Format(_cachedStringFormat,
				Minimum.ToString(format, CultureInfo.InvariantCulture),
				Maximum.ToString(format, CultureInfo.InvariantCulture));
		}
		#endregion // IFormattable.

		#region Object Overrides.
		public override bool Equals(object obj)
		{
			return this.Equals(obj as Range);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 29 + Minimum.GetHashCode();
				hash = hash * 29 + Maximum.GetHashCode();
				hash = hash * 29 + IsMinimumOpen.GetHashCode();
				hash = hash * 29 + IsMaximumOpen.GetHashCode();
				return hash;
			}
		}

		public override string ToString()
		{
			return ToString(null, CultureInfo.CurrentCulture);
		}
		#endregion //  Object Overrides.

		#region Comparison Operators.
		public static bool operator ==(Range left, Range right)
		{
			if (ReferenceEquals(left, null))
				return ReferenceEquals(right, null);

			return left.Equals(right);
		}

		public static bool operator !=(Range left, Range right)
		{
			if (ReferenceEquals(left, null))
				return !ReferenceEquals(right, null);

			return !left.Equals(right);
		}
		#endregion // Comparison Operators.

		#region IEquatable<Range>.

		public bool Equals(Range other)
		{
			if (ReferenceEquals(other, null))
				return false;

			if (ReferenceEquals(other, this))
				return true;

			return Minimum.IsEqual(other.Minimum) &&
				   Maximum.IsEqual(other.Maximum) &&
				   IsMinimumOpen == other.IsMinimumOpen &&
				   IsMaximumOpen == other.IsMaximumOpen;
		}
		#endregion // IEquatable<Range>.

		#region Properties.
		public double Minimum { get; private set; }

		public double Maximum { get; private set; }

		/// <summary>
		/// Gets <see cref="Range"/> length. Always positive.
		/// </summary>
		/// <remarks><see cref="Range.Length"/> = <see cref="Math"/>.Abs(
		/// <see cref="Range.Maximum"/> - <see cref="Range.Minimum"/>).</remarks>
		public double Length { get; private set; }

		/// <summary>
		/// Gets whether a lower boundary of <see cref="Range"/> is open 
		/// (i.e. minimum possible value is exclusive) or closed
		/// (i.e. minimum possible value is inclusive).
		/// </summary>
		public bool IsMinimumOpen { get; private set; }

		/// <summary>
		/// Gets whether an upper boundary of <see cref="Range"/> is open 
		/// (i.e. maximum possible value is exclusive) or closed
		/// (i.e. maximum possible value is inclusive).
		/// </summary>
		public bool IsMaximumOpen { get; private set; }

		/// <summary>
		/// Gets whether <see cref="Range"/> is open. <c>true</c> if both 
		/// <see cref="Range.IsMinimumOpen"/> and <see cref="Range.IsMaximumOpen"/>
		/// are <c>true</c>.
		/// </summary>
		public bool IsOpen { get; private set; }
		#endregion // Properties.

	}
}
