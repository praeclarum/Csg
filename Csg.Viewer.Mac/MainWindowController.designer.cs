// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Csg.Viewer.Mac
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		SceneKit.SCNView sceneView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (sceneView != null) {
				sceneView.Dispose ();
				sceneView = null;
			}
		}
	}
}
