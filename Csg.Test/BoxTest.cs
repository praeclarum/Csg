using NUnit.Framework;
using System;
using static Csg.Solids;

namespace Csg.Test
{
	[TestFixture]
	public class BoxTest : SolidTest
	{
		[Test]
		public void Unit()
		{
			var s = Box(1);
			Assert.AreEqual(6, s.Polygons.Count);
			Assert.AreEqual (1, s.BoundingBox.Size.X, 1e-5);
			AssertAcceptedStl(s, "BoxTest");
		}

		[Test]
		public void UnitNonCentered()
		{
			var s = Box(1, center: false);
			Assert.AreEqual(6, s.Polygons.Count);
			Assert.AreEqual (1, s.BoundingBox.Size.X, 1e-5);
			AssertAcceptedStl (s, "BoxTest");
		}

		[Test]
		public void UnitCentered()
		{
			var s = Box(1, center: true);
			Assert.AreEqual(6, s.Polygons.Count);
			var p0 = s.Polygons[0];
			Assert.GreaterOrEqual(p0.Plane.W, 0.4);
			Assert.LessOrEqual(p0.Plane.W, 0.6);
			Assert.AreEqual (1, s.BoundingBox.Size.X, 1e-5);
			AssertAcceptedStl (s, "BoxTest");
		}

		[Test]
		public void BigRadius()
		{
			var s = Box(1.0e12);
			Assert.AreEqual (1.0e12, s.BoundingBox.Size.X, 1e-5);
			AssertAcceptedStl (s, "BoxTest");
		}
	}
}

