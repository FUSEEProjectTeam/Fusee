
namespace Fusee.Engine.Core.ShaderShards
{
    /// <summary>
    /// Name declarations for varying variables (vert out, frag in). Ensures compatibility between shader shards.
    /// Use those in vert and frag files too, if you want to mix shards and complete files.
    /// </summary>
    public static class VaryingNameDeclarations
    {
        /// <summary>
        /// The variable name of the TBN (Tangent, Bitangent, Normal) matrix.
        /// </summary>
        public const string TBN = "TBN";

        /// <summary>
        /// The texture coordinates variable name.
        /// </summary>
        public const string TextureCoordinates = "vUV";

        /// <summary>
        /// The view direction variable name.
        /// </summary>
        public const string ViewDirection = "vViewDir";

        /// <summary>
        /// The camera position variable name.
        /// </summary>
        public const string CameraPosition = "vCamPos";

        /// <summary>
        /// The tangents variable name.
        /// </summary>
        public const string Tangent = "vT";

        /// <summary>
        /// The bitangents variable name.
        /// </summary>
        public const string Bitangent = "vB";

        /// <summary>
        /// The vertex colors variable name.
        /// </summary>
        public static readonly string Color = "vColors";

        /// <summary>
        /// The vertex position variable name.
        /// </summary>
        public const string Position = "vPos";

        /// <summary>
        /// The vertex normal variable name.
        /// </summary>
        public const string Normal = "vNormal";
    }
}