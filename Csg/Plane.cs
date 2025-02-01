using System;
using System.Collections.Generic;

namespace Csg
{
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
                    tag = Solid.GetTag();
                }
                return tag;
            }
        }
        public Plane(Vector3D normal, double w)
        {
            Normal = normal;
            W = w;
        }
        public bool Equals(Plane? otherPlane)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            return otherPlane is {} n ? Normal.Equals(n.Normal) && W == n.W : false;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
        }
        public Plane Flipped()
        {
            return new Plane(Normal.Negated, -W);
        }
        public unsafe void SplitPolygon(Polygon polygon, out SplitPolygonResult result)
        {
            result = new SplitPolygonResult();
            var planenormal = this.Normal;
            var vertices = polygon.Vertices;
            var numvertices = vertices.Count;
            if (polygon.Plane.Equals(this))
            {
                result.Type = 0;
            }
            else
            {
                var EPS = Plane.EPSILON;
                var thisw = this.W;
                var hasfront = false;
                var hasback = false;
                var vertexIsBack = stackalloc bool[numvertices];
                var MINEPS = -EPS;
                for (var i = 0; i < numvertices; i++)
                {
                    var t = planenormal.Dot(vertices[i].Pos) - thisw;
                    var isback = (t < 0);
                    vertexIsBack[i] = isback;
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
                else
                {
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
                            else
                            {
                                frontvertices.Add(vertex);
                            }
                        }
                        else
                        {
                            // line segment intersects plane:
                            var intersectionvertex = SplitLineBetweenVertices(vertex, vertices[nextvertexindex]);
                            if (isback)
                            {
                                backvertices.Add(vertex);
                                backvertices.Add(intersectionvertex);
                                frontvertices.Add(intersectionvertex);
                            }
                            else
                            {
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
        }
        Vertex SplitLineBetweenVertices(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos;
            var p2 = v2.Pos;
            var direction = p2 - (p1);
            var u = (W - Normal.Dot(p1)) / Normal.Dot(direction);
            if (double.IsNaN(u)) u = 0;
            if (u > 1) u = 1;
            if (u < 0) u = 0;
            var result = p1 + (direction * (u));
            var tresult = v1.Tex + (v2.Tex - v1.Tex) * u;
            return new Vertex(result, tresult);
        }
        public static Plane FromVector3Ds(Vector3D a, Vector3D b, Vector3D c)
        {
            var n = (b - a).Cross(c - a).Unit;
            return new Plane(n, n.Dot(a));
        }
        public Plane Transform(Matrix4x4 matrix4x4)
        {
            var ismirror = matrix4x4.IsMirroring;
            // get two vectors in the plane:
            var r = this.Normal.RandomNonParallelVector();
            var u = this.Normal.Cross(r);
            var v = this.Normal.Cross(u);
            // get 3 points in the plane:
            var point1 = this.Normal * (this.W);
            var point2 = point1 + (u);
            var point3 = point1 + (v);
            // transform the points:
            point1 = point1 * (matrix4x4);
            point2 = point2 * (matrix4x4);
            point3 = point3 * (matrix4x4);
            // and create a new plane from the transformed points:
            var newplane = Plane.FromVector3Ds(point1, point2, point3);
            if (ismirror)
            {
                // the transform is mirroring
                // We should mirror the plane:
                newplane = newplane.Flipped();
            }
            return newplane;
        }
    }

	public struct SplitPolygonResult
	{
		public int Type;
		public Polygon Front;
		public Polygon Back;
	}
}

