using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    class PointLight
    {

        private int _type;
        private float3 _position;
        private float4 _color;

        public PointLight(float3 position, float4 color)
        {
            _position = position;
            _color = color;
            _type = 1;
        }

        public PointLight()
        {
            _type = 1;
            _position = new float3(0, 0, 0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
        }

        virtual public void Traverse( ITraversalState _traversalState)
        {
            _traversalState.addLightDirectional(_position, _color);
        }
    }
}
