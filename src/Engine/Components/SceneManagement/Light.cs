using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Light is the base class for all lightsources in the scene.
    /// All lights are derived from this class and this class is derived from Component.
    /// </summary>

    public class Light : Component
    {
        #region Enums
        public enum LightType
        {
            Directional, // 0
            Point,       // 1 
            Spot,        // 2
        }
#endregion

        #region Fields
        protected LightType _type;
        protected float3 _position;
        protected float4 _color;
        protected int _channel;
        #endregion

        #region Constructors
        public  Light()
        {
            _type = LightType.Point;
            _position = new float3(0,0,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Overrides Traverse with empty body
        /// </summary>
        /// <param name="_traversalState"></param>
        override public void Traverse(ITraversalState _traversalState)
        {

        }
        #endregion 

    }
}
