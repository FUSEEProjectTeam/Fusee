using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Event arguments that are passed from an <see cref="Effect"/> to the <see cref="EffectManager"/>.
    /// </summary>
    public class EffectManagerEventArgs : EventArgs
    {
        /// <summary>
        /// Determines the action that needs to be taken for a uniform variable. 
        /// </summary>
        public UniformChangedEnum Changed { get; set; }

        /// <summary>
        /// The name of the uniform variable.
        /// </summary>
        public int ChangedUniformHash { get; set; }

        /// <summary>
        /// The value of the uniform variable.
        /// </summary>
        public object ChangedUniformValue { get; set; }

        /// <summary>
        /// Creates a new instance of type "EffectManagerEventArgs".
        /// </summary>
        /// <param name="changed">The <see cref="UniformChangedEnum"/>.</param>
        /// <param name="changedHash">The hash code of the uniform variable.</param>
        /// <param name="changedValue">The value of the uniform variable.</param>
        public EffectManagerEventArgs(UniformChangedEnum changed, int changedHash = 0, object changedValue = null)
        {
            Changed = changed;

            if (changedHash == 0 || changedValue == null) return;

            ChangedUniformHash = changedHash;
            ChangedUniformValue = changedValue;
        }
    }
}