using Blundergat.Io.Wavefront.Models;
using System.IO;

namespace Blundergat.Io.Wavefront.Parsers.Mtl
{
	public class MaterialFileParser : LineParser
	{
		private WavefrontObject _object;
		private MtlLineParserFactory _parserFactory;

		public MaterialFileParser(WavefrontObject @object)
		{
			_object = @object;
			_parserFactory = new MtlLineParserFactory(@object);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			// Material are directly added by the parser, no need to do anything here...
		}

		public override void Parse()
		{
			string filename = Words[1];
			string pathToMTL = Path.Combine(_object.Contextfolder, filename);

			using StreamReader reader = new StreamReader(pathToMTL);
			string currentLine;
			while ((currentLine = reader.ReadLine()) != null)
			{
				LineParser parser = _parserFactory.GetLineParser(currentLine);
				parser.Parse(filename);
				parser.IncorporateResults(_object);
			}
			reader.Close();
		}
	}
}