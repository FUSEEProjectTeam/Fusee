using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class RenderSpotLight : RenderJob
    {
        private float3 _position;
        private float3 _direction;
        private float4 _color;
        private Light.LightType _type;


        public RenderSpotLight(float3 position, float3 direction, float4 color, Light.LightType type)
        {
            _position = position;
            _direction = direction;
            _type = Light.LightType.Spot;
            _color = color;
        }
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //renderContext.setLight(_direction, _color, _type);
             //Console.WriteLine("Spotlight worked");
         }

    }
}
