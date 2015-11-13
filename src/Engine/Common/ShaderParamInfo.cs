using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// Contains information about a shader parameter (uniform parameter).
    /// </summary>
    public struct ShaderParamInfo
    {
        /// <summary>
        /// Contains the number of items stored under this parameter name. Only differs from 1 if the parameter is a uniform array.
        /// If Size > 1 this value represents the number of array entries.
        /// </summary>
        /// <remarks>
        /// See also <a href="https://www.khronos.org/opengles/sdk/docs/man/xhtml/glGetActiveUniform.xml">the khronos group's documentation on glGetActiveUniform</a>.
        /// </remarks>
        public int Size;
        /// <summary>
        /// Contains the type of the shader parameter.
        /// </summary>
        public Type Type;
        /// <summary>
        /// Contains the name of the shader parameter.
        /// </summary>
        public string Name;
        /// <summary>
        /// The Handle to be used when setting or getting the parameter value from the shader.
        /// </summary>
        public IShaderParam Handle;
    }
}
