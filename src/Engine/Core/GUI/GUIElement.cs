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
        private float4 _textColor;

        public float4 TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                CreateMesh();
            }
        }

        protected abstract void CreateMesh();
    }
}
