using Blundergat.Common.Model.Io;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SystemColor = System.Drawing.Color;

namespace Blundergat.Common.Helpers
{
	/// <summary>
	/// Padding alignments.
	/// </summary>
	public enum Padding { Left, Center, Right };

	public static class Extensions
	{
		public static string ToFormattedString(this TimeSpan timespan)
		{
			return $"{timespan.TotalHours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}.{timespan.Milliseconds:00}s";
		}

		/// <summary>
		/// Creates a deep clone of an object using serialization.
		/// </summary>
		/// <typeparam name="T">The type to be cloned/copied.</typeparam>
		/// <param name="o">The object to be cloned.</param>
		public static T DeepClone<T>(this T o)
		{
			using MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, o);
			stream.Position = 0;
			return (T)formatter.Deserialize(stream);
		}

		#region Angle Conversions.
		public static double ToRadians(this double degrees)
		{
			return degrees * Math.PI / 180;
		}

		public static float ToRadians(this float degrees)
		{
			return degrees * (float)(Math.PI / 180);
		}

		public static double ToDegrees(this double radians)
		{
			return radians * 180 / Math.PI;
		}

		public static float ToDegrees(this float radians)
		{
			return radians * (float)(180 / Math.PI);
		}
		#endregion // Angle Conversions.

		#region List Extensions.
		public static void Resize<T>(this List<T> list, int sz, T c)
		{
			int cur = list.Count;
			if (sz < cur)
				list.RemoveRange(sz, cur - sz);
			else if (sz > cur)
			{
				// This bit is purely an optimisation, to avoid multiple automatic capacity changes.
				if (sz > list.Capacity)
					list.Capacity = sz;
				list.AddRange(Enumerable.Repeat(c, sz - cur));
			}
		}

		public static void Resize<T>(this List<T> list, int sz) where T : new()
		{
			Resize(list, sz, new T());
		}
		#endregion // List Extensions.

		#region LINQ Extension Methods.
		public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
		{
			foreach (var element in toAdd)
			{
				@this.Add(element);
			}
		}

		/// <summary>
		/// Implement the LINQ Remove method for ObsevableCollections.
		/// </summary>
		/// <typeparam name="T">The type of the ObservableCollection.</typeparam>
		/// <param name="coll">The collection to remove from.</param>
		/// <param name="condition">The lambda expression.</param>
		/// <returns></returns>
		public static int Remove<T>(this ICollection<T> coll, Func<T, bool> condition)
		{
			var itemsToRemove = coll.Where(condition).ToList();
			foreach (var itemToRemove in itemsToRemove)
				coll.Remove(itemToRemove);
			return itemsToRemove.Count;
		}

		/// <summary>
		/// Remove the required item from the IList and return the object list.
		/// </summary>
		/// <param name="coll">The collection to search.</param>
		/// <param name="condition">The conditions for which the required objects will be removed.</param>
		/// <returns>The reqiored object.</returns>
		public static List<T> RemoveAndGet<T>(this ICollection<T> coll, Func<T, bool> condition)
		{
			lock (coll)
			{
				var itemsToRemove = coll.Where(condition).ToList();
				foreach (var itemToRemove in itemsToRemove)
					coll.Remove(itemToRemove);
				return itemsToRemove;
			}
		}

