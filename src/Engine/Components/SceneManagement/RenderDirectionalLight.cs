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
        
       
        public RenderDirectionalLight(float3 direction, float4 color, Light.LightType type)
        {
            _direction = direction;
            _type = Light.LightType.Directional;
            _color = color;
        }

         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //renderContext.SetLightDirection((int)_type, _direction);
             //renderContext.setLight(_direction, _color, _type);
             //Console.WriteLine("DirectionalLight worked");
         }

    }
}
