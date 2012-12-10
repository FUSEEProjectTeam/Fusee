using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    class SpotLight
    {
        private int _type;
        private float3 _position;
        private float3 _direction;
        private float4 _color;

        public SpotLight(float3 position, float3 direction, float4 color) 
        {
            _type = 2;
            _position = position;
            _direction = direction;
            _color = color;
        }

        public SpotLight() 
        {
            _type = 2;
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);   
        }

        virtual public void Traverse(ITraversalState _traversalState)
        {
            //TODO Typübergabe implementieren.
            _traversalState.addLightSpot(_position, _direction , _color);
        }
    }
}
