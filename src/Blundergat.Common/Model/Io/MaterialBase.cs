using Sdc = System.Drawing.Color;

namespace Blundergat.Common.Model.Io
{
	public class MaterialBase
	{
		public MaterialBase()
		{
			SpecularExponent = 16;
			Transparency = 1;

			DiffuseColor = new Color(Sdc.White.R, Sdc.White.G, Sdc.White.B, Sdc.White.A);
			SpecularColor = new Color(Sdc.White.R, Sdc.White.G, Sdc.White.B, Sdc.White.A);
			AmbientColor = new Color(Sdc.CornflowerBlue.R, Sdc.CornflowerBlue.G, Sdc.CornflowerBlue.B, Sdc.CornflowerBlue.A);
		}

		public MaterialBase(string name, string fileName) : this()
		{
			Name = name;
			FileName = fileName;
		}

		public string Name { get; set; }

		public string FileName { get; set; }

		public Color AmbientColor { get; set; }

		public Color DiffuseColor { get; set; }

		public Color SpecularColor { get; set; }

		public Color TransmissionFilter { get; set; }

		public Color EmissiveCoefficient { get; set; }

		public TextureMapMode DiffuseTextureMapMode { get; set; }

		public string TextureName { get; set; }

		public string DiffuseTextureName { get; set; }

		public float OpticalDensity { get; set; }

		public float Dissolve { get; set; }

		public float IlluminationModel { get; set; }

		public float SpecularExponent { get; set; }

		public float Transparency { get; set; }
	}
}