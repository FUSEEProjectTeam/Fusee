using System.Collections.Generic;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A ShaderProgram is a set of shaders (currently vertex, pixel and optionally geometry shaders), all compiled an 
    /// uploaded to the gpu ready to be used. 
    /// </summary>
    /// <remarks>See <see cref="RenderContext.CreateShader"/> how to create instances and 
    /// <see cref="RenderContext.SetShader"/> how to use instances as the current shaders.</remarks>
    internal class ShaderProgram
    {
        /// <summary>
        /// The handle that identifies the shader program on the gpu.
        /// </summary>
        internal IShaderHandle GpuHandle;

        /// <summary>
        /// All parameters of this ShaderProgramm as a Dictionary of type "string, ShaderParamInfo".
        /// </summary>
        internal Dictionary<string, ShaderParamInfo> ParamsByName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="paramsByName">Dictionary of the shader parameters.</param>
        /// <param name="gpuHandle">The <see cref="IShaderHandle"/>.</param>
        internal ShaderProgram(Dictionary<string, ShaderParamInfo> paramsByName, IShaderHandle gpuHandle)
        {
            GpuHandle = gpuHandle;
            ParamsByName = paramsByName;
        }

        /// <summary>
        /// Retrieves an identifier for the shader parameter name.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>A <see cref="IShaderParam"/> if paramName is declared and used as a uniform parameter within the shader program. Otherwise null</returns>
        public IShaderParam GetShaderParam(string paramName)
        {
            return GetShaderParamInfo(paramName).Handle;
        }

        /// <summary>
        /// Gets the shader parameter.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>A <see cref="ShaderParamInfo"/> object if paramName is declared and used as a uniform parameter within the shader program. Otherwise the Handle field of the returndes struct is null</returns>
        public ShaderParamInfo GetShaderParamInfo(string paramName)
        {
            ParamsByName.TryGetValue(paramName, out ShaderParamInfo ret);
            return ret;
        }
    }
}
