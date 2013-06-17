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

        protected float _angle;
        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight"/> class. Position, direction, color and the channel are needed.
        /// </summary>
        /// <param name="position">The position of the spotlight.</param>
        /// <param name="direction">The direction of the spotlight.</param>
        /// <param name="color">The color of the spotlight(Red, Green, Blue, Alpha).</param>
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
        /// <param name="color">The color of the light.</param>
        /// <param name="position">The position in the scene.</param>
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

        #region Methods

        /// <summary>
        /// TraverseForRendering add's Spotlight to the lightqueue.
        /// </summary>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightSpot(_position, _direction, _diffuseColor, _ambientColor, _specularColor, _angle, _type, _channel );
        }

        #endregion
        public override void Accept(SceneVisitor sv)
        {
            if (SceneEntity != null)
            {
                float4 tempPos = float4x4.Transpose(SceneManager.RC.View) * new float4(SceneEntity.transform.GlobalPosition.x, SceneEntity.transform.GlobalPosition.y, SceneEntity.transform.GlobalPosition.z, 1);
                _position = new float3(tempPos.x, tempPos.y, tempPos.z) / tempPos.w;
                _direction = new float3x3(float4x4.Transpose(SceneManager.RC.View)) * SceneEntity.transform.Forward; 
            }
            sv.Visit((SpotLight)this);
        }
    }
}
