using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using Fusee.Xirkit;


namespace Fusee.Engine.Core
{
    /// <summary>
    /// Axis-Aligned Bounding Box Calculator. Use instances of this class to calculate axis-aligned bounding boxes
    /// on scenes, list of scene nodes or individual scene nodes. Calculations always include any child nodes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class AABBCalculator : SceneVisitor
    {
        // ReSharper disable once InconsistentNaming
        public class AABBState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _modelView = new CollapsingStateStack<float4x4>();

            public float4x4 ModelView
            {
                set { _modelView.Tos = value; }
                get { return _modelView.Tos; }
            }

            public AABBState()
            {
                RegisterState(_modelView);
            }
        }

        //private SceneContainer _sc;
        private IEnumerable<SceneNodeContainer> _sncList;
        private AABBState _state = new AABBState();
        private bool _boxValid;
        private AABBf _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sc">The scene container to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneContainer sc)
        {
            _sncList = sc.Children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sncList">The list of scene nodes to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(IEnumerable<SceneNodeContainer> sncList)
        {
            _sncList = sncList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="snc">A single scene node to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneNodeContainer snc)
        {
            _sncList = SceneVisitorHelpers.SingleRootEnumerable(snc);
        }

        /// <summary>
        /// Performs the calculation and returns the resulting box on the object(s) passed in the constructor. Any calculation
        /// always includes a full traversal over all child nodes.
        /// </summary>
        /// <returns>The resulting axis-aligned bounding box.</returns>
        public AABBf? GetBox()
        {
            Traverse(_sncList);
            if (_boxValid)
                return _result;
            return null;
        }

        #region Visitors

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="transform">The transform component.</param>
        [VisitMethod]
        public void OnTransform(TransformComponent transform)
        {
            _state.ModelView *= transform.Matrix();
        }

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="meshComponent">The mesh component.</param>
        [VisitMethod]
        public void OnMesh(MeshComponent meshComponent)
        {
            var box = _state.ModelView * meshComponent.BoundingBox;
            if (!_boxValid)
            {
                _result = box;
                _boxValid = true;
            }
            else
            {
                _result = AABBf.Union(_result, box);
            }
        }

        #endregion

        #region HierarchyLevel

        protected override void InitState()
        {
            _boxValid = false;
            _state.Clear();
            _state.ModelView = float4x4.Identity;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
        }

        #endregion
    }

