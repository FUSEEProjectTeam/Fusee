namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Static class that defines names of <see cref="SurfaceEffect"/> dependent shader parameters, methods and structs.
    /// </summary>
    public static class SurfaceEffectNameDeclarations
    {
        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffectBase.SurfaceOutput"/>) always has this type in the shader code.
        /// </summary>
        public const string StructTypeName = "SurfOut";

        /// <summary>
        /// Name of the surface method in the fragment shader.
        /// </summary>
        public static readonly string ChangeSurfFrag = "ChangeSurfFrag";

        /// <summary>
        /// Name of the surface method in the vertex shader.
        /// </summary>
        public static readonly string ChangeSurfVert = "ChangeSurfVert";
    }
}