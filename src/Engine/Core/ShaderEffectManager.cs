using System;
using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    internal class ShaderEffectManager
    {
        private readonly IRenderContextImp _rci;

        private readonly Stack<ShaderEffect>  _shaderEffectsToBeDeleted = new Stack<ShaderEffect>();

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
                case ShaderEffectChangedEnum.CHANGED_EFFECT_PARAM:
                    // Nothing to do here, for further implementation
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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