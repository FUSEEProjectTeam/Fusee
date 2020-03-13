using Fusee.Base.Common;
using Fusee.Math.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fusee.Engine.Common
{
    
    public class Light : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int ArrayIdx;

        /// <summary>
        /// Represents the light status.
        /// </summary>
        public bool Active
        {
            get => _active;
            set
            {
                if (value != _active)
                {
                    _active = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _active;


        /// <summary>
        /// Represents the color.
        /// </summary>       
        public float4 Color
        {
            get => _color;
            set
            {
                if (value != _color)
                {
                    _color = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float4 _color;

        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>        
        public float MaxDistance;

        /// <summary>
        /// Represents the strength of the light (non-physically representation of the brightness).
        /// Should be a value between 0 and 1.
        /// </summary>        
        public float Strength;

        /// <summary>
        /// Represents the type of the light.
        /// </summary>        
        public LightType Type;

        /// <summary>
        /// Represents the outer spot angle of the light.
        /// </summary>        
        public float OuterConeAngle;

        /// <summary>
        /// Represents the spot inner angle of the light.
        /// </summary>       
        public float InnerConeAngle;

        /// <summary>
        /// Defines if a shadow map is created for this light.
        /// </summary>
        public bool IsCastingShadows;

        /// <summary>
        /// Bias for calculating shadows.
        /// </summary>
        public float Bias;

        /// <summary>
        /// Creates a new instance of type LightComponent.
        /// </summary>
        /// <param name="strength">Represents the strength of the light (non-physically representation of the brightness).</param>
        public Light(float strength = 1)
        {
            Strength = strength;
        }

        
    }
}