		/// <summary>
		/// Get distinct element using functions on specific properties.
		/// </summary>
		/// <typeparam name="TSource">The source collection type.</typeparam>
		/// <typeparam name="TKey">The key selector type.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="keySelector">The selection Func to match on.</param>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
			this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
					yield return element;
			}
		}

		/// <summary>
		/// Clone the IEnumerable to a Stack that preserves the order of the 
		/// initial collection.
		/// </summary>
		/// <typeparam name="T">The Stack type.</typeparam>
		/// <param name="source">The source data.</param>
		/// <returns>The ordered Stack.</returns>
		public static Stack<T> Clone<T>(this IEnumerable<T> source)
		{
			Contract.Requires(source != null);
			return new Stack<T>(new Stack<T>(source));
		}

		private static readonly System.Random rng = new System.Random();

		/// <summary>
		/// Randomize the passed IList.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">The list to radomize.</param>
		public static List<T> Shuffle<T>(this IList<T> l)
		{
			List<T> list = new List<T>(l);
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}

		///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="predicate">The expression to test the items against.</param>
		///<returns>The index of the first matching item, or -1 if no items match.</returns>
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			int retVal = 0;
			foreach (var item in items)
			{
				if (predicate(item)) return retVal;
				retVal++;
			}
			return -1;
		}
		///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="item">The item to find.</param>
		///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
		public static int IndexOf<T>(this IEnumerable<T> items, T item)
		{
			return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, null);
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			if (source == null) 
				throw new ArgumentNullException("source");

			if (selector == null) 
				throw new ArgumentNullException("selector");

			comparer ??= Comparer<TKey>.Default;

			using var sourceIterator = source.GetEnumerator();
			if (!sourceIterator.MoveNext())
				throw new InvalidOperationException("Sequence contains no elements");
			
			var min = sourceIterator.Current;
			var minKey = selector(min);
			while (sourceIterator.MoveNext())
			{
				var candidate = sourceIterator.Current;
				var candidateProjected = selector(candidate);
				if (comparer.Compare(candidateProjected, minKey) < 0)
				{
					min = candidate;
					minKey = candidateProjected;
				}
			}
			return min;
		}

		public static IEnumerable<T> TakeUntilIncluding<T>(this IEnumerable<T> list, Func<T, bool> predicate)
		{
			foreach (T item in list)
			{
				yield return item;
				if (predicate(item))
					yield break;
			}
		}
		#endregion // LINQ Extension Methods.

		#region String Extension Methods.

		public static string SubstringUpTo(this string s, string stopper) => s.Substring(0, Math.Max(0, s.IndexOf(stopper)));

		/// <summary>
		/// Compare without case consideration.
		/// </summary>
		/// <param name="strA">The first string to comapare.</param>
		/// <param name="strB">The second string to comapare.</param>
		/// <returns>Matches?</returns>
		public static bool CompareNoCase(this string strA, string strB)
		{
			return String.Compare(strA, strB, true) == 0;
		}

		/// <summary>
		/// Compare with case consideration.
		/// </summary>
		/// <param name="strA">The first string to comapare.</param>
		/// <param name="strB">The second string to comapare.</param>
		/// <returns>Matches?</returns>
		public static bool Compare(this string strA, string strB)
		{
			return String.Compare(strA, strB, false) == 0;
		}

		/// <summary>
		/// Extension method for checking the contents of 
		/// a string for another embedded string.
		/// </summary>
		/// <param name="strA">String to check.</param>
		/// <param name="strB">String to check for.</param>
		/// <param name="c">The comparison type.</param>
		/// <returns></returns>
		public static bool Contains(this string strA, string strB, StringComparison c)
		{
			return strA.IndexOf(strB, c) >= 0;
		}

		/// <summary>
		/// Replace a character in a string at the desired position.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="index">The index required.</param>
		/// <param name="newChar">The new character.</param>
		/// <returns>The new string.</returns>
		public static string ReplaceAt(this string input, int index, char newChar)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			StringBuilder builder = new StringBuilder(input);
			builder[index] = newChar;
			return builder.ToString();
		}

		/// <summary>
		/// Get the last n chars from a string.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="n">The tail length to return.</param>
		public static string GetLast(this string source, int n)
		{
			if (n >= source.Length)
				return source;

			return source.Substring(source.Length - n);
		}

		/// <summary>
		/// Converts the passed string to camel/title case.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		public static string ToCamelCase(this string s)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
		}

		/// <summary>
		/// Center one string with respect to another.
		/// </summary>
		/// <param name="s">The string to center.</param>
		/// <param name="against">The string to center against.</param>
		public static string CenterWithRespectTo(this string s, string against)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			if (against == null)
				throw new ArgumentNullException("against");
			if (s.Length > against.Length)
				throw new InvalidOperationException();

			int halfSpace = (against.Length - s.Length) / 2;
			return s.PadLeft(halfSpace + s.Length).PadRight(against.Length);
		}

		/// <summary>
		/// Pad a string left, right or centrally.
		/// </summary>
		/// <param name="s">The string to pad.</param>
		/// <param name="totalWidth">The total required size of the string.</param>
		/// <param name="paddingChar">The padding character.</param>
		/// <param name="padding">The direction.</param>
		/// <returns>Formatted string.</returns>
		public static string Pad(this string s, int totalWidth, char paddingChar, Padding padding)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			if (s.Length > totalWidth)
				throw new InvalidOperationException();

			switch (padding)
			{
				case Padding.Left:
					return s.PadLeft(totalWidth, paddingChar);
				case Padding.Center:
					int halfSpace = (totalWidth - s.Length) / 2;
					return s.PadLeft(halfSpace + s.Length).PadRight(totalWidth);
				case Padding.Right:
					return s.PadRight(totalWidth, paddingChar);
				default:
					throw new InvalidOperationException();
			}
		}

		public static string GetCommonSubstring(this string stringA, string stringB)
		{
			return String.Concat(stringA.TakeWhile((c, i) => c == stringB[i]));
		}
		#endregion // String Extension Methods.

		#region Color Methods.
		public static Color ToCustomColor(this SystemColor? c)
		{
			if (c == null)
				return null;

			return ToCustomColor((SystemColor)c);
		}

		public static Color ToCustomColor(this SystemColor c)
		{

			return new Color(c.R, c.G, c.B, c.A);
		}

		public static SystemColor ToSystemColor(this Color c)
		{
			return SystemColor.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
		}
		#endregion // Color Methods.
	}
}
