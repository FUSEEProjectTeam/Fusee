using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// RednerDirectionalLight is derived from Renderjob and is responible for passing the DirectionalLight towards the renderqueue.
    /// </summary>
    public class RenderDirectionalLight : RenderJob
    {
        #region Fields
        private float3 _direction;
        private float4 _color;
        private Light.LightType _type;
        private int _channel;
        #endregion

        #region Constructor
        /// <summary>
        /// The only constructor( float3, float4, Light.Lighttype, int).
        /// </summary>
        /// <param name="direction">Direction of the light. </param>
        /// <param name="color">Color of the light "Red Green Blue Alpha"</param>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        public RenderDirectionalLight(float3 direction, float4 color, Light.LightType type, int channel)
        {
            _direction = direction;
            _type = Light.LightType.Directional;
            _color = color;
            _channel = channel;
        }
        #endregion

         public override void SubmitWork(RenderContext renderContext)
         {
             //TODO Warten Auf Timon und Casper
             //renderContext.SetLight(_direction, _color, _type, _channel);
             //Console.WriteLine("DirectionalLight worked");
         }

    }
}
