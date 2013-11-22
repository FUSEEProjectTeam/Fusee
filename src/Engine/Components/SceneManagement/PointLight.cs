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
        /// Initializes a new instance of the <see cref="PointLight" /> class. Position, color and channel are needed.
        /// </summary>
        /// <param name="position">The position of the pointlight.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public PointLight(float3 position, float4 diffuse, float4 ambient, float4 specular, int channel)
        {
            _position = position;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
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
            _diffuseColor = new float4(1, 1, 1, 1);
            _channel = channel;
        }

        /// <summary>
        /// Creates a point light in the scene. Position, color, position, and channel is needed.
        /// It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="color">The color of the light.</param>
        /// <param name="position">The position in the scene.</param>
        /// <param name="channel">The memory space of the light.(0 - 7)</param>
        public PointLight(float4 color, float3 position, int channel)
        {
            _position = position;
            _diffuseColor = new float4(0.6f, 0.6f, 0.6f, 1);
            _ambientColor = new float4(0.3f, 0.3f, 0.3f, 1);
            _specularColor = new float4(0.1f, 0.1f, 0.1f, 1);
            _type = LightType.Point;
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
            sceneVisitorRendering.AddLightPoint(_position , _diffuseColor, _ambientColor, _specularColor, _type, _channel);
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
                _position = SceneEntity.transform.GlobalPosition * SceneManager.RC.View;
            }
            sv.Visit((PointLight)this);
        }
        #endregion
    }
}
