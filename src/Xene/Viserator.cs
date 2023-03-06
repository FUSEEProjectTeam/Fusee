﻿using Fusee.Engine.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusee.Xene
{
    /// <summary>
    /// Contains extensions methods to perform <see cref="Viserator{TItem,TState,TNode,TComponent}"/> actions on trees of scene nodes.
    /// </summary>
    public static class ViseratorExtensions
    {
        // Unfortunate construct, but there seems no other way. What we really needed here is a MixIn to make
        // a INode or SceneContainer implement IEnumerable (afterwards). All C# offers us is to
        // define ExtensionMethods returning an IEnumerable<>.
        // Thus we need some class to implement that:
        internal class ViseratorEnumerable<TViserator, TResult, TNode, TComponent> : IEnumerable<TResult>
            where TViserator : ViseratorBase<TResult, TNode, TComponent>, new()
            where TNode : class, INode
            where TComponent : class, IComponent
        {
            internal IEnumerable<TNode> _rootList;

            public IEnumerator<TResult> GetEnumerator()
            {
                var viserator = new TViserator();
                viserator.Init(_rootList);
                return viserator;
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        /// <summary>Performs a <see cref="Viserator{TItem,TState,TNode,TComponent}"/> action on the specified tree.</summary>
        /// <typeparam name="TViserator">The type of the viserator.</typeparam>
        /// <typeparam name="TResult">The type of the elements resulting from the Viserate traversal.</typeparam>
        /// <typeparam name="TNode">The root type of nodes the tree (given by root) is built from.</typeparam>
        /// <typeparam name="TComponent">The root type of components used in the given tree.</typeparam>
        /// <param name="root">The root where to start the traversal.</param>
        /// <returns>All items yielded from within the traversal (<see cref="ViseratorBase{TItem,TNode,TComponent}.YieldItem"/>).</returns>
        public static IEnumerable<TResult> Viserate<TViserator, TResult, TNode, TComponent>(this TNode root)
            where TViserator : ViseratorBase<TResult, TNode, TComponent>, new()
            where TNode : class, INode
            where TComponent : class, IComponent
        {
            return new ViseratorEnumerable<TViserator, TResult, TNode, TComponent> { _rootList = VisitorHelpers.SingleRootEnumerable(root) };
        }

        /// <summary>
        /// Performs a <see cref="Viserator{TItem,TState,TNode,TComponent}"/> action on the specified list of trees.
        /// </summary>
        /// <typeparam name="TViserator">The type of the viserator.</typeparam>
        /// <typeparam name="TResult">The type of the elements resulting from the Viserate traversal.</typeparam>
        /// <typeparam name="TNode">The root type of nodes the tree (given by root) is built from.</typeparam>
        /// <typeparam name="TComponent">The root type of components used in the given tree.</typeparam>
        /// <param name="rootList">The list of root items where to start the traversal with.</param>
        /// <returns>All items yielded from within the traversal (see <see cref="ViseratorBase{TItem,TNode,TComponent}.YieldItem"/>).</returns>
        public static IEnumerable<TResult> Viserate<TViserator, TResult, TNode, TComponent>(this IEnumerable<TNode> rootList)
            where TViserator : ViseratorBase<TResult, TNode, TComponent>, new()
            where TNode : class, INode
            where TComponent : class, IComponent
        {
            return new ViseratorEnumerable<TViserator, TResult, TNode, TComponent> { _rootList = rootList };
        }
    }

    /// <summary>
    /// Extract of the final Viserator without everything State related. This class is necessary
    /// to be able to define extension methods on node lists. Do not use this class directly.
    /// </summary>
    /// <typeparam name="TItem">The type of the result list items to be returned by a Viserator traversal.</typeparam>
    /// <typeparam name="TNode">The base type of nodes making up the tree.</typeparam>
    /// <typeparam name="TComponent">The base type of the components the tree is made up of.</typeparam>
    public abstract class ViseratorBase<TItem, TNode, TComponent> : Visitor<TNode, TComponent>, IEnumerator<TItem>
        where TNode : class, INode
        where TComponent : class, IComponent
    {
        private IEnumerable<TNode> _rootList;
        private bool _disposed;
        private readonly Queue<TItem> _itemQueue = new(1);

        // unfortunate Two-step instantiation forced by C#'s poor generic constraint system which doesn't allow to constraint a parameter-taking constructor
        // Step 1: Create the instance
        /// <summary>
        /// Initializes a new instance of the <see cref="ViseratorBase{TItem,TNode,TComponent}"/> class.
        /// </summary>
        public ViseratorBase()
        {
        }

        // Step2: initialize the instance
        /// <summary>
        /// Initializes this instance with the specified tree.
        /// </summary>
        /// <param name="rootList">The tree to traverse.</param>
        protected internal virtual void Init(IEnumerable<TNode> rootList)
        {
            _rootList = rootList;
            EnumInit(_rootList.GetEnumerator());
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_itemQueue.Count > 0)
                return true;
            return EnumMoveNext();
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            EnumInit(_rootList.GetEnumerator());
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public TItem Current => _itemQueue.Dequeue();

        object IEnumerator.Current => Current;

        /// <summary>
        /// Call this method in visitor methods of derived Viserator classes to signal that an item should
        /// be propagated to the traversal's result enumeration.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void YieldItem(TItem item)
        {
            YieldOnCurrentComponent = true;
            _itemQueue.Enqueue(item);
        }


        private class ViseratorInstanceEnumerable : IEnumerable<TItem>
        {
            internal ViseratorBase<TItem, TNode, TComponent> _this;

            public IEnumerator<TItem> GetEnumerator()
            {
                _this.Init(_this._rootList);
                return _this;
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        /// <summary>
        /// Start the traversal on the tree hosted by this instance.
        /// </summary>
        /// <returns>The items yielded during iteration.</returns>
        public IEnumerable<TItem> Viserate()
        {
            return new ViseratorInstanceEnumerable { _this = this };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// A Viserator is a scene visitor which returns an enumerator of a user defined type. Serves as a base class
    /// for use-cases where traversing a node-component-graph should yield a list (enumerator) of results.
    /// </summary>
    /// <typeparam name="TItem">The type of the result yielded by the enumerator.</typeparam>
    /// <typeparam name="TState">The type of the state to use during traversal. See <see cref="VisitorState"/> how to
    ///  implement your own state type.</typeparam>
    /// <typeparam name="TNode">The root type of nodes the trees passed to the constructor is built from.</typeparam>
    /// <typeparam name="TComponent">The root type of components used in the given trees.</typeparam>
    /// <remarks>
    /// Very often you want to traverse a node-component-graph while maintaining a traversal state keeping track of
    /// individual values and their changes while traversing. At certain points during the traversal a result arises that
    /// should be promoted to the outside of the traversal. Typically the result is derived from the state at a certain
    /// time during traversal and some additional information of the tree object currently visited.
    /// <para />
    /// To implement your own Viserator you should consider which state information the Viserator must keep track of.
    /// Either you assemble your own State type by deriving from <see cref="VisitorState"/> or choose to use one of
    /// the standard state types like <see cref="StandardState"/>. Then you need to derive your own class from
    /// <see cref="Viserator{TItem,TState,TNode,TComponent}"/>
    /// with the TState replaced by your choice of State and TItem replaced by the type of the items you want your Viserator to yield
    /// during the traversal.
    /// <para />
    /// The word Viserator is a combination of a visitor and enumerator. Look up "to viscerate" in a dictionary and
    /// judge for yourself if a Viserator's operation resembles disemboweling the innards of a tree structure.
    /// </remarks>
    public abstract class Viserator<TItem, TState, TNode, TComponent> : ViseratorBase<TItem, TNode, TComponent>
        where TState : IStateStack, new()
        where TNode : class, INode
        where TComponent : class, IComponent
    {
        /// <summary>
        /// The state to keep track of during traversal. You can use your own implementation (as long as it implements <see cref="IStateStack"/>
        /// or use one of the predefined implementations. See <see cref="VisitorState"/> how to implement your own state type.
        /// </summary>
        protected TState State;

        /// <summary>
        /// Initializes a new instance of the <see cref="Viserator{TItem, TState,TNode,TComponent}"/> class.
        /// </summary>
        public Viserator()
        {
        }

        /// <summary>
        /// Initializes this viserator using the specified root list.
        /// </summary>
        /// <param name="rootList">The tree to traverse.</param>
        protected internal override void Init(IEnumerable<TNode> rootList)
        {
            State = new TState();
            base.Init(rootList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viserator{TItem, TState,TNode,TComponent}"/> class.
        /// </summary>
        /// <param name="rootList">The root list.</param>
        /// <param name="customVisitorModules">Optional custom <see cref="IVisitorModule"/>. Needs to be passed to base.ctor,
        /// as the initialization and discovery of potential methods to visit is being done in <see cref="Init(IEnumerable{TNode})"/> </param>
        protected Viserator(IEnumerable<TNode> rootList, IEnumerable<IVisitorModule> customVisitorModules = null)
        {
            if (customVisitorModules != null)
                VisitorModules.AddRange(customVisitorModules);
            Init(rootList);
        }

        /// <summary>
        /// This method is called when traversal starts to initialize the traversal state. Override this method in derived classes to initialize any state beyond
        /// the standard <see cref="State" />.
        /// Most visitors will simply initialize the <see cref="State" /> and will thus NOT need to override this method. Make sure to call the base
        /// implementation in overridden versions.
        /// </summary>
        protected override void InitState()
        {
            State.Clear();
        }

        /// <summary>
        /// This method  is called when going down one hierarchy level while traversing. Pushes the traversal state stack <see cref="State" />.
        /// Most visitors will simply push the <see cref="State" /> and will thus NOT need to override this method. Make sure to call the base
        /// implementation in overridden versions.
        /// </summary>
        protected override void PushState()
        {
            State.Push();
        }

        /// <summary>
        /// This method  is called when going up one hierarchy level while traversing. Pops the traversal state stack <see cref="State" />.
        /// Most visitors will simply perform pop on the <see cref="State" /> and will thus NOT need to override this method. Make sure to call the base
        /// implementation in overridden versions.
        /// </summary>
        protected override void PopState()
        {
            State.Pop();
        }
    }
}