    /// <summary>
    /// All supported lightning calculation methods ShaderCodeBuilder.cs supports.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public enum LightningCalculationMethod
    {
        /// <summary>
        /// Simple Blinn Phong Shading without fresnel & distribution function
        /// </summary>
        SIMPLE,

        /// <summary>
        /// Physical based shading
        /// </summary>
        ADVANCED
    }

    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRenderer : SceneVisitor
    {
        // Choose Lightning Method
        public static LightningCalculationMethod LightningCalculationMethod;
        // All lights
        public static IList<LightResult> AllLightResults = new List<LightResult>();
        // Multipass
        private bool _renderWithShadows;
        private bool _renderDeferred;
        public float2 ShadowMapSize { set; get; } = new float2(1024,1024);


        public bool RenderShadows
        {
            set
            {
                if (!_renderWithShadows) return;

                if (_rc.GetHardwareCapabilities(HardwareCapability.DEFFERED_POSSIBLE) == 1U)
                {
                    _renderWithShadows = value;
                }
                   
                else
                {
                    throw new ArgumentException("Deferred Rendering not possible with the current renderpath!");
                }
            }
            get { return _renderWithShadows; }
        }

        public bool RenderDeferred
        {
            set
            {
                if (!_renderDeferred) return;

                if (_rc.GetHardwareCapabilities(HardwareCapability.DEFFERED_POSSIBLE) == 1U)
                {
                    _renderDeferred = value;
                }

                else
                {
                    throw new ArgumentException("Deferred Rendering not possible with the current renderpath!");
                }
            }
            get { return _renderDeferred; }
        }


       

        #region Traversal information

        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<MaterialLightComponent, ShaderEffect> _lightMatMap;
        private Dictionary<MaterialPBRComponent, ShaderEffect> _pbrComponent;
        private Dictionary<SceneNodeContainer, float4x4> _boneMap;
        private Dictionary<ShaderComponent, ShaderEffect> _shaderEffectMap;
        private Animation _animation;
        private SceneContainer _sc;

        private RenderContext _rc;


        private List<LightResult> _lightComponents; 

        private string _scenePathDirectory;
        private ShaderEffect _defaultEffect;



        #endregion

        #region State

        public class RendererState : VisitorState
        {
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            private StateStack<ShaderEffect> _effect = new StateStack<ShaderEffect>();

            public ShaderEffect Effect
            {
                set { _effect.Tos = value; }
                get { return _effect.Tos; }
            }

            public RendererState()
            {
                RegisterState(_model);
                RegisterState(_effect);
            }
        };

        private RendererState _state;
        private float4x4 _view;

        #endregion

        #region Initialization Construction Startup

        public SceneRenderer(SceneContainer sc, LightningCalculationMethod lCalcMethod, bool RenderShadows = false, bool RenderDeferred = false)
             : this(sc)
        {
            LightningCalculationMethod = lCalcMethod;
            
            if (RenderShadows)
                _renderWithShadows = true;

            if (RenderDeferred)
                _renderDeferred = true;
        }

     

        public SceneRenderer(SceneContainer sc /*, string scenePathDirectory*/)
        {
            // accumulate all lights and...
            _lightComponents = sc.Children.Viserate<LightSetup, LightResult>().ToList();
            // ...set them
            AllLightResults = _lightComponents;

            if (AllLightResults.Count == 0)
            {
                // if there is no light in scene then add one (legacyMode)
                AllLightResults.Add(new LightResult
                {
                    PositionWorldSpace = float3.UnitZ,
                    Position = float3.UnitZ,
                    Active = true,
                    AmbientCoefficient = 0.2f,
                    Attenuation = 3000,
                    Color = new float3(0.4f, 0.4f, 0.4f),
                    ConeAngle = 45f,
                    ConeDirection = float3.UnitZ,
                    ModelMatrix = float4x4.Identity,
                    Type = LightType.Parallel
                });
            }
           
            _sc = sc;
            // _scenePathDirectory = scenePathDirectory;
            _state = new RendererState();
            InitAnimations(_sc);
        }

        public void InitAnimations(SceneContainer sc)
        {
            _animation = new Animation();

            foreach (AnimationComponent ac in sc.Children.FindComponents<AnimationComponent>(c => true))
            {
                if (ac.AnimationTracks != null)
                {
                    foreach (AnimationTrackContainer animTrackContainer in ac.AnimationTracks)
                    {
                        Type t = animTrackContainer.KeyType;
                        if (typeof(int).IsAssignableFrom(t))
                        {
                            Channel<int> channel = new Channel<int>(Lerp.IntLerp);
                            foreach (AnimationKeyContainerInt key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<int>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof(float).IsAssignableFrom(t))
                        {
                            Channel<float> channel = new Channel<float>(Lerp.FloatLerp);
                            foreach (AnimationKeyContainerFloat key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof(float2).IsAssignableFrom(t))
                        {
                            Channel<float2> channel = new Channel<float2>(Lerp.Float2Lerp);
                            foreach (AnimationKeyContainerFloat2 key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float2>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof(float3).IsAssignableFrom(t))
                        {
                            Channel<float3>.LerpFunc lerpFunc;
                            switch (animTrackContainer.LerpType)
                            {
                                case LerpType.Lerp:
                                    lerpFunc = Lerp.Float3Lerp;
                                    break;
                                case LerpType.Slerp:
                                    lerpFunc = Lerp.Float3QuaternionSlerp;
                                    break;
                                default:
                                    // C# 6throw new InvalidEnumArgumentException(nameof(animTrackContainer.LerpType), (int)animTrackContainer.LerpType, typeof(LerpType));
                                    // throw new InvalidEnumArgumentException("animTrackContainer.LerpType", (int)animTrackContainer.LerpType, typeof(LerpType));
                                    throw new InvalidOperationException(
                                        "Unknown lerp type: animTrackContainer.LerpType: " +
                                        (int)animTrackContainer.LerpType);
                            }
                            Channel<float3> channel = new Channel<float3>(lerpFunc);
                            foreach (AnimationKeyContainerFloat3 key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float3>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof(float4).IsAssignableFrom(t))
                        {
                            Channel<float4> channel = new Channel<float4>(Lerp.Float4Lerp);
                            foreach (AnimationKeyContainerFloat4 key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float4>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        //TODO : Add cases for each type
                    }
                }
            }
        }

        public void Animate()
        {
            if (_animation.ChannelBaseList.Count != 0)
            {
                // Set the animation time here!
                _animation.Animate(Time.DeltaTime);
            }
        }

        private float2 _rcViewportOriginalSize;

        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");

            if (rc != _rc)
            {
                _rc = rc;
                _rcViewportOriginalSize = new float2(_rc.ViewportWidth, _rc.ViewportHeight);
                _meshMap = new Dictionary<MeshComponent, Mesh>();
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _lightMatMap = new Dictionary<MaterialLightComponent, ShaderEffect>();
                _pbrComponent = new Dictionary<MaterialPBRComponent, ShaderEffect>();
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
                _shaderEffectMap = new Dictionary<ShaderComponent, ShaderEffect>();
                _defaultEffect = MakeMaterial(new MaterialComponent
                {
                    Diffuse = new MatChannelContainer()
                    {
                        Color = new float3(0.5f, 0.5f, 0.5f)
                    },
                    Specular = new SpecularChannelContainer()
                    {
                        Color = new float3(1, 1, 1),
                        Intensity = 0.5f,
                        Shininess = 22
                    }
                });
                _defaultEffect.AttachToContext(_rc);

                // Check for hardware capabilities:
                RenderDeferred = _renderDeferred;
                RenderShadows = _renderWithShadows;
            }
        }

        #endregion
        

        public void Render(RenderContext rc)
        {
            if (RenderShadows)
            {
                RenderWithShadow(rc);
            }
            else if (RenderDeferred)
            {
                RenderDeferredPasses(rc);
            }
            else
            {
                rc.SetRenderTarget(null);
                SetContext(rc);
                Traverse(_sc.Children);
            }
         }


        private void RenderDeferredPasses(RenderContext rc)
        {
            SetContext(rc);
            
            if (DeferredShaderHelper.GBufferTexture == null)
                DeferredShaderHelper.GBufferTexture = rc.CreateWritableTexture(rc.ViewportWidth, rc.ViewportHeight, ImagePixelFormat.GBuffer);

            if (DeferredShaderHelper.GBufferPassShaderEffect == null)
                CreateGBufferPassEffect(rc);
            
            if (DeferredShaderHelper.GBufferDrawPassShaderEffect == null)
                CreateGBufferDrawPassEffect(rc);
            
            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    // Set RenderTarget to gBuffer
                    rc.SetRenderTarget(DeferredShaderHelper.GBufferTexture);
                    Traverse(_sc.Children);
                    DeferredShaderHelper.CurrentRenderPass++;
                }
                else
                {
                    // Set RenderTarget to Screenbuffer, but before, copy z-buffer from deferred pass to screenbuffer
                    // TODO: Evaluate if this could be written better.
                    //rc.SetRenderTarget(DeferredShaderHelper.GBufferTexture, true);
                    rc.SetRenderTarget(null);
                    Traverse(_sc.Children);
                    DeferredShaderHelper.CurrentRenderPass--;
                }
            }
        }


        private void RenderWithShadow(RenderContext rc)
        {

            // ShadowMap Size 1024x1024:
            ShadowMapSize = new float2(1024,1024);

            SetContext(rc);
            
            // Create ShadowTexture if none avaliable
            if (DeferredShaderHelper.ShadowTexture == null)
                DeferredShaderHelper.ShadowTexture = rc.CreateWritableTexture((int) ShadowMapSize.x, (int) ShadowMapSize.y, ImagePixelFormat.Depth);

            if (DeferredShaderHelper.ShadowPassShaderEffect == null)
                CreateShadowPassShaderEffect(rc);
            
            // Parse RenderNormalShadowPass
            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    // Set RenderTarget to FBO
                    rc.SetRenderTarget(DeferredShaderHelper.ShadowTexture);
                    Traverse(_sc.Children);
                    DeferredShaderHelper.CurrentRenderPass++;
                }
                else
                {
                    // Set RenderTarget to Screenbuffer
                    rc.SetRenderTarget(null);
                    Traverse(_sc.Children);
                    DeferredShaderHelper.CurrentRenderPass--;
                }
            }
        }

        private void CreateShadowPassShaderEffect(RenderContext rc)
        {
            var effectPass = new EffectPassDeclaration[1];
            effectPass[0] = new EffectPassDeclaration
            {
                PS = DeferredShaderHelper.OrtographicShadowMapMvPixelShader(),
                VS = DeferredShaderHelper.OrtographicShadowMapMvVertexShader(),
                StateSet = new RenderStateSet
                {
                    //CullMode = Cull.Clockwise // This is not working due to the fact, that we cant change the RenderStateSet for the normal render pass
                    // therefore we are using GL.Cull(Front / Back) in RenderContextImp!
                }
            };
            var effectParameter = new List<EffectParameterDeclaration>
                        {
                            new EffectParameterDeclaration {Name = "LightMVP", Value = DeferredShaderHelper.ShadowMapMVP}
                        };

            DeferredShaderHelper.ShadowPassShaderEffect = new ShaderEffect(effectPass, effectParameter);
            DeferredShaderHelper.ShadowPassShaderEffect.AttachToContext(_rc);
        }

        private static void CreateGBufferPassEffect(RenderContext rc)
        {
            var effectPass = new EffectPassDeclaration[1];
            effectPass[0] = new EffectPassDeclaration
            {
                VS = DeferredShaderHelper.DeferredPassVertexShader(),
                PS = DeferredShaderHelper.DeferredPassPixelShader(),
                StateSet = new RenderStateSet()
            };
            var effectParameter = new List<EffectParameterDeclaration>
                        {
                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float3.One },
                            new EffectParameterDeclaration {Name = "SpecularIntensity", Value = float3.One }
                        };

            DeferredShaderHelper.GBufferPassShaderEffect = new ShaderEffect(effectPass, effectParameter);
            DeferredShaderHelper.GBufferPassShaderEffect.AttachToContext(rc);
        }

        private static void CreateGBufferDrawPassEffect(RenderContext rc)
        {

            var effectPass = new EffectPassDeclaration[1];
            effectPass[0] = new EffectPassDeclaration
            {
                VS = DeferredShaderHelper.DeferredDrawPassVertexShader(),
                PS = DeferredShaderHelper.DeferredDrawPassPixelShader(),
                StateSet = new RenderStateSet()
            };

            var effectParameter = new List<EffectParameterDeclaration>
            {
                new EffectParameterDeclaration { Name = "gPosition", Value = DeferredShaderHelper.GBufferTexture },
                new EffectParameterDeclaration { Name = "gNormal", Value = DeferredShaderHelper.GBufferTexture },
                new EffectParameterDeclaration { Name = "gAlbedoSpec", Value = DeferredShaderHelper.GBufferTexture },
                new EffectParameterDeclaration { Name = "gDepth", Value = DeferredShaderHelper.GBufferTexture },
                new EffectParameterDeclaration { Name = "gShowCase", Value = DeferredShaderHelper.GBufferTexture }
            };

            DeferredShaderHelper.GBufferDrawPassShaderEffect = new ShaderEffect(effectPass, effectParameter);
            DeferredShaderHelper.GBufferDrawPassShaderEffect.AttachToContext(rc);
        }

        #region Visitors

        [VisitMethod]
        public void RenderBone(BoneComponent bone)
        {
            SceneNodeContainer boneContainer = CurrentNode;
            float4x4 transform;
            if (!_boneMap.TryGetValue(boneContainer, out transform))
                _boneMap.Add(boneContainer, _rc.ModelView); // Changed from Model to ModelView
            else
                _boneMap[boneContainer] = _rc.ModelView; // Changed from Model to ModelView
        }

        [VisitMethod]
        public void RenderWeight(WeightComponent weight)
        {
            float4x4[] boneArray = new float4x4[weight.Joints.Count()];
            for (int i = 0; i < weight.Joints.Count(); i++)
            {
                float4x4 tmp = weight.BindingMatrices[i];
                boneArray[i] = _boneMap[weight.Joints[i]] * tmp;
            }
            _rc.Bones = boneArray;
        }


        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            _state.Model *= transform.Matrix();
            _rc.ModelView = _view * _state.Model; // Changed from Model to ModelView
        }

