
namespace Fusee.Engine
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
