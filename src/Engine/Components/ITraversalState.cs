using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace SceneManagement
{
    interface ITraversalState
    {
        void StoreMesh(Mesh mesh);
        void AddTransform(float4x4 mtx);
        void StoreRenderer(Renderer renderer);
        void Push();
        void Pop();
    }
}