        [VisitMethod]
        public void RenderMaterial(MaterialComponent matComp)
        {
            if (matComp.GetType() == typeof(MaterialLightComponent)) return;
            if (matComp.GetType() == typeof(MaterialPBRComponent)) return;

            var effect = LookupMaterial(matComp);
            _state.Effect = effect;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialLightComponent matComp)
        {
            if (matComp.GetType() == typeof(MaterialPBRComponent)) return;

            var effect = LookupLightMaterial(matComp);
            _state.Effect = effect;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialPBRComponent matComp)
        {
            if (matComp.GetType() == typeof(MaterialLightComponent)) return;

            var effect = LookupPBRMaterial(matComp);
            _state.Effect = effect;
        }


        [VisitMethod]
        public void RenderShader(ShaderComponent shaderComponent)
        {
            var effect = BuildMaterialFromShaderComponent(shaderComponent);
            _state.Effect = effect;
        }

        [VisitMethod]
        public void RenderMesh(MeshComponent meshComponent)
        {
            Mesh rm;
            if (!_meshMap.TryGetValue(meshComponent, out rm))
            {
                rm = MakeMesh(meshComponent);
                _meshMap.Add(meshComponent, rm);
            }

            RenderCurrentPass(rm, _state.Effect);
        }

       [VisitMethod]
        public void AccumulateLight(LightComponent lightComponent)
        {
            
            // accumulate all lights and...
            _lightComponents = _sc.Children.Viserate<LightSetup, LightResult>().ToList();
            // ...set them
            AllLightResults = _lightComponents;
            // and multiply them with current modelview matrix
            // normalize etc.
            SetupLights();
            
        }
        private void SetupLights()
        {
            // Add ModelView Matrix to all lights
            for (var i = 0; i < AllLightResults.Count; i++)
            {
                var light = AllLightResults[i];
                
                // Multiply LightPosition with modelview
                light.PositionWorldSpace = _rc.ModelView * light.PositionWorldSpace;

                // float4 is really needed
                var lightConeDirectionFloat4 = new float4(light.ConeDirection.x, light.ConeDirection.y, light.ConeDirection.z,
                                          0.0f);
                lightConeDirectionFloat4 = _rc.ModelView * lightConeDirectionFloat4;
                lightConeDirectionFloat4.Normalize();
                light.ConeDirectionModelViewSpace = new float3(lightConeDirectionFloat4.x, lightConeDirectionFloat4.y, lightConeDirectionFloat4.z);   
                

                // convert spotlight angle from degrees to radians
                light.ConeAngle = M.DegreesToRadians(light.ConeAngle);                                   
                AllLightResults[i] = light;
            }
        }

