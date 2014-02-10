using System;
using System.Dynamic;
using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{

    struct ParamPair
    {
        public ShaderParamInfo ParamInfo;
        public object Value;
    }

    /// <summary>
    /// A material is the combination of a ShaderEffect together with a list of value that can be set on the parameters exposed by the ShaderEffect
    /// </summary>
    public class MaterialX : DynamicObject
    {
        private ShaderProgram _sp;
        private Dictionary<string, ParamPair> _list;

        // Create uninitialized Material. Param List will be stored, but Params have no value set.
        // You will need to set the value before using the material.
        public MaterialX(ShaderProgram program)
        {
            _sp = program;
            _list = new Dictionary<string, ParamPair>();
            foreach (KeyValuePair<string, ShaderParamInfo> k in _sp._paramsByName)
            {
                var pair = new ParamPair
                               {
                                   ParamInfo = k.Value,
                                   Value = null  //_sp._rci.GetParamValue(program._spi, k.Value.Handle)
                               };
                _list.Add(k.Key, pair);
            }
            
        }

        /*
        public override bool TryGetMember(GetMemberBinder binder, out Object result)
        {
            if (_list.TryGetValue(binder.Name, out ParamPair)
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, Object value)
        {
            return true;
        }
        */

        // Create Ready to Use Material. All Parameters initialized.
        //public ShaderMaterial(string materialname, RenderContext RC)
        //{
        //    _sp = Shaders.GetShader(materialname, RC);
        //    _list = Shaders.GetParams(materialname);
        //    RC.SetShader(_sp);
        //    UpdateMaterial(RC);
        //}

        //public void SetValue(string name, int value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}
        //public void SetValue(string name, float value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}
        //public void SetValue(string name, float2 value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}
        //public void SetValue(string name, float3 value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}
        //public void SetValue(string name, float4 value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}
        //public void SetValue(string name, float4x4 value)
        //{
        //    ShaderParamInfo info;
        //    if (_sp._paramsByName.TryGetValue(name, out info))
        //        _sp._rci.SetShaderParam(info.Handle, value);
        //    ParamPair pair;
        //    pair.ParamInfo = info;
        //    pair.Value = value;
        //    if (_list.ContainsKey(name))
        //        _list[name] = pair;
        //}


 

         public ShaderProgram GetShader()
        {
            return _sp;
        }

        public void UpdateMaterial(RenderContext rc)
        {
            
            foreach (KeyValuePair<string, ParamPair> k in _list)
            {
                IShaderParam shaderparam;
                if ((shaderparam = _sp.GetShaderParam(k.Key)) != null)
                {
                    if(k.Value.ParamInfo.Type == typeof(int))
                        rc.SetShaderParam(shaderparam, (int)k.Value.Value);
                    else if (k.Value.ParamInfo.Type == typeof(float))
                        rc.SetShaderParam(shaderparam, (float)k.Value.Value);
                    else if (k.Value.ParamInfo.Type == typeof(float2))
                        rc.SetShaderParam(shaderparam, (float2)k.Value.Value);
                    else if (k.Value.ParamInfo.Type == typeof(float3))
                        rc.SetShaderParam(shaderparam, (float3)k.Value.Value);
                    else if (k.Value.ParamInfo.Type == typeof(float4))
                        rc.SetShaderParam(shaderparam, (float4)k.Value.Value);
                    else if (k.Value.ParamInfo.Type == typeof(float4x4))
                        rc.SetShaderParam(shaderparam, (float4x4)k.Value.Value);
                }
            }
        }
    }   
}
