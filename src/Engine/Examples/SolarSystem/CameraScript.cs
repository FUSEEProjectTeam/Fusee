using System;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    internal class CameraScript : ActionCode
    {
        private Camera _sceneCam;
        private bool _perspective;

        public override void Start()
        {
            _perspective = true;
            _sceneCam = SceneEntity.GetComponent<Camera>();
        }

        public override void Update()
        {
            if (Math.Abs(Input.Instance.GetAxis(InputAxis.MouseWheel) - 0) > MathHelper.EpsilonFloat)
                transform.LocalPosition -= new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));

            if (Input.Instance.IsKeyDown(KeyCodes.P))
            {
                _sceneCam.ProjectionType(_perspective ? Projection.Orthographic : Projection.Perspective);
                _perspective = !_perspective;
            }
        }
    }
}