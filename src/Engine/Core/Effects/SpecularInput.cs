using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class SpecularInput
    {
        /// <summary>
        /// The albedo color.
        /// </summary>
        public float4 Albedo
        {
            get => _albedo;

            set
            {
                if (value != _albedo)
                {
                    _albedo = value;
                    NotifyPropertyChanged(_albedo.GetType(), nameof(Albedo), _albedo);
                }
            }
        }
        private float4 _albedo;
        
        /// <summary>
        /// The strength of the specular lighting.
        /// </summary>
        public float SpecularStrength
        {
            get => _specularStrength;
            set
            {
                if (value != _specularStrength)
                {
                    _specularStrength = value;
                    NotifyPropertyChanged(_specularStrength.GetType(), nameof(SpecularStrength), _specularStrength);
                }
            }
        }
        private float _specularStrength;
        
        /// <summary>
        /// The shininess of the specular lighting.
        /// </summary>
        public float Shininess
        {
            get => _shininess;
            set
            {
                if (value != _shininess)
                {
                    _shininess = value;
                    NotifyPropertyChanged(_shininess.GetType(), nameof(Shininess), _shininess);
                }
            }
        }
        private float _shininess;

        /// <summary>
        /// Event to notify a <see cref="SurfaceEffect"/> about a changed value of a property of this class.
        /// </summary>
        public event EventHandler<SurfaceEffectEventArgs> PropertyChanged;

        /// <summary>
        /// This method needs to be called by the Set accessor of each property.
        /// A <see cref="SurfaceEffect"/> can register <see cref="Effect.SetFxParam{T}(string, T)"/> to the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        protected void NotifyPropertyChanged(Type type, string name, object value)
        {
            PropertyChanged?.Invoke(this, new SurfaceEffectEventArgs(type, name, value));
        }
    }
}
