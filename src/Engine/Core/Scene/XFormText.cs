using Fusee.Math.Core;


namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Defines the alignment in x direction.
    /// </summary>
    public enum HorizontalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the left border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Left,

        /// <summary>
        /// The text will be aligned to the middle of the horizontal axis of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Center,

        /// <summary>
        /// The text will be aligned to the right border of the enclosing <see cref="MinMaxRect"/>.
        /// </summary>
        Right
    }

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
        Bottom,

    }

    /// <summary>
    /// Enables the scene renderer to treat GUI text differently.
    /// </summary>  
    public class XFormText : SceneComponent
    {
        /// <summary>
        /// The width of the text.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of the text.
        /// </summary>
        public float Height;

        /// <summary>
        /// The <see cref="HorizontalTextAlignment"/>.
        /// </summary>
        public HorizontalTextAlignment HorizontalAlignment;

        /// <summary>
        /// The <see cref="VerticalTextAlignment"/>.
        /// </summary>
        public VerticalTextAlignment VerticalAlignment;
    }
}