using System.Numerics;

namespace Blundergat.Io.Wavefront.Models
{
	public class Face
	{
		public static int GL_TRIANGLES = 1;
		public static int GL_QUADS = 2;

		public FaceType Type { get; set; }

		public int[] VertexIndices { get; set; }

		public int[] NormalIndices { get; set; }

		public int[] TextureIndices { get; set; }

		public Vector3[] Vertices { get; set; }

		public Vector3[] Normals { get; set; }

		public TextureCoordinate[] TextureCoordinates { get; set; }
	}
}