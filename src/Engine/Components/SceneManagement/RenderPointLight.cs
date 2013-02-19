using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class RenderPointLight : RenderJob
    {
        
        private float3 _position;
        private float4 _color;
        private Light.LightType _type;
        private int _channel;
        
       
        public RenderPointLight(float3 position, float4 color, Light.LightType type, int channel)
        {
            _position = position;
            _type = Light.LightType.Point;
            _color = color;
            _channel = channel;
        }
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //TODO overload the method for the different lighttypes
             //renderContext.SetLight(_position, _color, _type, _channel);
             //Console.WriteLine("Pointlight worked");
         }
    }
}
