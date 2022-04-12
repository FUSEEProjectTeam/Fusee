using System;

namespace Fusee.Engine.Common
{
    [Flags]
    public enum RenderFlags
    {
        None = 1,
        Bones = 2,
        Instanced = 4,
    }
}
