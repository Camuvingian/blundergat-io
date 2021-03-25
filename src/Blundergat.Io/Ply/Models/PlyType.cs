using System;

namespace Blundergat.Io.Ply.Models
{
	public class PlyType
	{
		public PlyType(PlyPropertyType plyPropertyType, Type netType, int byteCount, Func<byte[], int, object> byteConverter)
		{
			PlyPropertyType = plyPropertyType;
			NetType = netType;
			ByteCount = byteCount;
			ByteConverter = byteConverter;
 		}

		public Type NetType { get; set; }

		public int ByteCount { get; set; }

		public PlyPropertyType PlyPropertyType { get; set; }

		public Func<byte[], int, object> ByteConverter { get; }
	}
}
