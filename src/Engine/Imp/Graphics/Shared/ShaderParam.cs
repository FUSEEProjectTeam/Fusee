using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID
namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    /// <summary>
    /// Implementation of the <see cref="IShaderParam" /> interface.
    /// This object is passed to shaderprograms that are running on the graphics card to modify shader values.
    /// </summary>
    public class ShaderParam : IShaderParam
    {
        internal int handle;
    }
}