
namespace Fusee.Engine.Core
{
    /// <summary>
    /// Contains all types of render passes. Is used to control behavior in the SceneRenderer, based on the current type of pass. 
    /// </summary>
    public enum RenderPasses
    {
#pragma warning disable 1591
        ShaderEffectPre,
        Geometry,
        Ssao,
        SsaoBlur,
        Shadow,
        Fxaa,
        Lighting,
        Single
#pragma warning restore 1591
    }
}
