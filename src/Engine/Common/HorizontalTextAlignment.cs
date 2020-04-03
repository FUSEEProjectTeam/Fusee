
using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Defines the alignment in x direction.
    /// </summary>
    public enum HorizontalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the left border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        LEFT,

        /// <summary>
        /// The text will be aligned to the middle of the horizontal axis of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        CENTER,

        /// <summary>
        /// The text will be aligned to the right border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        RIGHT
    }
}
