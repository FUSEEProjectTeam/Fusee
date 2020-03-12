using System;

namespace Fusee.Engine.Core.ShaderEffects
{
    internal class ShaderEffectEventArgs : EventArgs
    {
        internal ShaderEffect Effect { get; }
        internal ChangedEnum Changed { get; set; }
        internal FxParam EffectParameter { get; }
        internal string ChangedEffectVarName { get; set; }
        internal object ChangedEffectVarValue { get; set; }

        internal ShaderEffectEventArgs(ShaderEffect effect, ChangedEnum changed, string changedName = null, object changedValue = null)
        {
            Effect = effect;
            Changed = changed;

            if (changedName == null || changedValue == null) return;

            ChangedEffectVarName = changedName;
            ChangedEffectVarValue = changedValue;
        }
    }
}
