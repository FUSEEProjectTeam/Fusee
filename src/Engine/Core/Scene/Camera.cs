using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Used to define the Projection method of a <see cref="Camera"/>. Defines how the projection matrix is calculated.
    /// </summary>
    public enum ProjectionMethod
    {
        /// <summary>
        /// Perspective projection method.
        /// </summary>
        Perspective,

        /// <summary>
        /// Orthographic projection method.
        /// </summary>
        Orthographic
    }

    /// <summary>
    /// Delegate that allows a user to give a custom projection calculation to a <see cref="Camera"/>.
    /// </summary>
    /// <param name="viewport">The viewport that gets rendered with the resulting projection matrix. Given in pixel. This is used in lower levels to set <see cref="IRenderContextImp.Viewport(int, int, int, int)"/></param>
    /// <param name="projection">The given method needs to calculate the projection matrix.</param>    
    public delegate void CustomCameraUpdate(out float4 viewport, out float4x4 projection);

    /// <summary>
    /// Use this in a <see cref="SceneNode"/> to create a Camera Node.
    /// A the Projection and View matrices, generated from a CameraComponent will overwrite calls you made directly on the RenderContext.
    /// </summary>
    public class Camera : SceneComponent
    {
        /// <summary>
        /// If set to false, the color bit won't be cleared before this camera is rendered.
        /// </summary>
        public bool ClearColor = true;

        /// <summary>
        /// If set to false, the depth bit won't be cleared before this camera is rendered.
        /// </summary>
        public bool ClearDepth = true;

        /// <summary>
        /// Determines whether frustum culling is enabled for this camera or not.
        /// Note that only <see cref="Mesh"/>s with a initialized (non zero-sized) bounding box (<see cref="Mesh.BoundingBox"/>) can be culled.
        /// </summary>
        public bool FrustumCullingOn = true;

        /// <summary>
        /// If there is more than one CameraComponent in one scene, the rendered output of the camera with a higher layer will be drawn above the output of a camera with a lower layer.        
        /// </summary>
        public int Layer;

        /// <summary>
        /// The background color for this camera's viewport.
        /// </summary>
        public float4 BackgroundColor;

        /// <summary>
        /// You can choose from a set of projection methods. This will automatically calculate the correct projection matrix.
        /// </summary>
        public ProjectionMethod ProjectionMethod;

        /// <summary>
        /// The vertical (y) field of view in radians.
        /// </summary>
        public float Fov;

        /// <summary>
        /// Distance to the near and far clipping planes.
        /// </summary>
        public float2 ClippingPlanes;

        /// <summary>
        /// The viewport in percent [0, 100].
        /// x: start
        /// y: end
        /// z: width
        /// w: height
        /// </summary>
        public float4 Viewport = new float4(0, 0, 100, 100);

        /// <summary>
        /// The texture given here will be used as render target.
        /// </summary>
        public IWritableTexture RenderTexture;

        /// <summary>
        /// A camera is active by default. Set this to false to deactivate it. 
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// Allows to overwrite the calculation of the projection matrix. E.g. if <see cref="ProjectionMethod"/> does not contain your desired projection calculation.
        /// </summary>
        public CustomCameraUpdate CustomCameraUpdate;

        /// <summary>
        /// Sets the RenderLayer for this camera.
        /// </summary>
        public RenderLayers RenderLayer;

        /// <summary>
        /// Creates a new instance of type CameraComponent.
        /// </summary>
        /// <param name="projectionMethod">The projection method. See <see cref="ProjectionMethod"/>.</param>
        /// <param name="zNear">The near clipping plane. See <see cref="ClippingPlanes"/>.</param>
        /// <param name="zFar">The far clipping plane. See <see cref="ClippingPlanes"/>.</param>
        /// <param name="fovY">The vertical field of view in radians.</param>
        /// <param name="renderLayer">The <see cref="RenderLayers"/> for this camera</param>
        public Camera(ProjectionMethod projectionMethod, float zNear, float zFar, float fovY, RenderLayers renderLayer = RenderLayers.Default)
        {
            ProjectionMethod = projectionMethod;
            ClippingPlanes = new float2(zNear, zFar);
            Fov = fovY;
            RenderLayer = renderLayer;
        }

        /// <summary>
        /// Calculates and returns the projection matrix.
        /// </summary>
        /// <param name="canvasWidthPx">The width of the render canvas.</param>
        /// <param name="canvasHeightPx">The height of the render canvas.</param>
        /// <param name="viewport">The viewport that gets rendered with the resulting projection matrix. Given in pixel. This is used in lower levels to set <see cref="IRenderContextImp.Viewport(int, int, int, int)"/></param>
        /// <returns></returns>
        public float4x4 GetProjectionMat(int canvasWidthPx, int canvasHeightPx, out float4 viewport)
        {
            if (CustomCameraUpdate != null)
            {
                CustomCameraUpdate(out viewport, out var proj);
                return proj;
            }

            var startX = (int)(canvasWidthPx * (Viewport.x / 100));
            var startY = (int)(canvasHeightPx * (Viewport.y / 100));

            var width = (int)(canvasWidthPx * (Viewport.z / 100));
            var height = (int)(canvasHeightPx * (Viewport.w / 100));

            viewport = new float4(startX, startY, width, height);

            switch (ProjectionMethod)
            {
                default:
                case ProjectionMethod.Perspective:
                    return float4x4.CreatePerspectiveFieldOfView(Fov, System.Math.Abs((float)width / height), ClippingPlanes.x, ClippingPlanes.y);

                case ProjectionMethod.Orthographic:
                    return float4x4.CreateOrthographic(width, height, ClippingPlanes.x, ClippingPlanes.y);
            }
        }

    }

}