using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization.V1
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
    /// Not yet completely implemented - Projection serialization currently not used.
    /// TODO (MR): check with/against blender
    /// </summary>
    [ProtoContract]
    public class FusCamera : FusComponent
    {
        /// <summary>
        /// If set to false, the color bit won't be cleared before this camera is rendered.
        /// </summary>
        [ProtoMember(1)]
        public bool ClearColor = true;

        /// <summary>
        /// If set to false, the depth bit won't be cleared before this camera is rendered.
        /// </summary>
        [ProtoMember(2)]
        public bool ClearDepth = true;

        /// <summary>
        /// If there is more than one CameraComponent in one scene, the rendered output of the camera with a higher layer will be drawn above the output of a camera with a lower layer.        
        /// </summary>
        [ProtoMember(3)]
        public int Layer;

        /// <summary>
        /// The background color for this camera's viewport.
        /// </summary>
        [ProtoMember(4)]
        public float4 BackgroundColor;

        /// <summary>
        /// You can choose from a set of projection methods. This will automatically calculate the correct projection matrix.
        /// </summary>
        [ProtoMember(5)]
        public ProjectionMethod ProjectionMethod;

        /// <summary>
        /// The vertical (y) field of view in radians.
        /// </summary>
        [ProtoMember(6)]
        public float Fov;

        /// <summary>
        /// Distance to the near and far clipping planes.
        /// </summary>
        [ProtoMember(7)]
        public float2 ClippingPlanes;

        /// <summary>
        /// The viewport in percent [0, 100].
        /// x: start
        /// y: end
        /// z: width
        /// w: height
        /// </summary>
        [ProtoMember(8)]
        public float4 Viewport = new float4(0, 0, 100, 100);

        /// <summary>
        /// A camera is active by default. Set this to false to deactivate it. 
        /// </summary>
        [ProtoMember(9)]
        public bool Active = true;
    }
}


    
