using System.Collections.Generic;

namespace Fusee.Engine
{
    public class ShaderProgram
    {
        internal IShaderProgramImp _spi;
        internal IRenderContextImp _rci;
        internal Dictionary<string, ShaderParamInfo> _paramsByName;

        public ShaderProgram(IRenderContextImp renderContextImp, IShaderProgramImp shaderProgramImp)
        {
            _spi = shaderProgramImp;
            _rci = renderContextImp;
            _paramsByName = new Dictionary<string, ShaderParamInfo>();
            foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            {
                ShaderParamInfo newInfo = new ShaderParamInfo()
                                              {
                                                  Handle = info.Handle,
                                                  Name = info.Name,
                                                  Type = info.Type,
                                                  Size = info.Size,
                                              };
                _paramsByName.Add(info.Name, newInfo);
            }
        }
        
        public IShaderParam GetShaderParam(string paramName)
        {
            ShaderParamInfo ret;
            if (_paramsByName.TryGetValue(paramName, out ret))
                return ret.Handle;
            //ret = _rci.GetShaderParam(_spi, paramName);
            //if (ret != null)
                _paramsByName[paramName] = ret;
            return ret.Handle;
        }

        

        // TODO: add SetParameter methods here (remove from render context).
    }
}
