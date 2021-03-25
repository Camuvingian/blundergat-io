using Blundergat.Common.Model.Io;
using Blundergat.Io.Wavefront.Models;
using Color = Blundergat.Common.Model.Io.Color;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class DiffuseColorParser : LineParser
	{
		private Color _diffuseColor;

		public override void Parse()
		{
			_diffuseColor = new Color
			{
				Red = byte.Parse(Words[1]),
				Green = byte.Parse(Words[2]),
				Blue = byte.Parse(Words[3]), 
				Alpha = byte.Parse(Words[4])
			};
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			MaterialBase currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.DiffuseColor = _diffuseColor;
		}
	}
}