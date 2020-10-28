using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Defines the alignment in y direction.
    /// </summary>
    public enum VerticalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the top border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Top,

        /// <summary>
        /// The text will be aligned to the middle of vertical axis of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Center,

        /// <summary>
        /// The text will be aligned to the bottom border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Bottom
    }
}