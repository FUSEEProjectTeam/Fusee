using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.SceneManagement;
using Fusee.Math;
using Fusee.Engine;

namespace Examples.Solar
{
    public class MoonAction : ActionCode
    {
        private SceneEntity _earth;
        private float3 _rotationspeed;
        private Transformation _moonpos;
        public MoonAction(SceneEntity earth, float3 rotationspeed)
        {
            _earth = earth;
            _rotationspeed = rotationspeed;
        }

        public override void Start()
        {
            transform.LocalPosition = _earth.transform.GlobalPosition;
            _moonpos = SceneEntity.FindSceneEntity("Moon").transform;
        }

        public override void Update()
        {
            transform.LocalPosition = _earth.transform.GlobalPosition;
            //Debug.WriteLine("Earth LocalPos: "+_earth.transform.LocalPosition+ " Earth Global Pos: "+_earth.transform.GlobalPosition);
            transform.LocalEulerAngles += _rotationspeed * (float)Time.Instance.DeltaTime;
            SceneManager.RC.DebugLine(transform.GlobalPosition, _moonpos.GlobalPosition, new float3(255, 0, 0));
        }
    }
}
