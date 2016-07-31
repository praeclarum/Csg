using System;
using System.Collections.Generic;

namespace Csg
{
	public class Polygon
	{
		public readonly List<Vertex> Vertices;
		public readonly Plane Plane;
		public readonly PolygonShared Shared;

		readonly bool debug = false;

		static readonly PolygonShared defaultShared = new PolygonShared(null);

		BoundingSphere cachedBoundingSphere;
		BoundingBox cachedBoundingBox;

		public Polygon(List<Vertex> vertices, PolygonShared shared = null, Plane plane = null)
		{
			Vertices = vertices;
			Shared = shared ?? defaultShared;
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
					cachedBoundingBox = new BoundingBox { Min = minpoint, Max = maxpoint };
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
		public int Tag;
		public PolygonShared(object color)
		{			
		}
	}

	public class Vertex
	{
		public Vector3D Pos;
		public Vertex(Vector3D pos)
		{
			Pos = pos;
		}
		public Vertex Flipped()
		{
			return this;
		}
		public override string ToString() => Pos.ToString();
	}

	public class BoundingBox
	{
		public Vector3D Min;
		public Vector3D Max;
		public override string ToString() => $"{Min} - {Max}";
	}

	public class BoundingSphere
	{
		public Vector3D Center;
		public double Radius;
	}

	public class Plane : IEquatable<Plane>
	{
		const double EPSILON = 1e-5;

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
		public bool Equals(Plane n)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			return Normal.Equals(n.Normal) && W == n.W;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}
		public Plane Flipped()
		{
			return new Plane(Normal.Negated, -W);
		}
		public SplitPolygonResult SplitPolygon(Polygon polygon)
		{
			var result = new SplitPolygonResult();
			var planenormal = this.Normal;
			var vertices = polygon.Vertices;
			var numvertices = vertices.Count;
			if (polygon.Plane.Equals(this))
			{
				result.Type = 0;
			}
			else {
				var EPS = Plane.EPSILON;
				var thisw = this.W;
				var hasfront = false;
				var hasback = false;
				var vertexIsBack = new List<bool>();
				var MINEPS = -EPS;
				for (var i = 0; i < numvertices; i++)
				{
					var t = planenormal.Dot(vertices[i].Pos) - thisw;
					var isback = (t < 0);
					vertexIsBack.Add(isback);
					if (t > EPS) hasfront = true;
					if (t < MINEPS) hasback = true;
				}
				if ((!hasfront) && (!hasback))
				{
					// all points coplanar
					var t = planenormal.Dot(polygon.Plane.Normal);
					result.Type = (t >= 0) ? 0 : 1;
				}
				else if (!hasback)
				{
					result.Type = 2;
				}
				else if (!hasfront)
				{
					result.Type = 3;
				}
				else {
					// spanning
					result.Type = 4;
					var frontvertices = new List<Vertex>();
					var backvertices = new List<Vertex>();
					var isback = vertexIsBack[0];
					for (var vertexindex = 0; vertexindex < numvertices; vertexindex++)
					{
						var vertex = vertices[vertexindex];
						var nextvertexindex = vertexindex + 1;
						if (nextvertexindex >= numvertices) nextvertexindex = 0;
						var nextisback = vertexIsBack[nextvertexindex];
						if (isback == nextisback)
						{
							// line segment is on one side of the plane:
							if (isback)
							{
								backvertices.Add(vertex);
							}
							else {
								frontvertices.Add(vertex);
							}
						}
						else {
							// line segment intersects plane:
							var point = vertex.Pos;
							var nextpoint = vertices[nextvertexindex].Pos;
							var intersectionpoint = SplitLineBetweenPoints(point, nextpoint);
							var intersectionvertex = new Vertex(intersectionpoint);
							if (isback)
							{
								backvertices.Add(vertex);
								backvertices.Add(intersectionvertex);
								frontvertices.Add(intersectionvertex);
							}
							else {
								frontvertices.Add(vertex);
								frontvertices.Add(intersectionvertex);
								backvertices.Add(intersectionvertex);
							}
						}
						isback = nextisback;
					} // for vertexindex
					// remove duplicate vertices:
					var EPS_SQUARED = Plane.EPSILON * Plane.EPSILON;
					if (backvertices.Count >= 3)
					{
						var prevvertex = backvertices[backvertices.Count - 1];
						for (var vertexindex = 0; vertexindex < backvertices.Count; vertexindex++)
						{
							var vertex = backvertices[vertexindex];
							if (vertex.Pos.DistanceToSquared(prevvertex.Pos) < EPS_SQUARED)
							{
								backvertices.RemoveAt(vertexindex);
								vertexindex--;
							}
							prevvertex = vertex;
						}
					}
					if (frontvertices.Count >= 3)
					{
						var prevvertex = frontvertices[frontvertices.Count - 1];
						for (var vertexindex = 0; vertexindex < frontvertices.Count; vertexindex++)
						{
							var vertex = frontvertices[vertexindex];
							if (vertex.Pos.DistanceToSquared(prevvertex.Pos) < EPS_SQUARED)
							{
								frontvertices.RemoveAt(vertexindex);
								vertexindex--;
							}
							prevvertex = vertex;
						}
					}
					if (frontvertices.Count >= 3)
					{
						result.Front = new Polygon(frontvertices, polygon.Shared, polygon.Plane);
					}
					if (backvertices.Count >= 3)
					{
						result.Back = new Polygon(backvertices, polygon.Shared, polygon.Plane);
					}
				}
			}
			return result;
		}
		Vector3D SplitLineBetweenPoints(Vector3D p1, Vector3D p2)
		{
			var direction = p2 - (p1);
			var labda = (W - Normal.Dot(p1)) / Normal.Dot(direction);
			if (double.IsNaN(labda)) labda = 0;
			if (labda > 1) labda = 1;
			if (labda < 0) labda = 0;
			var result = p1 + (direction * (labda));
			return result;
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

