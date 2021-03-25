namespace Blundergat.Common.Model.Io
{
	public class Color
	{
		public Color()
		{
			Red = 1;
			Green = 1;
			Blue = 1;
			Alpha = 1;
		}

		public Color(byte red, byte green, byte blue, byte alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3}", Red, Green, Blue, Alpha);
		}

		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public byte Alpha { get; set; }
	}
}