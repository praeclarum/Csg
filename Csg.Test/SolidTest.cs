using System;
using System.Runtime.CompilerServices;
using System.IO;
using NUnit.Framework;

namespace Csg.Test
{
	public class SolidTest
	{
		protected void AssertAcceptedStl(Csg csg, string fixtureName, [CallerMemberName] string testName = "")
		{
			var aname = $"{fixtureName}.{testName}.stl";
			var rname = $"{fixtureName}.{testName}_.stl";
			var asmPath = System.Reflection.Assembly.GetCallingAssembly().Location;
			var repoPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(asmPath)));
			var resultsPath = Path.Combine(repoPath, "Results");
			if (!Directory.Exists(resultsPath))
			{
				Console.WriteLine("Test results could not be found at {0}", resultsPath);
				return;
			}

			var acceptedPath = Path.Combine(resultsPath, aname);
			var rejectedPath = Path.Combine(resultsPath, rname);
			File.Delete(rejectedPath);

			var testStl = csg.ToStlString(testName);

			if (!File.Exists(acceptedPath))
			{
				File.WriteAllText(rejectedPath, testStl);
				Assert.Fail("No results have been marked as accepted.");
			}
			else {
				var acceptedStl = File.ReadAllText(acceptedPath);
				if (testStl != acceptedStl)
				{
					File.WriteAllText(rejectedPath, testStl);
					Assert.Fail("Result differs from accepted.");
				}
			}
		}
	}
}

