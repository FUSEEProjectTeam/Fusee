﻿using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.WebAsm;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
    /// <summary>
    /// Implementation of the <see cref="IShaderParam" /> interface. 
    /// This object is passed to shader programs that are running on the graphics card to modify shader values.
    /// </summary>
    public class ShaderParam : IShaderParam
    {
        internal WebGLUniformLocation handle;
    }
}
