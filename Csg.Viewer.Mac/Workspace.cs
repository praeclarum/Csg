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

		public byte[] Bytes => bytes;
		public string FilePath => path;
		public SCNNode Node => node;

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

		Task CreateNodeAsync(byte[] newBytes, string newPath)
		{
			return Task.Run(() =>
			{
				Csg csg;
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
					csg = new Csg();
				}
				var newNode = csg.ToSCNNode();

				if (path == newPath && Object.ReferenceEquals(bytes, newBytes))
				{
					node = newNode;
					NodeChanged?.Invoke();
				}
			});
		}

		static Csg CreateCsgFromCSharp(byte[] bytes, string path)
		{
			var text = new System.IO.StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd();
			if (string.IsNullOrEmpty(text))
			{
				return new Csg();
			}

			var tree = CSharpSyntaxTree.ParseText(text);
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
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var comp = CSharpCompilation.Create("CsgExec", new[] { tree }, refs, options);
			var asmStream = new System.IO.MemoryStream();
			var e = comp.Emit(asmStream);
			if (e.Success)
			{
				var asm = System.Reflection.Assembly.Load(asmStream.ToArray());
				Console.WriteLine(asm);
				var mainMethod =
					asm.
					GetTypes().
					SelectMany(t => t.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic)).
					FirstOrDefault(m => m.GetParameters().Length == 0 &&
					               m.ReturnType == typeof(Csg));
				if (mainMethod == null)
				{
					throw new Exception("Could not find Main method.");
				}
				else {
					return (Csg)mainMethod.Invoke(null, null);
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

