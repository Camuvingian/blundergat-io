using System;
using System.Numerics;

namespace Blundergat.Common.Model
{
	public class BoundingBox
	{
		public BoundingBox(Range xRange, Range yRange, Range zRange)
		{
			XRange = xRange;
			YRange = yRange;
			ZRange = zRange;
		}

		public Vector3 GetCenter()
		{
			return new Vector3(
				(float)(XRange.Maximum + XRange.Minimum) / 2,
				(float)(YRange.Maximum + YRange.Minimum) / 2,
				(float)(ZRange.Maximum + ZRange.Minimum) / 2);
		}

		public float GetDiagonalLength()
		{
			return (float)Math.Sqrt(
				XRange.Length * XRange.Length + 
				YRange.Length * YRange.Length + 
				ZRange.Length * ZRange.Length);
		}

		public BoundingBox GetCombinedBoundingBox(BoundingBox boundingBox)
		{
			return new BoundingBox(
				GetExtremeRange(XRange, boundingBox.XRange),
				GetExtremeRange(YRange, boundingBox.YRange),
				GetExtremeRange(ZRange, boundingBox.ZRange));
		}

		public BoundingBox GetSolutionSpace(BoundingBox boundingBox)
		{
			return new BoundingBox(
				new Range(XRange.Minimum - boundingBox.XRange.Length, XRange.Maximum + boundingBox.XRange.Length),
				new Range(YRange.Minimum - boundingBox.YRange.Length, YRange.Maximum + boundingBox.YRange.Length),
				new Range(ZRange.Minimum < boundingBox.ZRange.Minimum ? ZRange.Minimum : boundingBox.ZRange.Minimum,
						  ZRange.Maximum > boundingBox.ZRange.Maximum ? ZRange.Maximum : boundingBox.ZRange.Maximum));
				// #TODO might need to change the ZRange calculations for fully 3D solver. 
		}

		public (Vector3 Center, double Radius) GetBoundingSphereCenterAndRadius()
		{
			var center = GetCenter();
			var radius = Math.Sqrt(
				(XRange.Maximum - center.X) * (XRange.Maximum - center.X) +
				(YRange.Maximum - center.Y) * (YRange.Maximum - center.Y) +
				(ZRange.Maximum - center.Z) * (ZRange.Maximum - center.Z));
			return (center, radius);
		}

		public double GetVolume()
		{
			return XRange.Length * YRange.Length * ZRange.Length;
		}

		private Range GetExtremeRange(Range rangeA, Range rangeB)
		{
			double minimum = rangeA.Minimum;
			if (rangeB.Minimum < rangeA.Minimum)
				minimum = rangeB.Minimum;

			double maximum = rangeA.Maximum;
			if (rangeB.Maximum > rangeA.Maximum)
				maximum = rangeB.Maximum;

			return new Range(minimum, maximum);
		}

		public BoundingBox AlignCenterTo(Vector3 target)
		{
			var thisCenter = GetCenter();
			var delta = thisCenter - target;

			return new BoundingBox(
				new Range(XRange.Minimum - delta.X, XRange.Maximum - delta.X),
				new Range(YRange.Minimum - delta.Y, YRange.Maximum - delta.Y),
				new Range(ZRange.Minimum - delta.Z, ZRange.Maximum - delta.Z));
		}

		public Range XRange { get; }

		public Range YRange { get; }

		public Range ZRange { get; }

		public override string ToString()
		{
			return $"(XRange = {XRange.ToString()}, YRange = {YRange.ToString()}, ZRange = {ZRange.ToString()})";
		}
	}
}