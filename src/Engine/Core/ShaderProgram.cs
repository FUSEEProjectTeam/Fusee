using System.Collections.Generic;

namespace Fusee.Engine
{
    /// <summary>
    /// Instances of this class represent a pair of a Vertex and a Pixel shader code, both compiled an 
    /// uploaded to the gpu ready to be used. 
    /// </summary>
    /// <remarks>See <see cref="RenderContext.CreateShader"/> how to create instances and 
    /// <see cref="RenderContext.SetShader"/> how to use instances as the current shaders.</remarks>
    public class ShaderProgram
    {
        #region Fields

        internal IShaderProgramImp _spi;
        internal IRenderContextImp _rci;
        internal Dictionary<string, ShaderParamInfo> _paramsByName;

        #endregion

        #region Members

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="renderContextImp">The <see cref="IRenderContextImp"/>.</param>
        /// <param name="shaderProgramImp">The <see cref="IShaderProgramImp"/>.</param>
        internal ShaderProgram(IRenderContextImp renderContextImp, IShaderProgramImp shaderProgramImp)
        {
            _spi = shaderProgramImp;
            _rci = renderContextImp;
            _paramsByName = new Dictionary<string, ShaderParamInfo>();
            foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            {
                _paramsByName.Add(info.Name, info);
            }
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
            ShaderParamInfo ret;
            _paramsByName.TryGetValue(paramName, out ret);
            return ret;
        }

        #endregion

        // TODO: add SetParameter methods here (remove from render context).
    }
}
