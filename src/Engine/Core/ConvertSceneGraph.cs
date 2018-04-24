using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use ConVSceneToHighLevel to create new high level graph from a low level graph (made out of scene nodes and components) in order
    /// to have each visited element converted and/or split into its high level, render-ready components.
    /// </summary>
    public class ConvertSceneGraph : SceneVisitor
    {
        private SceneContainer _convertedScene;
        private Stack<SceneNodeContainer> _predecessors;
        private SceneNodeContainer _currentNode;

        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<MaterialLightComponent, ShaderEffect> _lightMatMap;
        private Dictionary<MaterialPBRComponent, ShaderEffect> _pbrComponent;


        protected override void PopState()
        {
            _predecessors.Pop();
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph by converting and/or spliting its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public SceneContainer Convert(SceneContainer sc)
        {
            _predecessors = new Stack<SceneNodeContainer>();
            _convertedScene = new SceneContainer();

            _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
            _lightMatMap = new Dictionary<MaterialLightComponent, ShaderEffect>();
            _pbrComponent = new Dictionary<MaterialPBRComponent, ShaderEffect>();

            Traverse(sc.Children);
            return _convertedScene;
        }

        #region Visitors

        [VisitMethod]
        public void ConvScneNodeContainer(SceneNodeContainer snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                    parent.Children = new List<SceneNodeContainer>();

                _currentNode = new SceneNodeContainer { Name = snc.Name + "_copy" };
                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new SceneNodeContainer { Name = CurrentNode.Name + "_copy" });
                _currentNode = _predecessors.Peek();
                _convertedScene.Children = new List<SceneNodeContainer> { _currentNode };
            }
        }

        [VisitMethod]
        public void ConvTransform(TransformComponent transform)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(transform);
        }

        [VisitMethod]
        public void ConvMaterial(MaterialComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(new ShaderEffectComponent(effect));
        }

        [VisitMethod]
        public void ConvMaterial(MaterialLightComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(new ShaderEffectComponent(effect));
        }

        [VisitMethod]
        public void ConvMaterial(MaterialPBRComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(new ShaderEffectComponent(effect));
        }

        [VisitMethod]
        public void ConvShader(ShaderComponent shaderComponent)
        {

        }


        [VisitMethod]
        public void ConvShaderEffect(ShaderEffectComponent shaderComponent)
        {

        }

        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(mesh);
        }

        [VisitMethod]
        public void ConvLight(LightComponent lightComponent)
        {

        }

        [VisitMethod]
        public void ConvBone(BoneComponent bone)
        {

        }

        [VisitMethod]
        public void ConVWeight(WeightComponent weight)
        {

        }

        [VisitMethod]
        public void ConCanvasTransform(CanvasTransformComponent ctc)
        {

        }

        [VisitMethod]
        public void ConvRectTransform(RectTransformComponent rtc)
        {

        }
        #endregion

        #region MakeMatrial

        private ShaderEffect MakeMaterial(MaterialComponent mc) //TODO: replace with (currently not existing) helper method in ShaderCodeBuilder (Mat --> ShaderEffect)
        {
            var wc = _currentNode.GetWeights();

            ShaderCodeBuilder scb = null;

            // If MaterialLightComponent is found call the LegacyShaderCodeBuilder with the MaterialLight
            // The LegacyShaderCodeBuilder is intelligent enough to handle all the necessary compilations needed for the VS & PS
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                if (mc is MaterialLightComponent lightMat) scb = new ShaderCodeBuilder(lightMat, null, wc);
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent pbrMaterial) scb = new ShaderCodeBuilder(pbrMaterial, null, LightingCalculationMethod.ADVANCED, wc);
            }
            else
            {
                scb = new ShaderCodeBuilder(mc, null, wc); // TODO, CurrentNode.GetWeights() != null);
            }

            var effectParameters = AssembleEffectParamers(mc);

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

        private IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.DiffuseColorName,
                    Value = mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.DiffuseTextureName,
                        //Value = LoadTexture(mc.Diffuse.Texture) TODO: uncomment if Texture issue is resolved
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularColorName,
                    Value = mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularShininessName,
                    Value = mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.SpecularIntensityName,
                    Value = mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.SpecularTextureName,
                        //Value = LoadTexture(mc.Specular.Texture) TODO: uncomment if Texture issue is resolved
                    });
                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.EmissiveColorName,
                    Value = mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = ShaderCodeBuilder.EmissiveTextureName,
                        //Value = LoadTexture(mc.Emissive.Texture) TODO: uncomment if Texture issue is resolved
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.BumpTextureName,
                    //Value = LoadTexture(mc.Bump.Texture)
                });
            }

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].position",
                Value = new float3(0,0,-1)
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].intensities",
                Value = float3.Zero
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].attenuation",
                Value = 0
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].ambientCoefficient",
                Value = 0
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].coneAngle",
                Value = 0
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].coneDirection",
                Value = float3.Zero
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "allLights[" + 0 + "].lightType",
                Value = 1
            });

            return effectParameters;
        }

        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            if (_matMap.TryGetValue(mc, out var mat)) return mat;

            mat = MakeMaterial(mc);
            //_rc.SetShaderEffect(mat);
            _matMap.Add(mc, mat);
            return mat;
        }
        private ShaderEffect LookupMaterial(MaterialLightComponent mc)
        {
            if (_lightMatMap.TryGetValue(mc, out var mat)) return mat;

            mat = MakeMaterial(mc);
            //_rc.SetShaderEffect(mat);
            _lightMatMap.Add(mc, mat);
            return mat;
        }

        private ShaderEffect LookupMaterial(MaterialPBRComponent mc)
        {
            if (_pbrComponent.TryGetValue(mc, out var mat)) return mat;

            mat = MakeMaterial(mc);
            //_rc.SetShaderEffect(mat);
            _pbrComponent.Add(mc, mat);
            return mat;
        }

        /*private ITexture LoadTexture(string path)
        {
            // string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = AssetStorage.Get<ImageData>(path);
            return _rc.CreateTexture(image); // TODO: Texture is not a Component, ShaderEffect references TextureObject. TextureObject needs to know the RenderContext?
        }*/

        #endregion
    }
}
