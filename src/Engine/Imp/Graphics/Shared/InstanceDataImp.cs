using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID

namespace Fusee.Engine.Imp.Graphics.Android
#endif
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
        public int InstanceColorBufferObject { get; set; }

        /// <summary>
        /// The buffer for the instances positions.
        /// </summary>
        public int InstanceTransformBufferObject { get; set; }

        /// <summary>
        /// The VAO of the mesh that is instanced.
        /// </summary>
        public int VertexArrayObject { get; set; }
    }
}