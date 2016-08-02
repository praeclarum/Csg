using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace Csg.Viewer.Mac
{
	[Register("WorkspaceDocument")]
	public class WorkspaceDocument : NSDocument
	{
		readonly Workspace workspace = new Workspace();

		public WorkspaceDocument()
		{
		}

		public WorkspaceDocument(IntPtr handle)
			: base(handle)
		{
		}

		[Export("autosavesInPlace")]
		static bool GetAutosavesInPlace() => true;

		public override NSData GetAsData(string typeName, out NSError outError)
		{
			var nsdata = NSData.FromArray(workspace.Bytes);
			outError = null;
			return nsdata;
		}

		public override bool ReadFromData(NSData data, string typeName, out NSError outError)
		{
			workspace.SetBytes(data.ToArray(), FileUrl.Path);
			outError = null;
			return true;
		}

		public override void MakeWindowControllers()
		{
			var wc = new MainWindowController(workspace);
			AddWindowController(wc);
		}
	}
}

