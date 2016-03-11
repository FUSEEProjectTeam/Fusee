using System;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Provides additional mathematical function needed for 3D graphics.
    /// </summary>
    internal class NamespaceDoc
    {
    }

    /// <summary>
    ///     Provides standard mathematical functions and helpers for different types.
    /// </summary>
    public static class M
    {
        #region Saturate

        /// <summary>
        ///     Saturates a given value. The returned value is guaranteed to be within the interval given by [lower;upper]
        /// </summary>
        /// <typeparam name="T">The type all the parameters and the return value are given (most likely a simple type)</typeparam>
        /// <param name="val">The value to be saturated</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns>
        ///     The saturated value within the given bounds
        /// </returns>
        public static T Saturate<T>(T val, T lower, T upper) where T : IComparable<T>
        {
            return (lower.CompareTo(val) < 0) ? ((val.CompareTo(upper) < 0) ? val : upper) : lower;
        }

        /// <summary>
        /// Calculates the minimum of a and b.
        /// </summary>
        /// <typeparam name="T">The type of a and b. Must be <see cref="IComparable{T}"/></typeparam>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The minimum of a and b.</returns>
        public static T Min<T>(T a, T b) where T : IComparable<T>
        {
            return (a.CompareTo(b) < 0) ? a : b;
        }

        /// <summary>
        /// Calculates the maximum of a and b.
        /// </summary>
        /// <typeparam name="T">The type of a and b. Must be <see cref="IComparable{T}"/></typeparam>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>The maximum of a and b.</returns>
        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            return (a.CompareTo(b) > 0) ? a : b;
        }
        #endregion
    }
}