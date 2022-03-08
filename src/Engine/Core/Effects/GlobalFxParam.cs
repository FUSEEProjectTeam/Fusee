using System.Diagnostics;

namespace Fusee.Engine.Core.Effects
{
    [DebuggerDisplay("Name = {Name}")]
    internal struct GlobalFxParam
    {
        public string Name;
        public object Value;
        public bool HasValueChanged;
    }
}
