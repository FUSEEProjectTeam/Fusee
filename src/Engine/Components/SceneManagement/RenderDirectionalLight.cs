using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class RenderDirectionalLight : RenderJob
    {
        
        private float3 _direction;
        private float4 _color;
        private Light.LightType _type;
        private int _channel;
        
       
        public RenderDirectionalLight(float3 direction, float4 color, Light.LightType type, int channel)
        {
            _direction = direction;
            _type = Light.LightType.Directional;
            _color = color;
            _channel = channel;
        }

         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //renderContext.SetLight(_direction, _color, _type, _channel);
             //Console.WriteLine("DirectionalLight worked");
         }

    }
}
