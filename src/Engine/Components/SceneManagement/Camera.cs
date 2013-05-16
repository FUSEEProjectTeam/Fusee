using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// This class is derived from Component and set's a Camera in the scene.
    /// </summary>
    public class Camera : Component
    {
        

        private Projection proj = Projection.Perspective;

        private bool _projectionDirty;
        private float _fieldOfView = 45;
        private float _near = 0.0001f;
        private float _far = 5000;
        private float _aspectRatio = 0.1f;

        private float4x4 _viewmatrix;
        private float4x4 _projMatrix;
        private float _width=640, _height=480;
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class. The ViewMatrix will be set to float4x4 identity matrix.
        /// </summary>
        public Camera()
        {
            ViewMatrix = float4x4.Identity;
            ProjectionType(proj);

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class. The ViewMatrix will be set to a provided float 4x4 matrix.
        /// </summary>
        /// <param name="cameratransformation">The matrix that will become the Viewmatrix.</param>
        public Camera(Transformation cameratransformation)
        {
            ViewMatrix = cameratransformation.GlobalMatrix;
            ProjectionType(proj);
        }

        public Camera(SceneEntity owner)
        {
            ViewMatrix = owner.transform.GlobalMatrix;
            ProjectionType(proj);
            owner.AddComponent(this);
            SceneEntity = owner;
        }

        /// <summary>
        /// Allows to set and get the Viewmatrix of the Camera.
        /// </summary>
        public float4x4 ViewMatrix
        {
            get { return _viewmatrix; }
            set { _viewmatrix = float4x4.Invert(value);}
        }

        public void ProjectionType(Projection projection)
        {
            if((int)projection == 0)
            {
                
                // Set Perspective Projection
                proj = projection;
                
                float size = _near*(float) System.Math.Tan(DegreesToRadians(_fieldOfView)/2.0f);
                float left = -size, right = size, bottom = -size/_aspectRatio, top = size/_aspectRatio;

                _projMatrix = new float4x4();
                
                _projMatrix.M11 = 2*_near/(right - left);
                _projMatrix.M12 = 0.0f;
                _projMatrix.M13 = 0.0f;
                _projMatrix.M14 = 0.0f;

                _projMatrix.M21 = 0.0f;
                _projMatrix.M22 = 2*_near/(top - bottom);
                _projMatrix.M23 = 0.0f;
                _projMatrix.M24 = 0.0f;

                _projMatrix.M31 = (right + left)/(right - left);
                _projMatrix.M32 = (top + bottom)/(top - bottom);
                _projMatrix.M33 = -(_far + _near)/(_far - _near);
                _projMatrix.M34 = -1.0f;

                _projMatrix.M41 = 0.0f;
                _projMatrix.M42 = 0.0f;
                _projMatrix.M43 = -(2*_far*_near)/(_far - _near);
                _projMatrix.M44 = 0.0f;
                
                _projectionDirty = true;

            }
            else if((int)projection == 1)
            {
                // Set Orthographic Projection
                proj = projection;
                //float left = -_height, right = _height, bottom = _width, top = -_width;
                _projMatrix = new float4x4();
                /*
                _projMatrix.M11 = 2.0f / (right - left);
                _projMatrix.M12 = 0.0f;
                _projMatrix.M13 = 0.0f;
                _projMatrix.M14 = 0.0f;

                _projMatrix.M21 = 0.0f;
                _projMatrix.M22 = 2.0f / (top - bottom);
                _projMatrix.M23 = 0.0f;
                _projMatrix.M24 = 0.0f;

                _projMatrix.M31 = 0.0f;
                _projMatrix.M32 = 0.0f;
                _projMatrix.M33 = -2.0f/ (_far - _near);
                _projMatrix.M34 = 0.0f;

                _projMatrix.M41 = -(right + left) / (right -left);
                _projMatrix.M42 = -(top+bottom)/(top-bottom);
                _projMatrix.M43 = -(_far+_near) / (_far - _near);
                _projMatrix.M44 = 1.0f;
                 * */

                _projMatrix = float4x4.CreateOrthographic(_width, _height, _near, _far);

                _projectionDirty = true;
            }
        }

        public void Resize()
        {
            ProjectionType(proj);
        }

        public void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            _aspectRatio = _width/_height;
            ProjectionType(proj);
        }

        private float DegreesToRadians(float deg)
        {
            return (deg*((float) System.Math.PI))/180;
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                Resize();
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                Resize();
            }
        }

        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                _fieldOfView = value;
                Resize();
            }
        }

        public float Near
        {
            get { return _near; }
            set
            {
                _near = value;
                Resize();
            }
        }

        public float Far
        {
            get { return _far; }
            set
            {
                _far = value;
                Resize();
            }
        }

        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                _aspectRatio = value;
                Resize();
            }
        }

        /// <summary>
        /// TraverseForRendering add's Camera to the lightqueue.
        /// </summary>
        /// <returns>A RenderingCamera Object that passes the Camera to the renderContext.</returns>
        public RenderCamera SubmitWork()
        {
           
            var job = new RenderCamera(_viewmatrix, _projMatrix, _projectionDirty);
            _projectionDirty = false;
            return job;
        }
        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((Camera)this);
            //DrawCameraView();
        }


        public void DrawCameraView()
        {
            //globalPosition
            float3 globalpos = SceneManager.RC.ModelView.Row3.xyz;

            // Forward Direction
            float3 forward = -SceneManager.RC.ModelView.Row2.xyz;
            float3 farvec = forward*_far;
            // Center Ray
            SceneManager.RC.DebugLine(new float3(0,0,0),forward*_far,new float4(0,1,0,1));

            // Far left up

            // Far right up

            // Far left down

            // Far right down

        }
    }
}
