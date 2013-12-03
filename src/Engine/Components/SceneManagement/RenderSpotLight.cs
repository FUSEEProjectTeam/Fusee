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
        private float4 _diffuseColor;
        private float4 _ambientColor;
        private float4 _specularColor;
        private Light.LightType _type;
        private int _channel;
        private float _angle;

        #endregion 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderSpotLight" /> class. Position, direction, color, type and channel.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="direction">The direction of the light.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="angle">The angle of the spot light.</param>
        /// <param name="type">The lighttype.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public RenderSpotLight(float3 position, float3 direction, float4 diffuse, float4 ambient, float4 specular, float angle, Light.LightType type, int channel)
        {
            _position = position;
            _direction = direction;
            _type = Light.LightType.Spot;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _channel = channel;
            _angle = angle;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Passes spotlight's parameters to RenderContext.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        public override void SubmitWork(RenderContext renderContext)
        {
            renderContext.SetLight(_position, _direction, _diffuseColor, _ambientColor, _specularColor, (int)_type, _channel);
            renderContext.SetLightSpotAngle(_channel, _angle);
        }

        #endregion
    }
}
