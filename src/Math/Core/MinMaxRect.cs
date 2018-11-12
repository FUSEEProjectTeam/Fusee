using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math.Core
{

    /// <summary>
    /// Class containing an axis aligned (two-dimensional) rectangle specified by its minimum (lower-left) and maximum (upper-right) 
    /// points in 2d space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [ProtoContract]
    public class MinMaxRect
    {
        [ProtoMember(1)]
        public float2 Min;

        [ProtoMember(2)]
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
            return $"Min: {Min} Max: {Max}";
        }
    }
}
