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
        void AddLightDirectional(float3 direction, float4 color, Light.LightType type);
        void AddLightPoint(float3 position, float4 color, Light.LightType type);
        void AddLightSpot(float3 position, float3 direction, float4 color, Light.LightType type);
        void SetInput(Input input);
        void SetDeltaTime(double delta);
        void GetInput(out Input input);
        void GetDeltaTime(out double deltaTime);
    }
}
