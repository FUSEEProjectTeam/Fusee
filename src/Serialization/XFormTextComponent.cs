using ProtoBuf;
namespace Fusee.Serialization
{
    /// <summary>
    /// Placeholder for later implemented UI Elements such as buttons, images, slider etc.
    /// </summary>
    [ProtoContract]
    public class XFormTextComponent : SceneComponentContainer
    {
        public float TextScaleFactor;

        public XFormTextComponent(float textScaleFactor = 1f)
        {
            TextScaleFactor = textScaleFactor;
        }
    }
}
