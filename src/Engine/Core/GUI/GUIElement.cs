using Fusee.Math;

namespace Fusee.Engine
{
    public abstract class GUIElement
    {
        protected RenderContext RContext;

        public Mesh GUIMesh { get; protected set; }

        protected float PosX;
        protected float PosY;
        protected float Width;
        protected float Height;

        protected string Text;
        protected IFont Font;

        public float4 TextColor { get; set; }

        protected abstract void CreateMesh();

        public void Refresh()
        {
            CreateMesh();
        }
    }
}
