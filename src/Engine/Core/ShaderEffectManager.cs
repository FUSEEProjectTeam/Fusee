using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    internal class ShaderEffectManager
    {
        private readonly RenderContext _rc;

        private readonly Stack<ShaderEffect> _shaderEffectsToBeDeleted = new Stack<ShaderEffect>();

        private readonly Dictionary<Suid, ShaderEffect> _allShaderEffects = new Dictionary<Suid, ShaderEffect>();

        private void Remove(ShaderEffect ef)
        {
            _rc.RemoveShader(ef);
        }

        private void ShaderEffectChanged(object sender, ShaderEffectEventArgs args)
        {
            if (args == null || sender == null) return;

            // ReSharper disable once InconsistentNaming
            var senderSF = sender as ShaderEffect;

            switch (args.Changed)
            {
                case ShaderEffectChangedEnum.DISPOSE:
                    Remove(senderSF);
                    break;
                case ShaderEffectChangedEnum.UNIFORM_VAR_UPDATED:
                    _rc.UpdateParameterInCompiledEffect(senderSF, args.ChangedEffectVarName, args.ChangedEffectVarValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"ShaderEffectChanged event called with unknown arguments: {args}, calling ShaderEffect: {sender as ShaderEffect}");
            }
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
        public ShaderEffectManager(RenderContext renderContextImp)
        {
            _rc = renderContextImp;
        }

        public ShaderEffect GetShaderEffect(ShaderEffect ef)
        {
            return _allShaderEffects.TryGetValue(ef.SessionUniqueIdentifier, out var shaderEffect) ? shaderEffect : null;
        }

        /// <summary>
        /// Call this method on the mainthread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            while (_shaderEffectsToBeDeleted.Count > 0)
            {
                var tmPop = _shaderEffectsToBeDeleted.Pop();
                // remove one ShaderEffect from _allShaderEffects
                _allShaderEffects.Remove(tmPop.SessionUniqueIdentifier);
                // Remove one ShaderEffect from Memory
                Remove(tmPop);
            }
        }

    }
}