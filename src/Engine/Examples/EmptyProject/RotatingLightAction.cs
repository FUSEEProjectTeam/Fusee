using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.SceneManagement;
using Fusee.Math;
using System.Diagnostics;

namespace Examples.LightTypeTest
{
    public class RotatingLightAction : ActionCode
    {
        private float3 rotationSpeed = new float3(0, 2, 0);


        public override void Update()
        {
            transform.GlobalEulerAngles += rotationSpeed;
            SceneManager.RC.DebugLine(transform.GlobalPosition, SceneEntity.parent.transform.GlobalPosition, new float4(1, 0, 0, 1));
            Debug.WriteLine("dummes teil");
        }
    }
}
