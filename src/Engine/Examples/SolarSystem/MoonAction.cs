using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Solar
{
    public class MoonAction : ActionCode
    {
        private SceneEntity _earth;
        private readonly float3 _rotationSpeed;

        public MoonAction(float3 rotationspeed)
        {
            _rotationSpeed = rotationspeed;
        }

        public override void Start()
        {
            _earth = SceneEntity.FindSceneEntity("Earth");
            transform.LocalPosition = _earth.transform.GlobalPosition;
        }

        public override void Update()
        {
            transform.LocalPosition = _earth.transform.GlobalPosition;
            transform.LocalEulerAngles += _rotationSpeed*(float) Time.Instance.DeltaTime;
        }
    }
}