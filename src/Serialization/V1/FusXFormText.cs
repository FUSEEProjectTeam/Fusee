namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Defines the alignment in y direction.
    /// </summary>
    public enum FusVerticalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the top border of the enclosing MinMaxRect.
        /// </summary>
        Top,

        /// <summary>
        /// The text will be aligned to the middle of vertical axis of the enclosing MinMaxRect.
        /// </summary>
        Center,

        /// <summary>
        /// The text will be aligned to the bottom border of the enclosing MinMaxRect.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Defines the alignment in y direction.
    /// </summary>
    public enum FusHorizontalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the left border of the enclosing MinMaxRect.
        /// </summary>
        Left,

        /// <summary>
        /// The text will be aligned to the center of vertical axis of the enclosing MinMaxRect.
        /// </summary>
        Center,

        /// <summary>
        /// The text will be aligned to right border of the enclosing MinMaxRect.
        /// </summary>
        Right
    }

    /// <summary>
    /// Enables the scene renderer to treat GUI text differently.
    /// </summary>

    public class FusXFormText : FusComponent
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
        /// The <see cref="FusHorizontalTextAlignment"/>.
        /// </summary>
        public FusHorizontalTextAlignment HorizontalAlignment;

        /// <summary>
        /// The <see cref="FusVerticalTextAlignment"/>.
        /// </summary>
        public FusVerticalTextAlignment VerticalAlignment;
    }

}