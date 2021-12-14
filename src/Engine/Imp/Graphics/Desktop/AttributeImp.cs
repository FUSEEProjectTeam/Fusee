using Fusee.Engine.Common;
using OpenTK.Graphics;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Contains a handle for any type of attribute buffer stored on GPU memory such as vertices, normals, uvs etc.
    /// </summary>
    public class AttributeImp : IAttribImp
    {
        internal BufferHandle AttributeBufferObject;
    }
}