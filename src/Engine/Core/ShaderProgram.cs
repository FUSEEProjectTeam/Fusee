using System.Collections.Generic;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    ///// <summary>
    ///// A ShaderProgram is a set of shaders (currently vertex, pixel and optionally geometry shaders), all compiled an 
    ///// uploaded to the gpu ready to be used. 
    ///// </summary>
    ///// <remarks>See <see cref="RenderContext.SetShaderProgram"/> how to use instances as the current shaders.</remarks>
    //internal class ShaderProgram
    //{
    //    /// <summary>
    //    /// The handle that identifies the shader program on the gpu.
    //    /// </summary>
    //    internal IShaderHandle GpuHandle;

    //    /// <summary>
    //    /// All parameters of this ShaderProgramm as a Dictionary of type "string, ShaderParamInfo".
    //    /// </summary>
    //    internal Dictionary<string, ShaderParamInfo> ParamsByName;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
    //    /// </summary>
    //    /// <param name="paramsByName">Dictionary of the shader parameters.</param>
    //    /// <param name="gpuHandle">The <see cref="IShaderHandle"/>.</param>
    //    internal ShaderProgram(Dictionary<string, ShaderParamInfo> paramsByName, IShaderHandle gpuHandle)
    //    {
    //        GpuHandle = gpuHandle;
    //        ParamsByName = paramsByName;
    //    }
    //}
}
