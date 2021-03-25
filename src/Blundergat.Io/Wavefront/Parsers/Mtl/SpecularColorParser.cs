using Blundergat.Common.Model.Io;
using Blundergat.Io.Wavefront.Models;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class SpecularColorParser : LineParser
	{
		private Color _specularColor;

		public override void Parse()
		{
			_specularColor = new Color
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
			currentMaterial.SpecularColor = _specularColor;
		}
	}
}