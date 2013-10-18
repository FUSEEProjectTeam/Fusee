using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RenderSpotLight is derived from Renderjob and is responible for passing the SpotLight towards the RenderContext.
    /// </summary>
    public class RenderSpotLight : RenderJob
    {
        #region Fields

        private float3 _position;
        private float3 _direction;
        private float4 _color;
        private Light.LightType _type;
        private int _channel;

        #endregion 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSpotLight"/> class. Position, direction, color, type and channel.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="direction">The direction of the light.</param>
        /// <param name="color">The lightcolor.</param>
        /// <param name="type">The type.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public RenderSpotLight(float3 position, float3 direction, float4 color, Light.LightType type, int channel)
        {
            _position = position;
            _direction = direction;
            _type = Light.LightType.Spot;
            _color = color;
            _channel = channel;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Passes spotlight's parameters to RenderContext.
        /// </summary>
        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.SetLight(_position, _direction, _color, (int)_type, _channel);
        }

        #endregion
    }
}
