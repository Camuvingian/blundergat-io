using Blundergat.Io.Wavefront.Parsers;
using Blundergat.Io.Wavefront.Objects.Parsers.Obj;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Blundergat.Common.Model.Io;

namespace Blundergat.Io.Wavefront.Models
{
	public class WavefrontObject
	{
		private ObjLineParserFactory _parserFactory;

		public WavefrontObject()
		{
			Vertices = new List<Vector3>();
			Normals = new List<Vector3>();
			Textures = new List<TextureCoordinate>();
			Groups = new List<Group>();
			Materials = new Dictionary<string, Material>();
		}

		public async static Task<WavefrontObject> LoadAsync(string fileName)
		{
			WavefrontObject result = new WavefrontObject();
			await result.LoadFromFileAsync(fileName);
			return result;
		}

		private Task LoadFromFileAsync(string fileName)
		{
			return Task.Run(() =>
			{
				FileName = fileName;
				Contextfolder = Path.GetDirectoryName(fileName);
				Parse(fileName);
				CalculateRadius();
			});
		}

		private void Parse(string fileName)
		{
			_parserFactory = new ObjLineParserFactory(this);

			using StreamReader reader = new StreamReader(fileName);
			string currentLine;
			while ((currentLine = reader.ReadLine()) != null)
				ParseLine(currentLine);

			reader.Close();
		}

		private void CalculateRadius()
		{
			foreach (Vector3 vertex in Vertices)
			{
				float currentNorm = vertex.Length();
				if (currentNorm > Radius)
					Radius = currentNorm;
			}
		}

		private void ParseLine(string currentLine)
		{
			if (string.IsNullOrEmpty(currentLine))
				return;

			LineParser parser = _parserFactory.GetLineParser(currentLine);
			parser.Parse();
			parser.IncorporateResults(this);
		}

		public Group CurrentGroup { get; set; }

		public MaterialBase CurrentMaterial { get; set; }

		public string FileName { get; set; }

		public List<Vector3> Vertices { get; set; }

		public List<Vector3> Normals { get; set; }

		public List<TextureCoordinate> Textures { get; set; }

		public List<Group> Groups { get; set; }

		public Dictionary<string, Material> Materials { get; set; }

		public string Contextfolder { get; private set; }

		public float Radius { get; set; }
	}
}
