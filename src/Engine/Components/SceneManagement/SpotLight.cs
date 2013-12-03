using System.Diagnostics;
using Fusee.Math;
using System;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Spotlight in the scene.
    /// </summary>
    public class SpotLight : Light
    {

        #region Fields 

        /// <summary>
        /// The angle of the spotlight.
        /// </summary>
        protected float _angle;
        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight" /> class. Position, direction, color and the channel are needed.
        /// </summary>
        /// <param name="position">The position of the spotlight.</param>
        /// <param name="direction">The direction of the spotlight.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="angle">The light angle of the spotlight.</param>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public SpotLight(float3 position, float3 direction, float4 diffuse, float4 ambient, float4 specular, float angle, int channel) 
        {
            _type = LightType.Spot;
            _position = position;
            _direction = direction;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _angle = angle;
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
            _diffuseColor = new float4(1, 1, 1, 1);
            _channel = channel;
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a spot light in the scene. Position, color, position, and channel is needed.
        /// It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="position">The lights position in the scene.</param>
        /// <param name="channel">The memory space of the light.(0 - 7)</param>
        public SpotLight(float3 direction, float4 diffuse, float4 ambient, float4 specular, float3 position, int channel)
        {
            _direction = direction;
            _position = position;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _type = LightType.Spot;
            _channel = channel;
        }

        #endregion

        #region Members

        /// <summary>
        /// Add's a Spotlight to the lightqueue.
        /// </summary>
        /// <param name="sceneVisitorRendering">The SceneVisitorRendering instance.</param>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightSpot(_position, _direction, _diffuseColor, _ambientColor, _specularColor, _angle, _type, _channel );
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Passes the Component to the SceneVisitor which decides what to do with that Component.
        /// </summary>
        /// <param name="sv">The SceneVisitor.</param>
        public override void Accept(SceneVisitor sv)
        {
            if (SceneEntity != null)
            {
                _position = SceneEntity.transform.GlobalPosition * SceneManager.RC.View;
                _direction = SceneEntity.transform.Forward * new float3x3(SceneManager.RC.View);
            }
            sv.Visit((SpotLight)this);
        }
        #endregion
    }
}
