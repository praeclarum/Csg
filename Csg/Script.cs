using System;
using System.Linq;

namespace Csg
{
	public static class Script
	{
		public static Csg Cube(double size, bool center = false)
		{
			var r = new Vector3D(size/2, size/2, size/2);
			var c = center ? new Vector3D(0, 0, 0) : r;
			return Solids.Cube(new CubeOptions { Radius = r, Center = c });
		}

		public static Csg Sphere(double r = 1, bool center = false)
		{
			var c = center ? new Vector3D(0, 0, 0) : new Vector3D(r, r, r);
			return Solids.Sphere(new SphereOptions { Radius = r, Center = c });
		}

		public static Csg Union(params Csg[] csgs)
		{
			if (csgs.Length == 0)
			{
				return new Csg();
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

		public static Csg Difference(params Csg[] csgs)
		{
			if (csgs.Length == 0)
			{
				return new Csg();
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

		public static Csg Intersection(params Csg[] csgs)
		{
			if (csgs.Length == 0 || csgs.Length == 1)
			{
				return new Csg();
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

