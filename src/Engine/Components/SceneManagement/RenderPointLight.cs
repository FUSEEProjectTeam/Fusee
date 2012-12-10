using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    class RenderPointLight :RenderJob
    {
        
        private float3 _position;
        private float4 _color;
        private int _type;
        
       
        public RenderPointLight( float3 position, float4 color)
        {
            _position = position;
            _type = 1;
            _color = color;
        }
         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //TODO overload the method for the different lighttypes
             //renderContext.setLight(_position, _color, _type);
         }
    }
}
