using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
namespace Fusee.SceneManagement
{
    public class Camera : Component
    {
        private float4x4 _cameramatrix;
        private float4x4 _viewmatrix;
        public Camera()
        {
            ViewMatrix = float4x4.Identity;
        }
        public Camera(Transformation cameratransformation)
        {
            ViewMatrix = cameratransformation.Matrix;
        }

        public float4x4 ViewMatrix
        {
            get { return _viewmatrix; }
            set { _viewmatrix = value;}
        }
        
        public RenderCamera SubmitWork()
        {
            var job = new RenderCamera(_viewmatrix);
            return job;
        }
    }
}
