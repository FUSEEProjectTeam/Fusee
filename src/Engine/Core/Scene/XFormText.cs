
using Fusee.Engine.Common;

namespace Fusee.Engine.Core.Scene
{
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
