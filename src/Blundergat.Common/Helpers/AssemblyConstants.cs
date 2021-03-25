using System;
using System.Reflection;

namespace Blundergat.Common.Helpers
{
	public static class AssemblyConstants
	{
		/// <summary>
		/// Gets the main assembly trademark name.
		/// </summary>
		public static readonly string Trademark =
			((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(
				Assembly.GetExecutingAssembly(),
				typeof(AssemblyTrademarkAttribute), false)).Trademark;

		/// <summary>
		/// The current version of Augur.
		/// </summary>
		public static readonly string Version =
			Assembly.GetExecutingAssembly().GetName().Version.ToString();

		/// <summary>
		/// The currently active user of the system.
		/// </summary>
		public static readonly string CurrentUser =
			System.Security.Principal.WindowsIdentity.GetCurrent().Name;
	}
}