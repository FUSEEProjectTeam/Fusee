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
            /// Disabled Light = 0
            /// </summary>
            Disabled,
            /// <summary>
            /// Directional = 1
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

        /// <summary>
        /// The <see cref="LightType"/>.
        /// </summary>
        protected LightType _type;
        /// <summary>
        /// The position in the scene.
        /// </summary>
        protected float3 _position;
        /// <summary>
        /// The direction of the light along the z-axis.
        /// </summary>
        protected float3 _direction;
        /// <summary>
        /// The diffuse light color.
        /// </summary>
        protected float4 _diffuseColor;
        /// <summary>
        /// The ambient light color.
        /// </summary>
        protected float4 _ambientColor;
        /// <summary>
        /// The specular light color.
        /// </summary>
        protected float4 _specularColor;
        /// <summary>
        /// The light channel (0-7).
        /// </summary>
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
            _channel = 0;
        }
        #endregion
    }
}
