using Fusee.Math;


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
        public DirectionalLight(float3 direction, float4 color, float3 position, int channel)
        {
            _position = position;
            _direction = direction;
            _color = color;
            _type = LightType.Directional;
            _channel = channel;
        }

        public DirectionalLight( int channel)
        {
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _type = LightType.Directional;
            _channel = channel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Overrides Traverse. Add's Directionallight to the lightqueue.
        /// </summary>
        /// <param name="_traversalState"></param>

        
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightDirectional(_direction, _color, _type, _channel);
        }

        #endregion
    }
}
