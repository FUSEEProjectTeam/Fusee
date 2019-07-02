using Fusee.Math.Core;
using WebAssembly;

namespace Samples
{
    public interface ISample
    {
        string Description { get; }

        bool EnableFullScreen { get; }

        void Init(JSObject canvas, float4 vector4);

        void Resize(int width, int height);

        void Run();

        void Update(double elapsedTime);

        void Draw();
    }
}