using ProtoBuf;
namespace Fusee.Serialization
{
    /// <summary>
    /// Placeholder for later implemented UI Elements such as buttons, images, slider etc.
    /// </summary>
    [ProtoContract]
    public class XFormTextComponent : SceneComponentContainer
    {
        /// <summary>
        /// The ui text size.
        /// </summary>
        public float TextScaleFactor;

        /// <summary>
        /// Implements the text component in the ui.
        /// </summary>
        /// <param name="textScaleFactor">The text size.</param>
        public XFormTextComponent(float textScaleFactor = 1f)
        {
            TextScaleFactor = textScaleFactor;
        }
    }
}
