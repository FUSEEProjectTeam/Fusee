using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    internal class MouseAction : ActionCode
    {
        public override void Update()
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                float mousemoveX = Input.Instance.GetAxis(InputAxis.MouseX);
                float mousemoveY = Input.Instance.GetAxis(InputAxis.MouseY);

                transform.LocalEulerAngles += new float3(0, -mousemoveX*100, 0);
                transform.LocalEulerAngles += new float3(-mousemoveY*100, 0, 0);
            }
        }
    }
}