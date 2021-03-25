using Blundergat.Io.Wavefront.Models;
using Blundergat.Io.Wavefront.Objects.Parsers.Obj;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class MtlLineParserFactory : LineParserFactory
	{
		public MtlLineParserFactory(WavefrontObject @object)
			: base(@object)
		{
			Parsers.Add("newmtl", new MaterialParser());
			Parsers.Add("Ka", new AmbientColorParser());
			Parsers.Add("Kd", new DiffuseColorParser());
			Parsers.Add("Ks", new SpecularColorParser());
			Parsers.Add("Ns", new SpecularExponentParser());
			Parsers.Add("map_Kd", new TextureNameParser(@object));
			Parsers.Add("#", new CommentParser());
		}
	}
}