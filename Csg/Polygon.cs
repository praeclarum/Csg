using System;
using System.Collections.Generic;

namespace Csg
{
	public class Polygon
	{
		public readonly Plane Plane;
		public readonly PolygonShared Shared;

		readonly List<Vertex> vertices;

		readonly bool debug = false;

		static readonly PolygonShared defaultShared = new PolygonShared(null);

		BoundingSphere cachedBoundingSphere;
		BoundingBox cachedBoundingBox;

		public Polygon(List<Vertex> vertices, PolygonShared shared, Plane plane)
		{
			this.vertices = vertices;
			this.Shared = shared ?? defaultShared;
			Plane = plane ?? Plane.FromVector3Ds(vertices[0].Pos, vertices[1].Pos, vertices[2].Pos);
			if (debug)
			{
				//CheckIfConvex();
			}
		}

		public BoundingSphere BoundingSphere
		{
			get
			{
				if (cachedBoundingSphere == null)
				{
					var box = BoundingBox;
					var middle = (box.Min + box.Max) * 0.5;
					var radius3 = box.Max - middle;
					var radius = radius3.Length;
					cachedBoundingSphere = new BoundingSphere { Center = middle, Radius = radius };
				}
				return cachedBoundingSphere;
			}
		}

		public BoundingBox BoundingBox
		{
			get
			{
				if (cachedBoundingBox == null)
				{
					Vector3D minpoint, maxpoint;
					var vertices = this.vertices;
					var numvertices = vertices.Count;
					if (numvertices == 0)
					{
						minpoint = new Vector3D(0, 0, 0);
					}
					else {
						minpoint = vertices[0].Pos;
					}
					maxpoint = minpoint;
					for (var i = 1; i < numvertices; i++)
					{
						var point = vertices[i].Pos;
						minpoint = minpoint.Min(point);
						maxpoint = maxpoint.Max(point);
					}
					cachedBoundingBox = new BoundingBox { Min = minpoint, Max = maxpoint };
				}
				return cachedBoundingBox;
			}
		}

		public Polygon Flipped()
		{
			throw new NotImplementedException();
		}
	}

	public class PolygonShared
	{
		public PolygonShared(object color)
		{
		}
		public int Tag;
	}

	public class Vertex
	{
		public Vector3D Pos;
	}

	public class BoundingBox
	{
		public Vector3D Min;
		public Vector3D Max;
	}

	public class BoundingSphere
	{
		public Vector3D Center;
		public double Radius;
	}

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
		public static Plane FromVector3Ds(Vector3D a, Vector3D b, Vector3D c)
		{
			var n = (b - a).Cross(c - a).Unit;
			return new Plane(n, n.Dot(a));
		}
	}

	public class SplitPolygonResult
	{
		public int Type;
		public Polygon Front;
		public Polygon Back;
	}
}

