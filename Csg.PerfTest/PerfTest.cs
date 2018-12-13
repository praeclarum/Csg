using System;
using System.Linq;
using System.Diagnostics;

using static Csg.Solids;

namespace Csg.PerfTest
{
	public class PerfTest
	{
		static void PrimeJit()
		{
			var sphere1 = Sphere(1000, new Vector3D(-500, 0, 0));
			var sphere2 = Sphere(1000, new Vector3D(500, 0, 0));
			var sub = Difference(sphere1, sphere2);
			Debug.Assert(sub.Polygons.Count > 0);
		}

		class TestResult
		{
			public int Resolution;
			public TimeSpan Time;
			public int Polygons;
			public int OutputPolygons;
			public long Compares => (long)Polygons * (long)Polygons;
		}

		static TestResult TestRes(int res)
		{
			var sw = new Stopwatch();
			var sphere1 = Sphere(new SphereOptions { Resolution = res, Radius = 1000, Center = new Vector3D(-500, 0, 0) });
			var sphere2 = Sphere(new SphereOptions { Resolution = res, Radius = 1000, Center = new Vector3D(500, 0, 0) });
			sw.Start();
			var sub = Difference(sphere1, sphere2);
			sw.Stop();
			var subTime = sw.Elapsed;
			return new TestResult
			{
				Resolution = res,
				Polygons = sphere1.Polygons.Count,
				OutputPolygons = sub.Polygons.Count,
				Time = subTime
			};
		}

		static void Test()
		{
			var tests = new[] {
				10,
				50,
				100,
				//110,
				//150,
			};
			var res = tests.Select(TestRes).ToArray();
			Console.WriteLine("Res,Time,Polygons,Compares");
			foreach (var r in res)
			{
				Console.WriteLine("{0},{1},{2},{3}", r.Resolution, r.Time.TotalSeconds, r.Polygons, r.Compares);
			}
		}

		public static void Run()
		{
			//var sphere1 = Sphere(new SphereOptions { Resolution = 1415, Radius = 1000, Center = new Vector3D(-500, 0, 0) });
			//Console.WriteLine("{0}", sphere1.Polygons.Count);
			PrimeJit();
			Test();
		}
	}
}
