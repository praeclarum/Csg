using NUnit.Framework;
using System;

using static Csg.Script;

namespace Csg.Test
{
	[TestFixture]
	public class ExamplesTest : TestWithStlResult
	{
		[Test]
		public void OpenJsCadLogo()
		{
			Union(
				Difference(
					Cube(size: 3, center: true),
					Sphere(r: 2, center: true)
				),
				Intersection(
					Sphere(r: 1.3, center: true),
					Cube(size: 2.1, center: true)
				)
			).Translate(0, 0, 1.5).Scale(10);
		}

		[Test]
		public void UnitSphere_NoOverlap_UnitSphere()
		{
			var sphere1 = Solids.Sphere(1, new Vector3D(-50, 0, 0));
			var sphere2 = Solids.Sphere(1, new Vector3D(50, 0, 0));
			var r = sphere1.Substract(sphere2);
			Assert.AreEqual(72, r.Polygons.Count);
			Assert.IsTrue(r.IsCanonicalized);
			Assert.IsTrue(r.IsRetesselated);
			AssertAccepted(r, "SubtractTest");
		}
	}
}

