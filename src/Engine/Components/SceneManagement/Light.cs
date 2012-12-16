using Fusee.Math;

namespace Fusee.SceneManagement
{
    public class Light : Component
    {
        public enum LightType
        {
            Directional,
            Point,
            Spot,
        }

        protected LightType _type;
        protected float3 _position;
        protected float4 _color;



        public  Light()
        {
            _type = LightType.Point;
            _position = new float3(0,0,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
        }

        override public void Traverse(ITraversalState _traversalState)
        {

        }

    }
}
