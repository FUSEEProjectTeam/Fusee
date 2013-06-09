using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
using Fusee.SceneManagement;
namespace Examples.Solar
{
    class CameraScript : ActionCode
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
                transform.LocalPosition -= new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));
            }

            if (Input.Instance.OnKeyDown(KeyCodes.P))
            {
                if(perspective)
                {
                    scenecam.ProjectionType(Projection.Orthographic); 
                }else
                {
                    scenecam.ProjectionType(Projection.Perspective);
                }
                perspective = !perspective;

            }
        }

        private void DrawCameraView()
        {
            // CenterRay
            SceneManager.RC.DebugLine(transform.GlobalPosition+transform.Forward, transform.Forward*scenecam.Far,new float4(0,1,0,1));
        }

    }
}
