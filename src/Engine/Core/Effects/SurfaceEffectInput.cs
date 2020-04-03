using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Provides an Event for notifying a property change.
    /// </summary>
    public interface INotifyInputChange
    {
        /// <summary>
        /// Event to notify a <see cref="SurfaceEffect"/> about a changed value of a property of this class.
        /// </summary>
        event EventHandler<SurfaceEffectEventArgs> PropertyChanged;

        /// <summary>
        /// This method needs to be called by the setter of each property.
        /// A <see cref="SurfaceEffect"/> can register <see cref="Effect.SetFxParam{T}(string, T)"/> to the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        void NotifyPropertyChanged(Type type, string name, object value);
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case it only contains the albedo color.
    /// </summary>
    public class ColorInput : INotifyInputChange
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
        /// Event to notify a <see cref="SurfaceEffect"/> about a changed value of a property of this class.
        /// </summary>
        public event EventHandler<SurfaceEffectEventArgs> PropertyChanged;

        /// <summary>
        /// This method needs to be called by the setter of each property.
        /// A <see cref="SurfaceEffect"/> can register <see cref="Effect.SetFxParam{T}(string, T)"/> to the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void NotifyPropertyChanged(Type type, string name, object value)
        {
            PropertyChanged?.Invoke(this, new SurfaceEffectEventArgs(type, name, value));
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class SpecularInput : ColorInput
    {
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
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class TextureInput : SpecularInput
    {
        /// <summary>
        /// The mix between albedo texture and albedo color.
        /// </summary>
        public float Mix
        {
            get => _mix;
            set
            {
                if (value != _mix)
                {
                    _mix = value;
                    NotifyPropertyChanged(_mix.GetType(), nameof(Mix), _mix);
                }
            }
        }
        private float _mix;

        /// <summary>
        /// The albedo texture.
        /// </summary>
        public Texture AlbedoTex
        {
            get => _albedoTex;
            set
            {
                if (value != _albedoTex)
                {
                    _albedoTex = value;
                    NotifyPropertyChanged(_albedoTex.GetType(), nameof(AlbedoTex), _albedoTex);
                }
            }
        }
        private Texture _albedoTex;

        /// <summary>
        /// The normal texture.
        /// </summary>
        public Texture NormalTex
        {
            get => _normalTex;
            set
            {
                if (value != _normalTex)
                {
                    _normalTex = value;
                    NotifyPropertyChanged(_normalTex.GetType(), nameof(NormalTex), _normalTex);
                }
            }
        }
        private Texture _normalTex;
    }
}
