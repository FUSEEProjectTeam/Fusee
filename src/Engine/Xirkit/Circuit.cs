using System;
using System.Collections.Generic;

namespace Fusee.Xirkit
{
    public class Circuit
    {
        private List<Node> _nodeList;
        private List<Node> _rootList;

        public List<Node> NodeList { get { return _nodeList; } }
        public List<Node> RootList { get { return _rootList; } }

        public Circuit()
        {
            _rootList = new List<Node>();
            _nodeList = new List<Node>();
        }


        // TODO: Implement some logic keeping users from identifying roots manually
        public void AddRoot(Node root)
        {
            _rootList.Add(root);
        }

        public void AddNode(Node node)
        {
            _nodeList.Add(node);
        }

        public void Reset()
        {
            foreach (Node node in _nodeList)
                node.Reset();
        }

        public void Execute()
        {
            Reset();
            foreach (Node root in _rootList)
                root.Propagate();
        }

        public void DeleteRoot(int pos)
        {
            _rootList[pos].RemoveAllPins();
            _rootList.RemoveAt(pos);
        }

        public void DeleteNode(int pos)
        {
            _nodeList[pos].RemoveAllPins();
            _nodeList.RemoveAt(pos);
        }
    }
}
