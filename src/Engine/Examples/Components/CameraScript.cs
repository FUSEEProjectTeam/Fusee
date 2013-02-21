using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
using Fusee.SceneManagement;
namespace Examples.Components
{
    class CameraScript : ActionCode
    {
        public override void Start()
        {
            //transform.LocalPosition= new float3(0,0,-100);
        }
        public override void Update()
        {
            //float mousemove = Input.GetAxis(InputAxis.MouseX);
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                transform.LocalPosition += new float3(0, 0, 0.1f);
            }
        }
    }
}
