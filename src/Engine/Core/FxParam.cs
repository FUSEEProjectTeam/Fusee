using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    public sealed class FxParam
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