using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Spotlight in the scene.
    /// </summary>
    public class SpotLight : Light
    {

        #region Fields

        private float3 _direction;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class. Position, direction, color and the channel are needed.
        /// </summary>
        /// <param name="position">The position of the spotlight.</param>
        /// <param name="direction">The direction of the spotlight.</param>
        /// <param name="color">The color of the spotlight(Red, Green, Blue, Alpha).</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public SpotLight(float3 position, float3 direction, float4 color, int channel) 
        {
            _type = LightType.Spot;
            _position = position;
            _direction = direction;
            _color = color;
            _channel = channel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class. Only the channel is needed.
        /// </summary>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public SpotLight( int channel) 
        {
            _type = LightType.Spot;
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = channel;
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a spot light in the scene. Position, color, position, and channel is needed.
        /// It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="direction">Direction of the light.</param> 
        /// <param name="color">The color of the light.</param>
        /// <param name="position">The position in the scene.</param>
        /// <param name="channel">The memory space of the light.(0 - 7)</param>
        public SpotLight(float3 direction, float4 color, float3 position, int channel)
        {
            _direction = direction;
            _position = position;
            _color = color;
            _type = LightType.Spot;
            _channel = channel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// TraverseForRendering add's Spotlight to the lightqueue.
        /// </summary>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            if (SceneEntity != null)
            {
                _position = SceneEntity.transform.GlobalPosition;
            }
            sceneVisitorRendering.AddLightSpot(_position, _direction, _color, _type, _channel );
        }

        #endregion
        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((SpotLight)this);
        }
    }
}
