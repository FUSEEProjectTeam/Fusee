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

    public class RoughnessInput : ColorInput
    {
        /// <summary>
        /// The roughness value. If 0.0 the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.
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
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for a BRDF lighting calculation.
    /// </summary>
    public class BRDFInput : ColorInput
    {
        /// <summary>
        /// The roughness value. If 0.0 the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.
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
        /// The albedo color.
        /// </summary>
        public float4 Emission
        {
            get => _emission;

            set
            {
                if (value != _emission)
                {
                    _emission = value;
                    NotifyPropertyChanged(_emission.GetType(), nameof(Emission), _emission);
                }
            }
        }
        private float4 _emission;

        /// <summary>
        /// Value used to blend between the metallic and the dielectric model.
        /// </summary>
        public float Metallic
        {
            get => _metallic;
            set
            {
                if (value != _metallic)
                {
                    _metallic = value;
                    NotifyPropertyChanged(_metallic.GetType(), nameof(Metallic), _metallic);
                }
            }
        }
        private float _metallic;

        /// <summary>
        /// Amount of dielectric specular reflection.
        /// </summary>
        public float Specular
        {
            get => _specular;
            set
            {
                if (value != _specular)
                {
                    _specular = value;
                    NotifyPropertyChanged(_specular.GetType(), nameof(Specular), _specular);
                }
            }
        }
        private float _specular;

        /// <summary>
        /// The index of refraction. Note that this is set to 0.04 for dielectrics when rendering deferred.
        /// </summary>
        public float IOR
        {
            get => _ior;
            set
            {
                if (value != _ior)
                {
                    _ior = value;
                    NotifyPropertyChanged(_ior.GetType(), nameof(IOR), _ior);
                }
            }
        }
        private float _ior;

        /// <summary>
        /// Mix between diffuse and subsurface scattering.
        /// </summary>
        public float Subsurface
        {
            get => _subsurface;
            set
            {
                if (value != _subsurface)
                {
                    _subsurface = value;
                    NotifyPropertyChanged(_ior.GetType(), nameof(Subsurface), _subsurface);
                }
            }
        }
        private float _subsurface;

        /// <summary>
        /// The color of the subsurface scattering.
        /// </summary>
        public float3 SubsurfaceColor
        {
            get => _subsurfaceColor;

            set
            {
                if (value != _subsurfaceColor)
                {
                    _subsurfaceColor = value;
                    NotifyPropertyChanged(_subsurfaceColor.GetType(), nameof(SubsurfaceColor), _subsurfaceColor);
                }
            }
        }
        private float3 _subsurfaceColor;
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class SpecularInput : RoughnessInput
    {
        /// <summary>
        /// The albedo color.
        /// </summary>
        public float4 Emission
        {
            get => _emission;

            set
            {
                if (value != _emission)
                {
                    _emission = value;
                    NotifyPropertyChanged(_emission.GetType(), nameof(Emission), _emission);
                }
            }
        }
        private float4 _emission;

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
    public class TextureInputSpecular : SpecularInput
    {
        /// <summary>
        /// The roughness value. If 0.0 the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.
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
        private float _normalMappingStrength = 1f;

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
        private float2 _texTiles = float2.One;
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class TextureInputColorUnlit : ColorInput
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
        private float2 _texTiles = float2.One;
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class TextureInputColor : TextureInputColorUnlit
    {
        /// <summary>
        /// The roughness value. If 0.0 the diffuse component gives standard Lambertian reflection, higher values activate the Oren-Nayar calculation.
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
        private float _normalMappingStrength = 1f;
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a specific lighting setup.
    /// In this case for specular lighting with strength and shininess.
    /// </summary>
    public class TextureInputBRDF : BRDFInput
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