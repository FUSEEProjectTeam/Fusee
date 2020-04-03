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
        public string ChangedUniformName { get; set; }

        /// <summary>
        /// The value of the uniform variable.
        /// </summary>
        public object ChangedUniformValue { get; set; }

        /// <summary>
        /// Creates a new instance of type "EffectManagerEventArgs".
        /// </summary>
        /// <param name="changed">The <see cref="UniformChangedEnum"/>.</param>
        /// <param name="changedName">The name of the uniform variable.</param>
        /// <param name="changedValue">The value of the uniform variable.</param>
        public EffectManagerEventArgs(UniformChangedEnum changed, string changedName = null, object changedValue = null)
        {
            Changed = changed;

            if (changedName == null || changedValue == null) return;

            ChangedUniformName = changedName;
            ChangedUniformValue = changedValue;
        }
    }
}
