using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
    /// <summary>
    /// Defines the minimum set of operations on any data structure to be used as a stack during traversal. The main difference
    /// between StateStacks and "normal" stacks (<see cref="Stack{T}"/>) is that the Push operation here doesn't take an argument
    /// but simply replicates the current state (or rather - memorizes the current state for restoring it later with Push). 
    /// </summary>
    public interface IStateStack
    {
        /// <summary>
        /// The stack's Push operation. Increases the stack Depth about one and copies the top of stack.
        /// </summary>
        void Push();

        /// <summary>
        /// The stack's Pop operation. Decreases the stack Depth about one and restores the previous state.
        /// </summary>
        void Pop();
        
        /// <summary>
        /// Clears the stack. The Depth will be reset to zero.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Retrieves the stack's depth.
        /// </summary>
        /// <value>
        /// The current depth of the stack. 
        /// </value>
        int Depth { get; }
    }

    /// <summary>
    /// A simple implementation of the <see cref="IStateStack"/> interface. Defines the <see cref="Tos"/> property granting read and write access to the current Top of stack.
    /// </summary>
    /// <typeparam name="T">The type of the entries stored within the stack.</typeparam>
    public class StateStack<T> : IStateStack
    {
        private T[] _array;
        private int _top;
        private const int _defaultCapacity = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the stack. This is the expected maximum stack depth. 
        /// If the stack depth grows bigger, the stack automatically doubles its capacity internally.</param>
        public StateStack(int capacity = _defaultCapacity)
        {
            _array = new T[capacity];
            _top = 0;
        }

        /// <summary>
        /// Gets or sets the Top of stack.
        /// </summary>
        /// <value>
        /// The top of stack.
        /// </value>
        public T Tos
        {
            set { _array[_top] = value; }
            get { return _array[_top]; }
        }

        /// <summary>
        /// The stack's Push operation. Increases the stack Depth about one and copies the top of stack.
        /// </summary>
        public void Push()
        {
            int size = _top + 1;
            if (size == _array.Length)
            {
                T[] objArray = new T[2 * _array.Length];
                Array.Copy(_array, 0, objArray, 0, size);
                _array = objArray;
            }
            _array[size] = _array[_top];
            _top++;
        }

        /// <summary>
        /// The stack's Pop operation. Decreases the stack Depth about one and restores the previous state.
        /// </summary>
        public void Pop()
        {
            _array[_top--] = default(T);
        }

        /// <summary>
        /// Clears the stack. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_array, 0, _top+1);
            _top = 0;
        }

        /// <summary>
        /// Retrieves the stack's depth.
        /// </summary>
        /// <value>
        /// The current depth of the stack.
        /// </value>
        public int Depth { get { return _top + 1; } }
    }


    /// <summary>
    /// An <see cref="IStateStack"/> implementation behaving better in situations where many subsequent Push (and Pop) operations occur
    /// without actually altering the Tos contents. 
    /// </summary>
    /// <remarks>
    /// Using instances of this class is recommended if the Type parameter is a large value type.
    /// Defines the <see cref="Tos"/> property granting read and write access the current Top of stack.
    /// <typeparam name="T">The type of the entries stored within the stack.</typeparam>
    /// </remarks>
    public class CollapsingStateStack<T> : IStateStack
    {
        private const int _defaultCapacity = 4;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsingStateStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the stack. This is the expected maximum stack depth. 
        /// If the stack depth grows bigger, the stack automatically doubles its capacity internally.</param>
        public CollapsingStateStack(int capacity = _defaultCapacity)
        {
            _impStack = new StateStack<T>(capacity);
            _countStack = new StateStack<int>(capacity);
        }

        private readonly StateStack<T> _impStack;
        private readonly StateStack<int> _countStack;
        
        /// <summary>
        /// The stack's Push operation. Increases the stack Depth about one and copies the top of stack.
        /// </summary>
        public void Push()
        {
            Depth++;
            _countStack.Tos++;
        }
        
        /// <summary>
        /// The stack's Pop operation. Decreases the stack Depth about one and restores the previous state.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">CollapsingState Stack depth is already 0. Cannot Pop stack.</exception>
        public void Pop()
        {
            if (Depth <= 0)
                throw new InvalidOperationException("CollapsingState Stack depth is already 0. Cannot Pop stack.");

            Depth--;
            if (_countStack.Tos > 1)
            {
                _countStack.Tos--;
            }
            else
            {
                _countStack.Pop();
                _impStack.Pop();
            }
        }
        
        /// <summary>
        /// Clears the stack. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            _impStack.Clear();
            _countStack.Clear();
            Depth = 0;
        }
        
        /// <summary>
        /// Retrieves the stack's depth.
        /// </summary>
        /// <value>
        /// The current depth of the stack.
        /// </value>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets or sets the Top of stack.
        /// </summary>
        /// <value>
        /// The top of stack.
        /// </value>
        public T Tos
        {
            set
            {
                if (_countStack.Tos > 0)
                {
                    _impStack.Push();
                    _countStack.Push();
                    _countStack.Tos = 0;
                }
                _impStack.Tos = value;
            }
            get { return _impStack.Tos;  }
        }
    }

    /// <summary>
    /// Dummy implementation of the <see cref="IStateStack"/> interface. Nothing can be stored within instance of this type.
    /// There's no Top of Stack object. Only the stack Depth is correctly tracked according to the number of Push() and Pop() 
    /// operations already performed on the EmptyStack.
    /// </summary>
    public class EmptyState : IStateStack
    {
        private int _depth;
        
        /// <summary>
        /// The stack's Push operation. Increases the stack Depth about one and copies the top of stack.
        /// </summary>
        public void Push()
        {
            _depth++;
        }

        /// <summary>
        /// The stack's Pop operation. Decreases the stack Depth about one and restores the previous state.
        /// </summary>
        public void Pop()
        {
            _depth--;
        }

        /// <summary>
        /// Clears the stack. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            _depth = 0;
        }

        /// <summary>
        /// Retrieves the stack's depth.
        /// </summary>
        /// <value>
        /// The current depth of the stack.
        /// </value>
        public int Depth
        {
            get { return _depth; }
        }
    }

    /// <summary>
    /// Use this as a base class for defining your own state for arbitrary SceneVisitors. 
    /// </summary>
    /// <remarks>
    /// A state is always a list of individual
    /// values that can be altered during traversal. To restore state along hierarchy levels the values are kept in <see cref="IStateStack"/>objects - one stack
    /// per tracked value. 
    /// The VisitorState itself represents an <see cref="IStateStack"/>. It delegates all interface methods to the individual value stacks registered.
    /// </remarks>
    /// <example>Here's an example of a VisitorState containing an integer and a string value:
    /// <code>
    /// class TestState : VisitorState
    /// {
    ///     private CollapsingStateStack&lt;string&gt; _stringState = new CollapsingStateStack&lt;string&gt;();
    ///     private CollapsingStateStack&lt;int&gt; _intState = new CollapsingStateStack&lt;int&gt;();
    /// 
    ///     public string StringState
    ///     {
    ///          get { return _stringState.Tos; }
    ///          set { _stringState.Tos = value; }
    ///     }
    /// 
    ///     public string IntState
    ///     {
    ///          get { return _intState.Tos; }
    ///          set { _intState.Tos = value; }
    ///     }
    /// 
    ///     TestState()
    ///     {
    ///        base();
    ///        RegisterState(_stringState);
    ///        RegisterState(_intState);
    ///     }
    /// </code>
    /// </example>
    public class VisitorState : IStateStack
    {
        private List<IStateStack> _stacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitorState"/> class.
        /// </summary>
        public VisitorState()
        {
            _stacks =  new List<IStateStack>();
        }

        /// <summary>
        /// Registers a state stack. State stacks need to be registered to be notified when the entire state is pushed or popped.
        /// </summary>
        /// <param name="stack">The state stack to register.</param>
        protected void RegisterState(IStateStack stack)
        {
            _stacks.Add(stack);
        }

        /// <summary>
        /// The visitor state's Push operation. Pushes all registered state stacks.
        /// </summary>
        public void Push()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Push();
            }
        }

        /// <summary>
        /// The visitor state's Pop operation. Pops all registered state stacks.
        /// </summary>
        public void Pop()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Pop();
            }
        }

        /// <summary>
        /// Clears all registered state stacks. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Clear();
            }
        }

        /// <summary>
        /// Retrieves the state's overall depth.
        /// </summary>
        /// <value>
        /// The current depth of the visitor state.
        /// </value>
        public int Depth
        {
            get { return _stacks.First().Depth; }
        }
    }

    class TestState : VisitorState
    {
        private CollapsingStateStack<string> _stringState;

        public string StringState
        {
            get { return _stringState.Tos; }
            set { _stringState.Tos = value; }
        }

    }

    /*
    class TestScratchCode
    {
        public delegate void StateMapper<TState, TComponent>(TState state, TComponent comp) where TState : IStateStack where TComponent : SceneComponentContainer;
        class VisitingEnumerable<TState> : SceneVisitor
        {
            public VisitingEnumerable(TState state, StateMapper<TState []> )

        }

        public void TestPicking(SceneContainer scene)
        {


            var Picker = new VisitingEnumerable(new { ModelView = new CollapsingStateStack<float4x4>() } , new [] { (state, MeshComponent m) => {} })
        }
        

    }


/*
   // <StateItem>
    class MyState : SomeStateBase
    {
        public StateItem<float4x4> World = new StateItem<float4x4>();
        public StateItem<MaterialComponent> Material = new StateItem<MaterialComponent>(); 

    }


    interface IStateItem<TItem> : IStateStack
    {
        TItem Tos { set; get; }
    }
    // </StateItem>
    */
}
