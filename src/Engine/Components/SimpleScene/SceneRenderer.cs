using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using System.Linq;
using System.Threading;
using Fusee.Engine;using Fusee.KeyFrameAnimation;
using Fusee.Math;
using Fusee.Serialization;
namespace Fusee.Engine.SimpleScene
{

    /// <summary>
    /// Axis-Aligned Bounding Box Calculator. Use instances of this class to calculate axis-aligned bounding boxes
    /// on scenes, list of scene nodes or individual scene nodes. Calculations always include any child nodes.
    /// </summary>
    public class AABBCalculator : SceneVisitor
    {
        public class AABBState : VisitorState
        {
            private CollapsingStateStack<float4x4> _modelView = new CollapsingStateStack<float4x4>();

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
            AABBf box = _state.ModelView * meshComponent.BoundingBox;
            if (!_boxValid)
            {
                _result = box;
                _boxValid = true;
            }
            else
            {
                _result = AABBf.Union((AABBf) _result, box);
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


    class LightInfo // Todo: TBD...
    {
    }


    /// <summary>
    /// Use a Scene Renderer to traverse a scene hierarchy (made out of scene nodes and components) in order
    /// to have each visited element contribute to the result rendered against a given render context.
    /// </summary>
    public class SceneRenderer : SceneVisitor
    {

        #region Traversal information
        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<SceneNodeContainer, float4x4> _boneMap;
        private Animation _animation;
        private SceneContainer _sc;

        private RenderContext _rc;
        private List<LightInfo> _lights;

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
        public SceneRenderer(SceneContainer sc, string scenePathDirectory)
        {
            _lights = new List<LightInfo>();
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
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
                            Channel<float3> channel = new Channel<float3>(Lerp.Float3Lerp);
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
                _animation.Animate();
        }

        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");
            
            if (rc != _rc)
            {
                _rc = rc;
                _meshMap = new Dictionary<MeshComponent, Mesh>();
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
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
            }
        }
        #endregion

        public void Render(RenderContext rc)
        {
            SetContext(rc);
            Traverse(_sc.Children);
        }

        #region Visitors

        [VisitMethod]
        public void RenderBone(BoneComponent bone)
        {
            SceneNodeContainer boneContainer = CurrentNode;
            float4x4 transform;
            if (!_boneMap.TryGetValue(boneContainer, out transform))
                _boneMap.Add(boneContainer, _rc.Model);
            else
                _boneMap[boneContainer] = _rc.Model;
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
            _rc.Model = _view * _state.Model;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialComponent matComp)
        {
            var effect = LookupMaterial(matComp);
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

            if (null != _state.Effect.GetEffectParam(ShaderCodeBuilder.LightDirectionName))
            {
                RenderWithLights(rm, _state.Effect);
            }
            else
            {
                _state.Effect.RenderMesh(rm);
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


        private void RenderWithLights(Mesh rm, ShaderEffect effect)
        {
            if (_lights.Count > 0)
            {
                foreach (LightInfo li in _lights)
                {
                    // SetupLight(li);
                    effect.RenderMesh(rm);
                }
            }
            else
            {
                // No light present - switch on standard light
                effect.SetEffectParam(ShaderCodeBuilder.LightColorName, new float3(1, 1, 1));
                // float4 lightDirHom = new float4(0, 0, -1, 0);
                float4 lightDirHom = _rc.InvModelView * new float4(0, 0, -1, 0);
                // float4 lightDirHom = _rc.TransModelView * new float4(0, 0, -1, 0);
                float3 lightDir = lightDirHom.xyz;
                lightDir.Normalize();
                effect.SetEffectParam(ShaderCodeBuilder.LightDirectionName, lightDir);
                effect.SetEffectParam(ShaderCodeBuilder.LightIntensityName, (float)1);
                effect.RenderMesh(rm);
            }
        }




        #region RenderContext/Asset Setup
        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            ShaderEffect mat;
            if (!_matMap.TryGetValue(mc, out mat))
            {
                mat = MakeMaterial(mc);
                mat.AttachToContext(_rc);
                _matMap.Add(mc, mat);
            }
            return mat;
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

                for (int i = 0; i < invertedWeightMap.GetLength(0); i++)
                {
                    boneWeights[i] = new float4(0, 0, 0, 0);
                    boneIndices[i] = new float4(0, 0, 0, 0);

                    var tempDictionary = new Dictionary<int, float>();

                    for (int j = 0; j < invertedWeightMap.GetLength(1); j++)
                    {
                        if (j < 4)
                        {
                            tempDictionary.Add(j, invertedWeightMap[i, j]);
                        }
                        else
                        {
                            float tmpWeight = invertedWeightMap[i, j];
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
                        boneIndices[i].x = keyValuePair.Key;
                        boneWeights[i].x = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].y = keyValuePair.Key;
                        boneWeights[i].y = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].z = keyValuePair.Key;
                        boneWeights[i].z = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].w = keyValuePair.Key;
                        boneWeights[i].w = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }

                    boneWeights[i].Normalize1();
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
            }


            return rm;
        }

