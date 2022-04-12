using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Jometri
{
    /// <summary>
    /// Represents a node in a (binary) tree.
    /// </summary>
    /// <typeparam name="TK">The key of the node as generic type.</typeparam>
    /// <typeparam name="TV">The payload of the node as generic type.</typeparam>
    public class Node<TK, TV>
    {

        /// <summary>
        /// The key of the node - determines how a node is sorted into the tree.
        /// </summary>
        public TK Key { get; set; }

        /// <summary>
        /// The payload of the node.
        /// </summary>
        public TV Value { get; set; }

        /// <summary>
        /// An item with lower value than the value of this node will become a LeftNode.
        /// </summary>
        public Node<TK, TV> LeftNode;
        /// <summary>
        /// An item with higher value than the value of this node will become a RightNode.
        /// </summary>
        public Node<TK, TV> RightNode;

        /// <summary>
        /// Constructor, creates a new node.
        /// </summary>
        /// <param name="key">The key of the new node.</param>
        /// <param name="value">Payload of the new node.</param>
        public Node(TK key, TV value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Data structure that stores items and allows fast lookup, insertion and deletion.
    /// </summary>
    /// <typeparam name="TK">The type of the tree's key.</typeparam>
    /// <typeparam name="TV">The type of the tree's value.</typeparam>
    public class BinarySearchTree<TK, TV> where TK : IComparable<TK>
    {
        /// <summary>
        /// The root node of the tree.
        /// </summary>
        protected Node<TK, TV> _globalRoot;

        /// <summary>
        /// Inserts a new node in a existing tree.
        /// </summary>
        /// <param name="key">The key of the node. </param>
        /// <param name="value">Value of the node to be inserted into the tree.</param>
        /// <returns></returns>
        public void InsertNode(TK key, TV value)
        {
            if (_globalRoot == null)
            {
                _globalRoot = new Node<TK, TV>(key, value);
            }
            else
            {
                InsertNode(_globalRoot, key, value);
            }
        }

        private static Node<TK, TV> InsertNode(Node<TK, TV> root, TK key, TV value)
        {
            if (root == null)
            {
                root = new Node<TK, TV>(key, value);
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

        /// <summary>
        /// Preorder traversal of the tree. Visits the root, then visits the left sub-tree, after that visits the right sub-tree.
        /// Returns the keys.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TK> PreorderTraverseTreeKeys()
        {
            if (_globalRoot == null) yield break;

            foreach (var node in PreorderTraverseTree(_globalRoot))
                yield return node.Key;
        }

        /// <summary>
        /// Preorder traversal of the tree. Visits the root, then visits the left sub-tree, after that visits the right sub-tree.
        /// Returns the values.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TV> PreorderTraverseTreeValues()
        {
            if (_globalRoot == null) yield break;
            var traverseGlobal = PreorderTraverseTree(_globalRoot).ToList();
            foreach (var node in traverseGlobal)
                yield return node.Value;
        }

        internal IEnumerable<Node<TK, TV>> PreorderTraverseTreeNodes()
        {
            if (_globalRoot == null) yield break;

            foreach (var node in PreorderTraverseTree(_globalRoot))
                yield return node;
        }

        private static IEnumerable<Node<TK, TV>> PreorderTraverseTree(Node<TK, TV> root)
        {
            if (root == null) yield break;
            yield return root;
            foreach (var v in PreorderTraverseTree(root.LeftNode))
            {
                yield return v;
            }

            foreach (var v in PreorderTraverseTree(root.RightNode))
            {
                yield return v;
            }
        }

        /// <summary>
        /// Inorder traversal of the tree.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TK> InOrderTraverseTree()
        {
            if (_globalRoot == null) yield break;

            foreach (var node in InOrderTraverseTree(_globalRoot))
                yield return node.Key;
        }

        private static IEnumerable<Node<TK, TV>> InOrderTraverseTree(Node<TK, TV> root)
        {
            if (root == null) yield break;
            foreach (var v in InOrderTraverseTree(root.LeftNode))
            {
                yield return v;
            }
            yield return root;
            foreach (var v in InOrderTraverseTree(root.RightNode))
            {
                yield return v;
            }
        }

        /// <summary>
        /// Deletes a node from the tree.
        /// </summary>
        /// <param name="key">Key of the node which is to be deleted.</param>
        public void DeleteNode(TK key)
        {
            if (_globalRoot == null) return;
            _globalRoot = DeleteNode(_globalRoot, key);
        }

        private static Node<TK, TV> DeleteNode(Node<TK, TV> root, TK key)
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

        /// <summary>
        /// Traverses the tree to find and return a node with a certain value.
        /// </summary>
        /// <param name="key">The key to be searched for.</param>
        /// <returns></returns>
        public TV FindNode(TK key)
        {
            var res = FindNode(_globalRoot, key);

            return res.Value;
        }

        private static Node<TK, TV> FindNode(Node<TK, TV> root, TK key)
        {
            Node<TK, TV> res = null;
            if (root.LeftNode != null)
                res = FindNode(root.LeftNode, key);

            if (key.CompareTo(root.Key) == 0)
                return root;

            if (res == null && root.RightNode != null)
                res = FindNode(root.RightNode, key);

            return res;
        }

        /// <summary>
        /// Returns the minimum value in the tree.
        /// </summary>
        /// <returns></returns>
        public TV FindMin()
        {
            return FindMin(_globalRoot).Value;
        }

        /// <summary>
        /// Returns the minimum value in the tree, starting at the given node.
        /// </summary>
        /// <param name="root">The node at which to start the search.</param>
        /// <returns>The node containing the minimum value.</returns>
        protected static Node<TK, TV> FindMin(Node<TK, TV> root)
        {
            var current = root;

            while (current.LeftNode != null)
            {
                current = current.LeftNode;
            }
            return current;

        }


        /// <summary>
        /// Balances a given tree.
        /// </summary>
        /// <returns></returns>
        public void BalanceTree()
        {
            var inorder = InOrderTraverseTree(_globalRoot).ToList();
            _globalRoot = BalanceTree(inorder, 0, inorder.Count - 1);
        }

        private static Node<TK, TV> BalanceTree(IList<Node<TK, TV>> inorder, int startIndex, int endIndex)
        {
            if (startIndex > endIndex) return null;

            var middIndex = (startIndex + endIndex) / 2;

            var root = new Node<TK, TV>(inorder[middIndex].Key, inorder[middIndex].Value)
            {
                LeftNode = BalanceTree(inorder, startIndex, middIndex - 1),
                RightNode = BalanceTree(inorder, middIndex + 1, endIndex)
            };

            return root;
        }

        ///<summary>
        /// Finds the value of a node whose key is the largest, smaller than the given.
        /// Only works with a balanced tree. It may be necessary to call BalanceTree before this method.
        /// </summary>
        /// <param name="key">The key that is used as search parameter.</param>
        /// <returns></returns>
        public TV FindLargestSmallerThanInBalanced(TK key)
        {
            var res = FindLargestSmallerThanInBalanced(_globalRoot, key);
            return res.Value;
        }

        private static Node<TK, TV> FindLargestSmallerThanInBalanced(Node<TK, TV> root, TK key)
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
    }
}