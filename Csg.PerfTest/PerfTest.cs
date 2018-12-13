using System;
using System.Linq;
using System.Diagnostics;

using static Csg.Solids;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Csg.PerfTest
{
	[MemoryDiagnoser]
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

		static readonly Stopwatch sw = new Stopwatch ();

		static TestResult TestRes(int res)
		{
			var st = sw.Elapsed;
			var sphere1 = Sphere(new SphereOptions { Resolution = res, Radius = 1000, Center = new Vector3D(-500, 0, 0) });
			var sphere2 = Sphere(new SphereOptions { Resolution = res, Radius = 1000, Center = new Vector3D(500, 0, 0) });
			var sub = Difference(sphere1, sphere2);
			var et = sw.Elapsed;
			var subTime = et - st;
			return new TestResult
			{
				Resolution = res,
				Polygons = sphere1.Polygons.Count,
				OutputPolygons = sub.Polygons.Count,
				Time = subTime
			};
		}

		static TestResult Test (int res)
		{
			TestRes (res);
			var r0 = TestRes (res);
			var r1 = TestRes (res);
			var r2 = TestRes (res);
			r2.Time = TimeSpan.FromMilliseconds ((r0.Time.TotalMilliseconds + r1.Time.TotalMilliseconds + r2.Time.TotalMilliseconds) / 3);
			return r2;
		}

		[Benchmark(Baseline = true)]
		public void Resolution10 () => TestRes (10);

		[Benchmark]
		public void Resolution50 () => TestRes (50);

		public static void Run()
		{
			sw.Start ();

#if __MOBILE__
			PrimeJit ();
			var tests = new[] {
				10,
				50,
				//100,
				//110,
				//150,
			};
			var res = tests.Select(Test).ToArray();
			Console.WriteLine("Res,Time (ms),Polygons,Compares");
			foreach (var r in res)
			{
				Console.WriteLine("{0},{1:##0.000},{2},{3}", r.Resolution, r.Time.TotalMilliseconds, r.Polygons, r.Compares);
			}
#else
			BenchmarkRunner.Run<PerfTest> ();
#endif
		}
	}
}
