namespace Fusee.Engine.Common
{
    /// <summary>
    /// This handle is used to switch between the different handles of an ITexture instance created by/for a GBuffer
    /// </summary>
    public enum GBufferHandle
    {
        gPositionHandle = 0,
        gNormalHandle = 1,
        gAlbedoSpecHandle = 2,
        gDepthHandle = 3
    }
}