using System;
using Fusee.Math;
using System.Diagnostics;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Directionallight in the scene.
    /// A direction, a color and the position of the lightsource is needed.
    /// </summary>

    public class DirectionalLight : Light
    {
        #region Constructors
        /// <summary>
        /// Creates a directional light in the scene. Direction, color, position, and channel is needed.
        /// It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="diffuse">The diffuse light color.</param>
        /// <param name="ambient">The ambient light color.</param>
        /// <param name="specular">The specular light color.</param>
        /// <param name="position">The position in the scene.</param>
        /// <param name="channel">The memory space of the light.(0 - 7)</param>
        public DirectionalLight(float3 direction, float4 diffuse, float4 ambient, float4 specular, float3 position, int channel)
        {
            _position = position;
            _direction = direction;
            _diffuseColor = diffuse;
            _ambientColor = ambient;
            _specularColor = specular;
            _type = LightType.Directional;
            _channel = channel;
        }

        /// <summary>
        /// Creates a directional light in the scene. Direction, color and position will get standart values.
        /// Channel is needed. It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="channel">The memory space of the light(0 - 7).</param>
        public DirectionalLight( int channel)
        {
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _diffuseColor = new float4(0.6f, 0.6f, 0.6f, 1);
            _ambientColor = new float4(0.3f, 0.3f, 0.3f, 1);
            _specularColor = new float4(0.1f, 0.1f, 0.1f, 1);
            _type = LightType.Directional;
            _channel = channel;
        }
        #endregion
        #region Methods
        /// <summary>
        /// TraverseForRendering add's Directionallight to the light queue.
        /// </summary>
        /// <param name="sceneVisitorRendering">The SceneVisitorRendering object that is passing the light information to the rendeirng queue.</param>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightDirectional(_direction, _diffuseColor, _ambientColor, _specularColor, _type, _channel);
        }

        #endregion
        #region Overrides
        /// <summary>
        /// Accept is called by the current visitor. This function is currently used for traversal and search algorithms by the SceneManager object. 
        /// </summary>
        /// <param name="sv">The visitor that is currently traversing the scene.</param>
        public override void Accept(SceneVisitor sv)
        {
            if (SceneEntity != null)
            {   
                _direction = SceneEntity.transform.Forward * new float3x3(SceneManager.RC.View); 
            }
            sv.Visit((DirectionalLight)this);
        }
        #endregion
    }
}
