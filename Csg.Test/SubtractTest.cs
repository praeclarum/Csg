using NUnit.Framework;
using System;

namespace Csg.Test
{
	[TestFixture]
	public class SubtractTest
	{
		[Test]
		public void UnitSphereSubtractUnitSphere()
		{
			var sphere1 = Solids.Sphere(1, new Vector3D(-0.5, 0, 0));
			var sphere2 = Solids.Sphere(1, new Vector3D(0.5, 0, 0));
			var r = sphere1.Substract(sphere2);
			Assert.AreEqual(84, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
		}
	}
}

