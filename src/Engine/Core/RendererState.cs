﻿using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
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
        protected CollapsingStateStack<float4x4> _model = new();

        /// <summary>
        /// State of the <see cref="MinMaxRect"/>.
        /// </summary>
        protected CollapsingStateStack<MinMaxRect> _uiRect = new();

        /// <summary>
        /// State of the <see cref="CanvasXForm"/>.
        /// </summary>
        protected CollapsingStateStack<float4x4> _canvasXForm = new();

        /// <summary>
        /// State of the <see cref="ShaderEffect"/>.
        /// </summary>
        protected CollapsingStateStack<Effect> _effect = new();

        /// <summary>
        /// State of the <see cref="RenderLayer"/>.
        /// </summary>
        protected CollapsingStateStack<RenderLayer> _renderLayer = new();

        /// <summary>
        /// State of the <see cref="RenderLayer"/>.
        /// </summary>
        protected CollapsingStateStack<RenderFlags> _renderModification = new();

        /// <summary>
        /// Gets and sets the top of stack of the Model Matrix state stack.
        /// </summary>
        public float4x4 Model
        {
            get => _model.Tos;
            set => _model.Tos = value;
        }

        /// <summary>
        /// Gets and sets the top of stack of the MinMaxRext state stack.
        /// </summary>
        public MinMaxRect UiRect
        {
            get => _uiRect.Tos;
            set => _uiRect.Tos = value;
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
        public Effect Effect
        {
            set => _effect.Tos = value;
            get => _effect.Tos;
        }

        /// <summary>
        /// Gets and sets the RenderLayer.
        /// </summary>
        public RenderLayer RenderLayer
        {
            set => _renderLayer.Tos = value;
            get => _renderLayer.Tos;
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
            RegisterState(_renderLayer);
            RegisterState(_renderModification);
        }
    }
}