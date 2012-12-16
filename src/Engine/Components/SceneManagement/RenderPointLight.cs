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
        
       
        public RenderPointLight(float3 position, float4 color, Light.LightType type)
        {
            _position = position;
            _type = Light.LightType.Point;
            _color = color;
        }
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //TODO overload the method for the different lighttypes
             //renderContext.setLight(_position, _color, _type);
             //Console.WriteLine("Pointlight worked");
         }
    }
}
