using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Provides an Event for notifying a property change.
    /// </summary>
    public interface INotifyValueChange<T> where T : EventArgs
    {
        /// <summary>
        /// Event to notify a about a changed value of a property of this class.
        /// </summary>
        event EventHandler<T> PropertyChanged;

        /// <summary>
        /// This method needs to be called by the setter of each property.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        void NotifyValueChanged(Type type, string name, object value);
    }
}