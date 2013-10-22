using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
using Fusee.SceneManagement;
namespace Examples.LightTypeTest
{
    class CamScript : ActionCode
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
                //Debug.WriteLine("mouse wheel");
                transform.LocalPosition -= new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));
            }

            if (Input.Instance.IsKeyDown(KeyCodes.P))
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
            //Debug.WriteLine("Current FPS: "+Time.Instance.FramePerSecondSmooth);
        }

    }
}
