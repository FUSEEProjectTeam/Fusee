using System;
using System.Collections.Generic;

namespace Fusee.Xirkit
{
    /// <summary>
    /// A Circuit contains arbitrary objects. Connections between the objects' members (fields and properties) automatically
    /// handle value propagation - if a source value changes, all the connected members will be set to the updated value.
    /// <para>
    /// This is the main object of the Xirkit package. Compare a Circuit to a real electronic circuit with the electronic
    /// components being instances of objects, their pins welded to the board being instances of <see cref="IInPin"/> 
    /// and <see cref="IOutPin"/> connected to each others by conductive paths. Instead of electricity floating along 
    /// the connections, a Circuit instance transmits values of certain types from node to node. If you are familiar
    /// with CINEMA 4D's XPresso, you know what a Circuit is. You can also compare Xirkit to WPF's Dependency Properties.
    /// </para>
    /// <para>
    /// To build up a Circuit in code, create a new Circuit. For each arbitrary object that should be connected in the Circuit,
    /// create a hosting <see cref="Node"/> instance, passing the participating object to the Node's constructor. Use
    /// <see cref="Circuit.AddNode"/> to add each new node (with the hosted object). Then use <see cref="Node.Attach"/> to connect
    /// members (fields and properties) of objects contained in two different nodes to each others. 
    /// </para>
    /// <para>
    /// Technically a Circuit is a container filled with <see cref="Node"/> instances that are related to each others by connected (in- and out-) pins.
    /// A number of these nodes are specified as root nodes. Whenever a circuit is executed by calling its <see cref="Execute"/> method,
    /// the values of all out-pins at the root nodes are propagated (and possibly converted) the their connected in-pins, 
    /// which in turn might trigger subsequent propagation along the graph. Nodes exposing the <see cref="ICalculationPerformer"/> 
    /// interface are triggered to perform their calculation before the values at their out-pins are further propagated.
    /// </para>
    /// </summary>
    public class Circuit
    {
        private List<Node> _nodeList;
        private List<Node> _rootList;

        /// <summary>
        /// Gets list of nodes within this Circuit
        /// </summary>
        /// <value>
        /// The node list.
        /// </value>
        public List<Node> NodeList { get { return _nodeList; } }
        /// <summary>
        /// Gets list of root nodes within this Circuit. Each root node should additionally be part of the node list.
        /// </summary>
        /// <value>
        /// The root list.
        /// </value>
        public List<Node> RootList { get { return _rootList; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circuit"/> class.
        /// </summary>
        public Circuit()
        {
            _rootList = new List<Node>();
            _nodeList = new List<Node>();
        }


        // TODO: Implement some logic keeping users from identifying roots manually
        /// <summary>
        /// Adds a node to list of root nodes. Root nodes are the starting point of the 
        /// value propagation when a graph is executed (when its <see cref="Execute"/> method is called).
        /// </summary>
        /// <param name="root">A Node object to be added to the list of root nodes.</param>
        public void AddRoot(Node root)
        {
            _rootList.Add(root);
        }

        /// <summary>
        /// Adds node to the circuit. The node only takes part in an execution as long as it is either
        /// also listed as a root node or it has in-pins connected to other nodes in the list.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddNode(Node node)
        {
            _nodeList.Add(node);
        }

        /// <summary>
        /// Resets all nodes within this circuit.
        /// </summary>
        public void Reset()
        {
            foreach (Node node in _nodeList)
                node.Reset();
        }

        /// <summary>
        /// Executes this circuit. 
        /// Propagates the values of all out-pins at the root nodes to the their connected in-pins. If these nodes have out-pins 
        /// connected to subsequent nodes' in-pins, their values will be further propagated. Nodes exposing the 
        /// <see cref="ICalculationPerformer"/> interface are triggered to perform their calculation before 
        /// the values at their out-pins are further propagated.
        /// </summary>
        public void Execute()
        {
            Reset();
            foreach (Node root in _rootList)
                root.Propagate();
        }

        /// <summary>
        /// This method is defunct since a user has no idea of what to specify at "pos".
        /// </summary>
        /// <param name="pos">The position.</param>
        public void DeleteRoot(int pos)
        {
            throw new NotImplementedException("This method is defunct since a user has no idea of what to specify at pos");
            //_rootList[pos].RemoveAllPins();
            //_rootList.RemoveAt(pos);
        }

        /// <summary>
        /// This method is defunct since a user has no idea of what to specify at "pos".
        /// </summary>
        /// <param name="pos">The position.</param>
        public void DeleteNode(int pos)
        {
            throw new NotImplementedException("This method is defunct since a user has no idea of what to specify at pos");
            //_nodeList[pos].RemoveAllPins();
            //_nodeList.RemoveAt(pos);
        }
    }
}
