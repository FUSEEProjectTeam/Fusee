using System;

namespace Fusee.Math
{
    /// <summary>
    /// Provides additional mathematical function needed for 3D graphics.
    /// </summary>
    internal class NamespaceDoc
    {
    }
    /// <summary>
	/// Provides standard mathematical functions and helpers for different types.
	/// </summary>
	public static class M
	{
        #region Saturate
        /// <summary>
        /// Saturates a given value. The returned value is guaranteed to be within the interval given by [lower;upper]
        /// </summary>
        /// <typeparam name="T">The type all the parameters and the return value are given (most likely a simple type)</typeparam>
        /// <param name="val">The value to be saturated</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns>
        /// The saturated value within the given bounds
        /// </returns>
        public static T Saturate<T>(T val, T lower, T upper) where T:IComparable<T>
        {
            return (lower.CompareTo(val) < 0) ? ((val.CompareTo(upper) < 0)? val : upper) : lower;
        }
        #endregion
	}
}
