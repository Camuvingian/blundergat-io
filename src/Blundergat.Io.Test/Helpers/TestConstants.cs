using System.IO;
using System.Reflection;

namespace Blundergat.Io.Test.Helpers
{
	public static class TestConstants
	{
		public static string ResourcesFolderPath
		{
			get
			{
				var location = Assembly.GetExecutingAssembly().Location;
				return Path.Combine(Path.Combine(Path.GetDirectoryName(location), "Resources"));
			}
		}
	}
}