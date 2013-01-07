using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public struct ShaderParamInfo
    {
        public int Size;
        public Type Type;
        public string Name;
        public IShaderParam Handle;
    }
}
