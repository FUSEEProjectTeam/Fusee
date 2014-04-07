using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    internal class CameraScript : ActionCode
    {
        private Camera scenecam;
        private bool perspective;

        public override void Start()
        {
            perspective = true;
            //transform.LocalPosition= new float3(0,0,-100);
            scenecam = SceneEntity.GetComponent<Camera>();
        }

        public override void Update()
        {
            if (Input.Instance.GetAxis(InputAxis.MouseWheel) != 0)
            {
                Debug.WriteLine("mouse wheel");
                transform.LocalPosition += new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));
            }

            if (Input.Instance.IsKeyDown(KeyCodes.P))
            {
                if (perspective)
                {
                    scenecam.ProjectionType(Projection.Orthographic);
                }
                else
                {
                    scenecam.ProjectionType(Projection.Perspective);
                }
                perspective = !perspective;
            }
            SceneManager.RC.DebugLine(new float3(0, 0, 0), new float3(7, 0, 0), new float4(1, 1, 1, 1));
            //Debug.WriteLine("Current FPS: "+Time.Instance.FramePerSecondSmooth);
            //DrawCameraView();
        }

        private void DrawCameraView()
        {
            // CenterRay
            SceneManager.RC.DebugLine(transform.GlobalPosition + transform.Forward, transform.Forward*scenecam.Far,
                                      new float4(0, 1, 0, 1));
        }
    }
}