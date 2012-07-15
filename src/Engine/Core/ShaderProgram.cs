using System.Collections.Generic;

namespace Fusee.Engine
{
    public class ShaderProgram
    {
        internal IShaderProgramImp _spi;
        internal IRenderContextImp _rci;
        private Dictionary<string, IShaderParam> _paramsByName;

        public ShaderProgram(IRenderContextImp renderContextImp, IShaderProgramImp shaderProgramImp)
        {
            _spi = shaderProgramImp;
            _rci = renderContextImp;
            _paramsByName = new Dictionary<string, IShaderParam>();
        }

        public IShaderParam GetShaderParam(string paramName)
        {
            IShaderParam ret;
            if (_paramsByName.TryGetValue(paramName, out ret))
                return ret;
            ret = _rci.GetShaderParam(_spi, paramName);
            if (ret != null)
                _paramsByName[paramName] = ret;
            return ret;
        }

        // TODO: add SetParameter methods here (remove from render context).
    }
}
