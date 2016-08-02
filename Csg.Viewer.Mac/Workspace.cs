using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using SceneKit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Reflection;

namespace Csg.Viewer.Mac
{
	public class Workspace
	{
		byte[] bytes = new byte[0];
		string path = "";
		SCNNode node = SCNNode.Create();

		public string FilePath => path;
		public SCNNode Node => node;
		public byte[] Bytes => bytes;

		public event Action NodeChanged;

		public Workspace()
		{
			try
			{
				CreateNodeAsync(bytes, path).Wait();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void SetBytes(byte[] newBytes, string newPath)
		{
			this.bytes = newBytes;
			this.path = newPath;

			CreateNodeAsync(newBytes, newPath).ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					Console.WriteLine(t.Exception);
				}
			});
		}

		public void Reload()
		{
			try
			{
				var newBytes = File.ReadAllBytes(path);
				SetBytes(newBytes, path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		Task CreateNodeAsync(byte[] newBytes, string newPath)
		{
			return Task.Run(() =>
			{
				Solid csg;
				if (newBytes.Length > 0)
				{
					switch (Path.GetExtension(newPath).ToLowerInvariant())
					{
						case ".cs":
							csg = CreateCsgFromCSharp(newBytes, newPath);
							break;
						default:
							throw new Exception($"Don't know how to open {newPath}.");
					}
				}
				else {
					csg = new Solid();
				}
				var newNode = csg.ToSCNNode();

				if (path == newPath && Object.ReferenceEquals(bytes, newBytes))
				{
					node = newNode;
					NodeChanged?.Invoke();
				}
			});
		}

		static readonly MetadataReference[] references;

		static Workspace()
		{
			var refs = new List<MetadataReference> {
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Solids).Assembly.Location),
			};
			var facadesDir = Path.Combine(Foundation.NSBundle.MainBundle.ResourcePath, "Facades");
			foreach (var f in Directory.GetFiles(facadesDir, "*.dll"))
			{
				refs.Add(MetadataReference.CreateFromFile(f));
			}
			references = refs.ToArray();
		}

		static Solid CreateCsgFromCSharp(byte[] bytes, string path)
		{
			var text = new System.IO.StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd();
			if (string.IsNullOrEmpty(text))
			{
				return new Solid();
			}

			var tree = CSharpSyntaxTree.ParseText(text);

			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var comp = CSharpCompilation.Create("a" + Guid.NewGuid().ToString("N"), new[] { tree }, references, options);
			var asmStream = new System.IO.MemoryStream();
			var e = comp.Emit(asmStream);
			if (e.Success)
			{
				var asm = System.Reflection.Assembly.Load(asmStream.ToArray());
				var mainMethod =
					asm.
					GetTypes().
					SelectMany(t => t.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic)).
					FirstOrDefault(m => m.GetParameters().Length == 0 &&
					               m.ReturnType == typeof(Solid));
				if (mainMethod == null)
				{
					throw new Exception("Could not find Main method.");
				}
				else {
					var sw = new System.Diagnostics.Stopwatch();
					sw.Start();
					var csg = (Solid)mainMethod.Invoke(null, null);
					sw.Stop();
					Console.WriteLine("CSG with {0} polygons in {1}", csg.Polygons.Count, sw.Elapsed);
					return csg;
				}
			}
			else {
				foreach (var d in e.Diagnostics)
				{
					if (d.Severity == DiagnosticSeverity.Error)
					{
						Console.WriteLine(d);
					}
				}
				throw new Exception("Failed to compile");
			}
		}
	}
}

