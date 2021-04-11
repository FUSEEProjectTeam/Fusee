using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    internal class EffectManager
    {
        private readonly RenderContext _rc;

        private readonly Stack<Effect> _effectsToBeDeleted = new Stack<Effect>();

        private readonly Dictionary<Suid, Effect> _allEffects = new Dictionary<Suid, Effect>();

        private void EffectChanged(object sender, EffectManagerEventArgs args)
        {
            if (args == null || sender == null) return;

            // ReSharper disable once InconsistentNaming
            var senderSF = sender as Effect;

            switch (args.Changed)
            {
                case UniformChangedEnum.Dispose:
                    _effectsToBeDeleted.Push(senderSF);
                    break;
                case UniformChangedEnum.Update:
                    _rc.UpdateParameterInCompiledEffect(senderSF, args.ChangedUniformName, args.ChangedUniformValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"EffectChanged event called with unknown arguments: {args}, calling Effect: {sender as Effect}");
            }
        }

        public void RegisterEffect(Effect ef)
        {
            if (GetEffect(ef) != null) return;

            // Setup handler to observe changes of the mesh data and dispose event (deallocation)
            ef.EffectChanged += EffectChanged;

            _allEffects.Add(ef.SessionUniqueIdentifier, ef);

        }

        /// <summary>
        /// Creates a new Instance of EffectManager. Th instance is handling the memory allocation and deallocation on the GPU by observing Effect.cs objects.
        /// </summary>
        /// <param name="renderContextImp">The RenderContextImp is used for GPU memory allocation and deallocation. See RegisterEffect.</param>
        public EffectManager(RenderContext renderContextImp)
        {
            _rc = renderContextImp;
        }

        public Effect GetEffect(Effect ef)
        {
            return _allEffects.TryGetValue(ef.SessionUniqueIdentifier, out var effect) ? effect : null;
        }

        /// <summary>
        /// Call this method on the main thread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {
            while (_effectsToBeDeleted.Count > 0)
            {
                var tmPop = _effectsToBeDeleted.Pop();
                // remove one Effect from _allEffects
                _allEffects.Remove(tmPop.SessionUniqueIdentifier);
                // Remove one Effect from Memory
                _rc.RemoveShader(tmPop);
            }
        }

    }
}