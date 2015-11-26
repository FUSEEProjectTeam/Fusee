using System;
using System.Collections;
using System.Collections.Generic;
using Fusee.Serialization;

namespace Fusee.Xene
{
    /// <summary>
    /// Various extensions methods to find nodes or components within trees of scene nodes.
    /// </summary>
    public static class SceneFinderExtensions
    {
        // Unfortunate construct, but there seems no other way. What we really needed here is a MixIn to make 
        // a SceneNodeContainer or SceneContainer implement IEnumerable (afterwards). All C# offers us is to 
        // define ExtensionMethods returning an IEnumerable<>.
        // Thus we need some class to implement that. Here it is - tada:
        internal class SceneNodeEnumerable<TNode> : IEnumerable<TNode> where TNode : SceneNodeContainer
        {
            internal Predicate<TNode> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TNode> GetEnumerator() { return new SceneNodeFinder<TNode>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        internal class SceneComponentEnumerable<TComponent> : IEnumerable<TComponent> where TComponent : SceneComponentContainer
        {
            internal Predicate<TComponent> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TComponent> GetEnumerator() { return new SceneComponentFinder<TComponent>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        internal class SceneNodeWhereComponentEnumerable<TNode, TComponent> : IEnumerable<TNode> where TNode : SceneNodeContainer where TComponent : SceneComponentContainer
        {
            internal Predicate<TComponent> _match;
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TNode> GetEnumerator() { return new SceneNodeWhereComponentFinder<TNode, TComponent>(_rootList, _match); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        #region FindNodes
        /// <summary>
        /// Creates an enumerable traversing the tree starting with the given node.
        /// </summary>
        /// <param name="root">The root node where to start the traversal.</param>
        /// <param name="match">The matching predicate. Enumeration will yield on every matching node.</param>
        /// <returns>An enumerable that can be used in foreach statements.</returns>
        public static IEnumerable<SceneNodeContainer> FindNodes(this SceneNodeContainer root, Predicate<SceneNodeContainer> match)
        {
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds nodes of a certain type and matching the given search predicate within a tree of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes to find.</typeparam>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All nodes of the specified type matching the predicate.</returns>
        public static IEnumerable<TNode> FindNodes<TNode>(this SceneNodeContainer root, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> {_match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root)};
        }

        /// <summary>
        /// Finds nodes matching the given search predicate within a list of trees.
        /// </summary>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All nodes matching the predicate.</returns>
        public static IEnumerable<SceneNodeContainer> FindNodes(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneNodeContainer> match)
        {
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        /// <summary>
        /// Finds nodes of a certain type and matching the given search predicate within a list of trees.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes to find.</typeparam>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All nodes of the specified type matching the predicate.</returns>
        public static IEnumerable<TNode> FindNodes<TNode>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

        #region FindComponents
        /// <summary>
        /// Finds components matching the given search predicate within a trees of nodes.
        /// </summary>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All components matching the predicate.</returns>
        public static IEnumerable<SceneComponentContainer> FindComponents(this SceneNodeContainer root, Predicate<SceneComponentContainer> match) 
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds components of the specified type and matching the given search predicate within a trees of nodes.
        /// </summary>
        /// <typeparam name="TComponent">The type of components to find.</typeparam>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All components of the specified type matching the predicate.</returns>
        public static IEnumerable<TComponent> FindComponents<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds components matching the given search predicate within a list of trees of nodes.
        /// </summary>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All components matching the predicate.</returns>
        public static IEnumerable<SceneComponentContainer> FindComponents(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        /// <summary>
        /// Finds components of the specified type and matching the given search predicate within a list of trees of nodes.
        /// </summary>
        /// <typeparam name="TComponent">The type of components to find.</typeparam>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All components of the specified type matching the predicate.</returns>
        public static IEnumerable<TComponent> FindComponents<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion


        #region FindNodesWhereComponent
        /// <summary>
        /// Finds all nodes containing one or more components matching a given search predicate within a tree of nodes.
        /// </summary>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All nodes containing matching components.</returns>
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this SceneNodeContainer root, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds all nodes containing one or more components matching the specified type and a given search predicate within a tree of nodes.
        /// </summary>
        /// <typeparam name="TComponent">The type of the components to look for.</typeparam>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>
        /// All nodes containing components matching both, type and predicate.
        /// </returns>
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds all nodes of a certain type containing one or more components matching the specified type and a given search predicate within a tree of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of the nodes to search in.</typeparam>
        /// <typeparam name="TComponent">The type of the components to look for.</typeparam>
        /// <param name="root">The root node where to start the search.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>
        /// All nodes of the specified type containing components matching both, type and predicate.
        /// </returns>
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this SceneNodeContainer root, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Finds all nodes containing one or more components matching a given search predicate within a list of trees of nodes.
        /// </summary>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>All nodes containing matching components.</returns>
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        /// <summary>
        /// Finds all nodes containing one or more components matching the specified type and a given search predicate within a list of trees of nodes.
        /// </summary>
        /// <typeparam name="TComponent">The type of the components to look for.</typeparam>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>
        /// All nodes containing components matching both, type and predicate.
        /// </returns>
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        /// <summary>
        /// Finds all nodes of a certain type containing one or more components matching the specified type and a given search predicate within a list of trees of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of the nodes to search in.</typeparam>
        /// <typeparam name="TComponent">The type of the components to look for.</typeparam>
        /// <param name="rootList">The list of root nodes of the trees to search in.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        /// <returns>
        /// All nodes of the specified type containing components matching both, type and predicate.
        /// </returns>
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

    }



    /// <summary>
    /// Allows various searches over scene graphs. 
    /// This class can be used directly but will be more commonly used by calling one 
    /// of the Find extension methods declared in <see cref="SceneFinderExtensions"/>.
    /// </summary>
    /// <remarks>
    /// Search criteria can be passed as (lambda) predicates matching scene nodes.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods. 
    /// </remarks>
    /// <typeparam name="TNode">The concrete type of nodes to look for.</typeparam>
    public class SceneNodeFinder<TNode> : SceneFinderBase<TNode>, IEnumerator<TNode> where TNode : SceneNodeContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneNodeFinder{TNode}"/> class.
        /// </summary>
        /// <param name="rootList">The root list where to start the seach.</param>
        /// <param name="match">The search predicate. Typically specified as a Lambda expression.</param>
        public SceneNodeFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TNode> match) : base(rootList, match) { }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext() { return EnumMoveNextNoComponent();}

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Reflected Viserator Callback. Performs the match test on the specified node.
        /// </summary>
        /// <param name="node">The node to check for the match.</param>
        [VisitMethod]
        protected void MatchNode(TNode node)
        {
            if (_match != null && _match(node))
            {
                YieldOnCurrentNode = true;
            }
        }
        
    }

    /// <summary>
    /// Allows various searches over scene graphs. 
    /// This class can be used directly but will be more commonly used by calling one 
    /// of the Find extension methods declared in <see cref="SceneFinderExtensions"/>.
    /// </summary>
    /// <remarks>
    /// Search criteria can be passed as (lambda) predicates matching scene nodes.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods. 
    /// </remarks>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    public class SceneComponentFinder<TComponent> : SceneFinderBase<TComponent>, IEnumerator<TComponent> where TComponent : SceneComponentContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneComponentFinder{TComponent}"/> class.
        /// </summary>
        /// <param name="rootList">The root list.</param>
        /// <param name="match">The match.</param>
        public SceneComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext() { return EnumMoveNext(); }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public TComponent Current { get { return (TComponent) CurrentComponent; } }
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Reflected Viserator Callback. Performs the match test on the specified component.
        /// </summary>
        /// <param name="component">The component to check for the match.</param>
        [VisitMethod]
        protected void MatchComponent(TComponent component)
        {
            if (_match != null && _match(component))
            {
                YieldOnCurrentComponent = true;
            }
        }
    
    }

    /// <summary>
    /// Allows various searches over scene graphs.
    /// This class can be used directly but will be more commonly used by calling one
    /// of the Find extension methods declared in <see cref="SceneFinderExtensions" />.
    /// </summary>
    /// <typeparam name="TNode">The type of the node.</typeparam>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    /// <remarks>
    /// Search criteria can be passed as (lambda) predicates matching scene nodes.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </remarks>
    public class SceneNodeWhereComponentFinder<TNode, TComponent> : SceneFinderBase<TComponent>, IEnumerator<TNode>
        where TNode : SceneNodeContainer
        where TComponent : SceneComponentContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneNodeWhereComponentFinder{TNode, TComponent}"/> class.
        /// </summary>
        /// <param name="rootList">The root list.</param>
        /// <param name="match">The match.</param>
        public SceneNodeWhereComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext() { return EnumMoveNext(); }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Reflected Viserator Callback. Performs the match test on the specified component.
        /// </summary>
        /// <param name="component">The component to check for the match.</param>
        [VisitMethod]
        protected void MatchComponent(TComponent component)
        {
            if (_match != null && _match(component))
            {
                YieldOnCurrentComponent = true;
            }
        }
    }


    /// <summary>
    /// Base class serving for various searches over scene graphs. See the list of derived classes.
    /// </summary>
    /// <typeparam name="TSceneElementType">The concrete type of the components to look for.</typeparam>
    public class SceneFinderBase<TSceneElementType> : SceneVisitor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected IEnumerator<SceneNodeContainer> _rootList;
        protected Predicate<TSceneElementType> _match;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneFinderBase{TSceneElementType}"/> class.
        /// </summary>
        /// <param name="rootList">The root list.</param>
        /// <param name="match">The match.</param>
        public SceneFinderBase(IEnumerator<SceneNodeContainer> rootList, Predicate<TSceneElementType> match)
        {
            _match = match;
            _rootList = rootList;
            EnumInit(_rootList);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _rootList = null;
            _match = null;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            EnumInit(_rootList);
        }
    }
}
