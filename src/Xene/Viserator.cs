using System.Collections;
using System.Collections.Generic;
using Fusee.Serialization;

namespace Fusee.Xene
{
    /// <summary>
    /// Contains extensions methods to perform <see cref="Viserator{TItem,TState}"/> actions on trees of scene nodes.
    /// </summary>
    public static class ViseratorExtensions
    {
        // Unfortunate construct, but there seems no other way. What we really needed here is a MixIn to make 
        // a SceneNodeContainer or SceneContainer implement IEnumerable (afterwards). All C# offers us is to 
        // define ExtensionMethods returning an IEnumerable<>.
        // Thus we need some class to implement that. Here it is - tada:
        internal class ViseratorEnumerable<TViserator, TResult> : IEnumerable<TResult> where TViserator : ViseratorBase<TResult>, new()
        {
            internal IEnumerator<SceneNodeContainer> _rootList;

            public IEnumerator<TResult> GetEnumerator()
            {
                var viserator = new TViserator();
                viserator.Init(_rootList);
                return viserator;
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        /// <summary>
        /// Performs a <see cref="Viserator{TItem,TState}"/> action on the specified tree.
        /// </summary>
        /// <typeparam name="TViserator">The type of the viserator.</typeparam>
        /// <typeparam name="TResult">The type of the elements resulting from the Viserate traversal.</typeparam>
        /// <param name="root">The root where to start the traversal.</param>
        /// <returns>All items yielded from within the traversal (see <see cref="ViseratorBase{TItem}.YieldItem"/>).</returns>
        public static IEnumerable<TResult> Viserate<TViserator, TResult>(this SceneNodeContainer root) where TViserator : ViseratorBase<TResult>, new()
        {
            return new ViseratorEnumerable<TViserator, TResult> { _rootList = SceneVisitorHelpers.SingleRootEnumerator(root) };
        }

        /// <summary>
        /// Performs a <see cref="Viserator{TItem,TState}"/> action on the specified list of trees.
        /// </summary>
        /// <typeparam name="TViserator">The type of the viserator.</typeparam>
        /// <typeparam name="TResult">The type of the elements resulting from the Viserate traversal.</typeparam>
        /// <param name="rootList">The list of root items where to start the traversal with.</param>
        /// <returns>All items yielded from within the traversal (see <see cref="ViseratorBase{TItem}.YieldItem"/>).</returns>
        public static IEnumerable<TResult> Viserate<TViserator, TResult>(this IEnumerable<SceneNodeContainer> rootList) where TViserator : ViseratorBase<TResult>, new()
        {
            return new ViseratorEnumerable<TViserator, TResult> { _rootList = rootList.GetEnumerator() };
        }
    }

    /// <summary>
    /// Extract of the final Viserator without everything State related. This class is necessary
    /// to be able to define extension methods on node lists. Do not use this class directly.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public abstract class ViseratorBase<TItem> : SceneVisitor, IEnumerator<TItem>
    {
        private IEnumerator<SceneNodeContainer> _rootList;
        private Queue<TItem> _itemQueue = new Queue<TItem>(1);

        // unfortunate Two-step instantiation forced by C#'s poor generic constraint system which doesn't allow to constraint a parameter-taking constructor
        // Step 1: Create the instance
        /// <summary>
        /// Initializes a new instance of the <see cref="ViseratorBase{TItem}"/> class.
        /// </summary>
        public ViseratorBase()
        {
        }

        // Step2: initialize the instance
        protected internal virtual void Init(IEnumerator<SceneNodeContainer> rootList)
        {
            _rootList = rootList;
            EnumInit(_rootList);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // _rootList = null;
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
            EnumInit(_rootList);
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public TItem Current
        {
            get { return _itemQueue.Dequeue(); }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

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


        /***/
        public class ViseratorInstanceEnumerable : IEnumerable<TItem>
        {
            internal ViseratorBase<TItem> _this;

            public IEnumerator<TItem> GetEnumerator()
            {
                _this.Init(_this._rootList);
                return _this;
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public IEnumerable<TItem> Viserate() 
        {
            return new ViseratorInstanceEnumerable { _this = this };
        }
        /***/
    }

    /// <summary>
    /// A Viserator is a scene visitor which returns an enumerator of a user defined type. Serves as a base class
    /// for use-cases where traversing a node-component-graph should yield a list (enumerator) of results.
    /// </summary>
    /// <typeparam name="TItem">The type of the result yielded by the enumerator.</typeparam>
    /// <typeparam name="TState">The type of the state to use during traversal. See <see cref="VisitorState"/> how to
    ///  implement your own state type.</typeparam>
    /// <remarks>
    /// Very often you want to traverse a node-component-graph while maintaining a traversal state keeping track of
    /// inividual values and their changes while traversing. At certain points during the traversal a result arises that
    /// should be promoted to the outside of the traversal. Typically the result is derived from the state at a certain
    /// time during traversal and some additional information of the tree object currently visited.
    /// <para />
    /// To implement your own Viserator you should consider which state information the Viserator must keep track of.
    /// Either you assemble your own State type by deriving from <see cref="VisitorState"/> or choose to use one of 
    /// the standard state types like <see cref="StandardState"/>. Then you need to derive your own class from 
    /// <see cref="Viserator{TItem,TState}"/>
    /// with the TState replaced by your choice of State and TItem replaced by the type of the items you want your Viserator to yield
    /// during the traversal.
    /// <para />
    /// The word Viserator is a combination of a visitor and and enumerator. Look up "to viscerate" in a dictionary and
    /// judge for yourself if a Viserator's operation resembles disembowelling the innards of a tree structure.
    /// </remarks>
    public abstract class Viserator<TItem, TState> : ViseratorBase<TItem> where TState : IStateStack, new()
    {
        /// <summary>
        /// The state to keep track of during traversal. You can use your own implementation (as long as it implements <see cref="IStateStack"/>
        /// or use one of the pre-defined implementations. See <see cref="VisitorState"/> how to implement your own state type.
        /// </summary>
        protected TState State;

        /// <summary>
        /// Initializes a new instance of the <see cref="Viserator{TItem, TState}"/> class.
        /// </summary>
        public Viserator()
        {
        }

        protected internal override void Init(IEnumerator<SceneNodeContainer> rootList)
        {
            State = new TState();
            base.Init(rootList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viserator{TItem, TState}"/> class.
        /// </summary>
        /// <param name="rootList">The root list.</param>
        public Viserator(IEnumerator<SceneNodeContainer> rootList)
        {
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
