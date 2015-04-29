using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class RotatingLightAction : ActionCode
    {
        private readonly float3 _rotationSpeed;

        public RotatingLightAction(float3 rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }

        public override void Start()
        {
            transform.LocalEulerAngles = new float3(0, 0, 0);
        }

        public override void Update()
        {
            transform.LocalEulerAngles += _rotationSpeed*(float) Time.Instance.DeltaTime;
        }
    }
}