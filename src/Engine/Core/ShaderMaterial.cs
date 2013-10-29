using System;
using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    // TODO: Implement proper Material handler
    /// <summary>
    /// Handles settings of shaderprograms. Currently not implemented.
    /// </summary>
    public class ShaderMaterial
    {
        #region Fields

        private ShaderProgram _sp;
        //private Dictionary<string, dynamic> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderMaterial"/> class.
        /// </summary>
        /// <param name="program">The <see cref="ShaderProgram"/>.</param>
        public ShaderMaterial(ShaderProgram program)
        {
            _sp = program;
            //_list = new Dictionary<string, dynamic>();
            //foreach (KeyValuePair<string, ShaderParamInfo> k in _sp._paramsByName)
            //{
            //    _list.Add(k.Key, _sp._rci.GetParamValue(program._spi, k.Value.Handle));
            //}
        }

//        public void SetValue(string name, dynamic value)
//        {
//            ShaderParamInfo info;
//            if (_sp._paramsByName.TryGetValue(name, out info))
//                _sp._rci.SetShaderParam(info.Handle, value);
//            if (_list.ContainsKey(name))
//                _list[name] = value;
//        }

        /// <summary>
        /// Gets the shaderprogram.
        /// </summary>
        /// <returns>A <see cref="ShaderProgram"/>.</returns>
        public ShaderProgram GetShader()
        {
            return _sp;
        }

        #endregion

        #region Members

        /// <summary>
        /// Updates the material.
        /// </summary>
        /// <param name="rc">The rc.</param>
        public void UpdateMaterial(RenderContext rc)
        {
            rc.SetShader(_sp);
        }

        #endregion
    }   
}
