using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Parameters used in <see cref="CompiledEffects"/>.
    /// </summary>
    internal sealed class FxParam
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