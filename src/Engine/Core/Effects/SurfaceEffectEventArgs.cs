using System;
using System.Runtime.CompilerServices;

namespace Fusee.Engine.Core.Effects
{
    public class SurfaceEffectEventArgs : EventArgs
    {
        public readonly Type Type;
        public readonly string Name;
        public readonly object Value;

        public SurfaceEffectEventArgs(Type type, string name, object value) => (Type, Name, Value) = (type, name, value);

    }
}
