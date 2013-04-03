using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.SolarSystem
{
    class RotationScript : ActionCode
    {
        public override void Update()
        {
            if(Input.Instance.IsButtonDown(MouseButtons.Left)){
                float mousemoveX = (float)Input.Instance.GetAxis(InputAxis.MouseX);
                float mousemoveY = (float)Input.Instance.GetAxis(InputAxis.MouseY);
                transform.LocalEulerAngles += new float3(0, -mousemoveX, 0);
                transform.LocalEulerAngles += new float3(mousemoveY, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                var _timeadd = Time.Instance.TimeFlow;
                if(Time.Instance.TimeFlow > 2)
                {
                    _timeadd += 0.1f;
                }
                else
                {
                    _timeadd += 0.0025f;
                }
         
                Time.Instance.TimeFlow = _timeadd;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                if(Time.Instance.TimeFlow >= 0)
                {
                var _timedec = Time.Instance.TimeFlow;

                if (Time.Instance.TimeFlow > 2)
                {
                    _timedec -= 0.1f;
                }
                else
                {
                    _timedec -= 0.0025f;
                }
                Time.Instance.TimeFlow = _timedec;
                   }
                else
                {
                    Time.Instance.TimeFlow = 0;
                }
            }
        }
    }
}
