
namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Placeholder for later implemented UI Elements such as buttons, images, slider etc.
    /// </summary>
    public class FusXFormText : FusComponent
    {
        /// <summary>
        /// The UI text size.
        /// </summary>
        public float TextScaleFactor;

        /// <summary>
        /// Implements the text component in the UI.
        /// </summary>
        /// <param name="textScaleFactor">The text size.</param>
        public FusXFormText(float textScaleFactor = 1f)
        {
            TextScaleFactor = textScaleFactor;
        }
    }
}
