using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RenderPointLight is derived from Renderjob and is responible for passing the PointLight towards the RenderContext.
    /// </summary>
    public class RenderPointLight : RenderJob
    {
        #region Fields
        private float3 _position;
        private float4 _diffuseColor;
        private float4 _ambientColor;
        private float4 _specularColor;
        private Light.LightType _type;
        private int _channel;
        #endregion

        #region Members
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPointLight" /> class. Position, color, type and channel are needed.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="type">The light type.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public RenderPointLight(float3 position, float4 diffuse, float4 ambient, float4 specular, Light.LightType type, int channel)
        {
            _position = position;
            _type = Light.LightType.Point;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _channel = channel;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Passes pointlight's parameters to RenderContext.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
         public override void SubmitWork(RenderContext renderContext)
         {
             renderContext.SetLight(_position, _diffuseColor, _ambientColor, _specularColor, (int)_type, _channel);
         }
        #endregion
    }
}
