using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Component and set's a Camera in the scene.
    /// </summary>
    public class Camera : Component
    {
        private float4x4 _cameramatrix;
        private float4x4 _viewmatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class. The ViewMatrix will be set to float4x4 identity matrix.
        /// </summary>
        public Camera()
        {
            ViewMatrix = float4x4.Identity;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class. The ViewMatrix will be set to a provided float 4x4 matrix.
        /// </summary>
        /// <param name="cameratransformation">The matrix that will become the Viewmatrix.</param>
        public Camera(Transformation cameratransformation)
        {
            ViewMatrix = cameratransformation.Matrix;
        }

        /// <summary>
        /// Allows to set and get the Viewmatrix of the Camera.
        /// </summary>
        public float4x4 ViewMatrix
        {
            get { return _viewmatrix; }
            set { _viewmatrix = float4x4.Invert(value);}
        }

        /// <summary>
        /// TraverseForRendering add's Camera to the lightqueue.
        /// </summary>
        /// <returns>A RenderingCamera Object that passes the Camera to the renderContext.</returns>
        public RenderCamera SubmitWork()
        {
            var job = new RenderCamera(_viewmatrix);
            return job;
        }
    }
}
