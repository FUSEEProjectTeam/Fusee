
namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Defines the alignment in y direction.
    /// </summary>
    public enum VerticalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the top border of the enclosing />.
        /// </summary>
        TOP,

        /// <summary>
        /// The text will be aligned to the middle of vertical axis of the enclosing />.
        /// </summary>
        CENTER,

        /// <summary>
        /// The text will be aligned to the bottom border of the enclosing />.
        /// </summary>
        BOTTOM
    }

    /// <summary>
    /// Defines the alignment in y direction.
    /// </summary>
    public enum HorizontalTextAlignment
    {
        /// <summary>
        /// The text will be aligned to the left border of the enclosing />.
        /// </summary>
        LEFT,

        /// <summary>
        /// The text will be aligned to the center of vertical axis of the enclosing />.
        /// </summary>
        CENTER,

        /// <summary>
        /// The text will be aligned to right border of the enclosing />.
        /// </summary>
        RIGHT
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
        /// The <see cref="HorizontalTextAlignment"/>.
        /// </summary>
        public HorizontalTextAlignment HorizontalAlignment;

        /// <summary>
        /// The <see cref="VerticalTextAlignment"/>.
        /// </summary>
        public VerticalTextAlignment VerticalAlignment;
    }

}
