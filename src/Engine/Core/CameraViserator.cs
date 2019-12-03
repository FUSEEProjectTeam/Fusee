using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    internal class CameraViseratorState : VisitorState
    {
        private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// Gets and sets the top of the Model matrix stack. The Model matrix transforms model coordinates into world coordinates.
        /// </summary>
        /// <value>
        /// The Model matrix.
        /// </value>
        public float4x4 Model
        {
            set { _model.Tos = value; }
            get { return _model.Tos; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightViseratorState"/> class.
        /// </summary>
        public CameraViseratorState()
        {
            RegisterState(_model);
        }
    }

    internal class CameraViserator : Viserator<CameraComponent, PrepassVisitorState>
    {
        protected override void InitState()
        {
            base.InitState();
            State.Model = float4x4.Identity;
        }

        [VisitMethod]
        public void OnTransform(TransformComponent xform)
        {
            State.Model *= xform.Matrix();
        }

        [VisitMethod]
        public void OnLight(CameraComponent pc)
        {
            YieldItem(pc);
        }
    }
}
