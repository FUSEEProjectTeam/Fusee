namespace Fusee.Jometri
{
    internal class TriSweepLineStatus : BinarySearchTree<float, TriStatusEdge>
    {
        //Updates the nodes (key and value) according to the sweep lines intersection point with a certain status edge.
        internal void UpdateNodes(Vertex eventPoint)
        {
            foreach (var node in PreorderTraverseTreeNodes())
            {
                node.Value.SetKey(eventPoint);
                node.Key = node.Value.IntersectionPointX;
            }
        }

        //Finds StatusEdge according to its Handle using preorder traversal.
        internal TriStatusEdge FindStatusEdgeWithHandle(int handle)
        {
            foreach (var n in PreorderTraverseTreeValues())
            {
                if (n.HalfEdgeHandle == handle)
                    return n;
            }
            return null;
        }

        //---------- The following methods are only needed because of JSIL's "CompareTo not found in context" error, delete them if this is fixed --------------//
        public new void InsertNode(float key, TriStatusEdge value)
        {
            if (_globalRoot == null)
            {
                _globalRoot = new Node<float, TriStatusEdge>(key, value);
            }
            else
            {
                InsertNode(_globalRoot, key, value);
            }
        }


        private static Node<float, TriStatusEdge> InsertNode(Node<float, TriStatusEdge> root, float key, TriStatusEdge value)
        {
            if (root == null)
            {
                root = new Node<float, TriStatusEdge>(key, value);
            }
            else
            {
                if (key.CompareTo(root.Key) <= 0)
                {
                    root.LeftNode = InsertNode(root.LeftNode, key, value);
                }
                else if (key.CompareTo(root.Key) > 0)//Items with the same value are ignored, use >= to insert them into the three
                {
                    root.RightNode = InsertNode(root.RightNode, key, value);
                }
            }
            return root;
        }

        public new void DeleteNode(float key)
        {
            if (_globalRoot == null) return;
            _globalRoot = DeleteNode(_globalRoot, key);
        }

        private static Node<float, TriStatusEdge> DeleteNode(Node<float, TriStatusEdge> root, float key)
        {
            if (root == null) return null;
            if (key.CompareTo(root.Key) < 0)
                root.LeftNode = DeleteNode(root.LeftNode, key);
            else if (key.CompareTo(root.Key) > 0)
                root.RightNode = DeleteNode(root.RightNode, key);
            else
            {
                if (root.LeftNode == null && root.RightNode == null)
                {
                    return null;
                }
                if (root.LeftNode == null)
                {
                    root = root.RightNode;
                    return root;
                }
                if (root.RightNode == null)
                {
                    root = root.LeftNode;
                    return root;
                }
                var temp = FindMin(root.RightNode);
                root.Value = temp.Value;
                root.Key = temp.Key;
                root.RightNode = DeleteNode(root.RightNode, temp.Key);
            }
            return root;
        }

        public new TriStatusEdge FindLargestSmallerThanInBalanced(float key)
        {
            var res = FindLargestSmallerThanInBalanced(_globalRoot, key);
            return res.Value;
        }

        private static Node<float, TriStatusEdge> FindLargestSmallerThanInBalanced(Node<float, TriStatusEdge> root, float key)
        {
            var res = root;

            while (root != null)
            {
                if (root.Key.CompareTo(key) > 0 || root.Key.CompareTo(key) == 0)
                {
                    root = root.LeftNode;
                }
                else
                {
                    res = root;
                    root = root.RightNode;
                }
            }
            return res;
        }

        public new TriStatusEdge FindNode(float key)
        {
            var res = FindNode(_globalRoot, key);

            return res.Value;
        }

        private static Node<float, TriStatusEdge> FindNode(Node<float, TriStatusEdge> root, float key)
        {
            Node<float, TriStatusEdge> res = null;
            if (root.LeftNode != null)
                res = FindNode(root.LeftNode, key);

            if (key.CompareTo(root.Key) == 0)
                return root;

            if (res == null && root.RightNode != null)
                res = FindNode(root.RightNode, key);

            return res;
        }
        //-----------------------------------------------------------------------------------------------------------------------//
    }
}
