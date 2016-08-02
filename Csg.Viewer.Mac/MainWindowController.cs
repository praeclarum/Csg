using System;
using System.Threading.Tasks;

using Foundation;
using SceneKit;
using AppKit;

namespace Csg.Viewer.Mac
{
	public partial class MainWindowController : NSWindowController
	{
		readonly Workspace workspace;
		readonly SCNScene scene = SCNScene.Create();
		readonly SCNNode cameraNode = SCNNode.Create();

		SCNNode solidNode = null;

		public Workspace Workspace => workspace;

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

		void Initialize()
		{			
			cameraNode.Camera = SCNCamera.Create();
			cameraNode.Camera.YFov = 60;
			cameraNode.Camera.ZNear = 1;
			cameraNode.Camera.ZFar = 1000;
			cameraNode.Position = new SCNVector3(0, 0, 300);
			scene.RootNode.Add(cameraNode);
		}

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();

			sceneView.Scene = scene;

			solidNode = workspace.Node;

			scene.RootNode.Add(solidNode);

			workspace.NodeChanged += Workspace_NodeChanged;
		}

		void Workspace_NodeChanged()
		{
			BeginInvokeOnMainThread(() =>
			{
				solidNode.RemoveFromParentNode();
				solidNode = workspace.Node;
				var c = SCNVector3.Zero;
				nfloat r = 1.0f;
				solidNode.GetBoundingSphere(ref c, ref r);
				scene.RootNode.Add(solidNode);
				cameraNode.Camera.YFov = 60;
				cameraNode.Position = new SCNVector3(0, 0, (float)r * 2.5f);
			});
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
