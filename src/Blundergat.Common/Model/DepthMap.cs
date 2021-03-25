using System;
using System.Collections.Generic;
using System.Text;

namespace Blundergat.Common.Model
{
	public class DepthMap
	{
		public int? Index { get; set; }

		public string Source { get; set; }

		public List<double> DepthMapPixels { get; set; }

		public int Height { get; set; }

		public int Width { get; set; }

		public override string ToString()
		{
			string source = !String.IsNullOrEmpty(Source) ? Source : "N/A";
			StringBuilder builder = new StringBuilder($"DepthMapRaw: Source \"{source}\"");
			if (DepthMapPixels != null)
				builder.Append($", Pixel count = {DepthMapPixels.Count:N0}");

			builder.Append($", Width = {Width:N0}, Height = {Height:N0}");
			return builder.ToString();
		}
	}
}