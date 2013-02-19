using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Light and set's a Spotlight in the scene.
    /// A position, a direction and the color of the lightsource is needed.
    /// </summary>

    public class SpotLight : Light
    {

        #region Fields

        private float3 _direction;

        #endregion

        #region Constructors
        public SpotLight(float3 position, float3 direction, float4 color, int channel) 
        {
            _type = LightType.Spot;
            _position = position;
            _direction = direction;
            _color = color;
            _channel = channel;
        }

        public SpotLight( int channel) 
        {
            _type = LightType.Spot;
            _position = new float3(0,0,0);
            _direction = new float3(0,-1,0);
            _color = new float4(0.5f, 0.5f, 0.5f, 0.5f);
            _channel = channel;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Overrides Traverse. Add's Spotlight to the lightqueue.
        /// </summary>
        /// <param name="_traversalState"></param>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            sceneVisitorRendering.AddLightSpot(_position, _direction, _color, _type, _channel );
        }

        #endregion
    }
}
