using Blundergat.Common.Types;

namespace Blundergat.Io.Settings
{
	public interface IIoSettings
	{
		public string BaseDirectory { get; set; }

		public PlyFileFormat DefaultPlyFileFormat { get; set; }
	}
}