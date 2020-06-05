using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    internal sealed class EffectParam
    {
        public ShaderParamInfo Info;

        public object Value
        {
            get { return _value; }
            set
            {
                HasValueChanged = true;
                _value = value;
            }
        }
        private object _value;

        public bool HasValueChanged { get; set; } = true;
    }
}