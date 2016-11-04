using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                _result = AABBf.Union((AABBf)_result, box);
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
        public static LightningCalculationMethod LightningCalculationMethod = LightningCalculationMethod.SIMPLE;
        // All lights
        public static IList<LightResult> AllLightResults = new List<LightResult>();


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

        public static bool RenderDeferred = false;


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

        public SceneRenderer(SceneContainer sc, LightningCalculationMethod lCalcMethod)
             : this(sc)
        {
            LightningCalculationMethod = lCalcMethod;
        }

        public SceneRenderer(SceneContainer sc /*, string scenePathDirectory*/)
        {
            // accumulate all lights and...
            _lightComponents = sc.Children.Viserate<LightSetup, LightResult>().ToList();
            // ...set them
            AllLightResults = _lightComponents;


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

        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");

            if (rc != _rc)
            {
                _rc = rc;
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

            RenderWithLights(rm, _state.Effect);
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
                lightConeDirectionFloat4 = _rc.ModelView* lightConeDirectionFloat4;
                lightConeDirectionFloat4.Normalize();
                light.ConeDirection = new float3(lightConeDirectionFloat4.x, lightConeDirectionFloat4.y, lightConeDirectionFloat4.z);   
                
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

        private void RenderWithLights(Mesh rm, ShaderEffect effect)
        {
            if (_lightComponents.Count > 0)
            {
                for (var i = 0; i < _lightComponents.Count; i++)
                {
                    SetupLight(i, _lightComponents[i], effect);
                    effect.RenderMesh(rm);
                }
            }
            else
            {
                // No light present - switch on standard light
                effect.SetEffectParam(ShaderCodeBuilder.LightColorName, new float3(1, 1, 1));
                // float4 lightDirHom = new float4(0, 0, -1, 0);
                var lightDirHom = _rc.InvModelView * new float4(0, 0, -1, 0);
                // float4 lightDirHom = _rc.TransModelView * new float4(0, 0, -1, 0);
                var lightDir = lightDirHom.xyz;
                lightDir.Normalize();
                effect.SetEffectParam(ShaderCodeBuilder.LightDirectionName, lightDir);
                effect.SetEffectParam(ShaderCodeBuilder.LightIntensityName, (float)1);
                effect.RenderMesh(rm);
            }
        }

        private void SetupLight(int position, LightResult light, ShaderEffect effect)
        {
            if (!light.Active) return;
            /*
            var thisLight = AllLightResults[position];
            thisLight.PositionWorldSpace = _rc.InvView *  thisLight.PositionWorldSpace;
            AllLightResults[position] = thisLight; */

           // light.ModelMatrix.Invert();
            effect.SetEffectParam($"allLights[{position}].position", light.PositionWorldSpace);
            effect.SetEffectParam($"allLights[{position}].intensities", light.Color);
            effect.SetEffectParam($"allLights[{position}].attenuation", light.Attenuation);
            effect.SetEffectParam($"allLights[{position}].ambientCoefficient", light.AmbientCoefficient);
            effect.SetEffectParam($"allLights[{position}].coneAngle", light.ConeAngle);
            effect.SetEffectParam($"allLights[{position}].coneDirection", light.ConeDirection);
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
        private ShaderEffect MakeShader(ShaderComponent shaderComponent)
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

        private EffectParameterDeclaration CreateEffectParameterDeclaration(TypeContainer effectParameter)
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

            ShaderEffect ret = new ShaderEffect(new[]
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

        private ShaderEffect DeferredRenderPathMaterial(MaterialComponent mc)
        {
            WeightComponent wc = CurrentNode.GetWeights();


            DeferredShaderCodeBuilder scb = null;

            // If MaterialLightCompoenent is found call the ShaderCodeBuilder with the MaterialLight
            // The ShaderCodeBuilder is intelligent enough to handle all the necessary compilations needed for the VS & PS
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                var lightMat = mc as MaterialLightComponent;
                if (lightMat != null) scb = new DeferredShaderCodeBuilder(lightMat, null, wc);
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                var pbrMaterial = mc as MaterialPBRComponent;
                if (pbrMaterial != null) scb = new DeferredShaderCodeBuilder(pbrMaterial, null, wc);
            }
            else
            {
                scb = new DeferredShaderCodeBuilder(mc, null, wc); // TODO, CurrentNode.GetWeights() != null);
            }

            var effectParameters = AssembleEffectParamers(mc, scb);

            ShaderEffect ret = new ShaderEffect(new[]
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
    
        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {
            if (_rc.GetHardwareCapabilities(HardwareCapability.DEFFERED_POSSIBLE) == 1U && RenderDeferred)
                return DeferredRenderPathMaterial(mc);
            return ForwardRenderPathMaterial(mc);
        }

        // TODO: Set TextureParams from DeferredPath here, etc.
        // TODO: Make polymorph
        private IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, DeferredShaderCodeBuilder scb)
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

            // More than one light in scene, no legacy mode
            if (AllLightResults.Count > 0)
            {
                SetLightEffectParameters(ref effectParameters);
            }
            // No LightComponent in Scene -> switch to legacy mode!
            else
            {
                Diagnostics.Log("legacy no lightcomponent");
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].position",
                    Value = new float3(0f, 0f, 1f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].intensities",
                    Value = new float3(0.3f, 0.3f, 0.3f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].attenuation",
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].ambientCoefficient",
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].coneAngle",
                    Value = (float)365
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].coneDirection",
                    Value = new float3(0, 0, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].lightType",
                    Value = 1
                });
            }

            return effectParameters;
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

            // More than one light in scene, no legacy mode
            if (AllLightResults.Count > 0)
            {
                SetLightEffectParameters(ref effectParameters);
            }
            // No LightComponent in Scene -> switch to legacy mode!
            else
            {
                Diagnostics.Log("legacy no lightcomponent");
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].position",
                    Value = new float3(0, 0, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].intensities",
                    Value = new float3(0.6f, 0.6f, 0.6f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].attenuation",
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].ambientCoefficient",
                    Value = (float)0.1f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].coneAngle",
                    Value = (float)365
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].coneDirection",
                    Value = new float3(0, 0, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[0].lightType",
                    Value = 1
                });
            }

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
    /// This class saves a light found by a Viserator with all parameters
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
        /// The light's Position in World Coordiantes.
        /// </summary>
        public float3 PositionWorldSpace;
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
                ConeDirection = State.Model * lightComponent.ConeDirection,
                AmbientCoefficient = lightComponent.AmbientCoefficient,
                ModelMatrix = State.Model,
                Position = lightComponent.Position,
                PositionWorldSpace = State.Model * lightComponent.Position,
                Active = lightComponent.Active,
                Attenuation = lightComponent.Attenuation
            };
            YieldItem(lightResult);
        }

    }
#endregion
}
