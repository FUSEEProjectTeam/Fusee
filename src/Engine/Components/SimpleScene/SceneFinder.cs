using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Fusee.Math;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
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
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<TNode> FindNodes<TNode>(this SceneNodeContainer root, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> {_match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root)};
        }

        public static IEnumerable<SceneNodeContainer> FindNodes(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneNodeContainer> match)
        {
            return new SceneNodeEnumerable<SceneNodeContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }

        public static IEnumerable<TNode> FindNodes<TNode>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TNode> match) where TNode : SceneNodeContainer
        {
            return new SceneNodeEnumerable<TNode> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

        #region FindComponents
        public static IEnumerable<SceneComponentContainer> FindComponents(this SceneNodeContainer root, Predicate<SceneComponentContainer> match) 
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<TComponent> FindComponents<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<SceneComponentContainer> FindComponents(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneComponentEnumerable<SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<TComponent> FindComponents<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneComponentEnumerable<TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

        #region FindNodesWhereComponent
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this SceneNodeContainer root, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this SceneNodeContainer root, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this SceneNodeContainer root, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent(this IEnumerable<SceneNodeContainer> rootList, Predicate<SceneComponentContainer> match)
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, SceneComponentContainer> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<SceneNodeContainer> FindNodesWhereComponent<TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match) where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<SceneNodeContainer, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        public static IEnumerable<TNode> FindNodesWhereComponent<TNode, TComponent>(this IEnumerable<SceneNodeContainer> rootList, Predicate<TComponent> match)
            where TNode : SceneNodeContainer
            where TComponent : SceneComponentContainer
        {
            return new SceneNodeWhereComponentEnumerable<TNode, TComponent> { _match = match, _rootList = rootList.GetEnumerator() };
        }
        #endregion

    }



    /// <summary>
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene nodes.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TNode">The concrete type of nodes to look for.</typeparam>
    public class SceneNodeFinder<TNode> : SceneFinderBase<TNode>, IEnumerator<TNode> where TNode : SceneNodeContainer
    {
        public SceneNodeFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TNode> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNextNoComponent();}

        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

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
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene components.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    public class SceneComponentFinder<TComponent> : SceneFinderBase<TComponent>, IEnumerator<TComponent> where TComponent : SceneComponentContainer
    {
        public SceneComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNext(); }

        public TComponent Current { get { return (TComponent) CurrentComponent; } }
        object IEnumerator.Current { get { return Current; } }

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
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates matching scene components.
    /// Results are returned as enumerator. Instead of directly using this class, users should use one of the FindNodes or
    /// FindComponents extension methods.
    /// </summary>
    /// <typeparam name="TComponent">The concrete type of the components to look for.</typeparam>
    public class SceneNodeWhereComponentFinder<TNode, TComponent> : SceneFinderBase<TComponent>, IEnumerator<TNode>
        where TNode : SceneNodeContainer
        where TComponent : SceneComponentContainer
    {
        public SceneNodeWhereComponentFinder(IEnumerator<SceneNodeContainer> rootList, Predicate<TComponent> match) : base(rootList, match) { }

        public bool MoveNext() { return EnumMoveNext(); }

        public TNode Current { get { return (TNode) CurrentNode; } }
        object IEnumerator.Current { get { return Current; } }

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
    /// Allows various searches over scene graphs. Search criteria can be passed as (lambda) predicates.
    /// Results are returned as enumerator.
    /// </summary>
    public class SceneFinderBase<TSceneElementType> : SceneVisitor
    {
        protected IEnumerator<SceneNodeContainer> _rootList;
        protected Predicate<TSceneElementType> _match;

        public SceneFinderBase(IEnumerator<SceneNodeContainer> rootList, Predicate<TSceneElementType> match)
        {
            _match = match;
            _rootList = rootList;
            EnumInit(_rootList);
        }

        public void Dispose()
        {
            _rootList = null;
            _match = null;
        }

        public void Reset()
        {
            EnumInit(_rootList);
        }
    }
}
