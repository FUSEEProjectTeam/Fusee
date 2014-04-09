using System;
using System.Collections.Generic;
using System.Dynamic;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// An effect pass declaration contains the vertex and pixel shader source code as well as a <see cref="RenderStateSet"/>
    /// declaration for the rendering pass declared by this instance.
    /// </summary>
    public struct EffectPassDeclaration
    {
        public RenderStateSet StateSet;
        public string VS;
        public string PS;
    }

    public struct EffectParameterDeclaration
    {
        public string Name;
        public object Value;
    }

    internal sealed class EffectParam
    {
        public ShaderParamInfo Info;
        public Object Value;
        public List<int> ShaderInxs;
    }

    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and a
    /// pair of Pixel and Vertex Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffect
    {
        private readonly RenderStateSet[] _states;
        private ShaderProgram[] _compiledShaders; 
        private readonly string[] _vertexShaderSrc;
        private readonly string[] _pixelShaderSrc;
        private Dictionary<string, EffectParam> _parameters;
        private List<List<EffectParam>> _paramsPerPass;
        private Dictionary<string, object> _paramDecl;

        internal RenderContext _rc;

        /// <summary>
        /// The constructor to create a shader effect.
        /// </summary>
        /// <param name="effectPasses">The ordered array of <see cref="EffectPassDeclaration"/> items. The first item
        /// in the array is the first pass applied to rendered geometry, and so on.</param>
        /// <param name="effectParameters">A list of (uniform) parameters possibliy occurring in one of the shaders in the various passes.
        /// Each array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
        /// parameter's type.
        /// </param>
        /// <remarks>Make sure to list any parameter in any of the different passes' shaders you want to change later on in the effectParameters
        /// list. Shaders must not contain paramaeters with names listed in the effectParameters but declared with different types than those of 
        /// the respective default values given here.</remarks>
        public ShaderEffect(EffectPassDeclaration[] effectPasses, IEnumerable<EffectParameterDeclaration> effectParameters)
        {
            if (effectPasses == null || effectPasses.Length == 0)
                throw new ArgumentNullException("effectPasses", "must not be null and must contain at least one pass");
            
            int nPasses = effectPasses.Length;
            
            _states = new RenderStateSet[nPasses];
            _compiledShaders = new ShaderProgram[nPasses];
            _vertexShaderSrc = new string[nPasses];
            _pixelShaderSrc = new string[nPasses];

            for (int i = 0; i < nPasses; i++)
            {
                _states[i] = effectPasses[i].StateSet;
                _vertexShaderSrc[i] = effectPasses[i].VS;
                _pixelShaderSrc[i] = effectPasses[i].PS;
            }

            _paramDecl = new Dictionary<string, object>();

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    _paramDecl.Add(param.Name, param.Value);
                }
            }
        }

        /// <summary>
        /// Attaches this instance to a RenderContext. 
        /// </summary>
        /// <param name="rc">The Render Context to attach to.</param>
        /// <remarks>A ShaderEffect must be attached to a context before you can render geometry with it. The main
        /// task performed in this method is compiling the provided shader source code and uploading the shaders to
        /// the gpu under the provided RenderContext.</remarks>
        public void AttachToContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc", "must pass a valid render context.");

            _rc = rc;
            int i=0, nPasses = _vertexShaderSrc.Length;

            try // to compile all the shaders
            {
                for (i = 0; i < nPasses; i++)
                {
                    _compiledShaders[i] = _rc.CreateShader(_vertexShaderSrc[i], _pixelShaderSrc[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while compiling shader for pass " + i, ex);
            }

            // Enumerate all shader parameters of all passes and enlist them in lookup tables
            _parameters = new Dictionary<string, EffectParam>();
            _paramsPerPass = new List<List<EffectParam>>();
            for (i = 0; i < nPasses; i++)
            {
                IEnumerable<ShaderParamInfo> paramList = _rc.GetShaderParamList(_compiledShaders[i]);
                _paramsPerPass.Add(new List<EffectParam>());
                foreach (var paramNew in paramList)
                {
                    Object initValue;
                    if (_paramDecl.TryGetValue(paramNew.Name, out initValue))
                    {
                        // IsAssignableFrom the boxed initValue object will cause JSIL to give an answer based on the value of the contents
                        // If the type originally was float but contains an integral value (e.g. 3), JSIL.GetType() will return Integer...
                        // Thus for primitve types (float, int, ) we hack a check ourselves. For other types (float2, ..) IsAssignableFrom works well.

                        // ReSharper disable UseMethodIsInstanceOfType
                        // ReSharper disable OperatorIsCanBeUsed
                        var initValType = initValue.GetType();
                        if ( !( ( (paramNew.Type == typeof (int) || paramNew.Type == typeof (float)) 
                                  &&
                                  (initValType == typeof (int) || initValType == typeof (float) || initValType == typeof (double))
                                )
                                ||
                                (paramNew.Type.IsAssignableFrom(initValType))
                              )
                           )
                        {
                            throw new Exception("Error preparing effect pass " + i + ". Shader parameter " + paramNew.Type.ToString() + " " + paramNew.Name +
                                                " was defined as " + initValType.ToString() + " " + paramNew.Name + " during initialization (different types).");                            
                        }
                        // ReSharper restore OperatorIsCanBeUsed
                        // ReSharper restore UseMethodIsInstanceOfType

                        // Parameter was declared by user and type is correct in shader - carry on.
                        EffectParam paramExisting;
                        if (_parameters.TryGetValue(paramNew.Name, out paramExisting))
                        {
                            // The parameter is already there from a previous pass.
                            if (paramExisting.Info.Size != paramNew.Size || paramExisting.Info.Type != paramNew.Type)
                            {
                                // This should never happen due to the previous error check. Check it anyway...
                                throw new Exception("Error preparing effect pass " + i + ". Shader parameter " +
                                                    paramNew.Name +
                                                    " already defined with a different type in effect pass " +
                                                    paramExisting.ShaderInxs[0]);
                            }
                            // List the current pass to use this shader parameter
                            paramExisting.ShaderInxs.Add(i);
                        }
                        else
                        {
                            paramExisting = new EffectParam()
                                {
                                    Info = paramNew,
                                    ShaderInxs = new List<int>(new int[] {i}),
                                    Value = initValue
                                };
                            _parameters.Add(paramNew.Name, paramExisting);
                        }
                        _paramsPerPass[i].Add(paramExisting);
                    }
                }
            }                   
        }

        /// <summary>
        /// Detaches the shader effect from a given context.
        /// </summary>
        public void DetachFromContext()
        {
            _parameters = null;
            _paramsPerPass = null;
            _paramDecl = null;
            _compiledShaders = null;
            _rc = null;
        }

        /// <summary>
        /// Renders geometry on the attached RenderContext using this shader effect. All rendering passes are applied 
        /// to the geometry in the order of appearance within the <see cref="EffectPassDeclaration"/> array provided
        /// in the constructor.
        /// </summary>
        /// <param name="mesh">The mesh to render.</param>
        public void RenderMesh(Mesh mesh)
        {
            int i = 0, nPasses = _vertexShaderSrc.Length;
            try
            {
                for (i = 0; i < nPasses; i++)
                {
                    // TODO: Use shared uniform paramters - currently SetShader will query the shader params and set all the common uniforms (like matrices and light)
                    _rc.SetShader(_compiledShaders[i]);
                    foreach (var param in _paramsPerPass[i])
                    {
                        if (param.Info.Type == typeof (int))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (int) param.Value);
                        }
                        else if (param.Info.Type == typeof (float))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (float) param.Value);
                        }
                        else if (param.Info.Type == typeof(float2))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (float2)param.Value);
                        }
                        else if (param.Info.Type == typeof(float3))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (float3)param.Value);
                        }
                        else if (param.Info.Type == typeof(float4))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (float4)param.Value);
                        }
                        else if (param.Info.Type == typeof(float4x4))
                        {
                            _rc.SetShaderParam(param.Info.Handle, (float4x4)param.Value);
                        }
                        else if (param.Info.Type == typeof(ITexture))
                        {
                            _rc.SetShaderParamTexture(param.Info.Handle, (ITexture) param.Value);
                        }
                    }
                    _rc.SetRenderState(_states[i]);

                    // TODO: split up RenderContext.Render into a preparation and a draw call so that we can prepare a mesh once and draw it for each pass.
                    _rc.Render(mesh);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while rendering pass " + i, ex);
            }
        }

        public void SetEffectParam(string name, object value)
        {
            EffectParam param;

            if (_parameters != null)
                if (_parameters.TryGetValue(name, out param))
                {
                    param.Value = value;
                }           
        }

        public object GetEffectParam(string name)
        {
            EffectParam param;
            if (_parameters.TryGetValue(name, out param))
            {
                return param.Value;
            }
            return null;
        }
    }
}
