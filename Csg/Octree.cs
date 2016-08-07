using System;
using System.Collections.Generic;
using System.Linq;

struct Vector3D
{
    public double X, Y, Z;
    public Vector3D(double x, double y, double z) {
        X = x; Y = y; Z = z;
    }
    public static Vector3D operator + (Vector3D a, Vector3D b)
    {
        return new Vector3D(a.X+b.X, a.Y+b.Y, a.Z+b.Z);
    }
    public static Vector3D operator - (Vector3D a, Vector3D b)
    {
        return new Vector3D(a.X-b.X, a.Y-b.Y, a.Z-b.Z);
    }
    public static Vector3D operator * (Vector3D a, Vector3D b)
    {
        return new Vector3D(a.X*b.X, a.Y*b.Y, a.Z*b.Z);
    }
    public static Vector3D operator / (Vector3D a, double b)
    {
        return new Vector3D(a.X/b, a.Y/b, a.Z/b);
    }
    public override string ToString() => $"[{X}, {Y}, {Z}]";
}

struct BoundingBox
{
    public Vector3D Min;
    public Vector3D Max;
    public BoundingBox(Vector3D min, Vector3D max) {
        Min = min;
        Max = max;
    }
    public BoundingBox At(Vector3D position, Vector3D size) {
        return new BoundingBox(position, position + size);
    }
    public BoundingBox(double dx, double dy, double dz) {
        Min = new Vector3D(-dx/2, -dy/2, -dz/2);
        Max = new Vector3D(dx/2, dy/2, dz/2);
    }
    public Vector3D Size => Max - Min;
    public Vector3D Center => (Min + Max) / 2;
    public static BoundingBox operator + (BoundingBox a, Vector3D b)
    {
        return new BoundingBox(a.Min + b, a.Max + b);
    }
    public bool Intersects(BoundingBox b) {
        if (Max.X < b.Min.X) return false;
        if (Max.Y < b.Min.Y) return false;
        if (Max.Z < b.Min.Z) return false;
        if (Min.X > b.Max.X) return false;
        if (Min.Y > b.Max.Y) return false;
        if (Min.Z > b.Max.Z) return false;
        return true;
    }
    public override string ToString() => $"{Center}, s={Size}";
}

class Polygon
{
    public BoundingBox BoundingBox;
    
}

class Octree
{
    public readonly OctreeNode RootNode;
    public static Octree Unit() {
        return new Octree(new BoundingBox(1, 1, 1), 2);
    }
    public Octree(BoundingBox bbox, int maxDepth)
    {
        RootNode = new OctreeNode(bbox, 0, maxDepth);
    }
    public static Octree FromPolygons(List<Polygon> polygons, int maxDepth)
    {
        if (polygons.Count == 0) return new Octree(new BoundingBox(1,1,1), maxDepth);
        var bbox = polygons[0].BoundingBox;
        var o = new Octree(bbox, maxDepth);
        o.RootNode.AddPolygons(polygons);
        return o;
    }
    public List<OctreeNode> AllNodes {
        get {
            var s = new Stack<OctreeNode>();
            s.Push(RootNode);
            var r = new List<OctreeNode>();
            while (s.Count > 0) {
                var n = s.Pop();
                r.Add(n);
                if (n.Children != null) {
                    foreach (var c in n.Children) s.Push(c);
                }
            }
            return r;
        }
    }
}

var xxx = 8*8*8;

class OctreeNode
{
    public readonly BoundingBox BoundingBox;
    public readonly OctreeNode[] Children;
    public readonly List<Polygon> Polygons = new List<Polygon>();
    static readonly Vector3D[] coffsets = new[] {
        new Vector3D(0.25, 0.25, 0.25),
        new Vector3D(-0.25, 0.25, 0.25),
        new Vector3D(-0.25, -0.25, 0.25),
        new Vector3D(0.25, -0.25, 0.25),
        new Vector3D(0.25, 0.25, -0.25),
        new Vector3D(-0.25, 0.25, -0.25),
        new Vector3D(-0.25, -0.25, -0.25),
        new Vector3D(0.25, -0.25, -0.25),
    };
    public OctreeNode(BoundingBox bbox, int depth, int maxDepth)
    {
        BoundingBox = bbox;
        if (depth <= maxDepth) {
            Children = new OctreeNode[8];
            var cbbox = new BoundingBox(bbox.Size.X/2, bbox.Size.X/2, bbox.Size.X/2) +
                bbox.Center;
            for (var i = 0; i < 8; i++) {
                Children[i] = new OctreeNode(cbbox + coffsets[i] * bbox.Size, depth+1, maxDepth);
            }
        }
        else {
            Children = null;
        }
    }
    public void AddPolygon(Polygon polygon)
    {
        this.Polygons.Add(polygon);
        if (Children == null) return;
        var pbox = polygon.BoundingBox;
        for (var i = 0; i < 8; i++) {
            if (Children[i].BoundingBox.Intersects(pbox)) {
                Children[i].AddPolygon(polygon);
            }
        }
    }
    public void AddPolygons(List<Polygon> polygons)
    {
        foreach (var p in polygons) {
            AddPolygon(p);
        }        
    }
    public override string ToString() => $"{Polygons.Count} @ [{BoundingBox}]";
}

var o = Octree.Unit();
o.RootNode.AddPolygon(new Polygon { BoundingBox = new BoundingBox(0.2, 0.2, 0.2) });

var nnn = 1 + 8 + 8*8 + 8*8*8;

