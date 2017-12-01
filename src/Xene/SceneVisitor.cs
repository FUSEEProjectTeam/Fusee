using System;
using System.Collections.Generic;
using System.Reflection;
using Fusee.Serialization;
using JSIL.Meta;

namespace Fusee.Xene
{
    /// <summary>
    /// Use this attribute to identify visitor methods. Visitor methods are called during traversal on 
    /// nodes or components with the specified type.
    /// WARNING: Currently no component or node type inheritance matching is done: A Visitor method is
    /// only called if it exaclty matches the type - TODO: fix this...
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VisitMethodAttribute : Attribute
    {
    }

    internal delegate void VisitNodeMethod(SceneVisitor visitor, SceneNodeContainer node);
    internal delegate void VisitComponentMethod(SceneVisitor visitor, SceneComponentContainer component);

    /// <summary>
    /// Static class containing helper methods around the SceneVisitor
    /// </summary>
    public static class SceneVisitorHelpers
    {
        /// <summary>
        /// Returns an enumerator enumerating only one item: the root parameter passed.
        /// This method can be used in situations where an IEnumerator is required (as a method argument)
        /// but only one single node needs to be passed.
        /// </summary>
        /// <param name="root">The root to enumerate.</param>
        /// <returns>An enumerator yielding only only one element: the node passed as root.</returns>
        public static IEnumerator<SceneNodeContainer> SingleRootEnumerator(SceneNodeContainer root)
        {
            yield return root;
        }

        /// <summary>
        /// Returns an enumerable enumerating only one item: the root parameter passed.
        /// This method can be used in situations where an IEnumerator is required (as a method argument)
        /// but only one single node needs to be passed.
        /// </summary>
        /// <param name="root">The root to enumerate.</param>
        /// <returns>An enumerable yielding only only one element: the node passed as root.</returns>
        public static IEnumerable<SceneNodeContainer> SingleRootEnumerable(SceneNodeContainer root)
        {
            yield return root;
        }

    }

    internal class VisitorCallerFactory
    {
        [JSExternal]
        public static VisitComponentMethod MakeComponentVisitor(MethodInfo info)
        {
            return delegate(SceneVisitor visitor, SceneComponentContainer component) { info.Invoke(visitor, new object[] { component }); };
        }

        [JSExternal]
        public static VisitNodeMethod MakeNodeVistor(MethodInfo info)
        {
            return delegate(SceneVisitor visitor, SceneNodeContainer node) { info.Invoke(visitor, new object[] { node }); };
        }

    }


    /// <summary>
    /// This class tries to serve three goals
    /// <list type="number">
    /// <item>As a base class for visitor patterns. Users can add visitor methods and provide code for different types of visited items.</item>
    /// <item>As building block for enumerators. Visitor methods can yield an enumeration.</item>
    /// <item>As a toolset to implement transformations on scenes. Transformations operate on scenes and alter their structure.</item>
    /// </list>
    /// 
    /// Visitors derived from this class may implement
    /// their own Visit methods for all kinds of scene graph elements. Visitor methods can be defined for scene nodes (although many implementations
    /// will most likely NOT have a very big inheritance tree for nodes) as well as for scene components.
    /// A Visitor method can be any instance method (not static) taking one parameter either derived from <see cref="SceneNodeContainer"/> or derived from
    /// <see cref="SceneComponentContainer"/>. To mark such a method as a Visitor method it needs to be decorated with the <see cref="VisitMethodAttribute"/> 
    /// attribute. Visitor methods can have arbitrary names and don't necessarily need to be virtual. 
    /// </summary>
    public class SceneVisitor
    {
        #region Declarative stuff
        internal class VisitorSet
        {
            public Dictionary<Type, VisitNodeMethod> Nodes = new Dictionary<Type, VisitNodeMethod>();
            public Dictionary<Type, VisitComponentMethod> Components = new Dictionary<Type, VisitComponentMethod>();
        }

 
        // The list of visitor methods defined in a concrete child class of SceneVisitor
        private VisitorSet _visitors;

        // The static list of all known sets of visitor methods
        // This is kept to avoid building the visitor map again and again different instances of the same visitor.
        private static Dictionary<Type, VisitorSet> _visitorMap;
        #endregion

        #region Public Traversal Methods
        /// <summary>
        /// Start traversing a scene graph starting with the given root node. Performs a recursive depth-first 
        /// traversal from the speciefiedd root.
        /// </summary>
        /// <param name="rootNode">The root node where to start the traversal.</param>
        public void Traverse(SceneNodeContainer rootNode)
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
        public void Traverse(IEnumerable<SceneNodeContainer> children)
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
        /*
        protected void SetState(IStateStack state)
        {
            if (state == null)
                throw new ArgumentNullException("state", "Always use a State. If necessary use an EmptyState.");
            _state = state;
        }

        private IStateStack _state = new EmptyState();
        */

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
        protected SceneNodeContainer CurrentNode { get; private set; }

        /// <summary>
        /// Returns the currently visited component during a traversal.
        /// </summary>
        /// <value>
        /// The current component.
        /// </value>
        protected SceneComponentContainer CurrentComponent { get; private set; }
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
        protected bool YieldEnumeration {get { return YieldOnCurrentComponent || YieldOnCurrentNode; }}
        private Stack<IEnumerator<SceneNodeContainer>> _nodeEnumeratorStack;
        private IEnumerator<SceneNodeContainer> _curNodeEnumerator;
        private IEnumerator<SceneComponentContainer> _curCompEnumerator;


