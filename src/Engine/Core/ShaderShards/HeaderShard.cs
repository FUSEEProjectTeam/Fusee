using Fusee.Serialization;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of Shader Shards, describing a shader's header information, such as the version precision and preprocessor instructions.
    /// </summary>
    public static class HeaderShard
    {
        /// <summary>
        /// Name of the number of bones preprocessor directive.
        /// </summary>
        public static string BoneDefineVar = "BONES";

        /// <summary>
        /// Sets the precision to highp float.
        /// </summary>
        public static string EsPrecisionHighpFloat = "precision highp float; \n";       

        /// <summary>
        /// Sets the version to 300es.
        /// </summary>
        public static string Version300Es = "#version 300 es\n";

        /// <summary>
        /// Sets preprocessor that defines the bone count.
        /// </summary>
        public static string DefineBones(EffectProps effectProps, WeightComponent wc)
        {
            if (effectProps.MeshProbs.HasWeightMap)
                return $"#define {BoneDefineVar} {wc.Joints.Count}";
            else return "";
        }
    }
}
