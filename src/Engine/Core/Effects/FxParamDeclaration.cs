using System;
using System.Diagnostics;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// A parameter declaration contains the name and type of the shader parameter, as well as a flag, that indicates in which types of shaders this parameter is used.
    /// </summary>
    public interface IFxParamDeclaration
    {
        /// <summary>
        /// The name of the parameter. Must be unique in the used Effect.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Hash code of the Name, set in the Setter of "Name".
        /// </summary>
        int Hash { get; }

        /// <summary>
        /// The Type of the parameter.
        /// </summary>
        Type ParamType { get; }

        /// <summary>
        /// Sets the value of this parameter declaration. Implementations should provide a type check using <see cref="ParamType"/>.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool SetValue(object val);
    }

    /// <summary>
    /// A data type for the list of (uniform) parameters possibly occurring in one of the shaders in the various passes.
    /// Each of this array entry consists of the parameter's name and its initial value. The concrete type of the object also indicates the
    /// parameter's type.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public struct FxParamDeclaration<T> : IFxParamDeclaration
    {
        /// <summary>
        /// The name of the parameter. Must be unique in the used Effect.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Hash = value.GetHashCode();
            }
        }
        private string _name;

        /// <summary>
        /// Hash code of the Name.
        /// </summary>
        public int Hash { get; private set; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public T Value;

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type ParamType => typeof(T);

        /// <summary>
        /// Sets the value of this parameter declaration.
        /// </summary>
        /// <param name="val">The new parameter value.</param>
        /// <returns></returns>
        public bool SetValue(object val)
        {
            if (ParamType != typeof(T))
                throw new ArgumentException($"{val} has the wrong Type!");
            else
            {
                if (Value != null && Value.Equals((T)val))
                    return false;
                Value = (T)val;
                return true;
            }
        }
    }
}