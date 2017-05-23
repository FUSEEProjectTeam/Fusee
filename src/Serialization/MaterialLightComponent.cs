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
        /// <para />Make sure that this is a method with the following signature: vec3 ApplyLight(vec3 position, vec3 intensities, vec3 coneDirection, float attenuation, float ambientCoefficient, float coneAngle, int lightType)
        /// <para />Within this method you can use plain GLSL, as well as FUSEE's builtin shader attributes (e.g. FUSEE_ITMV, ...), as long as they were specified from ShaderCodeBuilder during runtime.
        /// <para />Make sure you return your calculated light as vec3.
        /// <remarks>The ApplyLight method signature is going to change within the next month! (11/2016)</remarks>
        /// </summary>
        [ProtoMember(1)] public string ApplyLightString;

        //TODO: What else could a user need?
        //TODO: How to implement it?
        //TODO: Perhaps user can decide implementation method?
        //TODO: Describe normal and material settings for normal & deferred
        /// <summary>
        /// This string is converted and used for the calculation of the fragment shader during the render traversal.
        /// <remarks>Not yet implemented!</remarks>
        /// </summary>
        [ProtoMember(2)] public string FragmentShaderString;
    }

}