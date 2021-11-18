using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Serialization.V1;
using Fusee.Xene;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use <see cref="ConvertFrom(FusFile, string)"/> and <see cref="ConvertTo(SceneContainer)"/>, to create new high/low level graph from a low/high level graph (made out of scene nodes and components)
    /// in order to have each visited element converted and/or split into its high/low level, render-ready/serialization-ready components.
    /// </summary>
    public static class FusSceneConverter
    {
        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph <see cref="Scene"/> by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="fus"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SceneContainer ConvertFrom(FusFile fus, string id = null)
        {
            return ConvertFromAsync(fus, id).Result;
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph <see cref="Scene"/> by converting and/or splitting its components into the high level equivalents in an async manner.
        /// </summary>
        /// <param name="fus">The FusFile to convert.</param>
        /// <param name="id">Path to this scene used as an addition for asset retriving inside the correct folder</param>
        public static async Task<SceneContainer> ConvertFromAsync(FusFile fus, string id = null)
        {
            if (fus == null)
            {
                Diagnostics.Error("Could not read content of scene, file is null!");
                return new SceneContainer();
            }

            // try to cast, if this fails the content is empty or null
            if (!(fus.Contents is FusScene))
            {
                Diagnostics.Error($"Could not read content of scene from {fus.Header.CreationDate} created by {fus.Header.CreatedBy} with {fus.Header.Generator}");
                return new SceneContainer();
            }

            // if id is given set fields in FusFile
            if (!string.IsNullOrEmpty(id))
            {
                fus.Header.LoadFilename = Path.GetFileName(id);
                fus.Header.LoadPath = Path.GetDirectoryName(id);
            }

            var instance = new FusFileToSceneConvertV1();
            var payload = (FusScene)fus.Contents;


            // if path is set, update path dependent payload
            if (!string.IsNullOrEmpty(fus.Header.LoadPath))
            {
                for (int i = 0; i < payload.ComponentList.Count; i++)
                {
                    if (payload.ComponentList[i] is FusMaterialBase matcomp)
                    {
                        if (matcomp.HasAlbedoChannel)
                        {
                            matcomp.Albedo.Texture = Path.Combine(fus.Header.LoadPath, matcomp.Albedo.Texture);
                        }

                        if (matcomp.HasEmissiveChannel)
                        {
                            matcomp.Emissive.Texture = Path.Combine(fus.Header.LoadPath, matcomp.Emissive.Texture);
                        }

                        if (matcomp.HasNormalMapChannel)
                        {
                            matcomp.NormalMap.Texture = Path.Combine(fus.Header.LoadPath, matcomp.NormalMap.Texture);
                        }
                    }

                    if (payload.ComponentList[i] is FusMaterialStandard matcompstd)
                    {
                        if (matcompstd.HasSpecularChannel)
                        {
                            matcompstd.Specular.Texture = Path.Combine(fus.Header.LoadPath, matcompstd.Specular.Texture);
                        }
                    }
                }
            }

            var converted = await instance.Convert(payload);


            converted.Header = new SceneHeader
            {
                CreatedBy = fus.Header.CreatedBy,
                CreationDate = fus.Header.CreationDate,
                Generator = fus.Header.Generator
            };



            return converted;
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high low level graph <see cref="FusFile"/> by converting and/or splitting its components into the low level equivalents.
        /// </summary>
        /// <param name="sc">The Scene to convert.</param>
        public static FusFile ConvertTo(SceneContainer sc)
        {
            if (sc == null)
            {
                Diagnostics.Error("Could not read content of scene, file is null!");
                return new FusFile();
            }

            var instance = new SceneToFusFileConvertV1();
            var converted = instance.Convert(sc);

            converted.Header = new FusHeader
            {
                CreatedBy = sc.Header.CreatedBy,
                CreationDate = sc.Header.CreationDate,
                Generator = sc.Header.Generator,
                FileVersion = 1
            };

            return converted;
        }
    }

    internal class FusFileToSceneConvertV1 : Visitor<FusNode, FusComponent>
    {
        private FusScene _fusScene;
        private readonly SceneContainer _convertedScene;
        private readonly Stack<SceneNode> _predecessors;
        private SceneNode _currentNode;

        private readonly Dictionary<FusMaterialBase, Effect> _matMap;
        private readonly Dictionary<FusMesh, Mesh> _meshMap;
        private readonly ConcurrentDictionary<string, Texture> _texMap;
        private readonly Stack<SceneNode> _boneContainers;

        private readonly Dictionary<FusMaterialBase, List<SceneNode>> _allEffects;

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing. Override this method to perform pop on any self-defined state.
        /// </summary>
        protected override void PopState()
        {
            _predecessors.Pop();
        }

        internal FusFileToSceneConvertV1()
        {
            _predecessors = new Stack<SceneNode>();
            _convertedScene = new SceneContainer();

            _matMap = new Dictionary<FusMaterialBase, Effect>();
            _meshMap = new Dictionary<FusMesh, Mesh>();
            _texMap = new ConcurrentDictionary<string, Texture>();
            _boneContainers = new Stack<SceneNode>();

            _allEffects = new Dictionary<FusMaterialBase, List<SceneNode>>();
        }

        internal async Task<SceneContainer> Convert(FusScene sc)
        {
            _fusScene = sc;
            Traverse(sc.Children);

            // During scene traversal we collect all effects but do not create them, yet
            // within this loop the look up and texture retrival is being performed in an asynchronous way

            foreach (var mat in _allEffects.Keys)
            {
                Effect effect = null;

                if (mat is FusMaterialStandard m)
                    effect = await LookupMaterial(m);

                if (mat is FusMaterialGlossyBRDF g)
                    effect = await LookupMaterial(g);

                if (mat is FusMaterialDiffuseBRDF d)
                    effect = await LookupMaterial(d);

                if (mat is FusMaterialBRDF b)
                    effect = await LookupMaterial(b);

                if (effect == null)
                {
                    Diagnostics.Warn($"Material skipped.");
                    continue;
                }

                foreach (var node in _allEffects[mat])
                {
                    if (node.GetComponents<Effect>().Count() > 0)
                    {
                        Diagnostics.Warn($"Node {node} already contains an effect, multiple effects can't be rendered or be used, yet!");
                    }

                    // always insert after transform but before any other component to not break
                    // code which relies upon this oder
                    var hasTransform = node.GetComponent<Transform>() != null;
                    node.Components.Insert(hasTransform ? 1 : 0, effect);

                    // calculate tangents and bitangets if normal mapping is enabled for this material/effect
                    var mesh = node.GetComponent<Mesh>();
                    if (mesh != null)
                    {
                        mesh.Tangents = mesh.CalculateTangents();
                        mesh.BiTangents = mesh.CalculateBiTangents();
                    }
                }
            }

            return _convertedScene;
        }

        #region Visitors

        /// <summary>
        /// Converts the FusNode container.
        /// </summary>
        /// <param name="snc"></param>
        [VisitMethod]
        public void ConvFusNode(FusNode snc)
        {
            snc.Scene = _fusScene;

            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                {
                    parent.Children = new ChildList();
                }

                _currentNode = new SceneNode { Name = snc.Name };

                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new SceneNode { Name = CurrentNode.Name });

                _currentNode = _predecessors.Peek();

                if (_convertedScene.Children != null)
                {
                    _convertedScene.Children.Add(_currentNode);
                }
                else
                {
                    _convertedScene.Children = new List<SceneNode> { _currentNode };
                }
            }
        }

        ///<summary>
        /// Converts the animation component.
        ///</summary>
        [VisitMethod]
        public void ConvAnimation(FusAnimation a)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            // TODO: Test animation and refactor animation method from scene renderer to this converter
        }


        ///<summary>
        /// Converts the XForm component.
        ///</summary>
        [VisitMethod]
        public void ConvXForm(FusXForm xf)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.Components.Add(new XForm
            {
                Name = xf.Name
            });
        }

        ///<summary>
        /// Converts the XFormText component.
        ///</summary>
        [VisitMethod]
        public void ConvXFormText(FusXFormText xft)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.AddComponent(new XFormText
            {
                Height = xft.Height,
                Width = xft.Width,
                Name = xft.Name,
                HorizontalAlignment =
                xft.HorizontalAlignment == FusHorizontalTextAlignment.Center
                ? HorizontalTextAlignment.Center
                : xft.HorizontalAlignment == FusHorizontalTextAlignment.Left
                ? HorizontalTextAlignment.Left
                : HorizontalTextAlignment.Right,
                VerticalAlignment = xft.VerticalAlignment == FusVerticalTextAlignment.Center
                ? VerticalTextAlignment.Center
                : xft.VerticalAlignment == FusVerticalTextAlignment.Bottom
                ? VerticalTextAlignment.Bottom
                : VerticalTextAlignment.Top
            });
        }

        ///<summary>
        /// Converts the CanvasTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvCanvasTransform(FusCanvasTransform ct)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.AddComponent(new CanvasTransform(ct.CanvasRenderMode == Serialization.V1.CanvasRenderMode.Screen
                ? Scene.CanvasRenderMode.Screen
                : Scene.CanvasRenderMode.World)
            {
                Name = ct.Name,
                Scale = ct.Scale,
                ScreenSpaceSize = ct.ScreenSpaceSize,
                Size = ct.Size
            });
        }

        ///<summary>
        /// Converts the RectTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvRectTransform(FusRectTransform rt)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.AddComponent(new RectTransform
            {
                Name = rt.Name,
                Anchors = rt.Anchors,
                Offsets = rt.Offsets
            });
        }


        ///<summary>
        ///Converts the transform component.
        ///</summary>
        [VisitMethod]
        public void ConvTransform(FusTransform t)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.Components.Add(new Transform
            {
                Translation = t.Translation,
                Name = t.Name,
                Rotation = t.Rotation,
                Scale = t.Scale
            });
        }

        /// <summary>
        /// Converts the material.
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(FusMaterialStandard matComp)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            if (!_allEffects.TryGetValue(matComp, out var sfx))
            {
                sfx = new List<SceneNode>
                {
                    _currentNode
                };

                _allEffects[matComp] = sfx;
            }
            else
            {
                sfx.Add(_currentNode);
            }
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(FusMaterialBRDF matComp)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            if (!_allEffects.TryGetValue(matComp, out var sfx))
            {
                sfx = new List<SceneNode>
                {
                    _currentNode
                };

                _allEffects[matComp] = sfx;
            }
            else
            {
                sfx.Add(_currentNode);
            }
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(FusMaterialDiffuseBRDF matComp)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            if (!_allEffects.TryGetValue(matComp, out var sfx))
            {
                sfx = new List<SceneNode>
                {
                    _currentNode
                };

                _allEffects[matComp] = sfx;
            }
            else
            {
                sfx.Add(_currentNode);
            }
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(FusMaterialGlossyBRDF matComp)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            if (!_allEffects.TryGetValue(matComp, out var sfx))
            {
                sfx = new List<SceneNode>
                {
                    _currentNode
                };

                _allEffects[matComp] = sfx;
            }
            else
            {
                sfx.Add(_currentNode);
            }
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCamComp(FusCamera cc)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            var cam = new Camera(cc.ProjectionMethod == Serialization.V1.ProjectionMethod.Orthographic ? Fusee.Engine.Core.Scene.ProjectionMethod.Orthographic : Fusee.Engine.Core.Scene.ProjectionMethod.Perspective,
                cc.ClippingPlanes.x, cc.ClippingPlanes.y, cc.Fov)
            {
                Active = cc.Active,
                BackgroundColor = cc.BackgroundColor,
                ClearColor = cc.ClearColor,
                ClearDepth = cc.ClearDepth,
                Layer = cc.Layer,
                Name = cc.Name
            };

            _currentNode.Components.Add(cam);
        }

        /// <summary>
        /// Converts the mesh.
        /// </summary>
        /// <param name="m">The mesh to convert.</param>
        [VisitMethod]
        public void ConvMesh(FusMesh m)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            if (_meshMap.TryGetValue(m, out var mesh))
            {
                _currentNode.Components.Add(mesh);
                return;
            }

            // convert mesh
            mesh = new Mesh
            {
                MeshType = m.MeshType,
                Active = true,
                BiTangents = m.BiTangents,
                BoneIndices = m.BoneIndices,
                BoundingBox = m.BoundingBox,
                BoneWeights = m.BoneWeights,
                Colors = m.Colors,
                Name = m.Name,
                Normals = m.Normals,
                Tangents = m.Tangents,
                Triangles = m.Triangles,
                UVs = m.UVs,
                Vertices = m.Vertices
            };

            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.Components.Add(mesh);

            _meshMap.Add(m, mesh);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="l"></param>
        [VisitMethod]
        public void ConvLight(FusLight l)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.Components.Add(new Light
            {
                Name = l.Name,
                Active = l.Active,
                Bias = l.Bias,
                Color = l.Color,
                InnerConeAngle = l.InnerConeAngle,
                IsCastingShadows = l.IsCastingShadows,
                MaxDistance = l.MaxDistance,
                OuterConeAngle = l.OuterConeAngle,
                Strength = l.Strength,
                Type = l.Type
            });
        }

        /// <summary>
        /// Adds the bone component.
        /// </summary>
        /// <param name="bone"></param>
        [VisitMethod]
        public void ConvBone(FusBone bone)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.Components.Add(new Bone
            {
                Name = bone.Name
            });

            // Collect all bones, later, when a WeightComponent is found, we can set all Joints
            _boneContainers.Push(_currentNode);
        }

        /// <summary>
        /// Converts the weight component.
        /// </summary>
        /// <param name="w"></param>
        [VisitMethod]
        public void ConVWeight(FusWeight w)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            var weight = new Weight
            {
                WeightMap = w.WeightMap.Select(wm =>
                {

                    var currentWeightList = new Scene.VertexWeightList
                    {
                        VertexWeights = new List<Scene.VertexWeight>()
                    };

                    var currentVertexWeights = wm.VertexWeights.Select(ww => new Scene.VertexWeight { JointIndex = ww.JointIndex, Weight = ww.Weight }).ToList();

                    currentWeightList.VertexWeights.AddRange(currentVertexWeights);
                    return currentWeightList;

                }).ToList(),
                BindingMatrices = w.BindingMatrices,
                Joints = new List<SceneNode>(),
                Name = w.Name
            };

            // check if we have bones
            if (_boneContainers.Count >= 1)
            {
                if (weight.Joints == null) // initialize joint container
                {
                    weight.Joints = new List<SceneNode>();
                }

                // set all bones found until this WeightComponent
                while (_boneContainers.Count != 0)
                {
                    weight.Joints.Add(_boneContainers.Pop());
                }
            }

            _currentNode.Components.Add(weight);
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="cc"></param>
        [VisitMethod]
        public void ConvOctant(FusOctantD cc)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.AddComponent(
            new OctantD(cc.Center, cc.Size)
            {
                IsLeaf = cc.IsLeaf,
                Level = cc.Level,
                PosInParent = cc.PosInParent,

                Guid = cc.Guid,
                Name = cc.Name,
                NumberOfPointsInNode = cc.NumberOfPointsInNode,
                PosInHierarchyTex = cc.PosInHierarchyTex,
                VisibleChildIndices = cc.VisibleChildIndices,
                WasLoaded = cc.WasLoaded
            });
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="cc"></param>
        [VisitMethod]
        public void ConvOctant(FusOctantF cc)
        {
            if (_currentNode.Components == null)
            {
                _currentNode.Components = new List<SceneComponent>();
            }

            _currentNode.AddComponent(
            new OctantF(cc.Center, cc.Size)
            {
                IsLeaf = cc.IsLeaf,
                Level = cc.Level,
                PosInParent = cc.PosInParent,

                Guid = cc.Guid,
                Name = cc.Name,
                NumberOfPointsInNode = cc.NumberOfPointsInNode,
                PosInHierarchyTex = cc.PosInHierarchyTex,
                VisibleChildIndices = cc.VisibleChildIndices,
                WasLoaded = cc.WasLoaded
            });
        }

        #endregion

        #region Make Effect

        private async Task<Effect> LookupMaterial(FusMaterialStandard m)
        {
            if (_matMap.TryGetValue(m, out var sfx))
            {
                return sfx;
            }

            var lightingSetup = m.HasSpecularChannel ? LightingSetupFlags.DiffuseSpecular : LightingSetupFlags.DiffuseOnly;
            return await GetEffectForMat(m, lightingSetup, m.HasSpecularChannel ? m.Specular.Shininess : 0f, m.HasSpecularChannel ? m.Specular.Strength : 0f, 0f);
        }

        private async Task<Effect> LookupMaterial(FusMaterialDiffuseBRDF m)
        {
            if (_matMap.TryGetValue(m, out var sfx))
            {
                return sfx;
            }

            return await GetEffectForMat(m, LightingSetupFlags.DiffuseOnly, 0f, 0f, m.Roughness);
        }

        private async Task<Effect> LookupMaterial(FusMaterialGlossyBRDF m)
        {
            if (_matMap.TryGetValue(m, out var sfx))
            {
                return sfx;
            }

            return await GetEffectForMat(m, LightingSetupFlags.Glossy, 0f, 0f, m.Roughness);
        }

        private async Task<Effect> LookupMaterial(FusMaterialBRDF m)
        {
            if (_matMap.TryGetValue(m, out var sfx))
            {
                return sfx;
            }

            var lightingSetup = LightingSetupFlags.BRDF;
            if (m.Albedo.Texture != null && m.Albedo.Texture != "")
            {
                lightingSetup |= LightingSetupFlags.AlbedoTex;
            }

            if (m.NormalMap?.Texture != null && m.NormalMap.Texture != "")
            {
                lightingSetup |= LightingSetupFlags.NormalMap;
            }

            var emissive = m.Emissive?.Color == null ? new float4() : m.Emissive.Color;

            //TODO: Texture Tiles instead of float2.One - can they be exported?
            //TODO: Subsurface color is exported but not used in the MakeEffect Methods
            if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
            {
                if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                {
                    var tex = await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture);
                    albedoTex = new Texture(tex, true, TextureFilterMode.Linear)
                    {
                        PathAndName = m.Albedo.Texture
                    };
                    _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                }
                sfx = MakeEffect.FromBRDFAlbedoTexture(m.Albedo.Color, emissive, m.BRDF.Roughness, m.BRDF.Metallic, m.BRDF.Specular, m.BRDF.IOR, m.BRDF.Subsurface, albedoTex, m.Albedo.Mix, float2.One);
            }
            else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
            {
                if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                {
                    var tex = await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture);
                    normalTex = new Texture(tex, false, TextureFilterMode.Linear)
                    {
                        PathAndName = m.NormalMap.Texture
                    };
                    _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                }
                sfx = MakeEffect.FromBRDFNormalTexture(m.Albedo.Color, emissive, m.BRDF.Roughness, m.BRDF.Metallic, m.BRDF.Specular, m.BRDF.IOR, m.BRDF.Subsurface, normalTex, m.NormalMap.Intensity, float2.One);
            }
            else if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
            {
                if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                {
                    var tex = await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture);
                    albedoTex = new Texture(tex, true, TextureFilterMode.Linear)
                    {
                        PathAndName = m.Albedo.Texture
                    };
                    _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                }
                if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                {
                    var tex = await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture);
                    normalTex = new Texture(tex, false, TextureFilterMode.Linear)
                    {
                        PathAndName = m.NormalMap.Texture
                    };
                    _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                }
                sfx = MakeEffect.FromBRDFTexture(m.Albedo.Color, emissive, m.BRDF.Roughness, m.BRDF.Metallic, m.BRDF.Specular, m.BRDF.IOR, m.BRDF.Subsurface, albedoTex, normalTex, m.Albedo.Mix, float2.One, m.NormalMap.Intensity);
            }
            else if (lightingSetup == LightingSetupFlags.BRDF)
            {
                sfx = MakeEffect.FromBRDF(m.Albedo.Color, emissive, m.BRDF.Roughness, m.BRDF.Metallic, m.BRDF.Specular, m.BRDF.IOR, m.BRDF.Subsurface);
            }
            else
            {
                Diagnostics.Error($"Material {m} with lighting setup {lightingSetup} couldn't be resolved.");
            }

            _matMap.Add(m, sfx);
            return sfx;
        }

        private async Task<Effect> GetEffectForMat(FusMaterialBase m, LightingSetupFlags lightingSetup, float shininess, float specularStrenght, float roughness)
        {
            Effect sfx = null;

            if (m.Albedo.Texture != null && m.Albedo.Texture != "")
            {
                lightingSetup |= LightingSetupFlags.AlbedoTex;
            }

            if (m.NormalMap?.Texture != null && m.NormalMap.Texture != "")
            {
                lightingSetup |= LightingSetupFlags.NormalMap;
            }

            var emissive = m.Emissive?.Color == null ? new float4() : m.Emissive.Color;

            if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseSpecular))
            {
                if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    sfx = MakeEffect.FromDiffuseSpecularAlbedoTexture(m.Albedo.Color, emissive, albedoTex, m.Albedo.Mix, float2.One, shininess, specularStrenght, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromDiffuseSpecularNormalTexture(m.Albedo.Color, emissive, normalTex, m.NormalMap.Intensity, float2.One.PerpendicularLeft, shininess, specularStrenght, roughness);
                }
                else if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromDiffuseSpecularTexture(m.Albedo.Color, emissive, albedoTex, normalTex, m.Albedo.Mix, float2.One, shininess, specularStrenght, m.NormalMap.Intensity, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    sfx = MakeEffect.FromDiffuseSpecular(m.Albedo.Color, emissive, shininess, specularStrenght, roughness);
                }
                else
                {
                    Diagnostics.Error($"Material {m} with lighting setup {lightingSetup} couldn't be resolved.");
                }
            }

            else if (lightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly))
            {
                if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    sfx = MakeEffect.FromDiffuseAlbedoTexture(m.Albedo.Color, albedoTex, float2.One, m.Albedo.Mix, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromDiffuseNormalTexture(m.Albedo.Color, normalTex, m.NormalMap.Intensity, float2.One, roughness);
                }
                else if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromDiffuseTexture(m.Albedo.Color, albedoTex, normalTex, m.Albedo.Mix, float2.One, m.NormalMap.Intensity, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    sfx = MakeEffect.FromDiffuse(m.Albedo.Color, roughness);
                }
                else
                {
                    Diagnostics.Error($"Material {m} with lighting setup {lightingSetup} couldn't be resolved.");
                }
            }

            else if (lightingSetup.HasFlag(LightingSetupFlags.Glossy))
            {
                if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    sfx = MakeEffect.FromGlossyAlbedoTexture(m.Albedo.Color, albedoTex, float2.One, m.Albedo.Mix, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromGlossyNormalTexture(m.Albedo.Color, normalTex, m.NormalMap.Intensity, float2.One, roughness);
                }
                else if (lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    if (!_texMap.TryGetValue(m.Albedo.Texture, out var albedoTex))
                    {
                        albedoTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.Albedo.Texture), true, TextureFilterMode.Linear)
                        {
                            PathAndName = m.Albedo.Texture
                        };
                        _texMap.TryAdd(m.Albedo.Texture, albedoTex);
                    }
                    if (!_texMap.TryGetValue(m.NormalMap.Texture, out var normalTex))
                    {
                        normalTex = new Texture(await AssetStorage.GetAsync<ImageData>(m.NormalMap.Texture), false, TextureFilterMode.Linear)
                        {
                            PathAndName = m.NormalMap.Texture
                        };
                        _texMap.TryAdd(m.NormalMap.Texture, normalTex);
                    }
                    sfx = MakeEffect.FromGlossyTexture(m.Albedo.Color, albedoTex, normalTex, m.Albedo.Mix, float2.One, m.NormalMap.Intensity, roughness);
                }
                else if (!lightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) && !lightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    sfx = MakeEffect.FromGlossy(m.Albedo.Color, roughness);
                }
                else
                {
                    Diagnostics.Error($"Material {m} with lighting setup {lightingSetup} couldn't be resolved.");
                }
            }
            else
            {
                Diagnostics.Error($"Material {m} with lighting setup {lightingSetup} couldn't be resolved.");
            }

            _matMap.Add(m, sfx);
            return sfx;
        }
        #endregion
    }

    internal class SceneToFusFileConvertV1 : Visitor<SceneNode, SceneComponent>
    {
        private readonly FusFile _convertedScene;
        private readonly Stack<FusNode> _predecessors;
        private FusNode _currentNode;

        private readonly Stack<FusComponent> _boneContainers;

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing. Override this method to perform pop on any self-defined state.
        /// </summary>
        protected override void PopState()
        {
            _predecessors.Pop();
        }

        internal SceneToFusFileConvertV1()
        {
            _predecessors = new Stack<FusNode>();
            _convertedScene = new FusFile
            {
                Contents = new FusScene()
            };
            _boneContainers = new Stack<FusComponent>();
        }

        internal FusFile Convert(SceneContainer sc)
        {
            Traverse(sc.Children);
            return _convertedScene;
        }

        #region Visitors

        /// <summary>
        /// Converts the SceneNode container.
        /// </summary>
        /// <param name="snc"></param>
        [VisitMethod]
        public void ConvSceneNode(SceneNode snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                _currentNode = new FusNode { Name = snc.Name };

                parent.AddNode(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _currentNode = new FusNode { Name = CurrentNode.Name };
                ((FusScene)_convertedScene.Contents).AddNode(_currentNode);
                _predecessors.Push(_currentNode);
            }
        }

        ///<summary>
        /// Converts the animation component.
        ///</summary>
        [VisitMethod]
        public void ConvAnimation(Animation a)
        {
            // TODO: Test animation and refactor animation method from scene renderer to this converter
        }

        ///<summary>
        /// Converts the XForm component.
        ///</summary>
        [VisitMethod]
        public void ConvXForm(XForm xf)
        {
            _currentNode.AddComponent(new FusXForm
            {
                Name = xf.Name
            });
        }

        ///<summary>
        /// Converts the XFormText component.
        ///</summary>
        [VisitMethod]
        public void ConvXFormText(XFormText xft)
        {
            _currentNode.AddComponent(new FusXFormText
            {
                Height = xft.Height,
                Width = xft.Width,
                Name = xft.Name,

                HorizontalAlignment =
                xft.HorizontalAlignment == HorizontalTextAlignment.Center
                ? FusHorizontalTextAlignment.Center
                : xft.HorizontalAlignment == HorizontalTextAlignment.Left
                ? FusHorizontalTextAlignment.Left
                : FusHorizontalTextAlignment.Right,

                VerticalAlignment = xft.VerticalAlignment == VerticalTextAlignment.Center
                ? FusVerticalTextAlignment.Center
                : xft.VerticalAlignment == VerticalTextAlignment.Bottom
                ? FusVerticalTextAlignment.Bottom
                : FusVerticalTextAlignment.Top
            });
        }

        ///<summary>
        /// Converts the CanvasTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvCanvasTransform(CanvasTransform ct)
        {
            _currentNode.AddComponent(new FusCanvasTransform(ct.CanvasRenderMode == Scene.CanvasRenderMode.Screen
                ? Serialization.V1.CanvasRenderMode.Screen
                : Serialization.V1.CanvasRenderMode.World)
            {
                Name = ct.Name,
                Scale = ct.Scale,
                ScreenSpaceSize = ct.ScreenSpaceSize,
                Size = ct.Size
            });
        }

        ///<summary>
        /// Converts the RectTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvRectTransform(RectTransform rt)
        {
            _currentNode.AddComponent(new FusRectTransform
            {
                Name = rt.Name,
                Anchors = rt.Anchors,
                Offsets = rt.Offsets
            });
        }

        ///<summary>
        ///Converts the transform component.
        ///</summary>
        [VisitMethod]
        public void ConvTransform(Transform t)
        {
            _currentNode.AddComponent(new FusTransform
            {
                Translation = t.Translation,
                Name = t.Name,
                Rotation = t.Rotation,
                Scale = t.Scale
            });
        }

        //TODO: implement FusMaterialDiffuseBRDF and FusMaterialGlossyBRDF
        /// <summary>
        /// Converts an effect.
        /// </summary>
        /// <param name="effect"></param>
        [VisitMethod]
        public void ConvEffect(DefaultSurfaceEffect effect)
        {
            if (effect.LightingSetup.HasFlag(LightingSetupFlags.BRDF))
            {
                var mat = new FusMaterialBRDF() { Albedo = new AlbedoChannel() };
                if (effect.LightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) || effect.LightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    var surfaceInput = (TextureInputBRDF)effect.SurfaceInput;
                    mat.Albedo = new AlbedoChannel()
                    {
                        Color = surfaceInput.Albedo
                    };

                    if (surfaceInput.AlbedoTex != null)
                    {
                        mat.Albedo.Mix = surfaceInput.AlbedoMix;
                        mat.Albedo.Texture = surfaceInput.AlbedoTex?.PathAndName;
                    }

                    if (surfaceInput.NormalTex != null)
                    {
                        mat.NormalMap = new NormapMapChannel()
                        {
                            Texture = surfaceInput.NormalTex.PathAndName,
                            Intensity = surfaceInput.NormalMappingStrength
                        };
                    }

                    mat.BRDF = new BRDFChannel()
                    {
                        IOR = surfaceInput.IOR,
                        Metallic = surfaceInput.Metallic,
                        Roughness = surfaceInput.Roughness,
                        Specular = surfaceInput.Specular,
                        Subsurface = surfaceInput.Subsurface,
                        SubsurfaceColor = surfaceInput.SubsurfaceColor
                    };

                }
                else
                {
                    var surfaceInput = (BRDFInput)effect.SurfaceInput;
                    mat.Albedo = new AlbedoChannel()
                    {
                        Color = surfaceInput.Albedo
                    };
                    mat.BRDF = new BRDFChannel()
                    {
                        IOR = surfaceInput.IOR,
                        Metallic = surfaceInput.Metallic,
                        Roughness = surfaceInput.Roughness,
                        Specular = surfaceInput.Specular,
                        Subsurface = surfaceInput.Subsurface,
                        SubsurfaceColor = surfaceInput.SubsurfaceColor
                    };
                }

                _currentNode.AddComponent(mat);

            }
            else if (effect.LightingSetup.HasFlag(LightingSetupFlags.DiffuseSpecular) ||
                    effect.LightingSetup.HasFlag(LightingSetupFlags.DiffuseOnly) ||
                    effect.LightingSetup.HasFlag(LightingSetupFlags.Unlit) ||
                    effect.LightingSetup.HasFlag(LightingSetupFlags.Glossy))
            {
                var mat = new FusMaterialStandard();
                if (effect.LightingSetup.HasFlag(LightingSetupFlags.AlbedoTex) || effect.LightingSetup.HasFlag(LightingSetupFlags.NormalMap))
                {
                    var surfaceInput = (TextureInputSpecular)effect.SurfaceInput;

                    mat.Albedo = new AlbedoChannel()
                    {
                        Color = surfaceInput.Albedo
                    };

                    if (surfaceInput.AlbedoTex != null)
                    {
                        mat.Albedo.Mix = surfaceInput.AlbedoMix;
                        mat.Albedo.Texture = surfaceInput.AlbedoTex?.PathAndName;
                    }

                    if (surfaceInput.NormalTex != null)
                    {
                        mat.NormalMap = new NormapMapChannel()
                        {
                            Texture = surfaceInput.NormalTex.PathAndName,
                            Intensity = surfaceInput.NormalMappingStrength
                        };
                    }

                    mat.Specular = new SpecularChannel()
                    {
                        Shininess = surfaceInput.Shininess,
                        Strength = surfaceInput.SpecularStrength
                    };

                }
                else
                {
                    var surfaceInput = (SpecularInput)effect.SurfaceInput;
                    mat.Albedo = new AlbedoChannel()
                    {
                        Color = surfaceInput.Albedo
                    };

                    mat.Specular = new SpecularChannel()
                    {
                        Shininess = surfaceInput.Shininess,
                        Strength = surfaceInput.SpecularStrength
                    };
                }

                _currentNode.AddComponent(mat);
            }
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        /// <param name="cc">The camera to convert.</param>
        [VisitMethod]
        public void ConvCamComp(Camera cc)
        {
            _currentNode.AddComponent(new FusCamera
            {
                Active = cc.Active,
                BackgroundColor = cc.BackgroundColor,
                ClearColor = cc.ClearColor,
                ClearDepth = cc.ClearDepth,
                Layer = cc.Layer,
                Name = cc.Name,
                ClippingPlanes = cc.ClippingPlanes,
                Fov = cc.Fov,
                Viewport = cc.Viewport,
                ProjectionMethod = cc.ProjectionMethod == Fusee.Engine.Core.Scene.ProjectionMethod.Orthographic ? Serialization.V1.ProjectionMethod.Orthographic : Serialization.V1.ProjectionMethod.Perspective
            });
        }

        /// <summary>
        /// Converts the mesh.
        /// </summary>
        /// <param name="m">The mesh to convert.</param>
        [VisitMethod]
        public void ConvMesh(Mesh m)
        {
            // convert mesh
            var mesh = new FusMesh
            {
                MeshType = m.MeshType,
                BiTangents = m.BiTangents,
                BoneIndices = m.BoneIndices,
                BoundingBox = m.BoundingBox,
                BoneWeights = m.BoneWeights,
                Colors = m.Colors,
                Name = m.Name,
                Normals = m.Normals,
                Tangents = m.Tangents,
                Triangles = m.Triangles,
                UVs = m.UVs,
                Vertices = m.Vertices
            };

            _currentNode.AddComponent(mesh);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="l"></param>
        [VisitMethod]
        public void ConvLight(Light l)
        {
            _currentNode.AddComponent(new FusLight
            {
                Name = l.Name,
                Active = l.Active,
                Bias = l.Bias,
                Color = l.Color,
                InnerConeAngle = l.InnerConeAngle,
                IsCastingShadows = l.IsCastingShadows,
                MaxDistance = l.MaxDistance,
                OuterConeAngle = l.OuterConeAngle,
                Strength = l.Strength,
                Type = l.Type
            });
        }

        /// <summary>
        /// Converts the camera.
        /// </summary>
        /// <param name="cam"></param>
        [VisitMethod]
        public void ConvCamera(Camera cam)
        {
            _currentNode.AddComponent(new FusCamera
            {
                Active = cam.Active,
                BackgroundColor = cam.BackgroundColor,
                ClearColor = cam.ClearColor,
                ClearDepth = cam.ClearDepth,
                Layer = cam.Layer,
                Name = cam.Name,
                ClippingPlanes = cam.ClippingPlanes,
                Fov = cam.Fov,
                ProjectionMethod = cam.ProjectionMethod == Fusee.Engine.Core.Scene.ProjectionMethod.Orthographic ? Serialization.V1.ProjectionMethod.Orthographic : Serialization.V1.ProjectionMethod.Perspective
            });
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="oct"></param>
        [VisitMethod]
        public void ConvOctant(OctantD oct)
        {
            _currentNode.AddComponent(new FusOctantD
            {
                Center = new double3(oct.Center.x, oct.Center.y, oct.Center.z),
                Guid = oct.Guid,
                IsLeaf = oct.IsLeaf,
                Level = oct.Level,
                Name = oct.Name,
                NumberOfPointsInNode = oct.NumberOfPointsInNode,
                PosInHierarchyTex = oct.PosInHierarchyTex,
                PosInParent = oct.PosInParent,
                Size = oct.Size,
                VisibleChildIndices = oct.VisibleChildIndices,
                WasLoaded = oct.WasLoaded
            });
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="oct"></param>
        [VisitMethod]
        public void ConvOctant(OctantF oct)
        {
            _currentNode.AddComponent(new FusOctantF
            {
                Center = new float3(oct.Center.x, oct.Center.y, oct.Center.z),
                Guid = oct.Guid,
                IsLeaf = oct.IsLeaf,
                Level = oct.Level,
                Name = oct.Name,
                NumberOfPointsInNode = oct.NumberOfPointsInNode,
                PosInHierarchyTex = oct.PosInHierarchyTex,
                PosInParent = oct.PosInParent,
                Size = oct.Size,
                VisibleChildIndices = oct.VisibleChildIndices,
                WasLoaded = oct.WasLoaded
            });
        }

        /// <summary>
        /// Adds the bone component.
        /// </summary>
        /// <param name="bone"></param>
        [VisitMethod]
        public void ConvBone(Bone bone)
        {
            var currentBone = new FusBone
            {
                Name = bone.Name
            };
            _currentNode.AddComponent(currentBone);

            // Collect all bones, later, when a WeightComponent is found, we can set all Joints
            _boneContainers.Push(currentBone);
        }

        /// <summary>
        /// Converts the weight component.
        /// </summary>
        /// <param name="w"></param>
        [VisitMethod]
        public void ConVWeight(Weight w)
        {
            var weight = new FusWeight
            {
                WeightMap = w.WeightMap.Select(wm =>
                {

                    var currentWeightList = new Serialization.V1.VertexWeightList
                    {
                        VertexWeights = new List<Serialization.V1.VertexWeight>()
                    };

                    var currentVertexWeights = wm.VertexWeights.Select(ww => new Serialization.V1.VertexWeight { JointIndex = ww.JointIndex, Weight = ww.Weight }).ToList();

                    currentWeightList.VertexWeights.AddRange(currentVertexWeights);
                    return currentWeightList;

                }).ToList(),
                BindingMatrices = w.BindingMatrices,
                Joints = new List<FusComponent>(),
                Name = w.Name
            };

            // check if we have bones
            if (_boneContainers.Count >= 1)
            {
                if (weight.Joints == null) // initialize joint container
                {
                    weight.Joints = new List<FusComponent>();
                }

                // set all bones found until this WeightComponent
                while (_boneContainers.Count != 0)
                {
                    weight.Joints.Add(_boneContainers.Pop());
                }
            }

            _currentNode.AddComponent(weight);
        }
        #endregion
    }
}