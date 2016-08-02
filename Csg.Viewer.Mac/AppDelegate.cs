using System;
using System.Linq;

using AppKit;
using Foundation;

namespace Csg.Viewer.Mac
{
	[Register("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public override void DidBecomeActive(NSNotification notification)
		{
			var q = NSApplication.SharedApplication
								 .Windows
								 .Select(x => x.WindowController)
								 .OfType<MainWindowController>();
			foreach (var wc in q)
			{
				wc.Workspace.Reload();
			}
		}
	}
}
