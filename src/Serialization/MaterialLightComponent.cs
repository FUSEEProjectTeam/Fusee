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
        /// All supported lightning calculation methods ShaderCodeBuilder.cs supports.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public enum StandardLightningCalculationMethod
        {
            /// <summary>
            /// Blinn
            /// </summary>
            BLINN,

            /// <summary>
            /// Blinn Phong
            /// </summary>
            BLINN_PHONG,

            /// <summary>
            /// Cook Torrance, also known as physical based rendering
            /// </summary>
            COOK_TORRANCE
        }

        /// <summary>
        /// This string is converted and used for the light calculation during the render traversal.
        /// <para />Make sure that this is a method with the following signature: vec3 ApplyLight().
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

        /// <summary>
        /// If there is no ApplyLightString this lightning method is used during render traversal. 
        /// </summary>
        [ProtoMember(3)] public StandardLightningCalculationMethod LightningMethod;

    }

}