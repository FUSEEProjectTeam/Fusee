using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    public class RotateAction : ActionCode
    {
        private  float3 _rotationSpeed;
        public RotateAction(float3 rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }
        public override void Start()
        {
            transform.LocalEulerAngles = new float3(0, 0, 0);
        }

        public override void Update()
        {
            transform.LocalEulerAngles += _rotationSpeed*(float)Time.Instance.DeltaTime;
            SceneManager.RC.DebugLine(transform.GlobalPosition, transform.Forward * 10000, new float4(1, 1, 0, 1));
            

            if (SceneEntity.parent != null)
            {
                //SceneManager.RC.DebugLine(SceneEntity.parent.transform.GlobalPosition, transform.GlobalPosition, new float4(1, 0, 0, 1));
            }
        }
    }
}
