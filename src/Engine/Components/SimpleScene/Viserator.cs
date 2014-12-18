using System.Collections;
using System.Collections.Generic;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{

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

        public static IEnumerable<TResult> Viserate<TViserator, TResult>(this SceneNodeContainer root) where TViserator : ViseratorBase<TResult>, new()
        {
            return new ViseratorEnumerable<TViserator, TResult> { _rootList = SceneVisitorHelpers.SingleRootEnum(root) };
        }

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
    abstract public class ViseratorBase<TItem> : SceneVisitor, IEnumerator<TItem>
    {
        private IEnumerator<SceneNodeContainer> _rootList;

        // unfortunate Two-step instantiation forced by C#'s poor generic constraint system which doesn't allow to constraint a parameter-taking constructor

        // Step 1: Create the instance
        internal ViseratorBase()
        {
        }

        // Step2: initialize the instance
        internal void Init(IEnumerator<SceneNodeContainer> rootList)
        {
            _rootList = rootList;
            EnumInit(_rootList);
        }

        public void Dispose()
        {
            _rootList = null;
        }

        public bool MoveNext()
        {
            return EnumMoveNext();
        }

        public void Reset()
        {
            EnumInit(_rootList);
        }

        public abstract TItem Current { get; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    /// <summary>
    /// A Viserator is a scene visitor which returns an enumerator of a user defined type. Serves as a base class
    /// for use-cases where traversing a node-component-graph should yield a list (enumerator) of results.
    /// </summary>
    /// <typeparam name="TItem">The type of the result yielded by the enumerator.</typeparam>
    /// <typeparam name="TState">The type of the state to use during traversal.</typeparam>
    /// <remarks>
    /// Very often you want to traverse a node-component-graph while maintaining a traversal state keeping track of
    /// inividual values and their changes while traversing. At certain points during the traversal a result arises that
    /// should be promoted to the outside of the traversal. Typically the result is derived from the state at a certain
    /// time during traversal and some additional information of the tree object currently visited.
    /// <para />
    /// To implement your own Viserator you should consider which state information the Viserator shoudl keep track of.
    /// Either you assemble your own State type by deriving from <see cref="VisitorState"/> or choose to use one of 
    /// the standard state types like <see cref="StandardState"/>. Then you need to derive your own class from Viserator<TItem, TState>
    /// with the TState replaced by your choice of State and TItem replaced by the type of the items you want your Viserator to yield
    /// during the traversal.
    /// 
    /// <para />
    /// The word Viserator is a combination of a visitor and and enumerator. Look up "to viscerate" in a dictionary and
    /// judge for yourself if a Viserator's operation resembles disembowelling the innards of a tree structure.
    /// </remarks>
    abstract public class Viserator<TItem, TState> : ViseratorBase<TItem> where TState : IStateStack, new()
    {
        protected TState State;

        internal Viserator()
        {
        }

        protected void Init(IEnumerator<SceneNodeContainer> rootList)
        {
            State = new TState();
            base.Init(rootList);
        }

        public Viserator(IEnumerator<SceneNodeContainer> rootList)
        {
            Init(rootList);
        }

        protected override void InitState()
        {
            State.Clear();
        }

        protected override void PushState()
        {
            State.Push();
        }

        protected override void PopState()
        {
            State.Pop();
        }
    }
}
