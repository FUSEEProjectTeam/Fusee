using Fusee.Math.Core;

namespace Fusee.Serialization
{
    /// <summary>
    /// Used to define the Projection method of a <see cref="CameraComponent"/>. Defines how the projection matrix is calculated.
    /// </summary>
    public enum ProjectionMethod
    {
        /// <summary>
        /// Perspective projection method.
        /// </summary>
        PERSPECTIVE,

        /// <summary>
        /// Orthographic projection method.
        /// </summary>
        ORTHOGRAPHIC
    }

    public delegate void CustomCameraUpdate(out float4x4 projection, out float4 viewport);

    public class CameraComponent : SceneComponentContainer
    {
        public ProjectionMethod ProjectionMethod;

        public float Fov;

        public float2 ClippingPlanes;

        public float4 Viewport = new float4(0, 0, 100, 100);

        //public WritableTexture RenderTexture;

        public bool Active;

        public CustomCameraUpdate CustomCameraUpdate;

        public CameraComponent(ProjectionMethod projectionMethod, float zNear, float zFar, float fovY)
        {
            ProjectionMethod = projectionMethod;
            ClippingPlanes = new float2(zNear, zFar);
            Fov = fovY;
        }

        public float4x4 GetProjectionMat(int canvasWidthPx, int canvasHeightPx)
        {
            if (CustomCameraUpdate != null)
            {
                CustomCameraUpdate(out float4x4 proj, out float4 viewport);
                return proj;                
            }
          
            var width = (int)(canvasWidthPx * (Viewport.z / 100));
            var height = (int)(canvasHeightPx * (Viewport.w / 100));

            switch (ProjectionMethod)
            {
                default:
                case ProjectionMethod.PERSPECTIVE:
                    return float4x4.CreatePerspectiveFieldOfView(Fov, (float)width / height, ClippingPlanes.x, ClippingPlanes.y);
                    
                case ProjectionMethod.ORTHOGRAPHIC:
                    return float4x4.CreateOrthographic(width, height, ClippingPlanes.x, ClippingPlanes.y);
                    
            }
        }       

    }
    
}
