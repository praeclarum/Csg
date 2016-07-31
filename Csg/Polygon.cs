using System;

namespace Csg
{
	public class Plane
	{
		public readonly Vector3D Normal;
		public readonly double W;
		int tag = 0;
		public int Tag
		{
			get
			{
				if (tag == 0)
				{
					tag = Csg.GetTag();
				}
				return tag;
			}
		}
		public Plane(Vector3D normal, double w)
		{
			Normal = normal;
			W = w;
		}
		public SplitPolygonResult SplitPolygon(Polygon polygon)
		{
			throw new NotImplementedException();
		}
	}

	public class SplitPolygonResult
	{
		public int Type;
		public Polygon Front;
		public Polygon Back;
	}

	public class Polygon
	{
		public Plane Plane;
		public Shared Shared;

		BoundingSphere cachedBoundingSphere;

		public BoundingSphere BoundingSphere
		{
			get
			{
				if (cachedBoundingSphere == null)
				{
					var box = BoundingBox;
					var middle = (box[0] + box[1]) * 0.5;
					var radius3 = box[1] - middle;
					var radius = radius3.Length;
					cachedBoundingSphere = new BoundingSphere { Center = middle, Radius = radius };
				}
				return cachedBoundingSphere;
			}
		}

		public Vector3D[] BoundingBox
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Polygon Flipped()
		{
			throw new NotImplementedException();
		}
	}

	public class BoundingSphere
	{
		public Vector3D Center;
		public double Radius;
	}
}

