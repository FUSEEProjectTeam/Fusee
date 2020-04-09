using Fusee.Math.Core;
using System;
using System.Collections.Generic;

namespace Fusee.Xene
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
        private const int _defaultCapacity = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the stack. This is the expected maximum stack depth. 
        /// If the stack depth grows bigger, the stack automatically doubles its capacity internally.</param>
        public StateStack(int capacity = _defaultCapacity)
        {
            _array = new T[capacity];
            Depth = 0;
        }

        /// <summary>
        /// Gets and sets the Top of stack.
        /// </summary>
        /// <value>
        /// The top of stack.
        /// </value>
        public T Tos
        {
            set => _array[Depth] = value;
            get => _array[Depth];
        }

        /// <summary>
        /// The stack's Push operation. Increases the stack Depth about one and copies the top of stack.
        /// </summary>
        public void Push()
        {
            var size = Depth + 1;
            if (size == _array.Length)
            {
                var objArray = new T[2 * _array.Length];
                Array.Copy(_array, 0, objArray, 0, size);
                _array = objArray;
            }
            _array[size] = _array[Depth];
            Depth++;
        }

        /// <summary>
        /// The stack's Pop operation. Decreases the stack Depth about one and restores the previous state.
        /// </summary>
        public void Pop()
        {
            _array[Depth--] = default;
        }

        /// <summary>
        /// Clears the stack. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_array, 0, Depth + 1);
            Depth = 0;
        }

        /// <summary>
        /// Retrieves the stack's depth.
        /// </summary>
        /// <value>
        /// The current depth of the stack.
        /// </value>
        public int Depth { get; private set; }
    }


    /// <summary>
    /// An <see cref="IStateStack"/> implementation behaving better in situations where many subsequent Push (and Pop) operations occur
    /// without actually altering the TOS contents. 
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
            // The _impStack keeps the actual top-of-stack values. 
            _impStack = new StateStack<T>(capacity);
            // The _countStack keeps the number of "Push" operations that occurred between two Tos alterations
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
            if (_countStack.Tos >= 1)
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
        /// Gets and sets the Top of stack.
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
                    // delete one "Push" without Tos alteration because we are just about to alter the Tos
                    _countStack.Tos--;
                    // Do a real push on values
                    _impStack.Push();
                    // Also push the count stack and reset it to zero to start counting Pushes without Tos alteration again
                    _countStack.Push();
                    _countStack.Tos = 0;
                }
                _impStack.Tos = value;
            }
            get => _impStack.Tos;
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
        public int Depth => _depth;
    }

    /// <summary>
    /// Use this as a base class for defining your own state for arbitrary Visitors. 
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
    ///     TestState() : base()
    ///     {
    ///        RegisterState(_stringState);
    ///        RegisterState(_intState);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class VisitorState : IStateStack
    {
        private readonly List<IStateStack> _stacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitorState"/> class.
        /// </summary>
        public VisitorState()
        {
            _stacks = new List<IStateStack>();
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
            foreach (var stack in _stacks)
            {
                stack.Push();
            }
        }

        /// <summary>
        /// The visitor state's Pop operation. Pops all registered state stacks.
        /// </summary>
        public void Pop()
        {
            foreach (var stack in _stacks)
            {
                stack.Pop();
            }
        }

        /// <summary>
        /// Clears all registered state stacks. The Depth will be reset to zero.
        /// </summary>
        public void Clear()
        {
            foreach (var stack in _stacks)
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
        public int Depth => _stacks[0].Depth;
    }

    /// <summary>
    /// A standard state for typical traversals mimicking rendering activities. Keeps track of the main matrices
    /// as well as selected render states.
    /// </summary>
    public class StandardState : VisitorState
    {
        private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
        private readonly CollapsingStateStack<float4x4> _view = new CollapsingStateStack<float4x4>();
        private readonly CollapsingStateStack<float4x4> _projection = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// Gets and sets the top of the Model matrix stack. The Model matrix transforms model coordinates into world coordinates.
        /// </summary>
        /// <value>
        /// The Model matrix.
        /// </value>
        public float4x4 Model
        {
            set => _model.Tos = value;
            get => _model.Tos;
        }

        /// <summary>
        /// Gets and sets the top of the View matrix stack. The View matrix transforms world coordinates into view coordinates.
        /// The View matrix contains a camera's extrinsic parameters (position and orientation).
        /// </summary>
        /// <value>
        /// The View matrix.
        /// </value>
        public float4x4 View
        {
            set => _view.Tos = value;
            get => _view.Tos;
        }

        /// <summary>
        /// Gets and sets the top of the Projection matrix stack. The Projection matrix transforms view coordinates into projection coordinates.
        /// The Projection matrix contains a camera's intrinsic parameters (field-of-view/focal length for perspective projections).
        /// </summary>
        /// <value>
        /// The Projection matrix
        /// </value>
        public float4x4 Projection
        {
            set => _projection.Tos = value;
            get => _projection.Tos;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardState"/> class.
        /// </summary>
        public StandardState()
        {
            RegisterState(_model);
            RegisterState(_view);
            RegisterState(_projection);
        }
    }
}
