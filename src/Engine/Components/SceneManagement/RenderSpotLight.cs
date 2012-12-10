using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    class RenderSpotLight : RenderJob
    {
        private float3 _position;
        private float3 _direction;
        private float4 _color;
        private int _type;


        public RenderSpotLight(float3 position, float3 direction, float4 color)
        {
            _position = position;
            _direction = direction;
            _type = 2;
            _color = color;
        }
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //renderContext.setLight(_direction, _color, _type);
         }

    }
}
