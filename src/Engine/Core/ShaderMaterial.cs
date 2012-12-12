using System;
using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class ShaderMaterial
    {
        private ShaderProgram _sp;
        private Dictionary<string, dynamic> _list ; 

        public ShaderMaterial(ShaderProgram program)
        {
            _sp = program;
            foreach (KeyValuePair<string, ShaderParamInfo> k in _sp._paramsByName)
            {
                _list.Add(k.Key,_sp._rci.GetParamValue((IShaderProgramImp)program, k.Value.Handle));
            }
        }

        public void SetValue(string name, float f)
        {
            ShaderParamInfo info;
            if (_sp._paramsByName.TryGetValue(name, out info))
                _sp._rci.SetShaderParam(info.Handle, f);
            // TODO: save value for later use
        }

        public void UpdateMaterial(RenderContext rc)
        {
            IShaderParam sp;
            foreach (KeyValuePair<string, dynamic> k in _list)
            {
                if ((sp = _sp.GetShaderParam(k.Key)) != null)
                    rc.SetShaderParam(sp, k.Value);
            }
        }


        //public dynamic GetParam(int index)
        
        //}

        //public void SetParam(int index, dynamic value)
        //{
        //}
        /*
        public ShaderMaterial(string s, RenderContext rc)
        {
            if (s == "multiLight")
            {
                _sp = Shaders.GetShader("multiLight", rc);
                rc.SetShader(_sp);
                _list = Shaders.GetParams("multiLight");
                IShaderParam sp;
                
                foreach (ShaderParamInfo spi in rc.GetShaderParamAt(_sp))
                {
                    
                }
            }
<<<<<<< HEAD
            else if (s == "multiLight")
            {
                _sp = Shaders.GetShader("multiLight",rc);
                rc.SetShader(_sp);
                _shininess = 256.0f;
                UpdateMaterial(rc);
            }
            else if (s == "chess")
            {
                _sp = MoreShaders.GetShader("chess", rc);
                _shininess = 64;
                rc.SetShader(_sp);
                param1 = _sp.GetShaderParam("darkColor");
                param2 = _sp.GetShaderParam("brightColor");
                param3 = _sp.GetShaderParam("chessSize");
                param4 = _sp.GetShaderParam("smoothFactor");
                rc.SetShaderParam(param1, new float3(0, 0, 0));
                rc.SetShaderParam(param2, new float3(1, 1, 1));
                rc.SetShaderParam(param3, 100);
                rc.SetShaderParam(param4, 1);
                rc.SetLightPosition(0, new float3(0, 2000, 2000));
                
            }
            else
            {
                _sp = Shaders.GetShader("chess", rc);
                _shininess = 64;
                rc.SetShader(_sp);
            }
        }
        
        
        */
    }   
}
