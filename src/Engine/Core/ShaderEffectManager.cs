using System;
using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    internal class ShaderEffectManager
    {
        private readonly IRenderContextImp _rci;

        private readonly Stack<ShaderEffect> _shaderEffectsToBeDeleted = new Stack<ShaderEffect>();

        private readonly Dictionary<Suid, ShaderEffect> _allShaderEffects = new Dictionary<Suid, ShaderEffect>();

        private void Remove(ShaderEffect ef)
        {
            foreach (var program in ef.CompiledShaders)
            {
                _rci.RemoveShader(program._spi);
            }
        }

        private void ShaderEffectChanged(object sender, ShaderEffectEventArgs args)
        {
            if (args == null || sender == null) return;
            switch (args.Changed)
            {
                case ShaderEffectChangedEnum.DISPOSE:
                    Remove(sender as ShaderEffect);
                    break;
                case ShaderEffectChangedEnum.CHANGED_EFFECT_PARAM: // TODO: Redundant code ref: public void SetShaderParamT(EffectParam param) in RenderContext
                    SetShaderParams(args.EffectParameter);
                    break;
                case ShaderEffectChangedEnum.CHANGED_UNKNOWN_EFFECT_PARAM:
                    // update ShaderParamList
                    var ef = sender as ShaderEffect;
                    if (ef != null)
                    {
                        foreach (var program in args.Effect.CompiledShaders)
                        {
                            //var unknownParamHandle = _rci.GetShaderParam(program._spi, args.UnknownUniformName);
                            ShaderParamInfo param;
                            if (program._paramsByName.TryGetValue(args.UnknownUniformName, out param))
                            {
                                for (var i = 0; i < ef.VertexShaderSrc.Length; i++)
                                {
                                    var tmpEffectParam = new EffectParam
                                    {
                                        Info = new ShaderParamInfo
                                        {
                                            Handle = param.Handle,
                                            Name = args.UnknownUniformName,
                                            Type = args.UnknownUniformObject.GetType()
                                        },
                                        ShaderInxs = new List<int> { i },
                                        Value = args.UnknownUniformObject
                                    };
                                    SetShaderParams(tmpEffectParam);

                                    ef.ParamDecl.Add(args.UnknownUniformName, args.UnknownUniformObject);
                                    ef.Parameters.Add(args.UnknownUniformName, tmpEffectParam);

                                }
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void SetShaderParams(EffectParam param)
        {

            if (param.Info.Type == typeof(int))
            {
                _rci.SetShaderParam(param.Info.Handle, (int)param.Value);
            }
            else if (param.Info.Type == typeof(float))
            {
                _rci.SetShaderParam(param.Info.Handle, (float)param.Value);
            }
            else if (param.Info.Type == typeof(float2))
            {
                _rci.SetShaderParam(param.Info.Handle, (float2)param.Value);
            }
            else if (param.Info.Type == typeof(float3))
            {
                _rci.SetShaderParam(param.Info.Handle, (float3)param.Value);
            }
            else if (param.Info.Type == typeof(float4))
            {
                _rci.SetShaderParam(param.Info.Handle, (float4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4))
            {
                _rci.SetShaderParam(param.Info.Handle, (float4x4)param.Value);
            }
            else if (param.Info.Type == typeof(float4x4[]))
            {
                _rci.SetShaderParam(param.Info.Handle, (float4x4[])param.Value);
            }
            else if (param.Info.Type == typeof(ITextureHandle))
            {
                _rci.SetShaderParamTexture(param.Info.Handle, (ITextureHandle)param.Value);
            }
            // Nothing to do here, for further implementation
        }

        public void RegisterShaderEffect(ShaderEffect ef)
        {
            if (GetShaderEffect(ef) != null) return;

            // Setup handler to observe changes of the mesh data and dispose event (deallocation)
            ef.ShaderEffectChanged += ShaderEffectChanged;

            _allShaderEffects.Add(ef.SessionUniqueIdentifier, ef);

        }

        /// <summary>
        /// Creates a new Instance of ShaderEffectManager. Th instance is handling the memory allocation and deallocation on the GPU by observing ShaderEffect.cs objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation. See RegisterShaderEffect.</param>
        public ShaderEffectManager(IRenderContextImp renderContextImp)
        {
            _rci = renderContextImp;
        }

        public ShaderEffect GetShaderEffect(ShaderEffect ef)
        {
            ShaderEffect shaderEffect;
            return _allShaderEffects.TryGetValue(ef.SessionUniqueIdentifier, out shaderEffect) ? shaderEffect : null;
        }

        /// <summary>
        /// Call this method on the mainthread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            while (_shaderEffectsToBeDeleted.Count > 0)
            {
                var tmPop = _shaderEffectsToBeDeleted.Pop();
                Remove(tmPop);
            }
        }

    }
}