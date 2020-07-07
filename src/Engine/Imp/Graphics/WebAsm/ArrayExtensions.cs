using System;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// A static helper class to provide functions for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Casts an array to an array of the given type T.
        /// </summary>
        /// <typeparam name="T">The given type to cast to.</typeparam>
        /// <param name="array">The array to parse.</param>
        /// <param name="cast">The cast to make.</param>
        /// <returns></returns>
        public static T[] ToArray<T>(this WebAssembly.Core.Array array, Func<object, T> cast)
        {
            if (array == null)
            {
                return null;
            }

            var length = array.Length;
            var result = new T[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = cast(array[i]);
            }

            return result;
        }
    }
}
