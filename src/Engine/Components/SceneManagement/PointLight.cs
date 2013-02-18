using System;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class PointLight : Light
    {

        public PointLight(float3 position, float4 color)
        {
            _position = position;
            _color = color;
            _type = LightType.Point;
        }

        public PointLight()
        {
            _type = LightType.Point;
            _position = new float3(0, 0, 0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
        }

        override public void Traverse( ITraversalState _traversalState)
        {
            _traversalState.Visit(this);
        }
        public void TraverseForRendering(ITraversalState _traversalState)
        {
            _traversalState.AddLightPoint(_position, _color, _type);
        }
    }
}
