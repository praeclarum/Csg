using System;
using System.Collections.Generic;

namespace Csg
{
	public static class Solids
	{
		public static Csg Sphere(SphereOptions options)
		{
			var center = options.Center;
			var radius = options.Radius;
			var resolution = options.Resolution;
			var xvector = options.XAxis * radius;
			var yvector = options.YAxis * radius;
			var zvector = options.ZAxis * radius;
			if (resolution < 4) resolution = 4;
			var qresolution = resolution / 4;
			var prevcylinderpoint = new Vector3D(0,0,0);
			var polygons = new List<Polygon>();
			for (var slice1 = 0; slice1 <= resolution; slice1++)
			{
				var angle = Math.PI * 2.0 * slice1 / resolution;
				var cylinderpoint = xvector * (Math.Cos(angle)) + (yvector * (Math.Sin(angle)));
				if (slice1 > 0)
				{
					double prevcospitch = 0, prevsinpitch = 0;
					for (var slice2 = 0; slice2 <= qresolution; slice2++)
					{
						var pitch = 0.5 * Math.PI * (double)slice2 / qresolution;
						var cospitch = Math.Cos(pitch);
						var sinpitch = Math.Sin(pitch);
						if (slice2 > 0)
						{
							var vertices = new List<Vertex>();
							vertices.Add(new Vertex(center + (prevcylinderpoint * (prevcospitch) - (zvector * (prevsinpitch)))));
							vertices.Add(new Vertex(center + (cylinderpoint * (prevcospitch) - (zvector * (prevsinpitch)))));
							if (slice2 < qresolution)
							{
								vertices.Add(new Vertex(center + (cylinderpoint * (cospitch) - (zvector * (sinpitch)))));
							}
							vertices.Add(new Vertex(center + (prevcylinderpoint * (cospitch) - (zvector * (sinpitch)))));
							polygons.Add(new Polygon(vertices));
							vertices = new List<Vertex>();
							vertices.Add(new Vertex(center + (prevcylinderpoint * (prevcospitch) + (zvector * (prevsinpitch)))));
							vertices.Add(new Vertex(center + (cylinderpoint * (prevcospitch) + (zvector * (prevsinpitch)))));
							if (slice2 < qresolution)
							{
								vertices.Add(new Vertex(center + (cylinderpoint * (cospitch) + (zvector * (sinpitch)))));
							}
							vertices.Add(new Vertex(center + (prevcylinderpoint * (cospitch) + (zvector * (sinpitch)))));
							vertices.Reverse();
							polygons.Add(new Polygon(vertices));
						}
						prevcospitch = cospitch;
						prevsinpitch = sinpitch;
					}
				}
				prevcylinderpoint = cylinderpoint;
			}
			var result = Csg.FromPolygons(polygons);
			return result;
		}
		public static Csg Sphere(double radius)
		{
			return Sphere(new SphereOptions { Radius = radius });
		}
	}

	public class SphereOptions
	{
		public Vector3D XAxis = new Vector3D(1, 0, 0);
		public Vector3D YAxis = new Vector3D(0, -1, 0);
		public Vector3D ZAxis = new Vector3D(0, 0, 1);
		public Vector3D Center;
		public double Radius = 1;
		public int Resolution = Csg.DefaultResolution3D;
	}
}

