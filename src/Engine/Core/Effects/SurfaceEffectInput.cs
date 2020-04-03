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
    public class SpecularPbrInput : ColorInput
    {
        /// <summary>
        /// The roughness of the specular lighting.
        /// </summary>
        public float Roughness
        {
            get => _roughness;
            set
            {
                if (value != _roughness)
                {
                    _roughness = value;
                    NotifyPropertyChanged(_roughness.GetType(), nameof(Roughness), _roughness);
                }
            }
        }
        private float _roughness;

        /// <summary>
        /// The diffuse fraction of the specular lighting.
        /// </summary>
        public float DiffuseFraction
        {
            get => _diffuseFraction;
            set
            {
                if (value != _diffuseFraction)
                {
                    _diffuseFraction = value;
                    NotifyPropertyChanged(_diffuseFraction.GetType(), nameof(DiffuseFraction), _diffuseFraction);
                }
            }
        }
        private float _diffuseFraction;

        /// <summary>
        /// The diffuse fraction of the specular lighting.
        /// </summary>
        public float FresnelReflectance
        {
            get => _fresnelReflectance;
            set
            {
                if (value != _fresnelReflectance)
                {
                    _fresnelReflectance = value;
                    NotifyPropertyChanged(_fresnelReflectance.GetType(), nameof(FresnelReflectance), _fresnelReflectance);
                }
            }
        }
        private float _fresnelReflectance;
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
        public float AlbedoMix
        {
            get => _albedoMix;
            set
            {
                if (value != _albedoMix)
                {
                    _albedoMix = value;
                    NotifyPropertyChanged(_albedoMix.GetType(), nameof(AlbedoMix), _albedoMix);
                }
            }
        }
        private float _albedoMix;

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

        /// <summary>
        /// The normal texture.
        /// </summary>
        public float NormalMappingStrength
        {
            get => _normalMappingStrength;
            set
            {
                if (value != _normalMappingStrength)
                {
                    _normalMappingStrength = value;
                    NotifyPropertyChanged(_normalMappingStrength.GetType(), nameof(NormalMappingStrength), _normalMappingStrength);
                }
            }
        }
        private float _normalMappingStrength;

        /// <summary>
        /// The normal texture.
        /// </summary>
        public float2 TexTiles
        {
            get => _texTiles;
            set
            {
                if (value != _texTiles)
                {
                    _texTiles = value;
                    NotifyPropertyChanged(_texTiles.GetType(), nameof(TexTiles), _texTiles);
                }
            }
        }
        private float2 _texTiles;
    }
}
