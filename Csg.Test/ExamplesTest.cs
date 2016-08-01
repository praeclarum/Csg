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
			var r =
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
			AssertAccepted(r, "OpenJsCadLogo");
		}
	}
}

