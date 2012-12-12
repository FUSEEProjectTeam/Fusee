using System;
using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class Material
    {
        private ShaderProgram _sp;
        private Dictionary<string, dynamic> _list; 

        public Material(ShaderProgram program)
        {
            _sp = program;
            _list = new Dictionary<string, dynamic>();
            foreach (KeyValuePair<string, ShaderParamInfo> k in _sp._paramsByName)
            {
                Console.WriteLine(k.Key);
                _list.Add(k.Key,_sp._rci.GetParamValue(program._spi, k.Value.Handle));
            }
        }

        public void SetValue(string name, dynamic f)
        {
            ShaderParamInfo info;
            if (_sp._paramsByName.TryGetValue(name, out info))
                _sp._rci.SetShaderParam(info.Handle, f);
            // TODO: save value for later use
        }

        public void UpdateMaterial(RenderContext rc)
        {
            foreach (KeyValuePair<string, dynamic> k in _list)
            {
                IShaderParam sp;
                if ((sp = _sp.GetShaderParam(k.Key)) != null)
                    rc.SetShaderParam(sp, k.Value);
            }
        }


    }   
}
