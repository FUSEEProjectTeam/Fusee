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

        #region Fields
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
        /// Allows to set and get the Viewmatrix of the Camera.
        /// </summary>
        public float4x4 ViewMatrix
        {
            get { return _viewmatrix; }
            set { _viewmatrix = float4x4.Invert(value);}
        }
        private float DegreesToRadians(float deg)
        {
            return (deg * ((float)System.Math.PI)) / 180;
        }

        /// <summary>
        /// Sets the Cameras view width in pixels.
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                Resize();
            }
        }

        /// <summary>
        /// Sets the Cameras view height in pixels.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                Resize();
            }
        }

        /// <summary>
        /// Sets the Cameras field of View angle. Recommended default value is 60 (degrees).
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                _fieldOfView = value;
                Resize();
            }
        }

        /// <summary>
        /// Sets the Cameras near plane. The near plane describes the minimum distance from Camera to an object in scene. If the cameras distance to an object is smaller than the near value is set to, the scene object will not be rendered.
        /// </summary>
        public float Near
        {
            get { return _near; }
            set
            {
                _near = value;
                Resize();
            }
        }

        /// <summary>
        /// Sets the Cameras far plane. The far plane describes the maximum distance from Camera to an object in scene. Scene objects that are further away than the far value will not be rendered.
        /// </summary>
        public float Far
        {
            get { return _far; }
            set
            {
                _far = value;
                Resize();
            }
        }

        /// <summary>
        /// Sets the Cameras aspect ratio. The aspect ratio is the width to height relation of the screen, e.g. 16:9 (aspectratio=16/9).
        /// </summary>
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                _aspectRatio = value;
                Resize();
            }
        }
#endregion
        #region Constructors
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class. The SceneEntity will be set to apply its transformation to the Camera Component.
        /// </summary>
        /// <param name="owner">The SceneEntity where the Camera Component is attached to.</param>
        public Camera(SceneEntity owner)
        {
            ViewMatrix = owner.transform.GlobalMatrix;
            ProjectionType(proj);
            owner.AddComponent(this);
            SceneEntity = owner;
        }
        #endregion
        #region Public Members
        /// <summary>
        /// Sets the projection type of the Camera object.  
        /// </summary>
        /// <param name="projection">The Projection enum whereby Projection.Perspective or Projection.Orthographic is available.</param>
        public void ProjectionType(Projection projection)
        {
            if((int)projection == 0)
            {
                
                // Set Perspective Projection
                proj = projection;
                
                float size = _near*(float) System.Math.Tan(DegreesToRadians(_fieldOfView)/2.0f);
                float left = -size, right = size, bottom = -size/_aspectRatio, top = size/_aspectRatio;

                /*_projMatrix = new float4x4();
                
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
                _projMatrix.M44 = 0.0f;*/
                _projMatrix = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, _near, _far);
                //_projMatrix = float4x4.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _near, _far);
                _projectionDirty = true;

            }
            else if((int)projection == 1)
            {
                // Set Orthographic Projection
                proj = projection;
                _projMatrix = new float4x4();
                _projMatrix = float4x4.CreateOrthographic(_width, _height, _near, _far);
                _projectionDirty = true;
            }
        }

        /// <summary>
        /// This is called by the Resize function inside the entry point object in order to update the Cameras view frustum dimensions upon window resizing.
        /// </summary>
        public void Resize()
        {
            ProjectionType(proj);
        }

        /// <summary>
        /// Sets the Cameras view frustum dimensions with width and height units as pixels.
        /// </summary>
        /// <param name="width">The Camera width in pixels.</param>
        /// <param name="height">The Camera height in pixels.</param>
        public void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            _aspectRatio = _width/_height;
            ProjectionType(proj);
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
        #endregion
        #region Overrides
        /// <summary>
        /// Accept is called by the current visitor. This function is currently used for traversal and search algorithms by the SceneManager object. 
        /// </summary>
        /// <param name="sv">The visitor that is currently traversing the scene.</param>
        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((Camera)this);
            //DrawCameraView();
        }


        #endregion
    }
}
