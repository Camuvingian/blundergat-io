using Blundergat.Io.Wavefront.Models;
using Blundergat.Io.Wavefront.Parsers;
using System.Numerics;

namespace Blundergat.Io.Wavefront.Objects.Parsers.Obj
{
	public class FaceParser : LineParser
	{
		private Face _face;

		public int[] _vindices;
		public int[] _nindices;
		public int[] _tindices;

		private Vector3[] _vertices;
		private Vector3[] _normals;

		private TextureCoordinate[] _textures;
		private readonly WavefrontObject _object = null;

		public FaceParser(WavefrontObject @object)
		{
			_object = @object;
		}

		public override void Parse()
		{
			_face = new Face();
			switch (Words.Length)
			{
				case 4:
					ParseTriangles();
					break;
				case 5:
					ParseQuad();
					break;
				default:
					break;
			}
		}

		private void ParseTriangles()
		{
			_face.Type = FaceType.Triangles;
			ParseLine(3);
		}

		private void ParseLine(int vertexCount)
		{
			// Fix: explicitly set the array length.
			string[] rawFaces = new string[3];

			_vindices = new int[vertexCount];
			_nindices = new int[vertexCount];
			_tindices = new int[vertexCount];

			_vertices = new Vector3[vertexCount];
			_normals = new Vector3[vertexCount];
			_textures = new TextureCoordinate[vertexCount];

			for (int i = 1; i <= vertexCount; i++)
			{
				// Fix: Use additional array for parsed words
				var wordParts = Words[i].Split('/');

				// Fix: setup rawFaces array
				rawFaces[0] = wordParts[0];
				rawFaces[1] = (wordParts.Length == 3) ? wordParts[1] : null;
				rawFaces[2] = (wordParts.Length == 3)
					? wordParts[2]
					: (wordParts.Length == 2)
						? wordParts[1]
						: null;

				// v.
				int currentValue = int.Parse(rawFaces[0]);
				_vindices[i - 1] = currentValue - 1;

				// Save vertex. -1 because references starts at 1.
				_vertices[i - 1] = _object.Vertices[currentValue - 1];
				if (wordParts.Length == 1)
					continue;

				// Save texcoords.
				if (!string.IsNullOrEmpty(rawFaces[1]))
				{
					// This is to compensate the fact that if no texture is in the obj file, sometimes '1' is put 
					// instead of 'blank' (we find coord1/1/coord3 instead of coord1//coord3 or coord1/coord3).
					currentValue = int.Parse(rawFaces[1]);
					if (currentValue <= _object.Textures.Count) 
					{
						// -1 because references starts at 1.
						_tindices[i - 1] = currentValue - 1;
						_textures[i - 1] = _object.Textures[currentValue - 1];
					}
				}

				// Save normal. -1 because references starts at 1.
				if (!string.IsNullOrEmpty(rawFaces[2]))
				{
					currentValue = int.Parse(rawFaces[2]);

					_nindices[i - 1] = currentValue - 1;
					_normals[i - 1] = _object.Normals[currentValue - 1]; 
				}
			}
		}


		private void ParseQuad()
		{
			_face.Type = FaceType.Quads;
			ParseLine(4);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Group group = wavefrontObject.CurrentGroup;

			if (group == null)
			{
				group = new Group();
				wavefrontObject.Groups.Add(group);
				wavefrontObject.CurrentGroup = group;
			}

			if (_vertices.Length == 3)
			{
				// Add list of vertex/normal/texcoord to current group.
				// Each object keeps a list of its own data, apart from the global list.
				group.Vertices.Add(_vertices[0]);
				group.Vertices.Add(_vertices[1]);
				group.Vertices.Add(_vertices[2]);
				group.Normals.Add(_normals[0]);
				group.Normals.Add(_normals[1]);
				group.Normals.Add(_normals[2]);
				group.TextureCoordinates.Add(_textures[0]);
				group.TextureCoordinates.Add(_textures[1]);
				group.TextureCoordinates.Add(_textures[2]);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);	// create index list for current object
			}
			else
			{
				// Add list of vertex/normal/texcoord to current group.
				// Each object keeps a list of its own data, apart from the global list.
				group.Vertices.Add(_vertices[0]);
				group.Vertices.Add(_vertices[1]);
				group.Vertices.Add(_vertices[2]);
				group.Vertices.Add(_vertices[3]);
				group.Normals.Add(_normals[0]);
				group.Normals.Add(_normals[1]);
				group.Normals.Add(_normals[2]);
				group.Normals.Add(_normals[3]);
				group.TextureCoordinates.Add(_textures[0]);
				group.TextureCoordinates.Add(_textures[1]);
				group.TextureCoordinates.Add(_textures[2]);
				group.TextureCoordinates.Add(_textures[3]);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);	// create index list for current object
			}

			_face.VertexIndices = _vindices;
			_face.NormalIndices = _nindices;
			_face.TextureIndices = _tindices;
			_face.Normals = _normals;
			_face.Vertices = _vertices;
			_face.TextureCoordinates = _textures;

			wavefrontObject.CurrentGroup.AddFace(_face);
		}
	}
}