        private ITexture LoadTexture(string path)
        {
            string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = _rc.LoadImage(texturePath);
            return _rc.CreateTexture(image);
        }

        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {

            WeightComponent wc = CurrentNode.GetWeights();
            ShaderCodeBuilder scb = new ShaderCodeBuilder(mc, null, wc); // TODO, CurrentNode.GetWeights() != null);
            var effectParameters = AssembleEffectParamers(mc, scb);

            ShaderEffect ret = new ShaderEffect(new []
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

        private List<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, ShaderCodeBuilder scb)
        {
            List<EffectParameterDeclaration> effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.DiffuseColorName,
                    Value = (object) mc.Diffuse.Color
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
                    Value = (object) mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularShininessName,
                    Value = (object) mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularIntensityName,
                    Value = (object) mc.Specular.Intensity
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
                    Value = (object) mc.Emissive.Color
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
                    Value = (float) 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightDirectionName,
                    Value = new float3(0, 0, 1)
                });
            }

            return effectParameters;
        }
        #endregion
 
    }


    public class SceneRendererOld
    {
        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<SceneNodeContainer, float4x4> _boneMap;
        private SceneContainer _sc;
        private RenderContext _rc;
        private float4x4 _AABBXForm;
        private List<LightInfo> _lights;
        private Animation _animation;
        private RenderStateSet _stateSet = new RenderStateSet()
        {
            AlphaBlendEnable = false,
            SourceBlend = Blend.One,
            DestinationBlend = Blend.Zero,
            ZEnable = true,
            ZFunc = Compare.Less
        };
        private ShaderEffect _curMat;
        private string _scenePathDirectory;
        ShaderEffect CurMat
        {

            set { _curMat = value; }
            get { return _curMat; }
        }

        public SceneRendererOld(SceneContainer sc, string scenePathDirectory)
        {
            // Todo: scan for lights...
            _lights = new List<LightInfo>();
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
            InitAnimations(_sc);
        }
        public void InitShaders(RenderContext rc)
        {
            if (rc != _rc)
            {
                _rc = rc;
                _meshMap = new Dictionary<MeshComponent, Mesh>();
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
                _curMat = null;
            }
            if (_curMat == null)
            {
                _curMat = MakeMaterial(new MaterialComponent
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
                CurMat.AttachToContext(rc);
            }
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
                        if (typeof (int).IsAssignableFrom(t))
                        {
                            Channel<int> channel = new Channel<int>(Lerp.IntLerp);
                            foreach (AnimationKeyContainerInt key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<int>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof (float).IsAssignableFrom(t))
                        {
                            Channel<float> channel = new Channel<float>(Lerp.FloatLerp);
                            foreach (AnimationKeyContainerFloat key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof (float2).IsAssignableFrom(t))
                        {
                            Channel<float2> channel = new Channel<float2>(Lerp.Float2Lerp);
                            foreach (AnimationKeyContainerFloat2 key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float2>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof (float3).IsAssignableFrom(t))
                        {
                            Channel<float3> channel = new Channel<float3>(Lerp.Float3Lerp);
                            foreach (AnimationKeyContainerFloat3 key in animTrackContainer.KeyFrames)
                            {
                                channel.AddKeyframe(new Keyframe<float3>(key.Time, key.Value));
                            }
                            _animation.AddAnimation(channel, animTrackContainer.SceneComponent,
                                animTrackContainer.Property);
                        }
                        else if (typeof (float4).IsAssignableFrom(t))
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
            if(_animation.ChannelBaseList.Count != 0)
                _animation.Animate();
        }

        public AABBf? GetAABB()
        {
            AABBf? ret = null;
            _AABBXForm = float4x4.Identity;
            foreach (var soc in _sc.Children)
            {
                AABBf? nodeBox = VisitNodeAABB(soc);
                if (nodeBox != null)
                {
                    if (ret == null)
                    {
                        ret = nodeBox;
                    }
                    else
                    {
                        ret = AABBf.Union((AABBf)ret, (AABBf)nodeBox);
                    }
                }
            }
            return ret;
        }

        protected AABBf? VisitNodeAABB(SceneNodeContainer node)
        {
            AABBf? ret = null;
            float4x4 origMV = _AABBXForm;

            // _AABBXForm = _AABBXForm * node.Transform.Matrix();
            // throw new NotImplementedException("correctly handle transform");
            if (node.GetMesh() != null)
            {

                ret = _AABBXForm * node.GetMesh().BoundingBox;
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    AABBf? nodeBox = VisitNodeAABB(child);
                    if (nodeBox != null)
                    {
                        if (ret == null)
                        {
                            ret = nodeBox;
                        }
                        else
                        {
                            ret = AABBf.Union((AABBf)ret, (AABBf)nodeBox);
                        }
                    }
                }
            }
            _AABBXForm = origMV;
            return ret;
        }

        public void Render(RenderContext rc)
        {
            InitShaders(rc);
            foreach (var sbc in _sc.Children)
            {
                VisitNodeRender(sbc);
            }
        }

        protected void VisitNodeRender(SceneNodeContainer node)
        {
            float4x4 origMV = _rc.Model;
            ShaderEffect origMat = CurMat;

            //throw new NotImplementedException("correctly handle transform");
            _rc.Model = _rc.Model * node.GetTransform().Matrix();

            if (node.GetComponent<BoneComponent>() != null)
            {
                SceneNodeContainer bone = node as SceneNodeContainer;
                float4x4 transform;
                if (!_boneMap.TryGetValue(bone, out transform))
                    _boneMap.Add(bone, _rc.Model);
                else
                    _boneMap[bone] = _rc.Model;
            }

            SceneNodeContainer soc = node as SceneNodeContainer;
            if (soc != null)
            {
                if (soc.GetMaterial() != null)
                {
                    var mat = LookupMaterial(soc.GetMaterial());
                    CurMat = mat;
                }
                ////new
                if (soc.GetWeights() != null)
                {
                    float4x4[] boneArray = new float4x4[soc.GetWeights().Joints.Count()];
                    for (int i = 0; i < soc.GetWeights().Joints.Count(); i++)
                    {
                        float4x4 tmp = soc.GetWeights().BindingMatrices[i];
                        boneArray[i] = _boneMap[soc.GetWeights().Joints[i]]*tmp;
                    }
                    _rc.Bones = boneArray;
                }

                if (node.GetMaterial() != null)
                {
                    var mat = LookupMaterial(node.GetMaterial());
                    CurMat = mat;
                }
                if (node.GetMesh() != null)
                {
                    Mesh rm;
                    if (!_meshMap.TryGetValue(node.GetMesh(), out rm))
                    {
                        rm = MakeMesh(node);
                        _meshMap.Add(node.GetMesh(), rm);
                    }
                    if (null != CurMat.GetEffectParam(ShaderCodeBuilder.LightDirectionName))
                    {
                        RenderWithLights(rm, CurMat);
                    }
                    else
                    {
                        CurMat.RenderMesh(rm);
                    }
                }
                if (node.Children != null)
                {
                    foreach (var child in node.Children)
                    {
                        VisitNodeRender(child);
                    }
                }
                _rc.Model = origMV;
                CurMat = origMat;
            }
        }

        private void RenderWithLights(Mesh rm, ShaderEffect CurMat)
        {
            if (_lights.Count > 0)
            {
                foreach (LightInfo li in _lights)
                {
                    // SetupLight(li);
                    CurMat.RenderMesh(rm);
                }
            }
            else
            {
                // No light present - switch on standard light
                CurMat.SetEffectParam(ShaderCodeBuilder.LightColorName, new float3(1, 1, 1));
                // float4 lightDirHom = new float4(0, 0, -1, 0);
                float4 lightDirHom = _rc.InvModelView * new float4(0, 0, -1, 0);
                // float4 lightDirHom = _rc.TransModelView * new float4(0, 0, -1, 0);
                float3 lightDir = lightDirHom.xyz;
                lightDir.Normalize();
                CurMat.SetEffectParam(ShaderCodeBuilder.LightDirectionName, lightDir);
                CurMat.SetEffectParam(ShaderCodeBuilder.LightIntensityName, (float)1);
                CurMat.RenderMesh(rm);
            }
        }
        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            ShaderEffect mat;
            if (!_matMap.TryGetValue(mc, out mat))
            {
                mat = MakeMaterial(mc);
                mat.AttachToContext(_rc);
                _matMap.Add(mc, mat);
            }
            return mat;
        }
        public static Mesh MakeMesh(SceneNodeContainer soc)
        {
            MeshComponent mc = soc.GetMesh();
            WeightComponent wc = soc.GetWeights();
            Mesh rm;

            if (wc == null)
                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
            else // Create Mesh with weightdata
            {
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

                for (int i = 0; i < invertedWeightMap.GetLength(0); i++)
                {
                    boneWeights[i] = new float4(0,0,0,0);
                    boneIndices[i] = new float4(0,0,0,0);

                    var tempDictionary = new Dictionary<int, float>();

                    for (int j = 0; j < invertedWeightMap.GetLength(1); j++)
                    {
                        if (j < 4)
                        {
                            tempDictionary.Add(j, invertedWeightMap[i, j]);
                        }
                        else
                        {
                            float tmpWeight = invertedWeightMap[i, j];
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
                        boneIndices[i].x = keyValuePair.Key;
                        boneWeights[i].x = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].y = keyValuePair.Key;
                        boneWeights[i].y = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].z = keyValuePair.Key;
                        boneWeights[i].z = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].w = keyValuePair.Key;
                        boneWeights[i].w = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }

                    boneWeights[i].Normalize1();
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
            }


            return rm;
        }
        private ITexture LoadTexture(string path)
        {
            string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = _rc.LoadImage(texturePath);
            return _rc.CreateTexture(image);
        }
        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {
            ShaderCodeBuilder scb = new ShaderCodeBuilder(mc, null);
            var effectParameters = AssembleEffectParamers(mc, scb);


            ShaderEffect ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration()
                    {
                        VS = scb.VS,
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

        private List<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, ShaderCodeBuilder scb)
        {
            List<EffectParameterDeclaration> effectParameters = new List<EffectParameterDeclaration>();
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
            return effectParameters;
        }

    }
}
