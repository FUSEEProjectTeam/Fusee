﻿using Fusee.Engine.Common;
using System.Diagnostics;

namespace Fusee.Engine.Imp.SharedAll
{
    /// <summary>
    /// Internally used shader parameters.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public struct ActiveUniform : IActiveUniform
    {
        /// <summary>
        /// The Handle to be used when setting or getting the parameter value from the shader.
        /// </summary>
        public IUniformHandle Handle
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the name of the shader parameter.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Hash = value.GetHashCode();
            }
        }
        private string _name;

        /// <summary>
        /// Contains the number of items stored under this parameter name. Only differs from 1 if the parameter is a uniform array.
        /// If Size > 1 this value represents the number of array entries.
        /// </summary>
        /// <remarks>
        /// See also <a href="https://www.khronos.org/opengles/sdk/docs/man/xhtml/glGetActiveUniform.xml">the Khronos group's documentation on glGetActiveUniform</a>.
        /// </remarks>
        public int Size { get; set; }

        /// <summary>
        /// The hash code of this ShaderParamInfo.
        /// </summary>
        public int Hash { get; private set; }

        /// <summary>
        /// The method that returns the uniform value.
        /// </summary>
        public GetUniformValue UniformValueGetter { get; set; }

        /// <summary>
        /// Determines whether this uniform is global or limited to one shader.
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// If the value has changed the shader uniform must be changed too.
        /// </summary>
        public bool HasValueChanged { get; set; }
    }
}