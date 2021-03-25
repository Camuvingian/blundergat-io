using Blundergat.Io.Wavefront.Models;
using Blundergat.Common.Model.Io;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class SpecularExponentParser : LineParser
	{
		private float _specularExponent;

		public override void Parse()
		{
			_specularExponent = float.Parse(Words[1]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			MaterialBase currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.SpecularExponent = _specularExponent;
		}
	}
}