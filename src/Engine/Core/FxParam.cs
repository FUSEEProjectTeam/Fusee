using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    internal sealed class FxParam
    {
        internal ShaderParamInfo Info;

        internal object Value
        {
            get { return _value; }
            set
            {
                HasValueChanged = true;
                _value = value;
            }
        }
        private object _value;

        internal bool HasValueChanged { get; set; } = true;
        
        internal bool HasValueChangedOverride = false;
    }
}