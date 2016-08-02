using NUnit.Framework;
using System;
using static Csg.Solids;

namespace Csg.Test
{
	[TestFixture]
	public class CylinderTest : SolidTest
	{
		[Test]
		public void Unit()
		{
			var solid = Cylinder(1, 1);
			Assert.AreEqual(36, solid.Polygons.Count);
			AssertAcceptedStl(solid, "CylinderTest");
		}

		[Test]
		public void UnitCentered()
		{
			var solid = Cylinder(1, 1, center: true);
			Assert.AreEqual(36, solid.Polygons.Count);
			var pm = solid.Polygons[solid.Polygons.Count-2];
			Assert.GreaterOrEqual(pm.Plane.W, 0.9);
			Assert.LessOrEqual(pm.Plane.W, 1.1);
			AssertAcceptedStl(solid, "CylinderTest");
		}

		[Test]
		public void BigRadius()
		{
			var solid = Cylinder(1.0e12, 1);
			AssertAcceptedStl(solid, "CylinderTest");
		}

		[Test]
		public void BigRadiusCentered()
		{
			var solid = Cylinder(1.0e12, 1, center: true);
			var pm = solid.Polygons[solid.Polygons.Count-2];
			Assert.GreaterOrEqual(pm.Plane.W, 0.9e12);
			Assert.LessOrEqual(pm.Plane.W, 1.1e12);
			AssertAcceptedStl(solid, "CylinderTest");
		}
	}
}

