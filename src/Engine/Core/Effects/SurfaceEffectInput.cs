using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a effect that does no lighting calculation.
    /// In this case it only contains the albedo color.
    /// </summary>
    public abstract class SurfaceInput : INotifyValueChange<SurfaceEffectEventArgs>
    {
        /// <summary>
        /// The <see cref="ShaderShards.ShadingModel"/>, appropriate for this Input.
        /// </summary>
        [NoUniform]
        public ShadingModel ShadingModel { get; protected set; } = ShadingModel.Unlit;

        /// <summary>
        /// The <see cref="ShaderShards.TextureSetup"/>.
        /// </summary>
        [NoUniform]
        public TextureSetup TextureSetup { get; set; } = TextureSetup.NoTextures;

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
                    NotifyValueChanged(_albedo.GetType(), nameof(Albedo), _albedo);
                }
            }
        }
        private float4 _albedo;

        /// <summary>
        /// Event to notify a <see cref="SurfaceEffectBase"/> about a changed value of a property of this class.
        /// </summary>
        public event EventHandler<SurfaceEffectEventArgs> PropertyChanged;

        /// <summary>
        /// This method needs to be called by the setter of each property.
        /// A <see cref="SurfaceEffectBase"/> can register <see cref="Effect.SetFxParam{T}(string, T)"/> to the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void NotifyValueChanged(Type type, string name, object value)
        {
            PropertyChanged?.Invoke(this, new SurfaceEffectEventArgs(type, name, value));
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a effect that does no lighting calculation.
    /// In addition this input provides properties for albedo and normal textures.
    /// </summary>
    public class UnlitInput : SurfaceInput
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
                    NotifyValueChanged(_albedoMix.GetType(), nameof(AlbedoMix), _albedoMix);
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
                    NotifyValueChanged(_albedoTex.GetType(), nameof(AlbedoTex), _albedoTex);
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
                    NotifyValueChanged(_texTiles.GetType(), nameof(TexTiles), _texTiles);
                }
            }
        }
        private float2 _texTiles = float2.One;

        /// <summary>
        /// Creates a new instance of type <see cref="UnlitInput"/>.
        /// </summary>
        public UnlitInput()
        {
            ShadingModel = ShadingModel.Unlit;
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for diffuse only lighting calculation.
    /// In addition this input provides properties for albedo and normal textures.
    /// </summary>
    public class DiffuseInput : SurfaceInput
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
                    NotifyValueChanged(_roughness.GetType(), nameof(Roughness), _roughness);
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
                    NotifyValueChanged(_albedoMix.GetType(), nameof(AlbedoMix), _albedoMix);
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
                    NotifyValueChanged(_albedoTex.GetType(), nameof(AlbedoTex), _albedoTex);
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
                    NotifyValueChanged(_normalTex.GetType(), nameof(NormalTex), _normalTex);
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
                    NotifyValueChanged(_normalMappingStrength.GetType(), nameof(NormalMappingStrength), _normalMappingStrength);
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
                    NotifyValueChanged(_texTiles.GetType(), nameof(TexTiles), _texTiles);
                }
            }
        }
        private float2 _texTiles = float2.One;

        /// <summary>
        /// Creates a new instance of type <see cref="DiffuseInput"/>.
        /// </summary>
        public DiffuseInput()
        {
            ShadingModel = ShadingModel.DiffuseOnly;
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for specular lighting with strength and shininess.
    /// In addition this input provides properties for albedo and normal textures.
    /// </summary>
    public class SpecularInput : DiffuseInput
    {
        /// <summary>
        /// The albedo color.
        /// </summary>
        public float3 Emission
        {
            get => _emission;

            set
            {
                if (value != _emission)
                {
                    _emission = value;
                    NotifyValueChanged(_emission.GetType(), nameof(Emission), _emission);
                }
            }
        }
        private float3 _emission;

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
                    NotifyValueChanged(_specularStrength.GetType(), nameof(SpecularStrength), _specularStrength);
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
                    NotifyValueChanged(_shininess.GetType(), nameof(Shininess), _shininess);
                }
            }
        }
        private float _shininess;

        /// <summary>
        /// Creates a new instance of type <see cref="SpecularInput"/>.
        /// </summary>
        public SpecularInput()
        {
            ShadingModel = ShadingModel.DiffuseSpecular;
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a glossy lighting calculation.
    /// In addition to the albedo color this Input provides a Roughness value.
    /// </summary>
    public class GlossyInput : DiffuseInput
    {
        /// <summary>
        /// Creates a new instance of type <see cref="GlossyInput"/>.
        /// </summary>
        public GlossyInput()
        {
            ShadingModel = ShadingModel.Glossy;
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for a BRDF lighting calculation.
    /// In addition this input provides properties for albedo and normal textures.
    /// </summary>
    public class BRDFInput : DiffuseInput
    {
        /// <summary>
        /// The albedo color.
        /// </summary>
        public float3 Emission
        {
            get => _emission;

            set
            {
                if (value != _emission)
                {
                    _emission = value;
                    NotifyValueChanged(_emission.GetType(), nameof(Emission), _emission);
                }
            }
        }
        private float3 _emission;

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
                    NotifyValueChanged(_metallic.GetType(), nameof(Metallic), _metallic);
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
                    NotifyValueChanged(_specular.GetType(), nameof(Specular), _specular);
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
                    NotifyValueChanged(_ior.GetType(), nameof(IOR), _ior);
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
                    NotifyValueChanged(_ior.GetType(), nameof(Subsurface), _subsurface);
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
                    NotifyValueChanged(_subsurfaceColor.GetType(), nameof(SubsurfaceColor), _subsurfaceColor);
                }
            }
        }
        private float3 _subsurfaceColor;

        /// <summary>
        /// The albedo texture.
        /// </summary>
        public Texture ThicknessMap
        {
            get => _thicknessMap;
            set
            {
                if (value != _thicknessMap)
                {
                    _thicknessMap = value;
                    NotifyValueChanged(_thicknessMap.GetType(), nameof(ThicknessMap), _thicknessMap);
                }
            }
        }
        private Texture _thicknessMap;

        /// <summary>
        /// Creates a new instance of type <see cref="BRDFInput"/>.
        /// </summary>
        public BRDFInput()
        {
            ShadingModel = ShadingModel.BRDF;
        }
    }

    /// <summary>
    /// Class that can be used to collect properties that will serve as uniforms for eye dome lighting.
    /// NOTE: This Input is only compatible with <see cref="PointCloudSurfaceEffect"/>s right now.
    /// </summary>
    public class EdlInput : UnlitInput
    {
        /// <summary>
        /// Creates a new instance of type <see cref="EdlInput"/>.
        /// </summary>
        public EdlInput()
        {
            ShadingModel = ShadingModel.Edl;
        }
    }
}