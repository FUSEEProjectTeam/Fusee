using Fusee.Engine.Core.Scene;

namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Collection of shader code strings, describing a shader's header information, such as the version precision and preprocessor instructions.
    /// </summary>
    public sealed class Header
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Header() { }

        private Header() { }

        public static Header Instance => _instance;
        private static readonly Header _instance = new();


        /// <summary>
        /// Name of the number of bones preprocessor directive.
        /// </summary>
        public string BoneDefineVar = "BONES";

        /// <summary>
        /// Sets the precision to highp float.
        /// </summary>
        public string EsPrecisionHighpFloat = "precision highp float;";

        /// <summary>
        /// Sets the version to 330es.
        /// </summary>
        public string Version300Es = "#version 300 es\n";

        /// <summary>
        /// Adds PI as preprocessor directive (#define).
        /// </summary>
        public string DefinePi = "#define PI 3.14159265358979323846f\n";

        /// <summary>
        /// Sets preprocessor that defines the bone count.
        /// </summary>
        public string DefineBones(Weight wc)
        {
            return $"#define {BoneDefineVar} {wc.Joints.Count}";
        }
    }
}