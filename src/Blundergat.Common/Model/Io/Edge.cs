namespace Blundergat.Common.Model.Io
{
	public class Edge
	{
		public Edge(int startIndex, int endIndex, Color color)
		{
			StartIndex = startIndex;
			EndIndex = endIndex;
			Color = color;
		}

		public int StartIndex { get; set; }

		public int EndIndex { get; set; }

		public Color Color { get; set; }
	}
}