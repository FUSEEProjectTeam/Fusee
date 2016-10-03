using ProtoBuf;

namespace Fusee.Serialization
{

    /// <summary>
    /// Light definition. If contained within a node, the node's (and potentially child node's)
    /// light is rendered with the speicified lightsetup within the <see cref="ApplyLightString"/> ApplyLight Method.
    /// </summary>
    [ProtoContract]
    public class MaterialLightComponent : MaterialComponent
    {

        /// <summary>
        /// This string is converted and used for the light calculation during the render traversal.
        /// Make sure that this is a method with the following signature: vec3 ApplyLight().
        /// Within this method you can use plain GLSL, as well as all FUSEE's builtin Shader attributes (e.g. FUSEE_ITMV, ...).
        /// Make sure you return your calculated light as vec3.
        /// </summary>
        [ProtoMember(1)]
        public string ApplyLightString;
    }
}