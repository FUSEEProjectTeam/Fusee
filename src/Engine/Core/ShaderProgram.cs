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
        internal Dictionary<string, IShaderParam> _paramsByName;

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
            _paramsByName = new Dictionary<string, IShaderParam>();
            foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            {
                _paramsByName.Add(info.Name, info.Handle);
            }
            //_paramsByName = new Dictionary<string, ShaderParamInfo>();
            //foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            //{
            //    ShaderParamInfo newInfo = new ShaderParamInfo()
            //                                  {
            //                                      Handle = info.Handle,
            //                                      Name = info.Name,
            //                                      Type = info.Type,
            //                                      Size = info.Size,
            //                                  };
            //    _paramsByName.Add(info.Name, newInfo);
            //}
        }
        
        //public IShaderParam GetShaderParam(string paramName)
        //{
        //    ShaderParamInfo ret;
        //    if (_paramsByName.TryGetValue(paramName, out ret))
        //        return ret.Handle;
        //    //ret = _rci.GetShaderParam(_spi, paramName);
        //    //if (ret != null)
        //        _paramsByName[paramName] = ret;
        //    return ret.Handle;
        //}

        /// <summary>
        /// Gets the shader parameter.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>A <see cref="IShaderParam"/>.</returns>
        public IShaderParam GetShaderParam(string paramName)
        {
            IShaderParam ret;
            if (_paramsByName.TryGetValue(paramName, out ret))
                return ret;
            else
                return null;
            /*
            ret = _rci.GetShaderParam(_spi, paramName);
            if (ret != null)
                _paramsByName[paramName] = ret;
            return ret;*/
        }

        #endregion

        // TODO: add SetParameter methods here (remove from render context).
    }
}
