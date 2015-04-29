using System;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    internal class CamScript : ActionCode
    {
        public override void Update()
        {
            if (Math.Abs(Input.Instance.GetAxis(InputAxis.MouseWheel)) > MathHelper.EpsilonFloat)
            {
                transform.LocalPosition -= new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));
            }
        }
    }
}