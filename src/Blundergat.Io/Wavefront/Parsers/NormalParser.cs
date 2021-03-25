using Blundergat.Io.Wavefront.Models;
using System.Numerics;

namespace Blundergat.Io.Wavefront.Parsers
{
	public class NormalParser : LineParser
	{
		private Vector3 _normal;

		public override void Parse()
		{
			_normal = new Vector3
			{
				X = float.Parse(Words[1]),
				Y = float.Parse(Words[2]),
				Z = float.Parse(Words[3])
			};
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			wavefrontObject.Normals.Add(_normal);
		}
	}
}