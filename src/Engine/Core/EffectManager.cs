using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    internal class EffectManager : IDisposable
    {
        private readonly RenderContext _rc;
        private readonly Stack<Effect> _effectsToBeDeleted = new();
        private readonly Dictionary<Suid, Effect> _allEffects = new();

        // Track whether Dispose has been called.
        private bool disposed = false;

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
                // Remove one Effect from _allEffects
                _allEffects.Remove(tmPop.SessionUniqueIdentifier);
                // Remove one Effect from Memory
                _rc.RemoveShader(tmPop);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                Cleanup();

                for (int i = 0; i < _allEffects.Count; i++)
                {
                    var fx = _allEffects.ElementAt(i);
                    _allEffects.Remove(fx.Key);
                    // Remove one Effect from Memory
                    _rc.RemoveShader(fx.Value);
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~EffectManager()
        {
            Dispose(disposing: false);
        }
    }
}