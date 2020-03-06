using System;
using Fusee.Engine.Common;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Contains all supported projection methods. 
    /// Used to tell the Scene Renderer which projection matrix to create.
    /// </summary>
    public enum ProjectionMethod
    {
        /// <summary>
        /// Perspective projection
        /// </summary>
        PERSPECTIVE,
        /// <summary>
        /// Orthographic projection
        /// </summary>
        ORTHOGRAPHIC
    }

    /// <summary>
    /// First evolutionary stage of the soon to be camera component.
    /// At the moment every scene graph needs to have at least one projection component. 
    /// It will create the desired projection matrix that will be used for rendering the contents of the branch of the scene graph it is a part of (and only that branch).
    /// A transform component in the same node as the projection component will not have any effect at the projection at the moment.
    /// </summary>
    public class Projection : SceneComponent
    {
        /// <summary>
        /// The width of the current viewport. Gets set automatically.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the current viewport. Gets set automatically.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Vertical field of view.        
        /// </summary>       
        public float Fov;

        /// <summary>
        /// Distance to near clipping plane.        
        /// </summary>       
        public float ZNear;

        /// <summary>
        /// Distance to far clipping plane.        
        /// </summary>        
        public float ZFar;

        /// <summary>
        /// The projection method.        
        /// </summary>
        public ProjectionMethod ProjectionMethod { get; }

        /// <summary>
        /// Creates a new instance of the projection component class.        
        /// </summary>
        public Projection(ProjectionMethod projectionMethod, float zNear, float zFar, float fov)
        {
            ProjectionMethod = projectionMethod;
            Fov = fov;
            ZFar = zFar;
            ZNear = zNear;
            Width = 1;
            Height = 1;
        }

        /// <summary>
        /// Is called when the window is resized.        
        /// </summary>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

        }

    }
}
