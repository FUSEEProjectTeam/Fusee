using Fusee.Engine.Common;

#if PLATFORM_DESKTOP
namespace Fusee.Engine.Imp.Graphics.Desktop
#elif PLATFORM_ANDROID

namespace Fusee.Engine.Imp.Graphics.Android
#endif
{
    internal class InstanceDataImp : IInstanceDataImp
    {
        public int Amount { get; set; }

        public int InstanceColorBufferObject { get; set; }

        public int InstanceTransformBufferObject { get; set; }

        public int VertexArrayObject { get; set; }
    }
}
