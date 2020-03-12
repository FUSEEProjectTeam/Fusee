using Fusee.Engine.Core.ShaderEffects;
using Fusee.Math.Core;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Saves the current status of the rendered scene a set of <see cref="CollapsingStateStack{T}"/>
    /// </summary>
    public class RendererState : VisitorState
    {
        /// <summary>
        /// State of the Model Matrix.
        /// </summary>
        protected CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// State of the <see cref="MinMaxRect"/>.
        /// </summary>
        protected CollapsingStateStack<MinMaxRect> _uiRect = new CollapsingStateStack<MinMaxRect>();

        /// <summary>
        /// State of the <see cref="CanvasXForm"/>.
        /// </summary>
        protected CollapsingStateStack<float4x4> _canvasXForm = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// State of the <see cref="ShaderEffect"/>.
        /// </summary>
        protected CollapsingStateStack<ShaderEffect> _effect = new CollapsingStateStack<ShaderEffect>();

        /// <summary>
        /// State of the <see cref="RenderStateSet"/>.
        /// </summary>
        protected CollapsingStateStack<RenderStateSet> _renderStates = new CollapsingStateStack<RenderStateSet>();

        /// <summary>
        /// Gets and sets the top of stack of the Render states state stack.
        /// </summary>
        public RenderStateSet RenderUndoStates
        {
            get { return _renderStates.Tos; }
            set { _renderStates.Tos = value; }
        }

        /// <summary>
        /// Gets and sets the top of stack of the Model Matrix state stack.
        /// </summary>
        public float4x4 Model
        {
            get { return _model.Tos; }
            set { _model.Tos = value; }
        }

        /// <summary>
        /// Gets and sets the top of stack of the MinMaxRext state stack.
        /// </summary>
        public MinMaxRect UiRect
        {
            get { return _uiRect.Tos; }
            set { _uiRect.Tos = value; }
        }

        /// <summary>
        /// Gets and sets the top of stack of the CanvasXForm state stack.
        /// </summary>
        public float4x4 CanvasXForm
        {
            get => _canvasXForm.Tos;
            set => _canvasXForm.Tos = value;
        }

        /// <summary>
        /// Gets and sets the shader effect.
        /// </summary>
        public ShaderEffect Effect
        {
            set { _effect.Tos = value; }
            get { return _effect.Tos; }
        }

        /// <summary>
        /// Creates a new instance of type RenderState.
        /// </summary>
        public RendererState()
        {
            RegisterState(_model);
            RegisterState(_canvasXForm);
            RegisterState(_effect);
            RegisterState(_uiRect);
            RegisterState(_renderStates);
        }
    }
}