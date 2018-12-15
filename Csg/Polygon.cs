using System.Collections.Generic;

namespace Csg
{
	/// <summary>
	/// Convex polygons comprised of vertices lying on a plane.
	/// Each polygon also has "Shared" data which is any
	/// metadata (usually a material reference) that you need to
	/// share between sets of polygons.
	/// </summary>
	public class Polygon
	{
		public readonly List<Vertex> Vertices;
		public readonly Plane Plane;
		public readonly PolygonShared Shared;

		static readonly PolygonShared defaultShared = new PolygonShared(null);

		BoundingSphere? cachedBoundingSphere;
		BoundingBox? cachedBoundingBox;

		public Polygon(List<Vertex> vertices, PolygonShared? shared = null, Plane? plane = null)
		{
			Vertices = vertices;
			Shared = shared ?? defaultShared;
			Plane = plane ?? Plane.FromVector3Ds(vertices[0].Pos, vertices[1].Pos, vertices[2].Pos);
		}

		public Polygon(params Vertex[] vertices)
			: this(new List<Vertex>(vertices))
		{
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

		/// <summary>
		/// Axis-aligned bounding box for this polygon.
		/// </summary>
		public BoundingBox BoundingBox
		{
			get
			{
				if (cachedBoundingBox == null)
				{
					Vector3D minpoint, maxpoint;
					var vertices = this.Vertices;
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
					cachedBoundingBox = new BoundingBox(minpoint, maxpoint);
				}
				return cachedBoundingBox;
			}
		}

		public Polygon Flipped()
		{
			var newvertices = new List<Vertex>(Vertices.Count);
			for (int i = 0; i < Vertices.Count; i++)
			{
				newvertices.Add(Vertices[i].Flipped());
			}
			newvertices.Reverse();
			var newplane = Plane.Flipped();
			return new Polygon(newvertices, Shared, newplane);
		}
	}

	public class PolygonShared
	{
		int tag = 0;
		public int Tag {
			get {
				if (tag == 0) {
					tag = Solid.GetTag ();
				}
				return tag;
			}
		}
		public PolygonShared(object? color)
		{			
		}
		public string Hash
		{
			get
			{
				return "null";
			}
		}
	}
}