        /// <summary>
        /// Enumerator Building Block to be called in derived Visitors acting as enumerators. Use this to 
        /// initialize the traversing enumeration on a list of (root) nodes.
        /// </summary>
        /// <param name="nodes">The list of nodes.</param>
        protected void EnumInit(IEnumerator<SceneNodeContainer> nodes)
        {
            if (nodes == null)
                return;

            ScanForVisitors();

            YieldOnCurrentNode = false;
            YieldOnCurrentComponent = false;

            if (_nodeEnumeratorStack == null)
                _nodeEnumeratorStack = new Stack<IEnumerator<SceneNodeContainer>>();
            _nodeEnumeratorStack.Clear();

            InitState();
            nodes.Reset();
            _curNodeEnumerator = nodes;
        }

        /// <summary>
        /// This method implements a re-entrant (in terms of yield, not multi-threading) non-recursive traversal over combined node and component trees.
        /// Call this method in derived classes implementing enumerators, like in the various find extension methods or the <see cref="Viserator{TItem, TState}"/>
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
                        if (CurrentNode.Children == null)
                            PopState();
                    }
                    else
                    {
                        CurrentComponent = _curCompEnumerator.Current;
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
                    CurrentNode = _curNodeEnumerator.Current;
                    PushState();

                    // Prepare to traverse children
                    if (CurrentNode.Children != null)
                    {
                        var childEnumerator = CurrentNode.Children.GetEnumerator();
                        _nodeEnumeratorStack.Push(_curNodeEnumerator);
                        _curNodeEnumerator = childEnumerator;
                    }

                    // Prepare to traverse components
                    if (CurrentNode.Components != null)
                    {
                        _curCompEnumerator = CurrentNode.Components.GetEnumerator();
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
        /// or <see cref="YieldOnCurrentNode"/> to true to signal the enumration to yield.
        /// </summary>
        /// <returns>true if the enumeration is not finnished yet (i.e. if components/nodes are still unvisited). false otherwise.</returns>
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
                CurrentNode = _curNodeEnumerator.Current;
                PushState();

                // Prepare to traverse children
                if (CurrentNode.Children != null)
                {
                    var childEnumerator = CurrentNode.Children.GetEnumerator();
                    _nodeEnumeratorStack.Push(_curNodeEnumerator);
                    _curNodeEnumerator = childEnumerator;
                }

                // Traverse nodes
                DoVisitNode(CurrentNode);

                // If this node hasn't any children, PopState right now. 
                // Otherwise PopState will be called after traversing the children list (see while statement above).
                if (CurrentNode.Children == null)
                    PopState();

                if (YieldOnCurrentNode)
                    return true;
            }
        }
        #endregion


        /// <summary>
        /// Scans the Visitor class (which typically is derived from SceneVisitor) for any methods marked as VisitorMethods.
        /// </summary>
        private void ScanForVisitors()
        {
            if (_visitors != null) 
                return;

            if (_visitorMap == null)
                _visitorMap = new Dictionary<Type, VisitorSet>();

            Type myType = GetType();
            if (_visitorMap.TryGetValue(myType, out _visitors)) 
                return;

            _visitors = new VisitorSet();
            foreach (MethodInfo methodInfo in myType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsVisitor(methodInfo))
                    continue;

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != 1) 
                    continue;

                Type paramType = parameters[0].ParameterType;
                if (typeof(SceneComponentContainer).IsAssignableFrom(paramType))
                {
                    _visitors.Components[paramType] = VisitorCallerFactory.MakeComponentVisitor(methodInfo);
                }
                else if (typeof(SceneNodeContainer).IsAssignableFrom(paramType))
                {
                    _visitors.Nodes[paramType] = VisitorCallerFactory.MakeNodeVistor(methodInfo);
                }
            }
            _visitorMap.Add(myType, _visitors);
        }

        /* Maybe later
        /// <summary>
        /// Use this method in initialization code to add Visit
        /// </summary>
        /// <param name="paramType">The concrete component subtype to register the given method for.</param>
        /// <param name="visitComponentMethod">The method to call when a component of the specified type ist traversed.</param>
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
            object[] a = methodInfo.GetCustomAttributes(typeof(VisitMethodAttribute), false);
            return (a != null && a.Length > 0);
        }

 
        private void DoTraverseNoComponents(SceneNodeContainer node)
        {
            CurrentNode = node;
            PushState();

            DoVisitNode(node);

            // DO NOT visit components

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DoTraverseNoComponents(child);
                }
            }
            PopState();
            CurrentNode = null;
        }

        private void DoTraverse(SceneNodeContainer node)
        {
            CurrentNode = node;
            PushState();
            
            DoVisitNode(node);

            DoVisitComponents(node);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DoTraverse(child);
                }
            }
            PopState();
            CurrentNode = null;
        }

        private void DoVisitComponents(SceneNodeContainer node)
        {
            // Are there any components at all?
            if (node.Components == null)
                return;
            // Visit each component
            foreach (var component in node.Components)
            {
                CurrentComponent = component;
                DoVisitComponent(component);
                CurrentComponent = null;
            }
        }

        private void DoVisitComponent(SceneComponentContainer component)
        {
            VisitComponentMethod visitComponent;
            if (_visitors.Components.TryGetValue(component.GetType(), out visitComponent))
            {
                visitComponent(this, component);
            }
        }

        private void DoVisitNode(SceneNodeContainer node)
        {
            VisitNodeMethod visitNode;
            if (_visitors.Nodes.TryGetValue(node.GetType(), out visitNode))
            {
                visitNode(this, node);
            }
        }
    }
}
