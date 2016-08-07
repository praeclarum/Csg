using NUnit.Framework;
using System;
using static Csg.Solids;

namespace Csg.Test
{
	[TestFixture]
	public class UnionTest : SolidTest
	{
		[Test]
		public void UnitSphere_UnitSphere()
		{
			var sphere1 = Sphere(1, new Vector3D(-0.5, 0, 0));
			var sphere2 = Sphere(1, new Vector3D(0.5, 0, 0));
			var r = sphere1.Union(sphere2);
			Assert.AreEqual(136, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
			AssertAcceptedStl(r, "UnionTest");
		}

		[Test]
		public void UnitSphere_NoOverlap_UnitSphere()
		{
			var sphere1 = Sphere(1, new Vector3D(-50, 0, 0));
			var sphere2 = Sphere(1, new Vector3D(50, 0, 0));
			var r = sphere1.Union(sphere2);
			Assert.AreEqual(144, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
			AssertAcceptedStl(r, "UnionTest");
		}

		[Test]
		public void CoplanarExact()
		{
			var solid1 = Cube(4, new Vector3D(-2, 0, 0));
			var solid2 = Cube(4, new Vector3D(2, 0, 0));
			var r = solid1.Union(solid2);
			Assert.AreEqual(6, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
			AssertAcceptedStl(r, "UnionTest");
		}

		[Test]
		public void CoplanarInset()
		{
			var solid1 = Cube(4, new Vector3D(-2, 0, 0));
			var solid2 = Cube(2, new Vector3D(1, 0, 0));
			var r = solid1.Union(solid2);
			Assert.AreEqual(14, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
			AssertAcceptedStl(r, "UnionTest");
		}
	}
}

