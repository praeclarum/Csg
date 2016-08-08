using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg
{
	public class Tree
	{
		PolygonTreeNode polygonTree;
		Node rootnode;

		public Node RootNode => rootnode;

		public Tree(BoundingBox bbox, List<Polygon> polygons)
		{
			polygonTree = new PolygonTreeNode();
			rootnode = new Node(null);
			if (polygons != null) AddPolygons(polygons);
		}

		public void Invert()
		{
			polygonTree.Invert();
			rootnode.Invert();
		}

		public void ClipTo(Tree tree, bool alsoRemoveCoplanarFront = false)
		{
			rootnode.ClipTo(tree, alsoRemoveCoplanarFront);
		}

		public List<Polygon> AllPolygons()
		{
			var result = new List<Polygon>();
			polygonTree.GetPolygons(result);
			return result;
		}

		public void AddPolygons(List<Polygon> polygons)
		{
			var n = polygons.Count;
			var polygontreenodes = new List<PolygonTreeNode>(n);
			for (var i = 0; i < n; i++)
			{
				var p = polygonTree.AddChild(polygons[i]);
				polygontreenodes.Add(p);
			}
			rootnode.AddPolygonTreeNodes(polygontreenodes);
		}
	}

	public class Node
	{
		public Plane Plane;
		public Node Front;
		public Node Back;
		public List<PolygonTreeNode> PolygonTreeNodes;
		public readonly Node Parent;

		public Node(Node parent)
		{
			PolygonTreeNodes = new List<PolygonTreeNode>();
			Parent = parent;
		}

		public void Invert()
		{
			var queue = new Queue<Node>();
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				if (node.Plane != null) node.Plane = node.Plane.Flipped();
				if (node.Front != null) queue.Enqueue(node.Front);
				if (node.Back != null) queue.Enqueue(node.Back);
				var temp = node.Front;
				node.Front = node.Back;
				node.Back = temp;
			}
		}

		public void ClipPolygons(List<PolygonTreeNode> clippolygontreenodes, bool alsoRemoveCoplanarFront)
		{
			var args = new Args { Node = this, PolygonTreeNodes = clippolygontreenodes };
			Stack<Args> stack = null;

			while (args.Node != null)
			{
				var node = args.Node;
				var polygontreenodes = args.PolygonTreeNodes;

				if (node.Plane != null)
				{
					List<PolygonTreeNode> backnodes = null;
					List<PolygonTreeNode> frontnodes = null;
					var plane = node.Plane;
					var numpolygontreenodes = polygontreenodes.Count;
					for (var i = 0; i < numpolygontreenodes; i++)
					{
						var node1 = polygontreenodes[i];
						if (!node1.IsRemoved)
						{
							if (alsoRemoveCoplanarFront)
							{
								node1.SplitByPlane(plane, ref backnodes, ref backnodes, ref frontnodes, ref backnodes);
							}
							else
							{
								node1.SplitByPlane(plane, ref frontnodes, ref backnodes, ref frontnodes, ref backnodes);
							}
						}
					}

					if (node.Front != null && (frontnodes != null))
					{
						if (stack == null) stack = new Stack<Args>();
						stack.Push(new Args { Node = node.Front, PolygonTreeNodes = frontnodes });
					}
					var numbacknodes = backnodes == null ? 0 : backnodes.Count;
					if (node.Back != null && (numbacknodes > 0))
					{
						if (stack == null) stack = new Stack<Args>();
						stack.Push(new Args { Node = node.Back, PolygonTreeNodes = backnodes });
					}
					else {
						// there's nothing behind this plane. Delete the nodes behind this plane:
						for (var i = 0; i < numbacknodes; i++)
						{
							backnodes[i].Remove();
						}
					}
				}
				if (stack != null && stack.Count > 0) args = stack.Pop();
				else args.Node = null;
			}
		}

		public void ClipTo(Tree tree, bool alsoRemoveCoplanarFront)
		{
			var node = this;
			Stack<Node> stack = null;
			while (node != null)
			{
				if (node.PolygonTreeNodes.Count > 0)
				{
					tree.RootNode.ClipPolygons(node.PolygonTreeNodes, alsoRemoveCoplanarFront);
				}
				if (node.Front != null)
				{
					if (stack == null) stack = new Stack<Node>();
					stack.Push(node.Front);
				}
				if (node.Back != null)
				{
					if (stack == null) stack = new Stack<Node>();
					stack.Push(node.Back);
				}
				node = (stack != null && stack.Count > 0) ? stack.Pop() : null;
			}
		}

		public void AddPolygonTreeNodes(List<PolygonTreeNode> addpolygontreenodes)
		{
			var args = new Args { Node = this, PolygonTreeNodes = addpolygontreenodes };
			var stack = new Stack<Args>();
			while (args.Node != null)
			{
				var node = args.Node;
				var polygontreenodes = args.PolygonTreeNodes;

				if (polygontreenodes.Count == 0)
				{
					// Nothing to do
				}
				else {
					var _this = node;
					if (node.Plane == null)
					{
						var bestplane = polygontreenodes[0].GetPolygon().Plane;
						node.Plane = bestplane;
					}

					var frontnodes = new List<PolygonTreeNode>();
					var backnodes = new List<PolygonTreeNode>();

					for (int i = 0, n = polygontreenodes.Count; i < n; i++)
					{
						polygontreenodes[i].SplitByPlane(_this.Plane, ref _this.PolygonTreeNodes, ref backnodes, ref frontnodes, ref backnodes);
					}

					if (frontnodes.Count > 0)
					{
						if (node.Front == null) node.Front = new Node(node);
						stack.Push(new Args { Node = node.Front, PolygonTreeNodes = frontnodes });
					}
					if (backnodes.Count > 0)
					{
						if (node.Back == null) node.Back = new Node(node);
						stack.Push(new Args { Node = node.Back, PolygonTreeNodes = backnodes });
					}
				}

				if (stack.Count > 0) args = stack.Pop();
				else args.Node = null;
			}
		}
		struct Args
		{
			public Node Node;
			public List<PolygonTreeNode> PolygonTreeNodes;
		}
	}

	public class PolygonTreeNode
	{
		PolygonTreeNode parent;
		List<PolygonTreeNode> children;
		Polygon polygon;
		bool removed;

		public PolygonTreeNode()
		{
			parent = null;
			children = new List<PolygonTreeNode>();
			polygon = null;
			removed = false;
		}

		public BoundingBox BoundingBox => polygon?.BoundingBox;

		public void AddPolygons(List<Polygon> polygons)
		{
			if (!IsRootNode)
			{
				throw new InvalidOperationException("New polygons can only be added to  root nodes.");
			}
			for (var i = 0; i < polygons.Count; i++)
			{
				AddChild(polygons[i]);
			}
		}

		public void Remove()
		{
			if (!this.removed)
			{
				this.removed = true;

#if DEBUG
				if (this.IsRootNode) throw new InvalidOperationException("Can't remove root node");
				if (this.children.Count > 0) throw new InvalidOperationException("Can't remove nodes with children");
#endif

				// remove ourselves from the parent's children list:
				var parentschildren = this.parent.children;
				var i = parentschildren.IndexOf(this);
				if (i < 0) throw new InvalidOperationException("Child to be removed not found in parent");
				parentschildren.RemoveAt(i);

				// invalidate the parent's polygon, and of all parents above it:
				this.parent.RecursivelyInvalidatePolygon();
			}
		}

		public bool IsRemoved => removed;

		public bool IsRootNode => parent == null;

		public void Invert()
		{
			if (!IsRootNode) throw new InvalidOperationException("Only the root nodes are invertable.");
			InvertSub();
		}

		public Polygon GetPolygon()
		{
			if (polygon == null) throw new InvalidOperationException("Node is not associated with a polygon.");
			return this.polygon;
		}

		public void GetPolygons(List<Polygon> result)
		{
			var queue = new Queue<List<PolygonTreeNode>>();
			queue.Enqueue(new List<PolygonTreeNode> { this });
			while (queue.Count > 0)
			{
				var children = queue.Dequeue();
				var l = children.Count;
				for (int j = 0; j < l; j++)
				{
					var node = children[j];
					if (node.polygon != null)
					{
						result.Add(node.polygon);
					}
					else {
						queue.Enqueue(node.children);
					}
				}
			}
		}

		public void SplitByPlane(Plane plane, ref List<PolygonTreeNode> coplanarfrontnodes, ref List<PolygonTreeNode> coplanarbacknodes, ref List<PolygonTreeNode> frontnodes, ref List<PolygonTreeNode> backnodes)
		{
			if (children.Count > 0)
			{
				var queue = new Queue<List<PolygonTreeNode>>();
				queue.Enqueue(children);
				while (queue.Count > 0)
				{
					var nodes = queue.Dequeue();
					var l = nodes.Count;
					for (int j = 0; j < l; j++)
					{
						var node = nodes[j];
						if (node.children.Count > 0)
						{
							queue.Enqueue(node.children);
						}
						else {
							node.SplitPolygonByPlane(plane, ref coplanarfrontnodes, ref coplanarbacknodes, ref frontnodes, ref backnodes);
						}
					}
				}
			}
			else {
				SplitPolygonByPlane(plane, ref coplanarfrontnodes, ref coplanarbacknodes, ref frontnodes, ref backnodes);
			}
		}

		void SplitPolygonByPlane(Plane plane, ref List<PolygonTreeNode> coplanarfrontnodes, ref List<PolygonTreeNode> coplanarbacknodes, ref List<PolygonTreeNode> frontnodes, ref List<PolygonTreeNode> backnodes)
		{
			var polygon = this.polygon;
			if (polygon != null)
			{
				var bound = polygon.BoundingSphere;
				var sphereradius = bound.Radius + 1.0e-4;
				var planenormal = plane.Normal;
				var spherecenter = bound.Center;
				var d = planenormal.Dot(spherecenter) - plane.W;
				if (d > sphereradius)
				{
					if (frontnodes == null) frontnodes = new List<PolygonTreeNode>();
					frontnodes.Add(this);
				}
				else if (d < -sphereradius)
				{
					if (backnodes == null) backnodes = new List<PolygonTreeNode>();
					backnodes.Add(this);
				}
				else {
					SplitPolygonResult splitresult;
					plane.SplitPolygon(polygon, out splitresult);
					switch (splitresult.Type)
					{
						case 0:
							if (coplanarfrontnodes == null) coplanarfrontnodes = new List<PolygonTreeNode>();
							coplanarfrontnodes.Add(this);
							break;
						case 1:
							if (coplanarbacknodes == null) coplanarbacknodes = new List<PolygonTreeNode>();
							coplanarbacknodes.Add(this);
							break;
						case 2:
							if (frontnodes == null) frontnodes = new List<PolygonTreeNode>();
							frontnodes.Add(this);
							break;
						case 3:
							if (backnodes == null) backnodes = new List<PolygonTreeNode>();
							backnodes.Add(this);
							break;
						default:
							if (splitresult.Front != null)
							{
								var frontnode = AddChild(splitresult.Front);
								if (frontnodes == null) frontnodes = new List<PolygonTreeNode>();
								frontnodes.Add(frontnode);
							}
							if (splitresult.Back != null)
							{
								var backnode = AddChild(splitresult.Back);
								if (backnodes == null) backnodes = new List<PolygonTreeNode>();
								backnodes.Add(backnode);
							}
							break;
					}
				}
			}
		}

		public PolygonTreeNode AddChild(Polygon polygon)
		{
			var newchild = new PolygonTreeNode();
			newchild.parent = this;
			newchild.polygon = polygon;
			children.Add(newchild);
			return newchild;
		}

		void InvertSub()
		{
			var queue = new List<List<PolygonTreeNode>> {
				new List<PolygonTreeNode> { this }
			};
			for (var i = 0; i < queue.Count; i++)
			{
				var children = queue[i];
				var l = children.Count;
				for (int j = 0; j < l; j++)
				{
					var node = children[j];
					if (node.polygon != null)
					{
						node.polygon = node.polygon.Flipped();
					}
					queue.Add(node.children);
				}
			}
		}

		void RecursivelyInvalidatePolygon()
		{
			var node = this;
			while (node.polygon != null)
			{
				node.polygon = null;
				if (node.parent != null)
				{
					node = node.parent;
				}
			}
		}
	}
}

