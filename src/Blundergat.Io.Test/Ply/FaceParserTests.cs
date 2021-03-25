using Blundergat.Io.Wavefront.Models;
using Blundergat.Io.Wavefront.Objects.Parsers.Obj;
using NUnit.Framework;
using System.Numerics;

namespace Blundergat.Io.Ply.Test
{
	[TestFixture]
	public class FaceParserTests
	{
		[Test]
		public void CanParseStyle1()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1/1/1 1/1/1 1/1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle2()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1/1/1 1/1/1 1/1/1 1/1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle3()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1//1 1//1 1//1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle4()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1//1 1//1 1//1 1//1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle5()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1/1 1/1 1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle6()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1/1 1/1 1/1 1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}

		[Test]
		public void CanParseStyle7()
		{
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vector3());
			wavefrontObject.Normals.Add(new Vector3());
			const string line = "f 1 1 1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			parser.Parse();
		}
	}
}
