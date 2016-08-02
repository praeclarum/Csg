using System;
using System.Linq;

using Foundation;
using SceneKit;
using AppKit;

namespace Csg.Viewer.Mac
{
	public static class SceneKitHelpers
	{
		public static SCNGeometry ToSCNGeometry(this Csg csg)
		{
			var verts =
				csg.
				Polygons.
				SelectMany(x => x.Vertices).
				Select(ToSCNVector3).
				ToArray();
			var vertsSource = SCNGeometrySource.FromVertices(verts);
			var sources = new[] { vertsSource };
			var triStream = new System.IO.MemoryStream();
			var triWriter = new System.IO.BinaryWriter(triStream);
			var triCount = 0;
			var vi = 0;
			foreach (var p in csg.Polygons)
			{
				for (var i = 2; i < p.Vertices.Count; i++)
				{
					triWriter.Write(vi);
					triWriter.Write(vi + i - 1);
					triWriter.Write(vi + i);
				}
				triCount += p.Vertices.Count - 2;
				vi += p.Vertices.Count;
			}
			triWriter.Flush();
			var triData = NSData.FromArray(triStream.ToArray());
			var elem = SCNGeometryElement.FromData(triData, SCNGeometryPrimitiveType.Triangles, triCount, 4);
			var elements = new[] { elem };

			var g = SCNGeometry.Create(sources, elements);
			g.FirstMaterial.Diffuse.ContentColor = NSColor.Gray;

			return g;
		}

		public static SCNNode ToSCNNode(this Csg csg)
		{
			return SCNNode.FromGeometry(csg.ToSCNGeometry());
		}

		public static SCNVector3 ToSCNVector3(this Vertex v)
		{
			return new SCNVector3((nfloat)v.Pos.X, (nfloat)v.Pos.Y, (nfloat)v.Pos.Z);
		}

		public static SCNVector3 ToSCNVector3(this Vector3D v)
		{
			return new SCNVector3((nfloat)v.X, (nfloat)v.Y, (nfloat)v.Z);
		}
	}
}

