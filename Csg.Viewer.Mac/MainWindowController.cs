using System;

using Foundation;
using SceneKit;
using AppKit;

namespace Csg.Viewer.Mac
{
	public partial class MainWindowController : NSWindowController
	{
		readonly Workspace workspace;
		readonly SCNScene scene = SCNScene.Create();

		SCNNode solidNode = null;

		public MainWindowController(IntPtr handle) : base(handle)
		{
			workspace = new Workspace();
			Initialize();
		}

		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			workspace = new Workspace();
			Initialize();
		}

		public MainWindowController(Workspace workspace) : base("MainWindow")
		{
			this.workspace = workspace;
			Initialize();
		}

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();

			sceneView.Scene = scene;

			workspace.BytesChanged += Workspace_BytesChanged;
		}

		void Initialize()
		{
			var cameraNode = SCNNode.Create();
			cameraNode.Camera = SCNCamera.Create();
			cameraNode.Camera.YFov = 60;
			cameraNode.Camera.ZNear = 1;
			cameraNode.Camera.ZFar = 1000;
			cameraNode.Position = new SCNVector3(0, 0, 300);
			scene.RootNode.Add(cameraNode);
			SetCsg();
		}

		void SetCsg()
		{
			var csg = Solids.Sphere(100);
			if (solidNode != null) solidNode.RemoveFromParentNode();
			solidNode = csg.ToSCNNode();
			scene.RootNode.Add(solidNode);
		}

		void Workspace_BytesChanged()
		{
			SetCsg();
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public new MainWindow Window
		{
			get { return (MainWindow)base.Window; }
		}
	}
}
