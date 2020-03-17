using System;

namespace Fusee.Engine.Core.Effects
{
    public class EffectEventArgs : EventArgs
    {
        public Effect Effect { get; }
        public ChangedEnum Changed { get; set; }
        public FxParam EffectParameter { get; }
        public string ChangedEffectVarName { get; set; }
        public object ChangedEffectVarValue { get; set; }

        public EffectEventArgs(Effect effect, ChangedEnum changed, string changedName = null, object changedValue = null)
        {
            Effect = effect;
            Changed = changed;

            if (changedName == null || changedValue == null) return;

            ChangedEffectVarName = changedName;
            ChangedEffectVarValue = changedValue;
        }
    }
}
