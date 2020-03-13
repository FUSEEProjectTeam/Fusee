using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fusee.Xene
{
    /// <summary>
    /// Use this attribute to identify visitor methods. Visitor methods are called during traversal on 
    /// nodes or components with the specified type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class VisitMethodAttribute : Attribute
    {
    }

    internal delegate void VisitNodeMethod<TNode, TComponent>(Visitor<TNode, TComponent> visitor, TNode node) where TNode : class, INode where TComponent : class, IComponent;
    internal delegate void VisitComponentMethod<TNode, TComponent>(Visitor<TNode, TComponent> visitor, TComponent component) where TNode : class, INode where TComponent : class, IComponent;

    /// <summary>
    /// Static class containing helper methods around the Visitor
    /// </summary>
    public static class VisitorHelpers
    {
        /// <summary>
        /// Returns an enumerator enumerating only one item: the root parameter passed.
        /// This method can be used in situations where an IEnumerator is required (as a method argument)
        /// but only one single node needs to be passed.
        /// </summary>
        /// <param name="root">The root to enumerate.</param>
        /// <returns>An enumerator yielding only one element: the node passed as root.</returns>
        public static IEnumerator<TNode> SingleRootEnumerator<TNode>(TNode root)
        {
            yield return root;
        }

        /// <summary>
        /// Returns an enumerable enumerating only one item: the root parameter passed.
        /// This method can be used in situations where an IEnumerator is required (as a method argument)
        /// but only one single node needs to be passed.
        /// </summary>
        /// <param name="root">The root to enumerate.</param>
        /// <returns>An enumerable yielding only one element: the node passed as root.</returns>
        public static IEnumerable<TNode> SingleRootEnumerable<TNode>(TNode root)
        {
            yield return root;
        }

    }

    internal static class VisitorCallerFactory
    {
        public static VisitComponentMethod<TNode, TComponent> MakeComponentVisitor<TNode, TComponent>(MethodInfo info) where TNode : class, INode where TComponent : class, IComponent
        {
            return (Visitor<TNode, TComponent> visitor, TComponent component) => { info.Invoke(visitor, new object[] { component }); };
        }

        public static VisitNodeMethod<TNode, TComponent> MakeNodeVistor<TNode, TComponent>(MethodInfo info) where TNode : class, INode where TComponent : class, IComponent
        {
            return (Visitor<TNode, TComponent> visitor, TNode node) => { info.Invoke(visitor, new object[] { node }); };
        }

    }

    /// <summary>
    /// This class tries to serve three goals
    /// <list type="number">
    /// <item>As a base class for visitor patterns. Users can add visitor methods and provide code for different types of visited items.</item>
    /// <item>As building block for enumerators. Visitor methods can yield an enumeration.</item>
    /// <item>As a tool set to implement transformations on scenes. Transformations operate on scenes and alter their structure.</item>
    /// </list>
    /// 
    /// Visitors derived from this class may implement
    /// their own Visit methods for all kinds of scene graph elements. Visitor methods can be defined for scene nodes (although many implementations
    /// will most likely NOT have a very big inheritance tree for nodes) as well as for scene components.
    /// A Visitor method can be any instance method (not static) taking one parameter either derived from <see cref="INode"/> or derived from
    /// <see cref="IComponent"/>. To mark such a method as a Visitor method it needs to be decorated with the <see cref="VisitMethodAttribute"/> 
    /// attribute. Visitor methods can have arbitrary names and don't necessarily need to be virtual. 
    /// </summary>
    public class Visitor<TNode, TComponent> where TNode : class, INode where TComponent : class, IComponent
    {
        #region Declarative stuff
        internal class VisitorSet
        {
            public Dictionary<Type, VisitNodeMethod<TNode, TComponent>> Nodes = new Dictionary<Type, VisitNodeMethod<TNode, TComponent>>();
            public Dictionary<Type, VisitComponentMethod<TNode, TComponent>> Components = new Dictionary<Type, VisitComponentMethod<TNode, TComponent>>();
        }


        // The list of visitor methods defined in a concrete child class of Visitor
        private VisitorSet _visitors;

        // The static list of all known sets of visitor methods
        // This is kept to avoid building the visitor map again and again different instances of the same visitor.
        private static Dictionary<Type, VisitorSet> _visitorMap;
        #endregion

        #region Public Traversal Methods
        /// <summary>
        /// Start traversing a scene graph starting with the given root node. Performs a recursive depth-first 
        /// traversal from the specified root.
        /// </summary>
        /// <param name="rootNode">The root node where to start the traversal.</param>
        public void Traverse(TNode rootNode)
        {
            if (rootNode == null)
                return;

            ScanForVisitors();
            InitState();

            if (_visitors.Components.Count > 0)
                DoTraverse(rootNode);
            else
                DoTraverseNoComponents(rootNode);
        }

        /// <summary>
        /// Start traversing a list of nodes. Performs a recursive depth-first traversal 
        /// over the list starting with the first node in the list.
        /// </summary>
        /// <param name="children">The list of nodes to traverse over.</param>
        public void Traverse(IEnumerable<TNode> children)
        {
            if (children == null)
                return;

            ScanForVisitors();
            InitState();

            if (_visitors.Components.Count > 0)
            {
                foreach (var node in children)
                {
                    DoTraverse(node);
                }
            }
            else
            {
                foreach (var node in children)
                {
                    DoTraverseNoComponents(node);
                }
            }
        }
        #endregion

        #region Useful Stuff within Traversal  
        
        /// <summary>
        /// Method is called when traversal starts to initialize the traversal state. Override this method in derived classes to initialize any state.
        /// </summary>
        protected virtual void InitState()
        {
            // _state.Clear();
        }

        /// <summary>
        /// Method is called when going down one hierarchy level while traversing. Override this method to push any self-defined state.
        /// </summary>
        protected virtual void PushState()
        {
            // _state.Push();
        }

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing. Override this method to perform pop on any self-defined state.
        /// </summary>
        protected virtual void PopState()
        {
            // _state.Pop();
        }

        /// <summary>
        /// Returns currently visited node during a traversal.
        /// </summary>
        /// <value>
        /// The current node.
        /// </value>
        protected TNode CurrentNode { get; private set; }

        /// <summary>
        /// Returns the currently visited component during a traversal.
        /// </summary>
        /// <value>
        /// The current component.
        /// </value>
        protected TComponent CurrentComponent { get; private set; }

        #endregion

        #region Enumeration Building Blocks
        /// <summary>
        /// Can be called in derived visitors. Set this property to true during traversals to make the visitor yield the current node when used as an enumerator.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the enumeration should yield the current node; otherwise, <c>false</c>.
        /// </value>
        protected bool YieldOnCurrentNode { set; get; }

        /// <summary>
        /// Can be called in derived visitors. Set this property to true during traversals to make the visitor yield the current component when used as an enumerator.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the enumeration should yield the current component; otherwise, <c>false</c>.
        /// </value>
        protected bool YieldOnCurrentComponent { set; get; }

        /// <summary>
        /// Gets a value indicating whether the current enumeration should yield.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the current enumeration should yield; otherwise, <c>false</c>.
        /// </value>
        protected bool YieldEnumeration => YieldOnCurrentComponent || YieldOnCurrentNode;
        private Stack<IEnumerator<INode>> _nodeEnumeratorStack;
        private IEnumerator<INode> _curNodeEnumerator;
        private IEnumerator<IComponent> _curCompEnumerator;


        /// <summary>
        /// Enumerator Building Block to be called in derived Visitors acting as enumerators. Use this to 
        /// initialize the traversing enumeration on a list of (root) nodes.
        /// </summary>
        /// <param name="nodes">The list of nodes.</param>
        protected void EnumInit(IEnumerator<TNode> nodes)
        {
            if (nodes == null)
                return;

            ScanForVisitors();

            YieldOnCurrentNode = false;
            YieldOnCurrentComponent = false;

            if (_nodeEnumeratorStack == null)
                _nodeEnumeratorStack = new Stack<IEnumerator<INode>>();
            _nodeEnumeratorStack.Clear();

            InitState();

            // ===============================================================================================
            // no need to Reset() IEnumerators on start. In Fact, compiler-generated enumerators using yield
            // will throw a NotSupportedException."
            // See https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerator.reset?view=netcore-3.1:            
           // nodes.Reset(); // TODO (MR) -> This is needed for the picking.cs? => CMl??
            
            _curNodeEnumerator = nodes;
        }

        /// <summary>
        /// This method implements a re-entrant (in terms of yield, not multi-threading) non-recursive traversal over combined node and component trees.
        /// Call this method in derived classes implementing enumerators, like in the various find extension methods or the <see cref="Viserator{TItem, TState,TNode,TComponent}"/>
        /// </summary>
        /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of node-component-tree.</returns>
        protected bool EnumMoveNext()
        {
            YieldOnCurrentNode = false;
            YieldOnCurrentComponent = false;

            while (true)
            {
                // Precondition: We are BEFORE the next node or the next component
                if (_curCompEnumerator != null)
                {
                    // Traverse components   
                    if (!_curCompEnumerator.MoveNext())
                    {
                        _curCompEnumerator = null;
                        CurrentComponent = null;

                        // At the end of a Component List: If this node hasn't any children, PopState right now. 
                        // Otherwise PopState will be called after traversing the children list (see below).
                        if (CurrentNode.EnumChildren == null)
                            PopState();
                    }
                    else
                    {
                        CurrentComponent = (TComponent)_curCompEnumerator.Current;
                        DoVisitComponent(CurrentComponent);

                        if (YieldEnumeration)
                            return true;
                    }
                }
                else
                {
                    // Move to next node
                    while (!_curNodeEnumerator.MoveNext())
                    {
                        if (_nodeEnumeratorStack.Count > 0)
                        {
                            _curNodeEnumerator = _nodeEnumeratorStack.Pop();
                            PopState();
                        }
                        else
                        {
                            // Stop entire enumeration - clean up
                            _nodeEnumeratorStack.Clear();
                            _nodeEnumeratorStack = null;
                            _curCompEnumerator = null;
                            _curNodeEnumerator = null;
                            CurrentNode = null;
                            CurrentComponent = null;
                            return false;
                        }
                    }
                    CurrentNode = (TNode)_curNodeEnumerator.Current;
                    PushState();

                    // Prepare to traverse children
                    if (CurrentNode.EnumChildren != null)
                    {
                        var childEnumerator = CurrentNode.EnumChildren.GetEnumerator();
                        _nodeEnumeratorStack.Push(_curNodeEnumerator);
                        _curNodeEnumerator = childEnumerator;
                    }

                    // Prepare to traverse components
                    if (CurrentNode.EnumComponents != null)
                    {
                        _curCompEnumerator = CurrentNode.EnumComponents.GetEnumerator();
                    }

                    // Traverse nodes
                    DoVisitNode(CurrentNode);

                    if (YieldOnCurrentNode)
                        return true;
                }
            }
        }


        /// <summary>
        /// Continue a currently active enumeration. Call all registered Visit methods. Visitor methods may set <see cref="YieldOnCurrentComponent"/>
        /// or <see cref="YieldOnCurrentNode"/> to true to signal the enumeration to yield.
        /// </summary>
        /// <returns>true if the enumeration is not finished yet (i.e. if components/nodes are still unvisited). false otherwise.</returns>
        protected bool EnumMoveNextNoComponent()
        {
            YieldOnCurrentNode = false;
            YieldOnCurrentComponent = false;

            while (true)
            {
                // Precondition: We are BEFORE the next node or the next component
                // Move to next node
                while (!_curNodeEnumerator.MoveNext())
                {
                    if (_nodeEnumeratorStack.Count > 0)
                    {
                        _curNodeEnumerator = _nodeEnumeratorStack.Pop();
                        PopState();
                    }
                    else
                    {
                        // Stop entire enumeration - clean up
                        _nodeEnumeratorStack.Clear();
                        _nodeEnumeratorStack = null;
                        _curCompEnumerator = null;
                        _curNodeEnumerator = null;
                        CurrentNode = null;
                        CurrentComponent = null;
                        return false;
                    }
                }
                CurrentNode = (TNode)_curNodeEnumerator.Current;
                PushState();

                // Prepare to traverse children
                if (CurrentNode.EnumChildren != null)
                {
                    var childEnumerator = CurrentNode.EnumChildren.GetEnumerator();
                    _nodeEnumeratorStack.Push(_curNodeEnumerator);
                    _curNodeEnumerator = childEnumerator;
                }

                // Traverse nodes
                DoVisitNode(CurrentNode);

                // If this node hasn't any children, PopState right now. 
                // Otherwise PopState will be called after traversing the children list (see while statement above).
                if (CurrentNode.EnumChildren == null)
                    PopState();

                if (YieldOnCurrentNode)
                    return true;
            }
        }

        #endregion


        /// <summary>
        /// Scans the Visitor class (which typically is derived from Visitor) for any methods marked as VisitorMethods.
        /// </summary>
        private void ScanForVisitors()
        {
            if (_visitors != null)
                return;

            if (_visitorMap == null)
                _visitorMap = new Dictionary<Type, VisitorSet>();

            var myType = GetType();
            if (_visitorMap.TryGetValue(myType, out _visitors))
                return;

            _visitors = new VisitorSet();
            foreach (var methodInfo in myType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsVisitor(methodInfo))
                    continue;

                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1)
                    continue;

                var paramType = parameters[0].ParameterType;
                if (typeof(IComponent).IsAssignableFrom(paramType))
                {
                    if (_visitors.Components.ContainsKey(paramType)) continue;
                    _visitors.Components.Add(paramType, VisitorCallerFactory.MakeComponentVisitor<TNode, TComponent>(methodInfo));
                }
                else if (typeof(TNode).IsAssignableFrom(paramType))
                {
                    if (_visitors.Nodes.ContainsKey(paramType)) continue;
                    _visitors.Nodes.Add(paramType, VisitorCallerFactory.MakeNodeVistor<TNode, TComponent>(methodInfo));
                }
            }
            _visitorMap.Add(myType, _visitors);
        }

        /* Maybe later (CMl)
        /// <summary>
        /// Use this method in initialization code to add Visit
        /// </summary>
        /// <param name="paramType">The concrete component subtype to register the given method for.</param>
        /// <param name="visitComponentMethod">The method to call when a component of the specified type is traversed.</param>
        protected void AddComponentVisitor(Type paramType, VisitComponentMethod visitComponentMethod)
        {
            _visitors.Components.Add(paramType, visitComponentMethod);
        }

        protected void AddNodeVisitor(Type paramType, VisitNodeMethod visitNodeMethod)
        {
            _visitors.Nodes.Add(paramType, visitNodeMethod);
        }
        */

        private static bool IsVisitor(MethodInfo methodInfo)
        {
            // First try - only available in .Net 4.5
            // Attribute a = methodInfo.GetCustomAttribute(VisitMethodAttribute));
            // return (a != null);

            // Second try - runs on .Net since 2.0. JSIL doesn't implement "Attribute.GetCustomAttribute", however
            // Attribute a = Attribute.GetCustomAttribute(methodInfo, typeof (VisitMethodAttribute));
            // return (a != null);

            // Third try:
            var a = methodInfo.GetCustomAttributes(typeof(VisitMethodAttribute), false);
            return a?.Length > 0;
        }


        private void DoTraverseNoComponents(TNode node)
        {
            CurrentNode = node;
            PushState();

            DoVisitNode(node);

            // DO NOT visit components

            if (node.EnumChildren != null)
            {
                foreach (var child in node.EnumChildren)
                {
                    DoTraverseNoComponents((TNode)child);
                }
            }
            PopState();
            CurrentNode = null;
        }

        private void DoTraverse(TNode node)
        {
            CurrentNode = node;
            PushState();

            DoVisitNode(node);

            DoVisitComponents(node);

            if (node.EnumChildren != null)
            {
                foreach (var child in node.EnumChildren)
                {
                    DoTraverse((TNode)child);
                }
            }
            PopState();
            CurrentNode = null;
        }

        private void DoVisitComponents(TNode node)
        {
            // Are there any components at all?
            if (node?.EnumComponents == null)
                return;
            // Visit each component
            foreach (TComponent component in node.EnumComponents)
            {
                CurrentComponent = component;
                DoVisitComponent(component);
                CurrentComponent = null;
            }
        }

        private void DoVisitComponent(TComponent component)
        {
            var compType = component.GetType();

            if (_visitors.Components.TryGetValue(compType, out var visitComponent))
            {
                // Fast lane: we found a directly matching Visitor. Call it and leave.
                visitComponent(this, component);
            }
            else
            {
                // See if we find a matching Visitor up the inheritance hierarchy of the component's type
                var ancestorType = compType.BaseType;
                while (ancestorType != null)
                {
                    if (_visitors.Components.TryGetValue(ancestorType, out visitComponent))
                    {
                        // Found a handler. Register it for fast handling of future Visits of components with the same type!
                        _visitors.Components[compType] = visitComponent;
                        visitComponent(this, component);
                        return;
                    }
                    // one step up the inheritance hierarchy
                    ancestorType = ancestorType.BaseType;
                }
                // No matching Visitor among the ancestors. Add a dummy
                _visitors.Components[compType] = (th, comp) => { };
            }
        }

        private void DoVisitNode(TNode node)
        {
            var nodeType = node.GetType();

            if (_visitors.Nodes.TryGetValue(nodeType, out var visitNode))
            {
                // Fast lane: we found a directly matching Visitor. Call it and leave.
                visitNode(this, node);
            }
            else
            {
                // See if we find a matching Visitor up the inheritance hierarchy of the nodes's type
                var ancestorType = nodeType.BaseType;
                while (ancestorType != null)
                {
                    if (_visitors.Nodes.TryGetValue(ancestorType, out visitNode))
                    {
                        // Found a handler. Register it for fast handling of future Visits of nodes with the same type!
                        _visitors.Nodes[nodeType] = visitNode;
                        visitNode(this, node);
                        return;
                    }
                    // one step up the inheritance hierarchy
                    ancestorType = ancestorType.BaseType;
                }
                // No matching Visitor among the ancestors. Add a dummy
                _visitors.Nodes[nodeType] = (_, __) => { };
            }
        }
    }
}