namespace Fusee.Engine.Common
{
    /// <summary>
    /// This enum is used to switch between the different handles of an ITexture instance created by/for a GBuffer
    /// </summary>
    public enum GBufferHandle
    {
        // TODO: Implement SpecularHandle
        /// <summary>
        /// The MVP * fuVertex data handle
        /// </summary>
        GPositionHandle = 0,
        /// <summary>
        /// The normalize(mat3(FUSEE_ITMV) * fuNormal) data handle
        /// </summary>
        GNormalHandle = 1,
        /// <summary>
        /// The DiffuseColor data handle
        /// </summary>
        GAlbedoHandle = 2,
        /// <summary>
        /// The Depth from z-Buffer
        /// </summary>
        GDepth = 3
    }

    /// <summary>
    /// This enum is used to specify the type of writable texture 
    /// </summary>
    public enum WritableTextureFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Depth,
        /// <summary>
        /// 
        /// </summary>
        DepthCubeMap,
        /// <summary>
        /// 
        /// </summary>
        GBuffer
    }
}