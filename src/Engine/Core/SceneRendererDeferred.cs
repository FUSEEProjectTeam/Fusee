using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRendererDeferred : SceneRendererForward
    {
        /// <summary>
        /// Determines if the scene gets rendered with Fast Approximate Anti Aliasing.
        /// </summary>
        public bool FxaaOn { get; set; } = true;

        private float4 _texClearColor = new float4(1, 1, 1, 1);

        private readonly SceneContainer _quadScene = new SceneContainer();
        private readonly ShaderEffectComponent _quadShaderEffectComp;

        private readonly RenderTarget _gBufferRenderTarget;
        private readonly RenderTarget _ssaoRenderTarget;
        private readonly RenderTarget _blurRenderTarget;
        private readonly RenderTarget _lightedSceneRenderTarget;

        private readonly TexRes _texRes;
        private readonly ProjectionComponent _projectionComponent;

        private ShaderEffect _ssaoTexEffect;
        private ShaderEffect _lightingPassEffect;
        private ShaderEffect _blurEffect;
        private ShaderEffect _fxaaEffect;

        public float4 BackgroundColor;
        private bool _isBackgroundColorSet = false;

        /// <summary>
        /// Creates a new instance of type SceneRendererDeferred.
        /// </summary>
        /// <param name="sc">The SceneContainer, containing the scene that gets rendered.</param>
        /// <param name="texRes">Resolution of the render textures.</param>
        /// <param name="projComp">The projection component, that is used to render the scene.</param>
        public SceneRendererDeferred(SceneContainer sc, TexRes texRes, ProjectionComponent projComp) : base(sc)
        {
            _texRes = texRes;
            _projectionComponent = projComp;
            _gBufferRenderTarget = new RenderTarget(_texRes);
            _ssaoRenderTarget = new RenderTarget(_texRes);
            _blurRenderTarget = new RenderTarget(_texRes);
            _lightedSceneRenderTarget = new RenderTarget(_texRes);

            //Create ShaderEffect for deferred rendering ...TODO: add support for textures, create shader effect in Scene Converter to avoid another traversal.
            foreach (var child in sc.Children)
            {  
                var oldEffectComp = child.GetComponent<ShaderEffectComponent>();
                if (oldEffectComp == null) continue;
                var oldEffect = oldEffectComp.Effect;

                var col = (float4)oldEffect.GetEffectParam("DiffuseColor");
                var specStrength = (float)oldEffect.GetEffectParam("SpecularIntensity");
                col.a = specStrength;

                oldEffect.GetEffectParam("DiffuseTexture", out object tex);
                oldEffect.GetEffectParam("DiffuseMix", out object mix);

                var renderTargetMat = new ShaderEffectComponent();
                if (tex != null)
                {
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, (float)mix, (Texture)tex);                   
                }
                else
                {
                    renderTargetMat.Effect = ShaderCodeBuilder.GBufferTextureEffect(_gBufferRenderTarget, 1f);
                }

                child.RemoveComponent<ShaderEffectComponent>();

                child.Components.Insert(1, renderTargetMat);
                renderTargetMat.Effect.SetEffectParam("DiffuseColor", col);                
            }

            _quadScene = new SceneContainer()
            {
                Children = new List<SceneNodeContainer>()
                {
                    new SceneNodeContainer()
                    {
                        Components = new List<SceneComponentContainer>()
                        {
                            _projectionComponent,

                            new ShaderEffectComponent()
                            {
                                Effect = _lightingPassEffect
                            },
                            new Plane()

                        }
                    }
                }
            };

            _quadShaderEffectComp = (ShaderEffectComponent)_quadScene.Children[0].Components[1];
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext"/>.</param>        
        public void Render(RenderContext rc)
        {           

            AccumulateLight();
            SetContext(rc);
            
            if (!_isBackgroundColorSet)
            {
                BackgroundColor = _rc.ClearColor;
                _isBackgroundColorSet = true;
            }

            _rc.ClearColor = _texClearColor;
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution);

            //Pass 1: Geometry pass
            rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);

            //Pass 2: SSAO
            if (_ssaoTexEffect == null)
                _ssaoTexEffect = ShaderCodeBuilder.SSAORenderTargetTextureEffect(_ssaoRenderTarget, _gBufferRenderTarget, 64, new float2((float)_texRes, (float)_texRes), new float2(_projectionComponent.ZNear, _projectionComponent.ZNear));
            _quadShaderEffectComp.Effect = _ssaoTexEffect;
            rc.SetRenderTarget(_ssaoRenderTarget);
            Traverse(_quadScene.Children);

            //Pass 3: Blur SSAO Texture
            if (_blurEffect == null)
                _blurEffect = ShaderCodeBuilder.SSAORenderTargetBlurEffect(_ssaoRenderTarget, _blurRenderTarget);
            _quadShaderEffectComp.Effect = _blurEffect;
            rc.SetRenderTarget(_blurRenderTarget);
            Traverse(_quadScene.Children);

            //Set blurred SSAO Texture as SSAO Texture in gBuffer
            _gBufferRenderTarget.SetTextureFromRenderTarget(_blurRenderTarget, RenderTargetTextures.G_SSAO);
            
            //Pass 4 & 5: FXAA and Lighting
            if (!FxaaOn)
            {
                _rc.ClearColor = BackgroundColor;

                // ----FXAA OFF----
                _rc.Viewport(0, 0, _projectionComponent.Width, _projectionComponent.Height);
                if (_lightingPassEffect == null)
                    _lightingPassEffect = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, _lightComponents.Count); //create in Render() because ssao tex needs to be generated first.
                _quadShaderEffectComp.Effect = _lightingPassEffect;
                rc.SetRenderTarget();
                Traverse(_quadScene.Children);
            }
            else
            {
                _rc.ClearColor = BackgroundColor;

                // ---- FXAA ON ----
                if (_lightingPassEffect == null)
                    _lightingPassEffect = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, _lightedSceneRenderTarget, _lightComponents.Count); //create in RenderAFrame because ssao tex needs to be generated first.
                _quadShaderEffectComp.Effect = _lightingPassEffect;
                rc.SetRenderTarget(_lightedSceneRenderTarget);
                Traverse(_quadScene.Children);

                
                _rc.Viewport(0, 0, _projectionComponent.Width, _projectionComponent.Height);
                if (_fxaaEffect == null)
                    _fxaaEffect = ShaderCodeBuilder.FXAARenderTargetEffect(_lightedSceneRenderTarget, new float2((float)_texRes, (float)_texRes));
                _quadShaderEffectComp.Effect = _fxaaEffect;
                rc.SetRenderTarget();
                Traverse(_quadScene.Children);
            }
        }
    }
}
