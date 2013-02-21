using System;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Pointlight in the scene.
    /// A color and the position of the lightsource is needed.
    /// </summary>
    public class PointLight : Light
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class. Position, color and channel are needet.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="channel">The channel.</param>
        public PointLight(float3 position, float4 color, int channel)
        {
            _position = position;
            _color = color;
            _type = LightType.Point;
            _channel = channel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public PointLight(int channel)
        {
            _type = LightType.Point;
            _position = new float3(0, 0, 0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = channel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Overrides Traverse. Add's Pointlight to the lightqueue.
        /// </summary>
        /// <param name="sceneVisitorRendering">The scene visitor rendering.</param>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightPoint(_position , _color, _type, _channel);
        }

        #endregion
    }
}
