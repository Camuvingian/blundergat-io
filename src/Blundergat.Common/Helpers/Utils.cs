using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Blundergat.Common.Helpers
{
	public static class Utils
	{
		/// <summary>
		/// Routine to compare to IEnumerable values for equality.
		/// </summary>
		/// <typeparam name="T">The Type to compair.</typeparam>
		/// <param name="list1">The first collection.</param>
		/// <param name="list2">The second collection.</param>
		/// <returns>Equal?</returns>
		public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
		{
			var cnt = new Dictionary<T, int>();
			foreach (T s in list1)
			{
				if (cnt.ContainsKey(s))
					cnt[s]++;
				else
					cnt.Add(s, 1);
			}
			foreach (T s in list2)
			{
				if (cnt.ContainsKey(s))
					cnt[s]--;
				else
					return false;
			}
			return cnt.Values.All(c => c == 0);
		}

		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		#region Mathematics Helpers.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Pow2(int n)
		{
			uint x = (uint)(1 << n);
			return (int)x;
		}
		#endregion // Mathematics Helpers.

		#region File Helpers.
		/// <summary>
		/// Convert a byte count to a formatted string.
		/// </summary>
		/// <param name="byteCount"></param>
		/// <returns></returns>
		public static string BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
			if (byteCount == 0)
				return String.Format("0 {0}", suf[0]);

			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return String.Format("{0:N2} {1}", Math.Sign(byteCount) * num, suf[place]);
		}

		/// <summary>
		/// Sum the size of the passed file info.
		/// </summary>
		/// <param name="files">The files to get the size of.</param>
		/// <returns>The toal size of all of the passed files.</returns>
		public static long SumFileSizesAsync(IEnumerable<FileInfo> files)
		{
			long total = 0;
			Parallel.ForEach<FileInfo, long>(files,
				() => 0,
				(file, loop, subtotal) =>
				{
					subtotal += file.Length;
					return subtotal;
				},
				(result) => Interlocked.Add(ref total, result));
			return total;
		}

		/// <summary>
		/// Quickly establish if the specified directory is empty. 
		/// </summary>
		/// <param name="path">The path to check.</param>
		/// <returns>Is empty?</returns>
		public static bool IsDirectoryEmpty(string path)
		{
			if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
				return !Directory.EnumerateFileSystemEntries(path).Any();
			return false;
		}

		/// <summary>
		/// Returns the path to the executing assembly.
		/// </summary>
		public static string GetAssemblyDirectory(Assembly assembly)
		{
			string codeBase = assembly.CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		/// <summary>
		/// Return a list of the subdirectories of a given folder. This 
		/// does not recursively itterate through all layers/levels.
		/// </summary>
		/// <param name="directory">The directory to get the subdirectories for.</param>
		public static List<string> GetFlatSubdirectories(string directory)
		{
			List<string> l = new List<string>();
			foreach (string dir in Directory.GetDirectories(directory))
			{
				try
				{
					l.Add(dir);
				}
				catch (Exception)
				{
					// Do nothing. 	
				}
			}
			return l;
		}

		/// <summary>
		/// Remove log files older than the specified time span and with extenstion.
		/// </summary>
		/// <param name="logFileDirectory">The directory constaining the log files.</param>
		/// <param name="ts">The time span cut off.</param>
		/// <param name="extension">The specified extension.</param>
		/// <param name="protectCount">Protext at least n files.</param>
		public static int RemoveLogFilesOlderThan(string logFileDirectory,
			TimeSpan ts, string extension, int? protectCount = null)
		{
			int removalCount = 0;
			var files = Directory.GetFiles(logFileDirectory)
				.Where(s => Path.GetExtension(s).CompareNoCase(extension));

			if (protectCount != null && files.Count() <= protectCount)
				return 0;

			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);
				if (fi.LastAccessTime < DateTime.Now.Subtract(ts))
				{
					fi.Delete();
					removalCount++;
				}
			}
			return removalCount;
		}

		public static bool AreAnyFilesLocked(string directory, string searchPattern)
		{
			return Directory.GetFiles(directory, searchPattern)
				.Select(f => new FileInfo(f))
				.Any(fi => IsFileLocked(fi));
		}

		public static bool IsFileLocked(FileInfo file)
		{
			try
			{
				using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
				stream.Close();
			}
			catch (IOException)
			{
				return true;
			}
			return false;
		}
		#endregion // File Helpers.

		#region Enum Helpers.
		/// <summary>
		/// Gets all items for an enum type.
		/// </summary>
		/// <typeparam name="T">The type of the enum to itterate.</typeparam>
		/// <returns>All of the enum values of the given type.</returns>
		public static IEnumerable<T> GetAllEnums<T>() where T : struct, IConvertible
		{
			foreach (object item in Enum.GetValues(typeof(T)))
			{
				yield return (T)item;
			}
		}

		/// <summary>
		/// Convert a string to an enum.
		/// </summary>
		/// <typeparam name="T">The enum type.</typeparam>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The enum type.</returns>
		public static T ToEnum<T>(this string value) where T : struct, IConvertible
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		/// <summary>
		/// Convert a string to an enum with a fallback default value.
		/// </summary>
		/// <typeparam name="T">The enum type.</typeparam>
		/// <param name="value">The string value to parse.</param>
		/// <param name="defaultValue">The default enum value.</param>
		/// <returns>The enum type.</returns>
		public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IConvertible
		{
			if (string.IsNullOrEmpty(value))
				return defaultValue;

			return Enum.TryParse<T>(value, true, out T result) ? result : defaultValue;
		}
		#endregion // Enum Helpers.

		#region Converters.
		/// <summary>
		/// Convert an object to a byte array
		/// </summary>
		/// <param name="obj">The object to convert to byte array.</param>
		/// <returns>The required byte array.</returns>
		public static byte[] ToByteArray(this object o)
		{
			if (o == null)
				return null;

			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, o);
				return ms.ToArray();
			}
		}

		/// <summary>
		/// Convert a byte array to an Object
		/// </summary>
		/// <param name="arrBytes">The bytes to convert back to object.</param>
		/// <returns>The requested deserialized object.</returns>
		public static object ToObject(this byte[] arrBytes)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				ms.Write(arrBytes, 0, arrBytes.Length);
				ms.Seek(0, SeekOrigin.Begin);
				return bf.Deserialize(ms);
			}
		}
		#endregion // Converters.

		#region Dynamic.
		public static T MaximumValue<T>(T value) //where T : struct
		{
			return MaximumValue((dynamic)value);
		}

		public static int MaximumValue(int dummy)
		{
			return int.MaxValue;
		}

		public static double MaximumValue(double dummy)
		{
			return double.MaxValue;
		}

		public static byte MaximumValue(byte dummy)
		{
			return byte.MaxValue;
		}

		public static object MaximumValue(object dummy)
		{
			// This method will catch all types that has no specific method
			throw new NotSupportedException(dummy.GetType().Name);
		}

		public static T MinimumValue<T>(T value) //where T : struct
		{
			return MinimumValue((dynamic)value);
		}

		public static int MinimumValue(int dummy)
		{
			return int.MinValue;
		}

		public static double MinimumValue(double dummy)
		{
			return double.MinValue;
		}

		public static byte MinimumValue(byte dummy)
		{
			return byte.MinValue;
		}

		public static object MinimumValue(object dummy)
		{
			// This method will catch all types that has no specific method.
			throw new NotSupportedException(dummy.GetType().Name);
		}
		#endregion // Dynamic.
	}
}
