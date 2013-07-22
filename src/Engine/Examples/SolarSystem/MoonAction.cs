using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class MoonAction : ActionCode
    {
        private SceneEntity _earth;
        private readonly float3 _rotationSpeed;
        // private Transformation _moonPos;

        public MoonAction(float3 rotationspeed)
        {
            _rotationSpeed = rotationspeed;
        }

        public override void Start()
        {
            _earth = SceneEntity.FindSceneEntity("Earth");
            transform.LocalPosition = _earth.transform.GlobalPosition;

            // _moonPos = SceneEntity.FindSceneEntity("Moon").transform;
        }

        public override void Update()
        {
            transform.LocalPosition = _earth.transform.GlobalPosition;
            transform.LocalEulerAngles += _rotationSpeed*(float) Time.Instance.DeltaTime;

            // this can be activated for testing purposes
            // SceneManager.RC.DebugLine(transform.GlobalPosition, _moonPos.GlobalPosition, new float4(1, 0, 0, 1));
        }
    }
}