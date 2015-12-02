using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class PlanetAction : ActionCode
    {
        private readonly RenderContext _renderContext;

        private readonly float3 _rotationSpeed;
        private bool _isEarth;

        public PlanetAction(RenderContext rc, float3 rotationSpeed)
        {
            _renderContext = rc;
            _rotationSpeed = rotationSpeed;
        }

        public override void Start()
        {
            _isEarth = (SceneEntity.Name == "Earth");
        }

        public override void Update()
        {
            transform.LocalEulerAngles += _rotationSpeed*(float) Time.Instance.DeltaTime;

            if (SceneEntity.Parent != null)
            {
                _renderContext.DebugLine(SceneEntity.Parent.Transform.GlobalPosition, transform.GlobalPosition,
                    new float4(1, 0, 0, 1));
            }

            if (_isEarth)
            {
                _renderContext.DebugLine(transform.GlobalPosition, transform.Forward * 100, new float4(1, 1, 0, 1));
            }
        }
    }
}