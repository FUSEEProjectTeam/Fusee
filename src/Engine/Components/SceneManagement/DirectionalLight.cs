using Fusee.Math;


namespace Fusee.SceneManagement
{
    public class DirectionalLight : Light
    {

        private float3 _direction;


        public DirectionalLight(float3 direction, float4 color, float3 position)
        {
            _position = position;
            _direction = direction;
            _color = color;
            _type = LightType.Directional;
        }

        public DirectionalLight()
        {
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _type = LightType.Directional;
        }

        override public void Traverse(ITraversalState _traversalState)
        {
            _traversalState.AddLightDirectional(_direction, _color, _type);
        }
    }
}
