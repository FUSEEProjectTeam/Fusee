using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
    public interface IStateStack
    {
        void Push();
        void Pop();
        void Clear();
        int Depth { get; }
    }

    public class StateStack<T> : IStateStack
    {
        private T[] _array;
        private int _top;
        private const int _defaultCapacity = 8;

        public StateStack(int capacity = _defaultCapacity)
        {
            _array = new T[capacity];
            _top = 0;
        }

        public T Tos
        {
            set { _array[_top] = value; }
            get { return _array[_top]; }
        }


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

        public void Pop()
        {
            _array[_top--] = default(T);
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _top+1);
            _top = 0;
        }

        public int Depth { get { return _top + 1; } }
    }


    public class CollapsingStateStack<T> : IStateStack
    {
        public CollapsingStateStack()
        {
            _impStack = new StateStack<T>();
            _countStack = new StateStack<int>();
        }

        private StateStack<T> _impStack;
        private StateStack<int> _countStack; 
        public void Push()
        {
            _countStack.Tos++;
        }

        public void Pop()
        {
            if (_countStack.Tos > 0)
            {
                _countStack.Tos++;
            }
            else
            {
                _countStack.Pop();
                _impStack.Pop();
            }
        }

        public void Clear()
        {
            _impStack.Clear();
            _countStack.Clear();
        }

        public int Depth { get; private set; }

        public T Tos
        {
            set
            {
                _impStack.Push();
                _countStack.Push();
                _impStack.Tos = value;
            }
            get { return _impStack.Tos;  }
        }
    }

    public class EmptyState : IStateStack
    {
        private int _depth;
        public void Push()
        {
            _depth++;
        }

        public void Pop()
        {
            _depth--;
        }

        public void Clear()
        {
            _depth = 0;
        }

        public int Depth
        {
            get { return _depth; }
        }
    }

    public class VisitorState : IStateStack
    {
        private List<IStateStack> _stacks;

        public VisitorState()
        {
            _stacks =  new List<IStateStack>();
        }

        protected void RegisterState(IStateStack stack)
        {
            _stacks.Add(stack);
        }


        public void Push()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Push();
            }
        }

        public void Pop()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Pop();
            }
        }

        public void Clear()
        {
            foreach (IStateStack stack in _stacks)
            {
                stack.Clear();
            }
        }

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
