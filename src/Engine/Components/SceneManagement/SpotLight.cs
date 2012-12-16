using Fusee.Math;

namespace Fusee.SceneManagement
{

    public class SpotLight : Light
    {
      
        private float3 _direction;

        public SpotLight(float3 position, float3 direction, float4 color) 
        {
            _type = LightType.Spot;
            _position = position;
            _direction = direction;
            _color = color;
        }

        public SpotLight() 
        {
            _type = LightType.Spot;
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);   
        }

        override public void Traverse(ITraversalState _traversalState)
        {
            //TODO Typübergabe implementieren.
            _traversalState.AddLightSpot(_position, _direction , _color, _type);
        }
    }
}
