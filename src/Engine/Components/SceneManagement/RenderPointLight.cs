using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RenderPointLight is derived from Renderjob and is responible for passing the PointLight towards the RenderContext.
    /// </summary>
    public class RenderPointLight : RenderJob
    {

        private float3 _position;
        private float4 _diffuseColor;
        private float4 _ambientColor;
        private Light.LightType _type;
        private int _channel;


        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPointLight"/> class. Position, color, type and channel are needed.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="color">The color of the light (Red, Green, Blue, Alpha).</param>
        /// <param name="type">The light type.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public RenderPointLight(float3 position, float4 diffuse, float4 ambient, Light.LightType type, int channel)
        {
            _position = position;
            _type = Light.LightType.Point;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _channel = channel;
        }
        /// <summary>
        ///  Passes pointlight's parameters to RenderContext. 
        /// </summary>
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             renderContext.SetLight(_position, _diffuseColor, _ambientColor, (int)_type, _channel);
             //Console.WriteLine("Pointlight worked");
         }
    }
}
