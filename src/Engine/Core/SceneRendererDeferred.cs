using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core
{

    enum DeferredPasses
    {
        GEOMETRY,
        SSAO,
        SSAO_BLUR,
        SHADOW,
        FXAA,
        LIGHTING
    }

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

        private Dictionary<LightResult, ShadowParams> _shadowRenderTargets; //One per Light

        private readonly TexRes _texRes;
        private readonly ProjectionComponent _projectionComponent;

        private ShaderEffect _ssaoTexEffect;
        private ShaderEffect _lightingPassEffect;
        private ShaderEffect _blurEffect;
        private ShaderEffect _fxaaEffect;

        private ShaderEffect _shadowEffect;

        private DeferredPasses _currentPass;

        /// <summary>
        /// Sets the GL.ClearColor
        /// </summary>
        public float4 BackgroundColor;
        private bool _isBackgroundColorSet = false;

        internal class ShadowParams
        {
            public float4x4 LightSpaceMat;
            public RenderTarget RenderTarget;
            public float2 ClipPlanesForLightMat;

        }

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

            _shadowRenderTargets = new Dictionary<LightResult, ShadowParams>();

            _gBufferRenderTarget.DeleteBuffers += DeleteBuffers;
            _ssaoRenderTarget.DeleteBuffers += DeleteBuffers;
            _blurRenderTarget.DeleteBuffers += DeleteBuffers;
            _lightedSceneRenderTarget.DeleteBuffers += DeleteBuffers;

            _shadowEffect = ShaderCodeBuilder.ShadowMapEffect();

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
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public new void RenderShaderEffect(ShaderEffectComponent shaderComponent)
        {
            if (HasNumberOfLightsChanged)
            {
                //change #define MAX_LIGHTS... or rebuild shader effect?
                HasNumberOfLightsChanged = false;
            }

            if (_currentPass == DeferredPasses.SHADOW)
                _rc.SetShaderEffect(_shadowEffect);
            else
            {
                _rc.SetShaderEffect(shaderComponent.Effect);
                _state.Effect = shaderComponent.Effect;
            }
        }        

        private ShadowParams CalculateLightSpaceMat(LightResult lr)
        {
            float3 lightPos;
            float4x4 lightView;
            float4x4 lightProjection;

            float4x4 lightMat;

            float zNear;
            float zFar;

            switch (lr.Light.Type)
            {
                case Base.Common.LightType.Parallel:
                    //replace with a smarter way to get a proper light position from the scene (directional light has no pos) and to get values for dist to near and far clipping (has to contain the whole scene).
                    lightPos = new float3(0, 2000, 200);                    
                    zNear = 0.1f;
                    zFar = 2500;
                    lightProjection = float4x4.CreateOrthographic(_rc.ViewportWidth, _rc.ViewportWidth, zNear, zFar);
                    //lightProjection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, 16f/9f, zNear, zFar);
                    lightView = float4x4.LookAt(lightPos, lightPos + float3.Normalize(lr.Rotation * float3.UnitZ), float3.Normalize(lr.Rotation * float3.UnitY));
                    lightMat = lightProjection * lightView;
                break;
                //case Base.Common.LightType.Point:
                //    lightPos = lr.WorldSpacePos;
                //    break;
                //case Base.Common.LightType.Spot:
                //    lightPos = lr.WorldSpacePos;
                //    break;
                case Base.Common.LightType.Legacy:
                    lightPos = float4x4.Invert(_rc.View).Column3.xyz;
                    zNear = 0.1f;
                    zFar = 3500;
                    lightProjection = float4x4.CreateOrthographic(_rc.ViewportWidth, _rc.ViewportHeight, zNear, zFar);
                    lightView = float4x4.LookAt(lightPos, lightPos + (float3.Normalize((lr.Rotation * -float4.UnitY).xyz)), float3.Normalize(lr.Rotation * float3.UnitY).xyz);
                    lightMat = lightProjection * lightView;
                    break;
                default:
                    throw new ArgumentException("No Light Space Matrix created, light type not supported!");
            }
           

            if (!_shadowRenderTargets.TryGetValue(lr, out ShadowParams outParams))
            {
                var shadowRenderTarget = new RenderTarget(TexRes.LOW_RES);
                shadowRenderTarget.CreateDepthTex();

                outParams = new ShadowParams() { ClipPlanesForLightMat = new float2(zNear, zFar), LightSpaceMat = lightMat, RenderTarget = shadowRenderTarget };
                _shadowRenderTargets.Add(lr, outParams);
            }
            else
            {
                outParams.LightSpaceMat = lightMat;
                outParams.ClipPlanesForLightMat = new float2(zNear, zFar);
            }

            return outParams;
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

            //Create shadow textures in GBuffer RenderTarget and one RenderTarget for each shadow map.
            _rc.Viewport(0, 0, (int)TexRes.LOW_RES, (int)TexRes.LOW_RES);
            _currentPass = DeferredPasses.SHADOW;
            foreach (var lr in LightResults)
            {
                //TODO: Shadow support only for the first parallel light right now
                if (lr.Light.Type == Base.Common.LightType.Parallel)
                {
                    var shadowParams = CalculateLightSpaceMat(lr);                    
                    rc.SetRenderTarget(shadowParams.RenderTarget);                                       
                    _shadowEffect.SetEffectParam("LightSpaceMatrix", shadowParams.LightSpaceMat);
                    Traverse(_sc.Children);
                    break;
                }
            }

            //Pass 1: Geometry pass
            _rc.Viewport(0, 0, (int)_gBufferRenderTarget.TextureResolution, (int)_gBufferRenderTarget.TextureResolution);
            _currentPass = DeferredPasses.GEOMETRY;
            rc.SetRenderTarget(_gBufferRenderTarget);
            Traverse(_sc.Children);

            //Pass 2: SSAO
            _currentPass = DeferredPasses.SSAO;
            if (_ssaoTexEffect == null)
                _ssaoTexEffect = ShaderCodeBuilder.SSAORenderTargetTextureEffect(_ssaoRenderTarget, _gBufferRenderTarget, 64, new float2((float)_texRes, (float)_texRes), new float2(_projectionComponent.ZNear, _projectionComponent.ZNear));
            _quadShaderEffectComp.Effect = _ssaoTexEffect;
            rc.SetRenderTarget(_ssaoRenderTarget);
            Traverse(_quadScene.Children);

            //Pass 3: Blur SSAO Texture
            _currentPass = DeferredPasses.SSAO_BLUR;
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
                _currentPass = DeferredPasses.LIGHTING;
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
                _currentPass = DeferredPasses.LIGHTING;
                _rc.ClearColor = BackgroundColor;

                // ---- FXAA ON ----              
                foreach (var lr in LightResults)
                {
                    //TODO: Shadow support only for the first parallel light right now
                    if (lr.Light.Type == Base.Common.LightType.Parallel)
                    {
                        if (_lightingPassEffect == null)
                            _lightingPassEffect = ShaderCodeBuilder.DeferredLightingPassEffect(_gBufferRenderTarget, _lightedSceneRenderTarget, _lightComponents.Count, _shadowRenderTargets[lr].RenderTarget); //create in RenderAFrame because ssao tex needs to be generated first.
                        
                        _lightingPassEffect.SetEffectParam("ShadowMap", _shadowRenderTargets[lr].RenderTarget.RenderTextures[(int)RenderTargetTextures.G_DEPTH]);
                        _lightingPassEffect.SetEffectParam("LightSpaceMatrix", _shadowRenderTargets[lr].LightSpaceMat);
                        _lightingPassEffect.SetEffectParam("LightMatClipPlanes", _shadowRenderTargets[lr].ClipPlanesForLightMat);

                        _quadShaderEffectComp.Effect = _lightingPassEffect;
                        rc.SetRenderTarget(_lightedSceneRenderTarget);
                        Traverse(_quadScene.Children);
                        break;
                    }
                }

                _currentPass = DeferredPasses.FXAA;
                _rc.Viewport(0, 0, _projectionComponent.Width, _projectionComponent.Height);
                if (_fxaaEffect == null)
                    _fxaaEffect = ShaderCodeBuilder.FXAARenderTargetEffect(_lightedSceneRenderTarget, new float2((float)_texRes, (float)_texRes));
                _quadShaderEffectComp.Effect = _fxaaEffect;
                rc.SetRenderTarget();
                Traverse(_quadScene.Children);
            }
        }

        private void DeleteBuffers(object sender, EventArgs e)
        {
            var rt = (RenderTarget)sender;
            if (rt.GBufferHandle != null)
                _rc.DeleteFrameBuffer(rt.GBufferHandle);
            if (rt.DepthBufferHandle != null)
                _rc.DeleteRenderBuffer(rt.DepthBufferHandle);
        }
    }
}
