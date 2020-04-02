using Fusee.Math.Core;
using System;
using Fusee.Engine.Core.ShaderShards;

namespace Fusee.Engine.Core.Effects
{
    public class SpecularInput
    {
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
        
        public float4 Specular
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
        private float4 _specular;
        
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

        public event EventHandler<SurfaceEffectEventArgs> PropertyChanged;

        // This method needs to be called by the Set accessor of each property.
        protected void NotifyPropertyChanged(Type type, string name, object value)
        {
            PropertyChanged?.Invoke(this, new SurfaceEffectEventArgs(type, name, value));
        }
    }
}
