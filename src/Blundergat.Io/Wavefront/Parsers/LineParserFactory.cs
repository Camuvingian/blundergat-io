using Blundergat.Io.Wavefront.Models;
using Blundergat.Io.Wavefront.Parsers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blundergat.Io.Wavefront.Objects.Parsers.Obj
{
	public abstract class LineParserFactory
	{
		protected Dictionary<string, LineParser> Parsers = new Dictionary<string, LineParser>();

		protected LineParserFactory(WavefrontObject @object)
		{
			Object = @object;
		}

		public LineParser GetLineParser(string line)
		{
			if (line == null)
				return null;

			// String[] lineWords = line.split(" ");		
			// Nhaaaaaaaaaaa, 3DS max doesn't use clean space but some other shity character :( !
			// So I could use something like String regularExpression = "[A-Za-z]*([^\\-\\.0-9]*(\\-\\.0-9]*))";
			// or be nasty :P
			//line = Regex.Replace(line, "[^ \\.\\-A-Za-z0-9#/]", string.Empty);
			line = Regex.Replace(line, @"\s+", " ");
			string[] lineWords = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			// lineType is the first word in the line (except v, vp, vn, vt)
			if (lineWords.Length < 1)
				return new DefaultParser(); ;

			string lineType = lineWords[0];

			if (!Parsers.TryGetValue(lineType, out LineParser parser))
				parser = new DefaultParser();

			parser.Words = lineWords;
			return parser;
		}

		protected WavefrontObject Object { get; private set; }
	}
}