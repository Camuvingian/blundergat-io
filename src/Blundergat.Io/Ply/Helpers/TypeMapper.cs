using Blundergat.Common.Types;
using Blundergat.Io.Ply.Constants;
using Blundergat.Io.Ply.Models;
using System;
using System.Collections.Generic;

namespace Blundergat.Io.Ply.Helpers
{
	public static class TypeMapper
	{
		public static Dictionary<PlyFileFormat, string> FormatMapping
		{
			get
			{
				return new Dictionary<PlyFileFormat, string>()
				{
					{ PlyFileFormat.Ascii, PlyKeywords.Ascii },
					{ PlyFileFormat.BinaryLittleEndian, PlyKeywords.BinaryLittleEndian },
					{ PlyFileFormat.BinaryBigEndian, PlyKeywords.BinaryBigEndian }
				};
			}
		}

		public static PlyPropertyType GetPropertyType(string typeName)
		{
			switch (typeName)
			{
				case PlyTypes.Char:
					return PlyPropertyType.Char;
				case PlyTypes.Short:
					return PlyPropertyType.Short;
				case PlyTypes.Int:
				case PlyTypes.Int32:
					return PlyPropertyType.Int;
				case PlyTypes.UChar:
				case PlyTypes.UInt8:
					return PlyPropertyType.UChar;
				case PlyTypes.UShort:
					return PlyPropertyType.UShort;
				case PlyTypes.UInt:
					return PlyPropertyType.UInt;
				case PlyTypes.Float:
				case PlyTypes.Float32:
					return PlyPropertyType.Float;
				case PlyTypes.Double:
					return PlyPropertyType.Double;
				default:
					throw new ArgumentException($"{typeName} is an unknown PLY type");
			}
		}

		public static PlyType GetPlyType(PlyPropertyType propertyType)
		{
			switch (propertyType)
			{
				case PlyPropertyType.Char:
					return new PlyType(propertyType, typeof(char), 2, (b, i) => BitConverter.ToChar(b, i));
				case PlyPropertyType.Short:
					return new PlyType(propertyType, typeof(short), 2, (b, i) => BitConverter.ToInt16(b, i));
				case PlyPropertyType.Int:
					return new PlyType(propertyType, typeof(int), 4, (b, i) => BitConverter.ToInt32(b, i));
				case PlyPropertyType.UChar:
					return new PlyType(propertyType, typeof(byte), 1, (b, i) => { return b[0]; });
				case PlyPropertyType.UShort:
					return new PlyType(propertyType, typeof(ushort), 2, (b, i) => BitConverter.ToUInt16(b, i));
				case PlyPropertyType.UInt:
					return new PlyType(propertyType, typeof(uint), 4, (b, i) => BitConverter.ToUInt32(b, i));
				case PlyPropertyType.Float:
					return new PlyType(propertyType, typeof(float), 4, (b, i) => BitConverter.ToSingle(b, i));
				case PlyPropertyType.Double:
					return new PlyType(propertyType, typeof(double), 8, (b, i) => BitConverter.ToDouble(b, i));
				case PlyPropertyType.Invalid:
					return null;
				default:
					break;
			}
			return null;
		}

		public static object GetPropertyValues(string token, PlyType type, out int intValue, out uint uintValue, out double doubleValue)
		{
			switch (type.PlyPropertyType)
			{
				case PlyPropertyType.Char:
				case PlyPropertyType.UChar:
				case PlyPropertyType.Short:
				case PlyPropertyType.UShort:
				case PlyPropertyType.Int:
					intValue = Convert.ToInt32(token);
					uintValue = (uint)intValue;
					doubleValue = intValue;
					break;
				case PlyPropertyType.UInt:
					uintValue = Convert.ToUInt32(token);
					intValue = (int)uintValue;
					doubleValue = uintValue;
					break;
				case PlyPropertyType.Float:
				case PlyPropertyType.Double:
					doubleValue = Convert.ToDouble(token);
					intValue = (int)doubleValue;
					uintValue = (uint)doubleValue;
					break;
				default:
					throw new ArgumentException("Unknown type: " + type);
			}

			return type.PlyPropertyType switch
			{
				PlyPropertyType.Char => Convert.ToByte(token),
				PlyPropertyType.Double => Convert.ToDouble(token),
				PlyPropertyType.Float => Convert.ToSingle(token),
				PlyPropertyType.Int => Convert.ToInt32(token),
				PlyPropertyType.Short => Convert.ToInt16(token),
				PlyPropertyType.UChar => Convert.ToByte(token),
				PlyPropertyType.UInt => Convert.ToUInt32(token),
				PlyPropertyType.UShort => Convert.ToUInt16(token),
				_ => throw new ArgumentException("Unknown type: " + type),
			};
		}
	}
}
