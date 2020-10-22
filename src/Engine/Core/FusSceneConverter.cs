using Fusee.Base.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Serialization.V1;
using Fusee.Structures.Octree;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use <see cref="ConvertFrom(FusFile)"/> and <see cref="ConvertTo(SceneContainer)"/>, to create new high/low level graph from a low/high level graph (made out of scene nodes and components)
    /// in order to have each visited element converted and/or split into its high/low level, render-ready/serialization-ready components.
    /// </summary>
    public static class FusSceneConverter
    {
        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph <see cref="Scene"/> by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="fus">The FusFile to convert.</param>
        public static SceneContainer ConvertFrom(FusFile fus)
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

            var instance = new FusFileToSceneConvertV1();
            var payload = (FusScene)fus.Contents;
            var converted = instance.Convert(payload);

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

        private readonly Dictionary<FusMaterial, ShaderEffectProtoPixel> _matMap;
        private readonly Dictionary<FusMesh, Mesh> _meshMap;
        private readonly Stack<SceneNode> _boneContainers;

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

            _matMap = new Dictionary<FusMaterial, ShaderEffectProtoPixel>();
            _meshMap = new Dictionary<FusMesh, Mesh>();
            _boneContainers = new Stack<SceneNode>();
        }

        internal SceneContainer Convert(FusScene sc)
        {
            _fusScene = sc;
            Traverse(sc.Children);
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
                    parent.Children = new ChildList();

                _currentNode = new SceneNode { Name = snc.Name };

                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new SceneNode { Name = CurrentNode.Name });

                _currentNode = _predecessors.Peek();

                if (_convertedScene.Children != null)
                    _convertedScene.Children.Add(_currentNode);
                else
                    _convertedScene.Children = new List<SceneNode> { _currentNode };
            }
        }

        ///<summary>
        /// Converts the animation component.
        ///</summary>
        [VisitMethod]
        public void ConvAnimation(FusAnimation a)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

            // TODO: Test animation and refactor animation method from scene renderer to this converter 
        }


        ///<summary>
        /// Converts the XForm component.
        ///</summary>
        [VisitMethod]
        public void ConvXForm(FusXForm xf)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

            _currentNode.AddComponent(new XFormText
            {
                Height = xft.Height,
                Width = xft.Width,
                Name = xft.Name,
                HorizontalAlignment =
                xft.HorizontalAlignment == Serialization.V1.HorizontalTextAlignment.Center
                ? Scene.HorizontalTextAlignment.Center
                : xft.HorizontalAlignment == Serialization.V1.HorizontalTextAlignment.Left
                ? Scene.HorizontalTextAlignment.Left
                : Scene.HorizontalTextAlignment.Right,
                VerticalAlignment = xft.VerticalAlignment == Serialization.V1.VerticalTextAlignment.Center
                ? Scene.VerticalTextAlignment.Center
                : xft.VerticalAlignment == Serialization.V1.VerticalTextAlignment.Bottom
                ? Scene.VerticalTextAlignment.Bottom
                : Scene.VerticalTextAlignment.Top
            });
        }

        ///<summary>
        /// Converts the CanvasTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvCanvasTransform(FusCanvasTransform ct)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

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
        public void ConvMaterial(FusMaterial matComp)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(effect);
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(FusMaterialPBR matComp)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(effect);
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCamComp(FusCamera cc)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

            var currentNodeEffect = _currentNode.GetComponent<ShaderEffect>();

            if (currentNodeEffect?.GetEffectParam(UniformNameDeclarations.NormalMap) != null)
            {
                mesh.Tangents = mesh.CalculateTangents();
                mesh.BiTangents = mesh.CalculateBiTangents();
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
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

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
                _currentNode.Components = new List<SceneComponent>();

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
                    weight.Joints = new List<SceneNode>();

                // set all bones found until this WeightComponent
                while (_boneContainers.Count != 0)
                    weight.Joints.Add(_boneContainers.Pop());
            }

            _currentNode.Components.Add(weight);
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="cc"></param>
        [VisitMethod]
        public void ConvOctant(FusOctant cc)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponent>();

            _currentNode.AddComponent(
            new Octant() 
            { 
                OctantD = new OctantD
                {
                    Center = cc.Center,

                    IsLeaf = cc.IsLeaf,
                    Level = cc.Level,
                    PosInParent = cc.PosInParent,
                    Size = cc.Size,
                    
                },
                Guid = cc.Guid,
                Name = cc.Name,
                NumberOfPointsInNode = cc.NumberOfPointsInNode,
                PosInHierarchyTex = cc.PosInHierarchyTex,
                VisibleChildIndices = cc.VisibleChildIndices,
                WasLoaded = cc.WasLoaded
            });
        }

        #endregion

        #region Make ShaderEffect

        private ShaderEffectProtoPixel LookupMaterial(FusMaterial m)
        {
            if (_matMap.TryGetValue(m, out var sfx)) return sfx;

            var vals = new MaterialValues();

            if (m.HasNormalMap)
            {
                vals.NormalMap = m.NormalMap.Texture ?? null;
                vals.NormalMapIntensity = m.HasNormalMap ? m.NormalMap.Intensity : 0;
            }
            if (m.HasAlbedo)
            {
                vals.AlbedoColor = m.Albedo.Color;
                vals.AlbedoMix = m.Albedo.Mix;
                vals.AlbedoTexture = m.Albedo.Texture ?? null;
            }
            if (m.HasEmissive)
            {
                vals.EmissiveColor = m.Emissive.Color;
                vals.EmissiveMix = m.Emissive.Mix;
                vals.EmissiveTexture = m.Emissive.Texture ?? null;
            }

            if (m.HasSpecular)
            {
                vals.SpecularColor = m.Specular.Color;
                vals.SpecularMix = m.Specular.Mix;
                vals.SpecularTexture = m.Specular.Texture ?? null;
                vals.SpecularIntensity = m.Specular.Intensity;
                vals.SpecularShininess = m.Specular.Shininess;
            }

            sfx = ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(
                new ShaderEffectProps
                {
                    MatProbs =
                    {
                        HasAlbedo = m.HasAlbedo,
                        HasAlbedoTexture = m.HasAlbedo && m.Albedo.Texture != null,
                        HasSpecular = m.HasSpecular,
                        HasSpecularTexture = m.HasSpecular && m.Specular.Texture != null,
                        HasEmissive = m.HasEmissive,
                        HasEmissiveTexture = m.HasEmissive && m.Emissive.Texture != null,
                        HasNormalMap = m.HasNormalMap
                    },
                    MatType = MaterialType.Standard,
                    MatValues = vals
                });

            sfx.Name = m.Name ?? "";

            _matMap.Add(m, sfx);
            return sfx;
        }

        private ShaderEffect LookupMaterial(FusMaterialPBR m)
        {
            if (_matMap.TryGetValue(m, out var sfx)) return sfx;

            var vals = new MaterialValues();

            if (m.HasNormalMap)
            {
                vals.NormalMap = m.NormalMap.Texture ?? null;
                vals.NormalMapIntensity = m.HasNormalMap ? m.NormalMap.Intensity : 0;
            }
            if (m.HasAlbedo)
            {
                vals.AlbedoColor = m.Albedo.Color;
                vals.AlbedoMix = m.Albedo.Mix;
                vals.AlbedoTexture = m.Albedo.Texture ?? null;
            }
            if (m.HasEmissive)
            {
                vals.EmissiveColor = m.Emissive.Color;
                vals.EmissiveMix = m.Emissive.Mix;
                vals.EmissiveTexture = m.Emissive.Texture ?? null;
            }

            if (m.HasSpecular)
            {
                vals.SpecularColor = m.Specular.Color;
                vals.SpecularMix = m.Specular.Mix;
                vals.SpecularTexture = m.Specular.Texture ?? null;
                vals.SpecularIntensity = m.Specular.Intensity;
                vals.SpecularShininess = m.Specular.Shininess;
            }

            vals.DiffuseFraction = m.DiffuseFraction;
            vals.FresnelReflectance = m.FresnelReflectance;
            vals.RoughnessValue = m.RoughnessValue;

            sfx = ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(
                new ShaderEffectProps
                {
                    MatProbs =
                    {
                        HasAlbedo = m.HasAlbedo,
                        HasAlbedoTexture = m.HasAlbedo && m.Albedo.Texture != null,
                        HasSpecular = m.HasSpecular,
                        HasSpecularTexture = m.HasSpecular && m.Specular.Texture != null,
                        HasEmissive = m.HasEmissive,
                        HasEmissiveTexture = m.HasEmissive && m.Emissive.Texture != null,
                        HasNormalMap = m.HasNormalMap
                    },
                    MatType = MaterialType.MaterialPbr,
                    MatValues = vals
                });

            sfx.Name = m.Name ?? "";

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
        public void ConvAnimation(Core.Scene.Animation a)
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
                xft.HorizontalAlignment == Scene.HorizontalTextAlignment.Center
                ? Serialization.V1.HorizontalTextAlignment.Center
                : xft.HorizontalAlignment == Scene.HorizontalTextAlignment.Left
                ? Serialization.V1.HorizontalTextAlignment.Left
                : Serialization.V1.HorizontalTextAlignment.Right,
                VerticalAlignment = xft.VerticalAlignment == Scene.VerticalTextAlignment.Center
                ? Serialization.V1.VerticalTextAlignment.Center
                : xft.VerticalAlignment == Scene.VerticalTextAlignment.Bottom
                ? Serialization.V1.VerticalTextAlignment.Bottom
                : Serialization.V1.VerticalTextAlignment.Top
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

        /// <summary>
        /// Converts the shader effect.
        /// </summary>
        /// <param name="fx"></param>
        [VisitMethod]
        public void ConvShaderEffect(ShaderEffect fx)
        {
            // TODO: FusMaterialPBR not yet implemented, needs to be done when blender export to principle BRDF shader is implemented properly

            var mat = new FusMaterial();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.AlbedoColor))
                mat.Albedo = new MatChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.SpecularColor))
                mat.Specular = new SpecularChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.NormalMap))
                mat.NormalMap = new NormapMapChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.EmissiveColor))
                mat.Emissive = new MatChannelContainer();

            foreach (var decl in fx.ParamDecl)
            {
                switch (decl.Key)
                {
                    case UniformNameDeclarations.AlbedoColor:
                        mat.Albedo.Color = (float4)decl.Value;
                        break;
                    case UniformNameDeclarations.AlbedoTexture:
                        mat.Albedo.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.AlbedoMix:
                        mat.Albedo.Mix = (float)decl.Value;
                        break;


                    case UniformNameDeclarations.EmissiveColor:
                        mat.Emissive.Color = (float4)decl.Value;
                        break;
                    case UniformNameDeclarations.EmissiveTexture:
                        mat.Emissive.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.EmissiveMix:
                        mat.Emissive.Mix = (float)decl.Value;
                        break;


                    case UniformNameDeclarations.SpecularColor:
                        mat.Specular.Color = (float4)decl.Value;
                        break;
                    case UniformNameDeclarations.SpecularTexture:
                        mat.Specular.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.SpecularMix:
                        mat.Specular.Mix = (float)decl.Value;
                        break;
                    case UniformNameDeclarations.SpecularShininess:
                        mat.Specular.Shininess = (float)decl.Value;
                        break;
                    case UniformNameDeclarations.SpecularIntensity:
                        mat.Specular.Intensity = (float)decl.Value;
                        break;

                    case UniformNameDeclarations.NormalMap:
                        mat.NormalMap.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.NormalMapIntensity:
                        mat.NormalMap.Intensity = (float)decl.Value;
                        break;
                }
            }

            _currentNode.AddComponent(mat);
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
                ProjectionMethod = cam.ProjectionMethod == Scene.ProjectionMethod.Orthographic ? Serialization.V1.ProjectionMethod.Orthographic : Serialization.V1.ProjectionMethod.Perspective
            });
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="oct"></param>
        [VisitMethod]
        public void ConvOctant(Octant oct)
        {
            _currentNode.AddComponent(new FusOctant
            {
                Center = oct.OctantD.Center,
                Guid = oct.Guid,
                IsLeaf = oct.OctantD.IsLeaf,
                Level = oct.OctantD.Level,
                Name = oct.Name,
                NumberOfPointsInNode = oct.NumberOfPointsInNode,
                PosInHierarchyTex = oct.PosInHierarchyTex,
                PosInParent = oct.OctantD.PosInParent,
                Size = oct.OctantD.Size,
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
                    weight.Joints = new List<FusComponent>();

                // set all bones found until this WeightComponent
                while (_boneContainers.Count != 0)
                    weight.Joints.Add(_boneContainers.Pop());
            }

            _currentNode.AddComponent(weight);
        }
        #endregion
    }
}