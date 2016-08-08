using System;
using NUnit.Framework;

using Csg;
using static Csg.Solids;

namespace Csg.Test
{
	[TestFixture]
	public class OctreeTest : SolidTest
	{
		[Test]
		public void EmptyUnit3()
		{
			var o = new Octree(new BoundingBox(1, 1, 1), 3);
			Assert.AreEqual(1 + 8 + 8*8 + 8*8*8, o.AllNodes.Count);
		}

	}
}

