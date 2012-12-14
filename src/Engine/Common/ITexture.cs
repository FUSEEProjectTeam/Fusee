namespace Fusee.Engine
{
    /// <summary>
    ///  This is a markup-only for different types of Texture-handles.
    ///  The implementation is render-platform specific: e.g. int in OpenGL, 
    ///  ISurface in DirectX, TextureObject in WebGL, ...
    /// </summary>
    public interface ITexture
    {
    }
}