using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// After every Render call the values are reset to the ones saved here.
    /// This ensures that we do not necessarily need a Camera in the Scene Graph.
    /// The viewport width and height is updated with every resize.
    /// </summary>
    public class RenderContextDefaultState
    {
        /// <summary>
        /// This value should be equal to the window/canvas width and is set at every resize.
        /// If this value is changed the default Projection matrix is recalculated.
        /// </summary>
        public int CanvasWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                _aspect = (float)_width / _height;
                if (_aspect != 0)
                    Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
            }
        }

        /// <summary>
        /// This value should be equal to the window/canvas height and is set at every resize.
        /// If this value is changed the default Projection matrix is recalculated.
        /// </summary>
        public int CanvasHeight
        {
            get { return _height; }
            set
            {
                _height = value;
                _aspect = (float)_width / _height;
                if (_aspect != 0)
                    Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
            }
        }

        /// <summary>
        /// The view matrix.
        /// </summary>
        public readonly float4x4 View = float4x4.Identity;

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public float4x4 Projection { get; private set; }

        /// <summary>
        /// The default distance to the near clipping plane.
        /// </summary>
        public readonly float ZNearDefautlt = 0.1f;

        /// <summary>
        /// The default distance to the far clipping plane.
        /// </summary>
        public readonly float ZFarDefault = 3000;

        /// <summary>
        /// The default distance field of view.
        /// </summary>
        public readonly float FovDefault = M.DegreesToRadians(45);

        private int _height = 9;
        private int _width = 16;
        private float _aspect;

        /// <summary>
        /// Creates a new instance of type RenderContextDefaultState.
        /// </summary>
        public RenderContextDefaultState()
        {
            _aspect = (float)_width / _height;
            Projection = float4x4.CreatePerspectiveFieldOfView(FovDefault, _aspect, ZNearDefautlt, ZFarDefault);
        }
    }
}