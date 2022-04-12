using System;
using System.Globalization;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Class containing an axis aligned (two-dimensional) rectangle specified by its minimum (lower-left) and maximum (upper-right)
    /// points in 2d space.
    /// </summary>    
    public struct MinMaxRect
    {
        /// <summary>
        /// Returns the minimum (lower-left corner) as a float2 vector.
        /// </summary>       
        public float2 Min;

        /// <summary>
        /// Returns the maximum (upper-right corner) as a float2 vector.
        /// </summary>        
        public float2 Max;

        /// <summary>
        /// Calculates the center of this rectangle.
        /// </summary>
        /// <value>
        /// The rectangle's center.
        /// </value>
        public float2 Center => 0.5f * (Min + Max);

        /// <summary>
        /// Calculates the size of this rectangle.
        /// </summary>
        /// <value>
        /// The rectangle's size.
        /// </value>
        public float2 Size => Max - Min;

        /// <summary>
        /// Creates a new MinMaxRect from the specified center and size.
        /// </summary>
        /// <param name="center">The center of rect to be returned.</param>
        /// <param name="size">The size of the rect to be returned.</param>
        /// <returns>A new MinMaxRect located at the specified center with the given size.</returns>
        public static MinMaxRect FromCenterSize(float2 center, float2 size)
        {
            var min = center - 0.5f * size;
            return new MinMaxRect
            {
                Min = min,
                Max = min + size,
            };
        }

        /// <summary>
        /// Returns a System.String that represents the current MinMaxRect.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current MinMaxRect.
        /// </summary>
        /// <param name="provider">Provides information about a specific culture.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(provider);
        }

        internal string ConvertToString(IFormatProvider? provider)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            return string.Format(provider, "Min: {0} Max: {1}", Min.ToString(provider), Max.ToString(provider));
        }
    }
}