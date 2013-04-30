using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RednerDirectionalLight is derived from Renderjob and is responible for passing the DirectionalLight towards the RenderContext.
    /// </summary>
    public class RenderDirectionalLight : RenderJob
    {
        #region Fields
        
        private float3 _direction;        
        private float4 _diffuseColor;
        private float4 _ambientColor;
        private float4 _specularColor;
        private Light.LightType _type;
        private int _channel;

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a RenderDirectionalLight needed parameters:( float3, float4, Light.Lighttype, int).
        /// </summary>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="color">Color of the light "Red Green Blue Alpha"</param>
        /// <param name="type">The light type.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public RenderDirectionalLight(float3 direction, float4 diffuse, float4 ambient, float4 specular, Light.LightType type, int channel)
        {
            _direction = direction;
            _type = Light.LightType.Directional;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _channel = channel;
        }
        #endregion

        /// <summary>
        ///  Passes directionallight's parameters to RenderContext.
        /// </summary>
         public override void SubmitWork(RenderContext renderContext)
         {
             renderContext.SetLight(_direction, _diffuseColor, _ambientColor, _specularColor, (int)_type, _channel);
             //Console.WriteLine("DirectionalLight worked");
         }

    }
}
