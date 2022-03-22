namespace Fusee.Engine.Core.Effects
{
    public static class SurfaceEffectNameDeclarations
    {
        /// <summary>
        /// The surface effects "out"-struct (<see cref="SurfaceEffectBase.SurfaceOutput"/>) always has this type in the shader code.
        /// </summary>
        public const string StructTypeName = "SurfOut";

        public static readonly string ChangeSurfFrag = "ChangeSurfFrag";
        public static readonly string ChangeSurfVert = "ChangeSurfVert";
    }
}
