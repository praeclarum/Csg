using System;
using System.Collections.Generic;
using System.Linq;

namespace Csg
{
	public class Tree
	{
		PolygonTreeNode polygonTree;
		Node rootnode;

		public Tree(IEnumerable<Polygon> polygons)
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

		public void AddPolygons(IEnumerable<Polygon> polygons)
		{
			var polygontreenodes = polygons.Select(p => polygonTree.AddChild(p));
			rootnode.AddPolygonTreeNodes(polygontreenodes.ToList());
		}
	}

	public class Node
	{
		public Plane Plane;
		public Node Front;
		public Node Back;
		public readonly List<PolygonTreeNode> PolygonTreeNodes;
		public readonly Node Parent;

		public Node(Node parent)
		{
			PolygonTreeNodes = new List<PolygonTreeNode>();
			Parent = parent;
		}

		public void Invert()
		{
			var queue = new List<Node> { this };
			for (var i = 0; i < queue.Count; i++)
			{
				var node = queue[i];
				if (node.Plane != null) node.Plane = node.Plane.Flipped();
				if (node.Front != null) queue.Add(node.Front);
				if (node.Back != null) queue.Add(node.Back);
				var temp = node.Front;
				node.Front = node.Back;
				node.Back = temp;
			}
		}

		public void ClipTo(Tree tree, bool alsoRemoveCoplanarFront)
		{
			throw new NotImplementedException();
		}

		public void AddPolygonTreeNodes(List<PolygonTreeNode> addpolygontreenodes)
		{
			var args = new Args { Node = this, PolygonTreeNodes = addpolygontreenodes };
			var stack = new Stack<Args>();
			while (args != null)
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
						polygontreenodes[i].SplitByPlane(_this.Plane, _this.PolygonTreeNodes, backnodes, frontnodes, backnodes);
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
				else args = null;
			}
		}
		class Args
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
		//bool removed;

		public PolygonTreeNode()
		{
			parent = null;
			children = new List<PolygonTreeNode>();
			polygon = null;
			//removed = false;
		}

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

		bool IsRootNode => parent == null;

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
			var queue = new List<List<PolygonTreeNode>> {
			new List<PolygonTreeNode> { this } };
			for (var i = 0; i < queue.Count; i++)
			{
				var children = queue[i];
				for (int j = 0, l = children.Count; j < l; j++)
				{
					var node = children[j];
					if (node.polygon != null)
					{
						result.Add(node.polygon);
					}
					else {
						queue.Add(node.children);
					}
				}
			}
		}

		public void SplitByPlane(Plane plane, List<PolygonTreeNode> coplanarfrontnodes, List<PolygonTreeNode> coplanarbacknodes, List<PolygonTreeNode> frontnodes, List<PolygonTreeNode> backnodes)
		{
			if (children.Count > 0)
			{
				var queue = new List<List<PolygonTreeNode>> { children };
				for (var i = 0; i < queue.Count; i++)
				{
					var nodes = queue[i];
					for (int j = 0, l = nodes.Count; j < l; j++)
					{
						var node = nodes[j];
						if (node.children.Count > 0)
						{
							queue.Add(node.children);
						}
						else {
							node.SplitPolygonByPlane(plane, coplanarfrontnodes, coplanarbacknodes, frontnodes, backnodes);
						}
					}
				}
			}
			else {
				SplitPolygonByPlane(plane, coplanarfrontnodes, coplanarbacknodes, frontnodes, backnodes);
			}
		}

		void SplitPolygonByPlane(Plane plane, List<PolygonTreeNode> coplanarfrontnodes, List<PolygonTreeNode> coplanarbacknodes, List<PolygonTreeNode> frontnodes, List<PolygonTreeNode> backnodes)
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
					frontnodes.Add(this);
				}
				else if (d < -sphereradius)
				{
					backnodes.Add(this);
				}
				else {
					var splitresult = plane.SplitPolygon(polygon);
					switch (splitresult.Type)
					{
						case 0:
							coplanarfrontnodes.Add(this);
							break;
						case 1:
							coplanarbacknodes.Add(this);
							break;
						case 2:
							frontnodes.Add(this);
							break;
						case 3:
							backnodes.Add(this);
							break;
						default:
							if (splitresult.Front != null)
							{
								var frontnode = AddChild(splitresult.Front);
								frontnodes.Add(frontnode);
							}
							if (splitresult.Back != null)
							{
								var backnode = AddChild(splitresult.Back);
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
	}
}

