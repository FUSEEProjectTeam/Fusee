using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    public interface ITraversalState
    {
        void StoreMesh(Mesh mesh);
        void AddTransform(float4x4 mtx);
        void StoreRenderer(Renderer renderer);
        void Push();
        void Pop();
    }
}
