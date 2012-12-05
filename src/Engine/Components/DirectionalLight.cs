using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;


namespace Fusee.SceneManagement
{
    class DirectionalLight
    {
        private int _type = 0;
        private float3 _direction;
        private float4 _color;

        public DirectionalLight(float3 direction, float4 color, int typ)
        {
            _direction = direction;
            _color = color;
            _type = typ;
        }

        public DirectionalLight()
        {
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
        }

        virtual public void Traverse( ITraversalState _traversalState)
        {
            _traversalState.addLight(_direction, _color, _type);
        }


      
             
        

    }
}
