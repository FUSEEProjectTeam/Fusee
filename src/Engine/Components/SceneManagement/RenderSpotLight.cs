using System;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Passes the Spotlight parameters to the RenderContext
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
        
        public RenderSpotLight(float3 position, float3 direction, float4 color, Light.LightType type, int channel)
        {
            _position = position;
            _direction = direction;
            _type = Light.LightType.Spot;
            _color = color;
            _channel = channel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Passes spotlight's parameters to RenderContext.
        /// </summary>
        /// <param name="renderContext"></param>
        public override void SubmitWork(RenderContext renderContext)
        {
            //TODO Warten Auf Timon und Casper
            //renderContext.SetLight(_position, _direction, _color, _type, _channel);
            //Console.WriteLine("Spotlight worked");
        }

        #endregion
    }
}
