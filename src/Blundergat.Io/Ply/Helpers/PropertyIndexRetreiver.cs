using Blundergat.Io.Ply.Constants;
using Blundergat.Io.Ply.Models;
using System.Collections.Generic;

namespace Blundergat.Io.Ply.Helpers
{
	public static class PropertyIndexRetreiver
	{
		public static PropertyIndexes GetPropertyIndexes(IDictionary<string, PlyProperty> properties)
		{
			var propertyIndexes = new PropertyIndexes();

			if (properties.TryGetValue(PlyKeywords.X, out PlyProperty propertyX))
				propertyIndexes.X = propertyX.Index;

			if (properties.TryGetValue(PlyKeywords.Y, out PlyProperty propertyY))
				propertyIndexes.Y = propertyY.Index;

			if (properties.TryGetValue(PlyKeywords.Z, out PlyProperty propertyZ))
				propertyIndexes.Z = propertyZ.Index;

			if (properties.TryGetValue(PlyKeywords.NormalX, out PlyProperty propertyNx))
				propertyIndexes.Nx = propertyNx.Index;

			if (properties.TryGetValue(PlyKeywords.NormalY, out PlyProperty propertyNy))
				propertyIndexes.Ny = propertyNy.Index;

			if (properties.TryGetValue(PlyKeywords.NormalZ, out PlyProperty propertyNz))
				propertyIndexes.Nz = propertyNz.Index;

			if (properties.TryGetValue(PlyKeywords.Red, out PlyProperty propertyRed))
				propertyIndexes.Red = propertyRed.Index;

			if (properties.TryGetValue(PlyKeywords.Green, out PlyProperty propertyGreen))
				propertyIndexes.Green = propertyGreen.Index;

			if (properties.TryGetValue(PlyKeywords.Blue, out PlyProperty propertyBlue))
				propertyIndexes.Blue = propertyBlue.Index;

			if (properties.TryGetValue(PlyKeywords.Alpha, out PlyProperty propertyAlpha))
				propertyIndexes.Alpha = propertyAlpha.Index;

			if (properties.TryGetValue(PlyKeywords.S, out PlyProperty propertyS))
				propertyIndexes.S = propertyS.Index;

			if (properties.TryGetValue(PlyKeywords.T, out PlyProperty propertyT))
				propertyIndexes.T = propertyT.Index;

			if (properties.TryGetValue(PlyKeywords.Vertex1, out PlyProperty propertyVertex1))
				propertyIndexes.Vertex1 = propertyVertex1.Index;

			if (properties.TryGetValue(PlyKeywords.Vertex2, out PlyProperty propertyVertex2))
				propertyIndexes.Vertex2 = propertyVertex2.Index;

			return propertyIndexes;
		}
	}
}
