using Blundergat.Common.Types;

namespace Blundergat.Io.Settings
{
	public interface ISettings
	{
		public string BaseDirectory { get; set; }

		public PlyFileFormat DefaultPlyFileFormat { get; set; }
	}
}