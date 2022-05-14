using Fusee.Engine.Core.Scene;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of shader code strings, describing a shader's header information, such as the version precision and preprocessor instructions.
    /// </summary>
    public static class Header
    {
        /// <summary>
        /// Name of the number of bones preprocessor directive.
        /// </summary>
        public static string BoneDefineVar = "BONES";

        /// <summary>
        /// Sets the precision to highp float.
        /// </summary>
        public static string EsPrecisionHighpFloat = "precision highp float;";

        /// <summary>
        /// Sets the version to 330 es.
        /// </summary>
        public static string Version300Es = "#version 330 es\n";

        /// <summary>
        /// Sets the version to 330 core.
        /// </summary>
        public static string Version330Core = "#version 330 core\n";

        /// <summary>
        /// Sets the version to 440 core.
        /// </summary>
        public static string Version440Core = "#version 440 core\n";

        /// <summary>
        /// Adds PI as preprocessor directive (#define).
        /// </summary>
        public static string DefinePi = "#define PI 3.14159265358979323846f\n";

        /// <summary>
        /// Sets preprocessor that defines the bone count.
        /// </summary>
        public static string DefineBones(Weight wc)
        {
            return $"#define {BoneDefineVar} {wc.Joints.Count}";
        }
    }
}