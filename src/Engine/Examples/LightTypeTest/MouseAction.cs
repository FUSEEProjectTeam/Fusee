using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.LightTypeTest
{
    class MouseAction : ActionCode
    {
        private String[] _planets = {  "Sphere1", "Cube1", "Root1"};
        private int _currentTargetId = 0;
        private SceneEntity _target;
        public override void Start()
        {
            _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
            Debug.WriteLine(_target.name);
        }

        public override void Update()
        {
            if(Input.Instance.IsButton(MouseButtons.Left)){
                float mousemoveX = (float)Input.Instance.GetAxis(InputAxis.MouseX);
                float mousemoveY = (float)Input.Instance.GetAxis(InputAxis.MouseY);

                //transform.GlobalQuaternion = Quaternion.FromAxisAngle(new float3(0, 0, 1), -mousemoveY) * Quaternion.FromAxisAngle(new float3(0, 1, 0), -mousemoveX) * transform.GlobalQuaternion;
                
                transform.LocalEulerAngles += new float3(0, -mousemoveX*100, 0);
                transform.LocalEulerAngles += new float3(-mousemoveY*100, 0, 0);
            }

            if (Input.Instance.IsKey(KeyCodes.Up))
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
            if (Input.Instance.IsKey(KeyCodes.Down))
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
                //Debug.WriteLine(Time.Instance.TimeFlow.ToString());
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _currentTargetId++;
                if(_currentTargetId >= _planets.Length)
                {
                    _currentTargetId = 0;
                }
                _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
                Debug.WriteLine("Current Planet: " + _target.name);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _currentTargetId--;
                if (_currentTargetId <= 0)
                {
                    _currentTargetId = _planets.Length-1;
                }
                _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
                Debug.WriteLine("Current Planet: " + _target.name);
            }
            transform.GlobalPosition = _target.transform.GlobalPosition;
            //Debug.WriteLine("Target is cool & RotationScript GlobalPos: " + _target.transform.GlobalPosition);   
        }
    }
}
