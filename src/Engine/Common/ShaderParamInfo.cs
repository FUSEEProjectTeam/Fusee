using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// Holds information for any shader parameter.
    /// </summary>
    public struct ShaderParamInfo
    {
        /// <summary>
        /// Holds the size in bytes.
        /// </summary>
        public int Size;
        /// <summary>
        /// Holds the type of the shader parameter.
        /// </summary>
        public Type Type;
        /// <summary>
        /// Holds the name of the shader parameter the shader contains.
        /// </summary>
        public string Name;
        /// <summary>
        /// The Handle represents the shader parameter.
        /// </summary>
        public IShaderParam Handle;
    }
}
