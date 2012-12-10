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
        private int _type;
        private float3 _direction;
        private float4 _color;

        public DirectionalLight(float3 direction, float4 color)
        {
            _direction = direction;
            _color = color;
            _type = 0;
        }

        public DirectionalLight()
        {
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _type = 0;
        }

        virtual public void Traverse( ITraversalState _traversalState)
        {
            _traversalState.addLightDirectional(_direction, _color);
        }
    }
}
