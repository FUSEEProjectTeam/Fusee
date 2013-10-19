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
        /// <summary>
        /// Enums for lighttypes at the moment there are 3
        /// </summary>
        public enum LightType
        {
            /// <summary>
            /// Directional = 0
            /// </summary>
            Directional,
            /// <summary>
            /// Point = 1
            /// </summary>
            Point,
            /// <summary>
            /// Spot = 2
            /// </summary>
            Spot,
        }
        #endregion

        #region Fields

        protected LightType _type;
        protected float3 _position;
        protected float4 _color;
        protected int _channel;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a light object.
        /// </summary>
        public  Light()
        {
            _type = LightType.Point;
            _position = new float3(0,0,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = 0;
        }
        #endregion
    }
}
