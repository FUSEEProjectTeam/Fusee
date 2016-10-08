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
        /// <para />Make sure that this is a method with the following signature: vec3 ApplyLight(Light light).
        /// <para />Within this method you can use plain GLSL, as well as all FUSEE's builtin shader attributes (e.g. FUSEE_ITMV, ...).
        /// <para />Make sure you return your calculated light as vec3.
        /// </summary>
        [ProtoMember(1)] public string ApplyLightString;

        //TODO: What else could a user need?
        //TODO: How to implement it?
        //TODO: Perhaps user can decide implementation method?
        /// <summary>
        /// This string is converted and used for the calculation of the fragment shader during the render traversal.
        /// </summary>
        [ProtoMember(2)] public string FragmentShaderString;
    }

}