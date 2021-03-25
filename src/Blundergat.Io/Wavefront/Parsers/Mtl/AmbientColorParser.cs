using Blundergat.Common.Model.Io;
using Blundergat.Io.Wavefront.Models;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class AmbientColorParser : LineParser
	{
		private Color _ambientColor;

		public override void Parse()
		{
			_ambientColor = new Color
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
			currentMaterial.AmbientColor = _ambientColor;
		}
	}
}