using Blundergat.Io.Wavefront.Parsers;
using Blundergat.Io.Wavefront.Models;
using System.Numerics;

namespace Blundergat.Io.Wavefront.Objects.Parsers.Obj
{
	public class VertexParser : LineParser
	{
		private Vector3 _vertex;

		public override void Parse()
		{
			_vertex = new Vector3();

			_vertex.X = float.Parse(Words[1]);
			_vertex.Y = float.Parse(Words[2]);
			_vertex.Z = float.Parse(Words[3]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			wavefrontObject.Vertices.Add(_vertex);
		}
	}
}