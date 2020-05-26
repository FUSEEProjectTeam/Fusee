using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Event arguments for notifying a <see cref="SurfaceEffect"/> about changes in a property of a class (for example see <see cref="ColorInput"/>).
    /// </summary>
    public class SurfaceEffectEventArgs : EventArgs
    {
        /// <summary>
        /// The type of the property.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// The name of the property.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The value of the property.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Creates a new instance of type "SurfaceEffectEventArgs".
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public SurfaceEffectEventArgs(Type type, string name, object value) => (Type, Name, Value) = (type, name, value);

    }
}