using Blundergat.Io.Wavefront.Models;
using System.IO;
using Blundergat.Common.Model.Io;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class TextureNameParser : LineParser
	{
		private readonly WavefrontObject _object;
		private string _texName;
		private string _fullTextureFileName;

		public TextureNameParser(WavefrontObject @object)
		{
			_object = @object;
		}

		public override void Parse()
		{
			string textureFileName = Words[Words.Length - 1];
			_texName = textureFileName;
			_fullTextureFileName = Path.Combine(_object.Contextfolder, textureFileName);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			MaterialBase currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.TextureName = _fullTextureFileName;
		}
	}
}