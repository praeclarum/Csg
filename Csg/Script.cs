using System;
using System.Linq;

namespace Csg
{
	public static class Script
	{
		public static Solid Cube(double size, bool center = false)
		{
			var r = new Vector3D(size/2, size/2, size/2);
			var c = center ? new Vector3D(0, 0, 0) : r;
			return Solids.Cube(new CubeOptions { Radius = r, Center = c });
		}

		public static Solid Sphere(double r = 1, bool center = true)
		{
			var c = center ? new Vector3D(0, 0, 0) : new Vector3D(r, r, r);
			return Solids.Sphere(new SphereOptions { Radius = r, Center = c });
		}

		public static Solid Sphere(double r, Vector3D center)
		{
			return Solids.Sphere(new SphereOptions { Radius = r, Center = center });
		}

		public static Solid Union(params Solid[] csgs)
		{
			if (csgs.Length == 0)
			{
				return new Solid();
			}
			else if (csgs.Length == 1)
			{
				return csgs[0];
			}
			else
			{
				var head = csgs[0];
				var rest = csgs.Skip(1).ToArray();
				return head.Union(rest);
			}
		}

		public static Solid Difference(params Solid[] csgs)
		{
			if (csgs.Length == 0)
			{
				return new Solid();
			}
			else if (csgs.Length == 1)
			{
				return csgs[0];
			}
			else
			{
				var head = csgs[0];
				var rest = csgs.Skip(1).ToArray();
				return head.Substract(rest);
			}
		}

		public static Solid Intersection(params Solid[] csgs)
		{
			if (csgs.Length == 0 || csgs.Length == 1)
			{
				return new Solid();
			}
			else
			{
				var head = csgs[0];
				var rest = csgs.Skip(1).ToArray();
				return head.Intersect(rest);
			}
		}
	}
}

