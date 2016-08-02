using NUnit.Framework;
using System;

namespace Csg.Test
{
	[TestFixture]
	public class SphereTest : SolidTest
	{
		[Test]
		public void Unit()
		{
			var sphere = Script.Sphere(1);
			Assert.AreEqual(72, sphere.Polygons.Count);
			var p0 = sphere.Polygons[0];
			Assert.GreaterOrEqual(p0.Plane.W, 0.9);
			Assert.LessOrEqual(p0.Plane.W, 1.1);
			AssertAcceptedStl(sphere, "SphereTest");
		}

		[Test]
		public void BigRadius()
		{
			var sphere = Script.Sphere(1.0e12);
			var p0 = sphere.Polygons[0];
			Assert.GreaterOrEqual(p0.Plane.W, 0.9e12);
			Assert.LessOrEqual(p0.Plane.W, 1.1e12);
			AssertAcceptedStl(sphere, "SphereTest");
		}
	}
}

