using Blundergat.Io.Wavefront.Models;

namespace Blundergat.Io.Wavefront.Parsers
{
	public abstract class LineParser
	{
		public string[] Words { get; set; }

		public virtual void Parse(string fileName)
		{
			Parse();
		}

		public abstract void Parse();

		public abstract void IncorporateResults(WavefrontObject wavefrontObject);
	}
}