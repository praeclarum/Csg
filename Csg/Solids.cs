using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg
{
	public static class Solids
	{
		static readonly int[][][] cubeData =
			{
				new[] {
					new[] { 0, 4, 6, 2 },
					new[] { -1, 0, 0 }
				},
				new[] {
					new[] {1, 3, 7, 5},
					new[] {+1, 0, 0}
				},
				new[] {
					new[] {0, 1, 5, 4},
					new[] {0, -1, 0},
				},
				new[] {
					new[] {2, 6, 7, 3},
					new[] { 0, +1, 0}
				},
				new[] {
					new[] {0, 2, 3, 1},
					new[] { 0, 0, -1}
				},
				new[] {
					new[] {4, 5, 7, 6},
					new[] { 0, 0, +1}
				}
			};

		public static Solid Cube(CubeOptions options)
		{
			var c = options.Center;
			var r = options.Radius.Abs; // negative radii make no sense
			var result = Solid.FromPolygons(cubeData.Select(info =>
			{
				//var normal = new CSG.Vector3D(info[1]);
				//var plane = new CSG.Plane(normal, 1);
				var vertices = info[0].Select(i =>
				{
					var pos = new Vector3D(
						c.X + r.X * (2 * ((i & 1) != 0 ? 1 : 0) - 1),
							c.Y + r.Y * (2 * ((i & 2) != 0 ? 1 : 0) - 1),
							c.Z + r.Z * (2 * ((i & 4) != 0 ? 1 : 0) - 1));
					return new Vertex(pos);
				});
				return new Polygon(vertices.ToList());
			}).ToList());
			return result;
		}

		public static Solid Cube(Vector3D radius, Vector3D center)
		{
			return Cube(new CubeOptions { Radius = radius, Center = center });
		}

		public static Solid Cube(double radius, Vector3D center)
		{
			var radius3 = new Vector3D(radius, radius, radius);
			return Cube(new CubeOptions { Radius = radius3, Center = center });
		}

		public static Solid Cube(double radius)
		{
			var center = new Vector3D(0, 0, 0);
			var radius3 = new Vector3D(radius, radius, radius);
			return Cube(new CubeOptions { Radius = radius3, Center = center });
		}

		public static Solid Sphere(SphereOptions options)
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
			var result = Solid.FromPolygons(polygons);
			return result;
		}
		public static Solid Sphere(double radius, int resolution = Solid.DefaultResolution3D)
		{
			return Sphere(new SphereOptions { Radius = radius, Resolution = resolution });
		}
		public static Solid Sphere(double radius, Vector3D center, int resolution = Solid.DefaultResolution3D)
		{
			return Sphere(new SphereOptions { Radius = radius, Center = center, Resolution = resolution });
		}
	}

	public class CubeOptions
	{
		public Vector3D Center;
		public Vector3D Radius = new Vector3D(1, 1, 1);
	}

	public class SphereOptions
	{
		public Vector3D XAxis = new Vector3D(1, 0, 0);
		public Vector3D YAxis = new Vector3D(0, -1, 0);
		public Vector3D ZAxis = new Vector3D(0, 0, 1);
		public Vector3D Center;
		public double Radius = 1;
		public int Resolution = Solid.DefaultResolution3D;
	}
}

