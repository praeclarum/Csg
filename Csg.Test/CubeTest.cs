using NUnit.Framework;
using System;

namespace Csg.Test
{
	[TestFixture]
	public class CubeTest : TestWithStlResult
	{
		[Test]
		public void Unit()
		{
			var s = Solids.Cube(1);
			Assert.AreEqual(6, s.Polygons.Count);
			var p0 = s.Polygons[0];
			Assert.GreaterOrEqual(p0.Plane.W, 0.9);
			Assert.LessOrEqual(p0.Plane.W, 1.1);
			AssertAccepted(s, "CubeTest");
		}

		[Test]
		public void BigRadius()
		{
			var s = Solids.Cube(1.0e12);
			var p0 = s.Polygons[0];
			Assert.GreaterOrEqual(p0.Plane.W, 0.9e12);
			Assert.LessOrEqual(p0.Plane.W, 1.1e12);
			AssertAccepted(s, "CubeTest");
		}
	}
}

