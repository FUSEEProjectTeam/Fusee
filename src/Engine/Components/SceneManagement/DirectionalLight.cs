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
        #region Fields

        private float3 _direction;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a directional light in the scene. Direction, color, position, and channel is needed.
        /// It is possible to set up to 8 lights in the scene.
        /// </summary>
        /// <param name="direction">Direction of the light.</param>
        /// <param name="color">The color of the light.</param>
        /// <param name="position">The position in the scene.</param>
        /// <param name="channel">The memory space of the light.(0 - 7)</param>
        public DirectionalLight(float3 direction, float4 diffuse, float4 ambient, float3 position, int channel)
        {
            _position = position;
            _direction = direction;
            _diffuseColor = new float4(0.6f, 0.6f, 0.6f, 1);
            _ambientColor = new float4(0.3f, 0.3f, 0.3f, 1);
            _specularColor = new float4(0.1f, 0.1f, 0.1f, 1);
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
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _diffuseColor = new float4(0.6f, 0.6f, 0.6f, 1);
            _ambientColor = new float4(0.3f, 0.3f, 0.3f, 1);
            _type = LightType.Directional;
            _channel = channel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// TraverseForRendering add's Directionallight to the lightqueue.
        /// </summary>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightDirectional(_direction, _diffuseColor, _ambientColor, _type, _channel);
        }

        #endregion
        public override void Accept(SceneVisitor sv)
        {
            if (SceneEntity != null)
            {
                _position = SceneEntity.transform.GlobalPosition;
                _direction = SceneEntity.transform.Forward;
                //SceneManager.RC.DebugLine(_position, _direction * 100, new float4(1, 1, 0,1));
            }
            sv.Visit((DirectionalLight)this);
        }
    }
}
