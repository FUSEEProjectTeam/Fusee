using System;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Pointlight in the scene.
    /// A color and the position of the lightsource are needed.
    /// </summary>
    public class PointLight : Light
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class. Position, color and channel are needet.
        /// </summary>
        /// <param name="position">The position of the pointlight.</param>
        /// <param name="color">The color of the pointlight.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public PointLight(float3 position, float4 color, int channel)
        {
            _position = position;
            _color = color;
            _type = LightType.Point;
            _channel = channel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointLight"/> class. Only the channel is needed. Other params
        /// will be set to default value. 
        /// </summary>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public PointLight(int channel)
        {
            _type = LightType.Point;
            _position = new float3(0, 0, 0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = channel;
        }
        #endregion
        #region Members

        /// <summary>
        /// Traverses this pointligt for rendering. The light information is passed to the SceneVisitorRendering.
        /// </summary>
        /// <param name="sceneVisitorRendering">The scene visitor rendering object grabs the light information upon render time.</param>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightPoint(_position , _color, _type, _channel);
        }

        #endregion
        #region Overrides
        /// <summary>
        /// Passes the Component to the SceneVisitor which decides what to do with that Component.
        /// </summary>
        /// <param name="sv">The SceneVisitor instance updates the position of this pointlight according to the transformation of the pointlights parent.</param>
        public override void Accept(SceneVisitor sv)
        {
            if (SceneEntity != null)
            {
                _position = SceneEntity.transform.GlobalPosition;
            }
            sv.Visit((PointLight)this);
        }
        #endregion
    }
}
