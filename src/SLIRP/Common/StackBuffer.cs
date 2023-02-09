using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Common
{
    /// <summary>
    /// Works like a stack but pushes down all older elements if the stack is full when pushed.
    /// The oldest item is the overwritten.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackBuffer<T>
    {
        private T[] stack;
        private int top;

        private int depth;

        public StackBuffer(int depth)
        {
            this.depth = depth;
            stack = new T[depth];
        }

        public void Push(T item)
        {
            //if the stack is full, push all older elements down the stack 
            if (top == depth - 1)
            {
                for (int i = 0; i < top; i++)
                {
                    stack[i] = stack[i + 1];
                }
                top--;
            }

            stack[++top] = item;
        }


        public T Pop()
        {
            if (top == -1)
                throw new InvalidOperationException("Stack is empty");

            return stack[top--];
        }

        /// <summary>
        /// Pop at a specific position "n" and all younger items in the stack.
        /// </summary>
        /// <param name="n">The position at which the item should be poped</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T Pop(int n)
        {
            if (n > top + 1)
                throw new InvalidOperationException("Not enough elements in the stack");

            var item = stack[top - n + 1];
            top = top - n;
            return item;
        }

        public T Peek()
        {
            if(top == -1)
                throw new InvalidOperationException("Stack is empty");

            return stack[top];
        }

        /// <summary>
        /// Peek at a specific position "i" without altering the stack.
        /// </summary>
        /// <param name="n">The position at which the item should be peeked. 0 = latest, "depth"-1 = oldest.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T PeekAt(int i)
        {
            if(i >= depth)
                throw new ArgumentOutOfRangeException("Passed index is bigger or equal to the depth:\ndepth: "+depth+"\ti: "+i);

            int realIndex = depth -1 - i;

            return stack[i];
        }


    }
}
