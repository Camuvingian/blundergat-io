using System;
using System.Globalization;

namespace Blundergat.Common.Helpers
{
	public static class ExceptionHelper
	{
		/// <summary>
		/// Throws an ArgumentNullException if <paramref name="val"/> is null; otherwise
		/// returns val.
		/// </summary>
		/// <example>
		/// Use this method to throw an ArgumentNullException when using parameters for base
		/// constructor calls.
		/// <code>
		/// public VisualLineText(string text) : base(ThrowUtil.CheckNotNull(text, "text").Length)
		/// </code>
		/// </example>
		public static T CheckNotNull<T>(T val, string parameterName) where T : class
		{
			if (val == null)
				throw new ArgumentNullException(parameterName);
			return val;
		}

		/// <summary>
		/// Check if an int is not negative.
		/// </summary>
		/// <param name="val">The value to check.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <returns>The unchanged checked value.</returns>
		public static int CheckNotNegative(int val, string parameterName)
		{
			if (val < 0)
				throw new ArgumentOutOfRangeException(parameterName, val, "Value must not be negative");
			return val;
		}

		/// <summary>
		/// Check if a given value is in the specified range.
		/// </summary>
		/// <param name="val">The value to check.</param>
		/// <param name="parameterName">The name of the checked parameter.</param>
		/// <param name="lower">The lower range.</param>
		/// <param name="upper">The upper range.</param>
		/// <returns>The unchanged checked value.</returns>
		public static int CheckInRangeInclusive(int val, string parameterName, int lower, int upper)
		{
			if (val < lower || val > upper)
			{
				throw new ArgumentOutOfRangeException(parameterName, val,
					"Expected: " + lower.ToString(CultureInfo.InvariantCulture) + " <= " +
					parameterName + " <= " + upper.ToString(CultureInfo.InvariantCulture));
			}
			return val;
		}
	}
}
