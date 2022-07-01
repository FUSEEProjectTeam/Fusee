using Fusee.Engine.Common;


namespace Fusee.Engine.Imp.Graphics.Blazor
{
    /// <summary>
    /// Platform specific instance data implementation.
    /// </summary>
    internal class InstanceDataImp : IInstanceDataImp
    {
        /// <summary>
        /// The amount of instances that will get rendered.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The buffer for the instances colors.
        /// </summary>
        public WebGLBuffer InstanceColorBufferObject { get; set; }

        /// <summary>
        /// The buffer for the instances positions.
        /// </summary>
        public WebGLBuffer InstanceTransformBufferObject { get; set; }

        /// <summary>
        /// The VAO of the mesh that is instanced.
        /// </summary>
        public WebGLVertexArrayObject VertexArrayObject { get; set; }
    }
}