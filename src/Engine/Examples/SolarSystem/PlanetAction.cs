using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class PlanetAction : ActionCode
    {
        private  float3 _rotationSpeed;
        private bool isEarth = false;
        public PlanetAction(float3 rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }
        public override void Start()
        {
            //transform.LocalEulerAngles = new float3(0, 0, 0);
            if (SceneEntity.name == "Earth")
            {
                
                isEarth = true;
            }
        }

        public override void Update()
        {
            transform.LocalEulerAngles += _rotationSpeed*(float)Time.Instance.DeltaTime;
            if (SceneEntity.parent != null)
            {
                SceneManager.RC.DebugLine(SceneEntity.parent.transform.GlobalPosition, transform.GlobalPosition, new float4(1, 0, 0, 1));
            }
             if (isEarth)
            {
                SceneManager.RC.DebugLine(transform.GlobalPosition, transform.Forward*100, new float4(1, 1, 0, 1));
            }
             if (SceneEntity.name == "Neptun")
             {
                 Debug.WriteLine("Neptun is at: "+transform.GlobalPosition);
             }
        }
    }
}
