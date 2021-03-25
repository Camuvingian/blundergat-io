using Blundergat.Common.Types;
using Blundergat.Common.Settings.Types;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Blundergat.Common.Settings.Impl
{
	[Serializable]
	public class DataSettings 
	{
		public DataSettings()
		{
			BaseDirectory = new Setting<string>("C:\\Data", "The default directory for IO operations");
			DefaultPlyFileFormat = new Setting<PlyFileFormat>(PlyFileFormat.BinaryLittleEndian, "The default file format to use for PLY exports");
		}

		[XmlElement, Browsable(false)]
		public Setting<string> BaseDirectory { get; set; }

		[XmlElement, Browsable(false)]
		public Setting<PlyFileFormat> DefaultPlyFileFormat { get; set; }
	}
}