        #endregion

        #region HierarchyLevel

        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _view = _rc.ModelView;

            _state.Effect = _defaultEffect;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
            _rc.ModelView = _view * _state.Model;
        }

        #endregion

        public void RenderCurrentPass(Mesh rm, ShaderEffect effect)
        {
            if (RenderShadows)
            {
                if (DeferredShaderHelper.CurrentRenderPass == 0)
                {
                    RenderShadowMapPass(rm, effect);
                }
                else
                {
                    RenderNormalShadowPass(rm, effect);
                }
            }
            else if (RenderDeferred)
            {
                if (DeferredShaderHelper.CurrentRenderPass == 0)
                {
                    RenderDeferredPass(rm, effect);
                }
                else
                {
                    RenderNormalDeferredPass(rm);
                }
            }
            else
            {
                RenderStandardPass(rm, effect);
            }
        }


        private void RenderStandardPass(Mesh rm, ShaderEffect effect)
        {
            for (var i = 0; i < _lightComponents.Count; i++)
            {
                SetupLight(i, _lightComponents[i], effect);
                effect.RenderMesh(rm);
            }
        }

        private static void RenderDeferredPass(Mesh rm, ShaderEffect effect) {

        /*    var diffuse = float3.One;
            if (effect._rc.CurrentShader != null && effect.GetEffectParam("DiffuseColor") != null)
                 diffuse = (float3) effect.GetEffectParam("DiffuseColor");

            var specularIntensity = 1.0f;
            if (effect._rc.CurrentShader != null && effect.GetEffectParam("SpecularIntensity") != null)
                specularIntensity = (float)effect.GetEffectParam("SpecularIntensity");*/

            DeferredShaderHelper.GBufferPassShaderEffect.SetEffectParam("DiffuseColor", float3.One);
            DeferredShaderHelper.GBufferPassShaderEffect.RenderMesh(rm);
        }

        private static void RenderNormalDeferredPass(Mesh rm) {
            
            if(DeferredShaderHelper.GBufferDrawPassShaderEffect == null) return;
         
                // Set textures from first GBuffer pass
                var gPosition = DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.CurrentShader.GetShaderParam("gPosition");
                if (gPosition != null)
                    DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.SetShaderParamTexture(gPosition, DeferredShaderHelper.GBufferTexture, GBufferHandle.gPositionHandle);

                var gNormal = DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.CurrentShader.GetShaderParam("gNormal");
                if (gNormal != null)
                    DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.SetShaderParamTexture(gNormal, DeferredShaderHelper.GBufferTexture, GBufferHandle.gNormalHandle);

                var gAlbedoSpec = DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.CurrentShader.GetShaderParam("gAlbedoSpec");
                if (gAlbedoSpec != null)
                    DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.SetShaderParamTexture(gAlbedoSpec, DeferredShaderHelper.GBufferTexture, GBufferHandle.gAlbedoSpecHandle);

                var gDepth = DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.CurrentShader.GetShaderParam("gDepth");
                if (gDepth != null)
                    DeferredShaderHelper.GBufferDrawPassShaderEffect._rc.SetShaderParamTexture(gDepth, DeferredShaderHelper.GBufferTexture, GBufferHandle.gDepthHandle);
                
                // This DeferredFullscreenQuad lets FUSEE crash!
                DeferredShaderHelper.GBufferDrawPassShaderEffect.RenderMesh(DeferredShaderHelper.DeferredFullscreenQuad());


             /*for (var i = 0; i < _lightComponents.Count; i++)
               {
                   SetupLight(i, _lightComponents[i], effect);

               }*/

        }

        private void RenderShadowMapPass(Mesh rm, ShaderEffect effect)
        {
           // if(_shadowPassShaderEffect == null) return;
            if(AllLightResults.Count == 0) return;

            // Set viewport to ShadowMapSize
            _rc.Viewport(0, 0, (int) ShadowMapSize.x, (int) ShadowMapSize.y);

            DeferredShaderHelper.SetShadowMapMVP(AllLightResults[0].Position, AllLightResults[0].ConeDirection, 1.0f, _view);
            DeferredShaderHelper.ShadowPassShaderEffect.SetEffectParam("LightMVP", DeferredShaderHelper.ShadowMapMVP);
            DeferredShaderHelper.ShadowPassShaderEffect.RenderMesh(rm);
        }

        private void RenderNormalShadowPass(Mesh rm, ShaderEffect effect)
        {
            if(effect._rc.CurrentShader == null) return;

            // reset Viewport to orginal size; is wrong size due to creation of shadowmap with different viewportsize
            _rc.Viewport(0, 0, (int)_rcViewportOriginalSize.x, (int)_rcViewportOriginalSize.y);

            var handleLight = effect._rc.GetShaderParam(effect._rc.CurrentShader, "shadowMVP");
            if (handleLight != null)
                effect._rc.SetShaderParam(handleLight, DeferredShaderHelper.ShadowMapMVP);

            var handle = effect._rc.GetShaderParam(effect._rc.CurrentShader, "firstPassTex");
            if (handle != null)
                effect._rc.SetShaderParamTexture(handle, DeferredShaderHelper.ShadowTexture);


            for (var i = 0; i < _lightComponents.Count; i++)
            {
                SetupLight(i, _lightComponents[i], effect);
                effect.RenderMesh(rm);
            }
        }

        private static void SetupLight(int position, LightResult light, ShaderEffect effect)
        {
            if (!light.Active) return;

            // Set params in modelview space since the lightning calculation is in modelview space
            effect.SetEffectParam($"allLights[{position}].position", light.PositionModelViewSpace);
            effect.SetEffectParam($"allLights[{position}].intensities", light.Color);
            effect.SetEffectParam($"allLights[{position}].attenuation", light.Attenuation);
            effect.SetEffectParam($"allLights[{position}].ambientCoefficient", light.AmbientCoefficient);
            effect.SetEffectParam($"allLights[{position}].coneAngle", light.ConeAngle);
            effect.SetEffectParam($"allLights[{position}].coneDirection", light.ConeDirectionModelViewSpace);
            effect.SetEffectParam($"allLights[{position}].lightType", light.Type);            
        }

        #region RenderContext/Asset Setup


        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            ShaderEffect mat;
            if (_matMap.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            mat.AttachToContext(_rc);
            _matMap.Add(mc, mat);
            return mat;
        }
        private ShaderEffect LookupLightMaterial(MaterialLightComponent mc)
        {
            ShaderEffect mat;
            if (_lightMatMap.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            mat.AttachToContext(_rc);
            _lightMatMap.Add(mc, mat);
            return mat;
        }

        private ShaderEffect LookupPBRMaterial(MaterialPBRComponent mc)
        {
            ShaderEffect mat;
            if (_pbrComponent.TryGetValue(mc, out mat)) return mat;

            mat = MakeMaterial(mc);
            mat.AttachToContext(_rc);
            _pbrComponent.Add(mc, mat);
            return mat;
        }


        private ShaderEffect BuildMaterialFromShaderComponent(ShaderComponent shaderComponent)
        {
            ShaderEffect shaderEffect;
            if (!_shaderEffectMap.TryGetValue(shaderComponent, out shaderEffect))
            {
                shaderEffect = MakeShader(shaderComponent);
                shaderEffect.AttachToContext(_rc);
                _shaderEffectMap.Add(shaderComponent, shaderEffect);
            }
            return shaderEffect;
        }

        public Mesh MakeMesh(MeshComponent mc)
        {
            WeightComponent wc = CurrentNode.GetWeights();
            Mesh rm;
            if (wc == null)
            {
                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
            }
            else // Create Mesh with weightdata
            {
                float4[] boneWeights = new float4[wc.WeightMap.Count];
                float4[] boneIndices = new float4[wc.WeightMap.Count];

                // Iterate over the vertices
                for (int iVert = 0; iVert < wc.WeightMap.Count; iVert++)
                {
                    VertexWeightList vwl = wc.WeightMap[iVert];

                    // Security guard. Sometimes a vertex has no weight. This should be fixed in the model. But
                    // let's just not crash here. Instead of having a completely unweighted vertex, bind it to
                    // the root bone (index 0).
                    if (vwl == null)
                        vwl = new VertexWeightList();
                    if (vwl.VertexWeights == null)
                        vwl.VertexWeights =
                            new List<VertexWeight>(new[] { new VertexWeight { JointIndex = 0, Weight = 1.0f } });
                    int nJoints = System.Math.Min(4, vwl.VertexWeights.Count);
                    for (int iJoint = 0; iJoint < nJoints; iJoint++)
                    {
                        // boneWeights[iVert][iJoint] = vwl.VertexWeights[iJoint].Weight;
                        // boneIndices[iVert][iJoint] = vwl.VertexWeights[iJoint].JointIndex;
                        // JSIL cannot handle float4 indexer. Map [0..3] to [x..z] by hand
                        switch (iJoint)
                        {
                            case 0:
                                boneWeights[iVert].x = vwl.VertexWeights[iJoint].Weight;
                                boneIndices[iVert].x = vwl.VertexWeights[iJoint].JointIndex;
                                break;
                            case 1:
                                boneWeights[iVert].y = vwl.VertexWeights[iJoint].Weight;
                                boneIndices[iVert].y = vwl.VertexWeights[iJoint].JointIndex;
                                break;
                            case 2:
                                boneWeights[iVert].z = vwl.VertexWeights[iJoint].Weight;
                                boneIndices[iVert].z = vwl.VertexWeights[iJoint].JointIndex;
                                break;
                            case 3:
                                boneWeights[iVert].w = vwl.VertexWeights[iJoint].Weight;
                                boneIndices[iVert].w = vwl.VertexWeights[iJoint].JointIndex;
                                break;
                        }
                    }
                    boneWeights[iVert].Normalize1();
                }

                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    BoneIndices = boneIndices,
                    BoneWeights = boneWeights,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };


                /*
                // invert weightmap to handle it easier
                float[,] invertedWeightMap = new float[wc.WeightMap[0].JointWeights.Count, wc.Joints.Count];
                for (int i = 0; i < wc.WeightMap.Count; i++)
                {
                    for (int j = 0; j < wc.WeightMap[i].JointWeights.Count; j++)
                    {
                        invertedWeightMap[j, i] = (float) wc.WeightMap[i].JointWeights[j];
                    }
                }

                float4[] boneWeights = new float4[invertedWeightMap.GetLength(0)];
                float4[] boneIndices = new float4[invertedWeightMap.GetLength(0)];

                // Contents of the invertedWeightMap:
                // ----------------------------------
                // Imagine the weight table as seen in 3d modelling programs, i.e. cinema4d;
                // wij are values in the range between 0..1 and specify to which percentage 
                // the vertex (i) is controlled by the bone (j).
                //
                //            bone 0   bone 1   bone 2   bone 3   ....  -> indexed by j
                // vertex 0:   w00      w01      w02      w03
                // vertex 1:   w10      w11      w12      w13
                // vertex 2:   w20      w21      w22      w23
                // vertex 3:   w30      w31      w32      w33
                //   ...
                //  indexed 
                //   by i

                // Iterate over the vertices
                for (int iVert = 0; iVert < invertedWeightMap.GetLength(0); iVert++)
                {
                    boneWeights[iVert] = new float4(0, 0, 0, 0);
                    boneIndices[iVert] = new float4(0, 0, 0, 0);

                    var tempDictionary = new Dictionary<int, float>();

                    // For the given vertex i, see which bones control us
                    for (int j = 0; j < invertedWeightMap.GetLength(1); j++)
                    {
                        if (j < 4)
                        {
                            tempDictionary.Add(j, invertedWeightMap[iVert, j]);
                        }
                        else
                        {
                            float tmpWeight = invertedWeightMap[iVert, j];
                            var keyAndValue = tempDictionary.OrderBy(kvp => kvp.Value).First();
                            if (tmpWeight > keyAndValue.Value)
                            {
                                tempDictionary.Remove(keyAndValue.Key);
                                tempDictionary.Add(j, tmpWeight);
                            }
                        }
                    }

                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].x = keyValuePair.Key;
                        boneWeights[iVert].x = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].y = keyValuePair.Key;
                        boneWeights[iVert].y = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].z = keyValuePair.Key;
                        boneWeights[iVert].z = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[iVert].w = keyValuePair.Key;
                        boneWeights[iVert].w = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }

                    boneWeights[iVert].Normalize1();
                }

                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    BoneIndices = boneIndices,
                    BoneWeights = boneWeights,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
                */
            }

            return rm;
        }

        private ITexture LoadTexture(string path)
        {
            // string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = AssetStorage.Get<ImageData>(path);
            return _rc.CreateTexture(image);
        }
        
        // Creates Shader from given shaderComponent
        private static ShaderEffect MakeShader(ShaderComponent shaderComponent)
        {
            var effectParametersFromShaderComponent = new List<EffectParameterDeclaration>();
            var renderStateSet = new RenderStateSet();

            if (shaderComponent.EffectParameter != null)
            {
                effectParametersFromShaderComponent.AddRange(shaderComponent.EffectParameter.Select(CreateEffectParameterDeclaration));
            }

            // no Effectpasses
            if (shaderComponent.EffectPasses == null)
                throw new InvalidDataException("No EffectPasses in Shader Component! Please specify at least one pass");

            var effectPasses = new EffectPassDeclaration[shaderComponent.EffectPasses.Count];

            for (var i = 0; i < shaderComponent.EffectPasses.Count; i++)
            {
                var newEffectPass = new EffectPassDeclaration();
                var effectPass = shaderComponent.EffectPasses[i];

                if (effectPass.RenderStateContainer != null)
                {
                    renderStateSet = new RenderStateSet();
                    renderStateSet.SetRenderStates(effectPass.RenderStateContainer);
                }


                newEffectPass.VS = effectPass.VS;
                newEffectPass.PS = effectPass.PS;
                newEffectPass.StateSet = renderStateSet;

                effectPasses[i] = newEffectPass;
            }

            return new ShaderEffect(effectPasses, effectParametersFromShaderComponent);
        }

        private static EffectParameterDeclaration CreateEffectParameterDeclaration(TypeContainer effectParameter)
        {
            if (effectParameter.Name == null)
                throw new InvalidDataException("EffectParameterDeclaration: Name is empty!");

            var returnEffectParameterDeclaration = new EffectParameterDeclaration { Name = effectParameter.Name };

            var t = effectParameter.KeyType;

            if (typeof(int).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerInt;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(double).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerDouble;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(float).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerFloat;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(float2).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerFloat2;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(float3).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerFloat3;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(float4).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerFloat4;
                if (effectParameterType != null) returnEffectParameterDeclaration.Value = effectParameterType.Value;
            }
            else if (typeof(bool).IsAssignableFrom(t))
            {
                var effectParameterType = effectParameter as TypeContainerBoolean;
                returnEffectParameterDeclaration.Value = effectParameterType != null && effectParameterType.Value;
            }

            if (returnEffectParameterDeclaration.Value == null)
                throw new InvalidDataException("EffectParameterDeclaration:" + effectParameter.Name + ", value is empty or of unknown type!");

            return returnEffectParameterDeclaration;
        }

        private ShaderEffect ForwardRenderPathMaterial(MaterialComponent mc)
        {
            WeightComponent wc = CurrentNode.GetWeights();


            ForwardShaderCodeBuilder scb = null;

            // If MaterialLightCompoenent is found call the ShaderCodeBuilder with the MaterialLight
            // The ShaderCodeBuilder is intelligent enough to handle all the necessary compilations needed for the VS & PS
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                var lightMat = mc as MaterialLightComponent;
                if (lightMat != null) scb = new ForwardShaderCodeBuilder(lightMat, null, wc);
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                var pbrMaterial = mc as MaterialPBRComponent;
                if (pbrMaterial != null) scb = new ForwardShaderCodeBuilder(pbrMaterial, null, wc);
            }
            else
            {
                scb = new ForwardShaderCodeBuilder(mc, null, wc); // TODO, CurrentNode.GetWeights() != null);
            }

            var effectParameters = AssembleEffectParamers(mc, scb);

            if (scb != null)
            {
                var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration()
                    {
                        VS = scb.VS,
                        //VS = VsBones,
                        PS = scb.PS,
                        StateSet = new RenderStateSet()
                        {
                            ZEnable = true,
                            AlphaBlendEnable = false
                        }
                    }
                },
                    effectParameters
                    );
                return ret;
            }

            throw new Exception("Material could not be evaluated or be built!");
        }
    
        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {
            return ForwardRenderPathMaterial(mc);
        }
    
        private IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, ForwardShaderCodeBuilder scb)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.DiffuseColorName,
                    Value = (object)mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularColorName,
                    Value = (object)mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularShininessName,
                    Value = (object)mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularIntensityName,
                    Value = (object)mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.SpecularTextureName,
                        Value = LoadTexture(mc.Specular.Texture)
                    });
                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.EmissiveColorName,
                    Value = (object)mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            // Any light calculation needed at all?
            if (mc.HasDiffuse || mc.HasSpecular)
            {
                // Light calculation parameters
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightColorName,
                    Value = new float3(1, 1, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightIntensityName,
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightDirectionName,
                    Value = new float3(0, 0, 1)
                });
            }

            SetLightEffectParameters(ref effectParameters);

            return effectParameters;
        }

        private static void SetLightEffectParameters(ref List<EffectParameterDeclaration> effectParameters)
        {
            for (var i = 0; i < AllLightResults.Count; i++)
            {
                if (!AllLightResults[i].Active)
                    continue;

                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].position",
                    Value = AllLightResults[i].PositionWorldSpace
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].intensities",
                    Value = AllLightResults[i].Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].attenuation",
                    Value = AllLightResults[i].Attenuation
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].ambientCoefficient",
                    Value = AllLightResults[i].AmbientCoefficient
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].coneAngle",
                    Value = AllLightResults[i].ConeAngle
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].coneDirection",
                    Value = AllLightResults[i].ConeDirection
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].lightType",
                    Value = (int)AllLightResults[i].Type
                });
            }
        }

        #endregion
    }

    #region LightViserator

    /// <summary>
    /// This struct saves a light found by a Viserator with all parameters
    /// </summary>
    public struct LightResult
    {
        /// <summary>
        /// Represents the light status.
        /// </summary>
        public bool Active;
        /// <summary>
        /// Represents the position of the light.
        /// </summary>
        public float3 Position;
        /// <summary>
        /// Represents the color.
        /// </summary>
        public float3 Color;
        /// <summary>
        /// Represents the attenuation of the light.
        /// </summary>
        public float Attenuation;
        /// <summary>
        /// Represents the ambient coefficient of the light.
        /// </summary>
        public float AmbientCoefficient;
        /// <summary>
        /// Represents the type of the light.
        /// </summary>
        public LightType Type;
        /// <summary>
        /// Represents the spot angle of the light.
        /// </summary>
        public float ConeAngle;
        /// <summary>
        /// Represents the cone direction of the light.
        /// </summary>
        public float3 ConeDirection;
        /// <summary>
        /// The ModelMatrix of the light
        /// </summary>
        public float4x4 ModelMatrix;
        /// <summary>
        /// The light's position in World Coordiantes.
        /// </summary>
        public float3 PositionWorldSpace;
        /// <summary>
        /// The cone's direction in WorldSpace
        /// </summary>
        public float3 ConeDirectionWorldSpace;
        /// <summary>
        /// The lights's position in ModelView Coordinates.
        /// </summary>
        public float3 PositionModelViewSpace;
        /// <summary>
        /// The cone's position in ModelViewCoordinates
        /// </summary>
        public float3 ConeDirectionModelViewSpace;
    }

    public class LightSetupState : VisitorState
    {
        private readonly CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();

        /// <summary>
        /// Gets or sets the top of the Model matrix stack. The Model matrix transforms model coordinates into world coordinates.
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
        /// Initializes a new instance of the <see cref="LightSetupState"/> class.
        /// </summary>
        public LightSetupState()
        {
            RegisterState(_model);
        }
    }

    public class LightSetup : Viserator<LightResult, LightSetupState>
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
        public void OnLight(LightComponent lightComponent)
        {
            var lightResult = new LightResult
            {
                Type = lightComponent.Type,
                Color = lightComponent.Color,
                ConeAngle = lightComponent.ConeAngle,
                ConeDirection = lightComponent.ConeDirection,
                AmbientCoefficient = lightComponent.AmbientCoefficient,
                ModelMatrix = State.Model,
                Position = lightComponent.Position,
                PositionWorldSpace = State.Model * lightComponent.Position,
                ConeDirectionWorldSpace = State.Model * lightComponent.ConeDirection,
                Active = lightComponent.Active,
                Attenuation = lightComponent.Attenuation
            };
            YieldItem(lightResult);
        }
    }
#endregion
}
