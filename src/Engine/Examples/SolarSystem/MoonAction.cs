using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.SceneManagement;
using Fusee.Math;
using Fusee.Engine;

namespace Examples.SolarSystem
{
    public class MoonAction : ActionCode
    {
        private SceneEntity _earth;
        private float3 _rotationspeed;
        public MoonAction(SceneEntity earth, float3 rotationspeed)
        {
            _earth = earth;
            _rotationspeed = rotationspeed;
        }

        public override void Start()
        {
            transform.LocalPosition = _earth.transform.LocalPosition;
        }

        public override void Update()
        {
            transform.Matrix = _earth.transform.Matrix;
            
            //transform.LocalEulerAngles += _rotationspeed * (float)Time.Instance.DeltaTime;
        }
    }
}
