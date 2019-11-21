
namespace Fusee.Engine.Core
{
    /// <summary>
    /// Contains all types of render passes. Is used to control behavior in the SceneRenderer, based on the current type of pass. 
    /// </summary>
    public enum RenderPasses
    {
        SHADER_EFFECT_PRE,
        GEOMETRY,
        SSAO,
        SSAO_BLUR,
        SHADOW,
        FXAA,
        LIGHTING,
        SINGLE
    }
}
