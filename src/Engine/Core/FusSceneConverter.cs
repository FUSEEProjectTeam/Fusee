using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Jometri;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Serialization.V1;
using Fusee.Xene;
using Fusee.Xirkit;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use <see cref="ConvertFrom(FusFile)"/> and <see cref="ConvertTo(Scene)"/>, to create new high/low level graph from a low/high level graph (made out of scene nodes and components)
    /// in order to have each visited element converted and/or split into its high/low level, render-ready/serialization-ready components.
    /// </summary>
    public static class FusSceneConverter
    {
        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph <see cref="Scene"/> by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="fus">The FusFile to convert.</param>
        public static Scene ConvertFrom(FusFile fus)
        {
            if (fus == null)
            {
                Diagnostics.Error("Could not read content of scene, file is null!");
                return new Scene();
            }

            // try to cast, if this fails the content is empty or null
            if (!(fus.Contents is FusScene))
            {
                Diagnostics.Error($"Could not read content of scene from {fus.Header.CreationDate} created by {fus.Header.CreatedBy} with {fus.Header.Generator}");
                return new Scene();
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
        public static FusFile ConvertTo(Scene sc)
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
        private readonly Scene _convertedScene;
        private readonly Stack<SceneNode> _predecessors;
        private SceneNode _currentNode;

        private readonly Dictionary<Material, ShaderEffect> _matMap;
        private readonly Dictionary<MaterialPBR, ShaderEffect> _pbrComponent;
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
            _convertedScene = new Scene();

            _matMap = new Dictionary<Material, ShaderEffect>();
            _pbrComponent = new Dictionary<MaterialPBR, ShaderEffect>();
            _boneContainers = new Stack<SceneNode>();
        }

        internal Scene Convert(FusScene sc)
        {
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
                // TODO: implement and test!

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
                xft.HorizontalAlignment == Serialization.V1.HorizontalTextAlignment.CENTER
                ? Common.HorizontalTextAlignment.CENTER
                : xft.HorizontalAlignment == Serialization.V1.HorizontalTextAlignment.LEFT
                ? Common.HorizontalTextAlignment.LEFT
                : Common.HorizontalTextAlignment.RIGHT,
                VerticalAlignment = xft.VerticalAlignment == Serialization.V1.VerticalTextAlignment.CENTER
                ? Common.VerticalTextAlignment.CENTER
                : xft.VerticalAlignment == Serialization.V1.VerticalTextAlignment.BOTTOM
                ? Common.VerticalTextAlignment.BOTTOM
                : Common.VerticalTextAlignment.TOP
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

            _currentNode.AddComponent(new CanvasTransform(ct.CanvasRenderMode == Serialization.V1.CanvasRenderMode.SCREEN
                ? Common.CanvasRenderMode.SCREEN
                : Common.CanvasRenderMode.WORLD)
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
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(effect);
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCamComp(FusCamera cc)
        {
            var cam = new Camera(cc.ProjectionMethod == Serialization.V1.ProjectionMethod.ORTHOGRAPHIC ? Common.ProjectionMethod.ORTHOGRAPHIC : Common.ProjectionMethod.PERSPECTIVE,
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
            // convert mesh
            var mesh = new Mesh
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

            if (currentNodeEffect?.GetEffectParam(UniformNameDeclarations.BumpTexture) != null)
            {
                mesh.Tangents = mesh.CalculateTangents();
                mesh.BiTangents = mesh.CalculateBiTangents();
            }

            _currentNode.Components.Add(mesh);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="l"></param>
        [VisitMethod]
        public void ConvLight(FusLight l)
        {
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
            var weight = new Weight
            {
                WeightMap = w.WeightMap.Select(wm =>
                {

                    var currentWeightList = new Common.VertexWeightList
                    {
                        VertexWeights = new List<Common.VertexWeight>()
                    };

                    var currentVertexWeights = wm.VertexWeights.Select(ww => new Common.VertexWeight { JointIndex = ww.JointIndex, Weight = ww.Weight }).ToList();

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
        /// <param name="c"></param>
        [VisitMethod]
        public void ConvOctant(FusOctant cc)
        {
            _currentNode.AddComponent(new Octant
            {
                Center = cc.Center,
                Guid = cc.Guid,
                IsLeaf = cc.IsLeaf,
                Level = cc.Level,
                Name = cc.Name,
                NumberOfPointsInNode = cc.NumberOfPointsInNode,
                PosInHierarchyTex = cc.PosInHierarchyTex,
                PosInParent = cc.PosInParent,
                Size = cc.Size,
                VisibleChildIndices = cc.VisibleChildIndices,
                WasLoaded = cc.WasLoaded
            });
        }

        #endregion

        #region Make ShaderEffect

        private ShaderEffect LookupMaterial(FusMaterial m)
        {
            var mc = new Material
            {
                Name = m.Name ?? ""
            };
            if (m.HasBump)
            {
                mc.Bump = new BumpChannel
                {
                    Intensity = m.HasBump ? m.Bump.Intensity : 0,
                    Texture = m.Emissive.Texture ?? ""
                };
            }
            if (m.HasDiffuse)
            {
                mc.Diffuse = new MatChannel
                {
                    Color = m.Diffuse.Color,
                    Texture = m.Diffuse.Texture ?? "",
                    Mix = m.Diffuse.Mix
                };
            }
            if (m.HasEmissive)
            {
                mc.Emissive = new MatChannel
                {
                    Color = m.Emissive.Color,
                    Texture = m.Emissive.Texture ?? "",
                    Mix = m.Emissive.Mix
                };
            }
            if (m.HasSpecular)
            {
                mc.Specular = new SpecularChannel
                {
                    Color = m.Specular.Color,
                    Mix = m.Specular.Mix,
                    Texture = m.Specular.Texture ?? "",
                    Intensity = m.Specular.Intensity,
                    Shininess = m.Specular.Shininess
                };
            }

            if (_matMap.TryGetValue(mc, out var mat)) return mat;
            mat = ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(mc, _currentNode.GetWeights()); // <- broken
            _matMap.Add(mc, mat);
            return mat;
        }

        private ShaderEffect LookupMaterial(FusMaterialPBR m)
        {
            var mc = new MaterialPBR
            {
                Name = m.Name ?? ""
            };
            if (m.HasBump)
            {
                mc.Bump = new BumpChannel
                {
                    Intensity = m.HasBump ? m.Bump.Intensity : 0,
                    Texture = m.Emissive.Texture ?? ""
                };
            }
            if (m.HasDiffuse)
            {
                mc.Diffuse = new MatChannel
                {
                    Color = m.Diffuse.Color,
                    Texture = m.Diffuse.Texture ?? "",
                    Mix = m.Diffuse.Mix
                };
            }
            if (m.HasEmissive)
            {
                mc.Emissive = new MatChannel
                {
                    Color = m.Emissive.Color,
                    Texture = m.Emissive.Texture ?? "",
                    Mix = m.Emissive.Mix
                };
            }
            if (m.HasSpecular)
            {
                mc.Specular = new SpecularChannel
                {
                    Color = m.Specular.Color,
                    Mix = m.Specular.Mix,
                    Texture = m.Specular.Texture ?? "",
                    Intensity = m.Specular.Intensity,
                    Shininess = m.Specular.Shininess
                };
            }

            mc.DiffuseFraction = m.DiffuseFraction;
            mc.FresnelReflectance = m.FresnelReflectance;
            mc.RoughnessValue = m.RoughnessValue;

            if (_pbrComponent.TryGetValue(mc, out var mat)) return mat;
            mat = ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(mc, _currentNode.GetWeights());
            _pbrComponent.Add(mc, mat);
            return mat;
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
            _convertedScene = new FusFile();

            _boneContainers = new Stack<FusComponent>();
        }

        internal FusFile Convert(Scene sc)
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
                // TODO: implement and test!

                _predecessors.Push(new FusNode { Name = CurrentNode.Name });

                _currentNode = _predecessors.Peek();

                _convertedScene.Contents = new FusScene();

                ((FusScene)_convertedScene.Contents).AddNode(_currentNode);

                //if (_convertedScene.Children != null)
                //    _convertedScene.Children.Add(_currentNode);
                //else
                //    _convertedScene.Children = new List<SceneNode> { _currentNode };
            }
        }

        ///<summary>
        /// Converts the animation component.
        ///</summary>
        [VisitMethod]
        public void ConvAnimation(Common.Animation a)
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
                xft.HorizontalAlignment == Common.HorizontalTextAlignment.CENTER
                ? Serialization.V1.HorizontalTextAlignment.CENTER
                : xft.HorizontalAlignment == Common.HorizontalTextAlignment.LEFT
                ? Serialization.V1.HorizontalTextAlignment.LEFT
                : Serialization.V1.HorizontalTextAlignment.RIGHT,
                VerticalAlignment = xft.VerticalAlignment == Common.VerticalTextAlignment.CENTER
                ? Serialization.V1.VerticalTextAlignment.CENTER
                : xft.VerticalAlignment == Common.VerticalTextAlignment.BOTTOM
                ? Serialization.V1.VerticalTextAlignment.BOTTOM
                : Serialization.V1.VerticalTextAlignment.TOP
            });
        }

        ///<summary>
        /// Converts the CanvasTransform component.
        ///</summary>
        [VisitMethod]
        public void ConvCanvasTransform(CanvasTransform ct)
        {
            _currentNode.AddComponent(new FusCanvasTransform(ct.CanvasRenderMode == Common.CanvasRenderMode.SCREEN
                ? Serialization.V1.CanvasRenderMode.SCREEN
                : Serialization.V1.CanvasRenderMode.WORLD)
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

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.DiffuseColor))
                mat.Diffuse = new MatChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.SpecularColor))
                mat.Specular = new SpecularChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.BumpTexture))
                mat.Bump = new BumpChannelContainer();

            if (fx.ParamDecl.ContainsKey(UniformNameDeclarations.EmissiveColor))
                mat.Emissive = new MatChannelContainer();

            foreach (var decl in fx.ParamDecl)
            {
                switch (decl.Key)
                {
                    case UniformNameDeclarations.DiffuseColor:
                        mat.Diffuse.Color = (float4)decl.Value;
                        break;
                    case UniformNameDeclarations.DiffuseTexture:
                        mat.Diffuse.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.DiffuseMix:
                        mat.Diffuse.Mix = (float)decl.Value;
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

                    case UniformNameDeclarations.BumpTexture:
                        mat.Bump.Texture = (string)decl.Value;
                        break;
                    case UniformNameDeclarations.BumpIntensity:
                        mat.Bump.Intensity = (float)decl.Value;
                        break;
                }
            }

            _currentNode.AddComponent(mat);
        }



        /// <summary>
        /// Converts the shader.
        /// </summary>
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
                ProjectionMethod = cc.ProjectionMethod == Common.ProjectionMethod.ORTHOGRAPHIC ? Serialization.V1.ProjectionMethod.ORTHOGRAPHIC : Serialization.V1.ProjectionMethod.PERSPECTIVE
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
        /// <param name="c"></param>
        [VisitMethod]
        public void ConvCamera(Camera cc)
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
                ProjectionMethod = cc.ProjectionMethod == Common.ProjectionMethod.ORTHOGRAPHIC ? Serialization.V1.ProjectionMethod.ORTHOGRAPHIC : Serialization.V1.ProjectionMethod.PERSPECTIVE
            });
        }

        /// <summary>
        /// Converts the octant.
        /// </summary>
        /// <param name="c"></param>
        [VisitMethod]
        public void ConvOctant(Octant cc)
        {
            _currentNode.AddComponent(new FusOctant
            {
                Center = cc.Center,
                Guid = cc.Guid,
                IsLeaf = cc.IsLeaf,
                Level = cc.Level,
                Name = cc.Name,
                NumberOfPointsInNode = cc.NumberOfPointsInNode,
                PosInHierarchyTex = cc.PosInHierarchyTex,
                PosInParent = cc.PosInParent,
                Size = cc.Size,
                VisibleChildIndices = cc.VisibleChildIndices,
                WasLoaded = cc.WasLoaded
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
