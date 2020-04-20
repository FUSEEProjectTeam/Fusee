﻿using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Serialization.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fusee.Test.Serialization.V1
{
    public class SimpleConvertSceneGraphV1
    {
        [Fact]
        public async void V1_SimpleScene_Convert()
        {
            var scene = new FusFile
            {
                Header = new FusHeader
                {
                    FileVersion = 1,
                    CreationDate = DateTime.Today.ToString(),
                    Generator = "SerializationTest",
                    CreatedBy = "Fusee Test Code"
                },
                Contents = new FusScene
                {
                    ComponentList = new List<FusComponent>(),
                    Children = new List<FusNode>()
                }
            };

            // one mesh inside the scene graph
            var cube = new Cube();

            var daMesh = new FusMesh
            {
                BiTangents = cube.BiTangents,
                BoneIndices = cube.BoneIndices,
                BoneWeights = cube.BoneWeights,
                BoundingBox = cube.BoundingBox,
                Colors = cube.Colors,
                MeshType = cube.MeshType,
                Name = cube.Name,
                Normals = cube.Normals,
                Tangents = cube.Tangents,
                Triangles = cube.Triangles,
                UVs = cube.UVs,
                Vertices = cube.Vertices
            };

            #region Root

            ((FusScene)scene.Contents).AddNode(new FusNode
            {
                Name = "Base"
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusCanvasTransform(Fusee.Serialization.V1.CanvasRenderMode.SCREEN)
            {
                Name = "CanvasTransform",
                Scale = float2.One * 2,
                ScreenSpaceSize = new MinMaxRect
                {
                    Max = float2.One * 22,
                    Min = float2.One * -1

                },
                Size = new MinMaxRect
                {
                    Min = float2.One * 22,
                    Max = float2.One * -1

                }
            });


            ((FusScene)scene.Contents).Children[0].AddComponent(new FusXFormText
            {
                Name = "XFormText",
                Height = 10,
                HorizontalAlignment = Fusee.Serialization.V1.HorizontalTextAlignment.CENTER,
                VerticalAlignment = Fusee.Serialization.V1.VerticalTextAlignment.TOP,
                Width = 200
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusXForm
            {
                Name = "XForm"
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusRectTransform
            {
                Anchors = new MinMaxRect
                {
                    Max = float2.Zero,
                    Min = float2.One
                },
                Name = "Rect",
                Offsets = new MinMaxRect
                {
                    Max = float2.Zero,
                    Min = float2.One
                }
            });


            ((FusScene)scene.Contents).Children[0].AddComponent(new FusTransform
            {
                Scale = new float3(100, 20, 100)
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusBone
            {
                Name = "MyBone"
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusWeight
            {
                BindingMatrices = new List<float4x4>(),
                Joints = new List<FusComponent>(),
                Name = "MyWeight",
                WeightMap = new List<Fusee.Serialization.V1.VertexWeightList>
                           {
                               new Fusee.Serialization.V1.VertexWeightList
                               {
                                   VertexWeights = new List<Fusee.Serialization.V1.VertexWeight>
                                   {
                                       new Fusee.Serialization.V1.VertexWeight
                                       {
                                           Weight = 20,
                                           JointIndex = 0
                                       },
                                        new Fusee.Serialization.V1.VertexWeight
                                       {
                                           Weight = 30,
                                           JointIndex = 1
                                       },
                                   }
                               }
                           }
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusMaterial
            {
                Albedo = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Red) },
                Specular = new SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusLight
            {
                Name = "MyLight",
                Bias = 0,
                MaxDistance = 100,
                Active = true,
                Color = float4.One,
                InnerConeAngle = 20,
                IsCastingShadows = true,
                OuterConeAngle = 20,
                Strength = 100,
                Type = LightType.Point
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusOctant
            {
                Center = double3.One,
                Guid = new Guid(1, 2, 3, new byte[] { 4, 4, 4, 4, 4, 4, 4, 4 }),
                IsLeaf = false,
                Level = 10,
                Name = "MyOctant",
                NumberOfPointsInNode = 2,
                PosInHierarchyTex = 0,
                PosInParent = 5,
                Size = 20,
                VisibleChildIndices = 1,
                WasLoaded = true
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusCamera
            {
                Fov = 2000,
                ProjectionMethod = Fusee.Serialization.V1.ProjectionMethod.Orthographic,
                ClippingPlanes = new float2(0, 500)
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(new FusMaterialPBR
            {
                FresnelReflectance = 100,
                DiffuseFraction = 200,
                RoughnessValue = 1
            });

            ((FusScene)scene.Contents).Children[0].AddComponent(daMesh);

            #endregion

            #region Arm01

            ((FusScene)scene.Contents).Children[0].AddNode(new FusNode
            {
                Name = "Arm01"
            });

            ((FusScene)scene.Contents).Children[0].Children[0].AddComponent(new FusTransform
            {
                Translation = new float3(0, 60, 0),
                Scale = new float3(20, 100, 20)
            });

            ((FusScene)scene.Contents).Children[0].Children[0].AddComponent(new FusMaterial
            {
                Albedo = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Green) },
                Specular = new SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
            });

            ((FusScene)scene.Contents).Children[0].Children[0].AddComponent(daMesh);

            #endregion

            #region Arm02

            ((FusScene)scene.Contents).Children[0].Children[0].AddNode(new FusNode
            {
                Name = "Arm02Rot"
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].AddComponent(new FusBone
            {
                Name = "MyBone2"
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].AddComponent(new FusTransform
            {
                Translation = new float3(-20, 40, 0),
                Rotation = new float3(0.35f, 0, 0),
                Scale = float3.One
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].AddNode(new FusNode
            {
                Name = "Arm02"
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].AddComponent(new FusTransform
            {
                Translation = new float3(0, 40, 0),
                Scale = new float3(20, 100, 20)
            });


            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].AddComponent(new FusMaterial
            {
                Albedo = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Yellow) },
                Specular = new SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
            });


            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].AddComponent(daMesh);

            #endregion

            #region Arm03

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].AddNode(new FusNode
            {
                Name = "Arm03Rot"
            });


            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].AddComponent(new FusTransform
            {
                Translation = new float3(20, 40, 0),
                Rotation = new float3(0.25f, 0, 0),
                Scale = float3.One
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].AddNode(new FusNode
            {
                Name = "Arm03"
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].AddComponent(new FusTransform
            {
                Translation = new float3(0, 40, 0),
                Scale = new float3(20, 100, 20)
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].AddComponent(new FusMaterial
            {
                Albedo = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Blue) },
                Specular = new SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].AddComponent(daMesh);

            #endregion

            var groundTruth = await SceneShouldGT();
            var gtFlattened = new List<Xene.IComponent>();
            FlattenScene(gtFlattened, groundTruth.Children[0]);

            var GTConvertedToFusFile = FusSceneConverter.ConvertTo(await SceneShouldGT());
            var fusFileFlattened = new List<Xene.IComponent>();
            FlattenScene(fusFileFlattened, ((FusScene)GTConvertedToFusFile.Contents).Children[0]);

            Assert.Equal(fusFileFlattened.Count, gtFlattened.Count);

            for (var i = 0; i < gtFlattened.Count; i++)
            {
                var gtComp = gtFlattened[i];
                var fusFileComp = fusFileFlattened[i];

                if (gtComp is Transform t)
                {
                    Assert.Equal((t).Name, ((FusTransform)fusFileComp).Name);
                    Assert.Equal((t).Rotation, ((FusTransform)fusFileComp).Rotation);
                    Assert.Equal((t).Scale, ((FusTransform)fusFileComp).Scale);
                    Assert.Equal((t).Translation, ((FusTransform)fusFileComp).Translation);
                }

                if (gtComp is Bone bone)
                {
                    Assert.Equal((bone).Name, ((FusBone)fusFileComp).Name);
                }

                if (gtComp is Camera camera)
                {
                    Assert.Equal(camera.Name, ((FusCamera)fusFileComp).Name);
                    Assert.Equal(camera.Layer, ((FusCamera)fusFileComp).Layer);
                    Assert.Equal(camera.ProjectionMethod, (((FusCamera)fusFileComp).ProjectionMethod == Fusee.Serialization.V1.ProjectionMethod.Orthographic ?
                        Engine.Core.Scene.ProjectionMethod.Orthographic : Engine.Core.Scene.ProjectionMethod.Perspective));
                    Assert.Equal(camera.Viewport, ((FusCamera)fusFileComp).Viewport);
                    Assert.Equal(camera.Fov, ((FusCamera)fusFileComp).Fov);
                    Assert.Equal(camera.BackgroundColor, ((FusCamera)fusFileComp).BackgroundColor);
                    Assert.Equal(camera.ClearColor, ((FusCamera)fusFileComp).ClearColor);
                    Assert.Equal(camera.ClearDepth, ((FusCamera)fusFileComp).ClearDepth);
                    Assert.Equal(camera.ClippingPlanes, ((FusCamera)fusFileComp).ClippingPlanes);
                }

                if (gtComp is Light light)
                {
                    Assert.Equal(light.Name, ((FusLight)fusFileComp).Name);
                    Assert.Equal(light.Bias, ((FusLight)fusFileComp).Bias);
                    Assert.Equal(light.Color, ((FusLight)fusFileComp).Color);
                    Assert.Equal(light.InnerConeAngle, ((FusLight)fusFileComp).InnerConeAngle);
                    Assert.Equal(light.IsCastingShadows, ((FusLight)fusFileComp).IsCastingShadows);
                    Assert.Equal(light.MaxDistance, ((FusLight)fusFileComp).MaxDistance);
                    Assert.Equal(light.OuterConeAngle, ((FusLight)fusFileComp).OuterConeAngle);
                    Assert.Equal(light.Strength, ((FusLight)fusFileComp).Strength);
                    Assert.Equal(light.Type.ToString(), ((FusLight)fusFileComp).Type.ToString());
                }

                if (gtComp is ShaderEffect fx)
                {
                    Assert.Equal(fx.Name, ((FusMaterial)fusFileComp).Name);
                    if (fx.GetEffectParam(UniformNameDeclarations.NormalMapIntensity) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.NormalMapIntensity), ((FusMaterial)fusFileComp).NormalMap.Intensity);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.NormalMap), ((FusMaterial)fusFileComp).NormalMap.Texture);
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.AlbedoColor) != null)
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoColor), ((FusMaterial)fusFileComp).Albedo.Color);

                    if (fx.GetEffectParam(UniformNameDeclarations.AlbedoMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoMix), ((FusMaterial)fusFileComp).Albedo.Mix);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoTexture), ((FusMaterial)fusFileComp).Albedo.Texture);
                    }

                  
                    if (fx.GetEffectParam(UniformNameDeclarations.SpecularMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularMix), ((FusMaterial)fusFileComp).Specular.Mix);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularTexture), ((FusMaterial)fusFileComp).Specular.Texture);
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.SpecularColor) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularColor), ((FusMaterial)fusFileComp).Specular.Color);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularShininess), ((FusMaterial)fusFileComp).Specular.Shininess);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularIntensity), ((FusMaterial)fusFileComp).Specular.Intensity);
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.EmissiveColor) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveColor), ((FusMaterial)fusFileComp).Emissive.Color);
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.EmissiveMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveMix), ((FusMaterial)fusFileComp).Emissive.Mix);
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveTexture), ((FusMaterial)fusFileComp).Emissive.Texture);
                    }
                }

                if (gtComp is Mesh mesh)
                {
                    Assert.Equal(mesh.Name, ((FusMesh)fusFileComp).Name);
                    Assert.Equal(mesh.BoundingBox, ((FusMesh)fusFileComp).BoundingBox);
                    Assert.Equal(mesh.Colors, ((FusMesh)fusFileComp).Colors);
                    Assert.Equal(mesh.Vertices, ((FusMesh)fusFileComp).Vertices);
                    Assert.Equal(mesh.Triangles, ((FusMesh)fusFileComp).Triangles);
                    Assert.Equal(mesh.UVs, ((FusMesh)fusFileComp).UVs);
                    Assert.Equal(mesh.MeshType.ToString(), ((FusMesh)fusFileComp).MeshType.ToString());
                    Assert.Equal(mesh.Tangents, ((FusMesh)fusFileComp).Tangents);
                    Assert.Equal(mesh.BiTangents, ((FusMesh)fusFileComp).BiTangents);
                }

                if (gtComp is Octant octant)
                {
                    Assert.Equal(octant.Name, ((FusOctant)fusFileComp).Name);
                    Assert.Equal(octant.Center, ((FusOctant)fusFileComp).Center);
                    Assert.Equal(octant.Guid, ((FusOctant)fusFileComp).Guid);
                    Assert.Equal(octant.IsLeaf, ((FusOctant)fusFileComp).IsLeaf);
                    Assert.Equal(octant.Level, ((FusOctant)fusFileComp).Level);
                    Assert.Equal(octant.NumberOfPointsInNode, ((FusOctant)fusFileComp).NumberOfPointsInNode);
                    Assert.Equal(octant.PosInHierarchyTex, ((FusOctant)fusFileComp).PosInHierarchyTex);
                    Assert.Equal(octant.PosInParent, ((FusOctant)fusFileComp).PosInParent);
                    Assert.Equal(octant.Size, ((FusOctant)fusFileComp).Size);
                    Assert.Equal(octant.VisibleChildIndices, ((FusOctant)fusFileComp).VisibleChildIndices);
                    Assert.Equal(octant.WasLoaded, ((FusOctant)fusFileComp).WasLoaded);
                }

                if (gtComp is Weight weight)
                {
                    Assert.Equal(weight.Name, ((FusWeight)fusFileComp).Name);
                    Assert.Equal(weight.BindingMatrices, ((FusWeight)fusFileComp).BindingMatrices);

                    for (var j = 0; j < weight.Joints.Count; j++)
                    {
                        Assert.Equal(weight.Joints[j].Name, ((FusWeight)fusFileComp).Joints[j].Name);
                    }

                    for (var k = 0; k < weight.WeightMap.Count; k++)
                    {
                        for (var l = 0; l < weight.WeightMap[k].VertexWeights.Count; l++)
                        {
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].JointIndex, ((FusWeight)fusFileComp).WeightMap[k].VertexWeights[l].JointIndex);
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].Weight, ((FusWeight)fusFileComp).WeightMap[k].VertexWeights[l].Weight);

                        }
                    }
                }

                if (gtComp is RectTransform rt)
                {
                    Assert.Equal(rt.Name, ((FusRectTransform)fusFileComp).Name);
                    Assert.Equal(rt.Offsets.Min, ((FusRectTransform)fusFileComp).Offsets.Min);
                    Assert.Equal(rt.Offsets.Max, ((FusRectTransform)fusFileComp).Offsets.Max);
                    Assert.Equal(rt.Anchors.Min, ((FusRectTransform)fusFileComp).Anchors.Min);
                    Assert.Equal(rt.Anchors.Max, ((FusRectTransform)fusFileComp).Anchors.Max);
                }

                if (gtComp is XForm xf)
                {
                    Assert.Equal(xf.Name, ((FusXForm)fusFileComp).Name);
                }

                if (gtComp is XFormText xft)
                {
                    Assert.Equal(xft.Name, ((FusXFormText)fusFileComp).Name);
                    Assert.Equal(xft.Height, ((FusXFormText)fusFileComp).Height);
                    Assert.Equal(xft.Width, ((FusXFormText)fusFileComp).Width);
                    Assert.Equal(xft.HorizontalAlignment.ToString(), ((FusXFormText)fusFileComp).HorizontalAlignment.ToString());
                    Assert.Equal(xft.VerticalAlignment.ToString(), ((FusXFormText)fusFileComp).VerticalAlignment.ToString());
                }

                if (gtComp is CanvasTransform ct)
                {
                    Assert.Equal(ct.Name, ((FusCanvasTransform)fusFileComp).Name);
                    Assert.Equal(ct.Scale, ((FusCanvasTransform)fusFileComp).Scale);
                    Assert.Equal(ct.ScreenSpaceSize, ((FusCanvasTransform)fusFileComp).ScreenSpaceSize);
                    Assert.Equal(ct.Size, ((FusCanvasTransform)fusFileComp).Size);
                    Assert.Equal(ct.CanvasRenderMode.ToString(), ((FusCanvasTransform)fusFileComp).CanvasRenderMode.ToString());

                }
            }

            // now we are sure our created fus file is correct, so we can deserialize it and test those methods
            var FusFileConvertedToGT = FusSceneConverter.ConvertFrom(GTConvertedToFusFile);
            var sceneFileFlattenedAgain = new List<Xene.IComponent>();
            FlattenScene(sceneFileFlattenedAgain, (FusFileConvertedToGT.Children[0]));

            Assert.Equal(sceneFileFlattenedAgain.Count, gtFlattened.Count);

            // check against gt, they should be equal in every manner (expect mesh!)
            for (var i = 0; i < sceneFileFlattenedAgain.Count; i++)
            {
                var gtComp = gtFlattened[i];
                var sceneFileComp = sceneFileFlattenedAgain[i];

                if (gtComp is Transform t)
                {
                    Assert.Equal((t).Name, ((Transform)sceneFileComp).Name);
                    Assert.Equal((t).Rotation, ((Transform)sceneFileComp).Rotation);
                    Assert.Equal((t).Scale, ((Transform)sceneFileComp).Scale);
                    Assert.Equal((t).Translation, ((Transform)sceneFileComp).Translation);
                }

                if (gtComp is Bone bone)
                {
                    Assert.Equal((bone).Name, ((Bone)sceneFileComp).Name);
                }

                if (gtComp is Camera camera)
                {
                    Assert.Equal(camera.Name, ((Camera)sceneFileComp).Name);
                    Assert.Equal(camera.Layer, ((Camera)sceneFileComp).Layer);
                    Assert.Equal(camera.ProjectionMethod.ToString(), ((Camera)sceneFileComp).ProjectionMethod.ToString());
                    Assert.Equal(camera.Viewport, ((Camera)sceneFileComp).Viewport);
                    Assert.Equal(camera.Fov, ((Camera)sceneFileComp).Fov);
                    Assert.Equal(camera.BackgroundColor, ((Camera)sceneFileComp).BackgroundColor);
                    Assert.Equal(camera.ClearColor, ((Camera)sceneFileComp).ClearColor);
                    Assert.Equal(camera.ClearDepth, ((Camera)sceneFileComp).ClearDepth);
                    Assert.Equal(camera.ClippingPlanes, ((Camera)sceneFileComp).ClippingPlanes);
                }

                if (gtComp is Light light)
                {
                    Assert.Equal(light.Name, ((Light)sceneFileComp).Name);
                    Assert.Equal(light.Bias, ((Light)sceneFileComp).Bias);
                    Assert.Equal(light.Color, ((Light)sceneFileComp).Color);
                    Assert.Equal(light.InnerConeAngle, ((Light)sceneFileComp).InnerConeAngle);
                    Assert.Equal(light.IsCastingShadows, ((Light)sceneFileComp).IsCastingShadows);
                    Assert.Equal(light.MaxDistance, ((Light)sceneFileComp).MaxDistance);
                    Assert.Equal(light.OuterConeAngle, ((Light)sceneFileComp).OuterConeAngle);
                    Assert.Equal(light.Strength, ((Light)sceneFileComp).Strength);
                    Assert.Equal(light.Type.ToString(), ((Light)sceneFileComp).Type.ToString());
                }

                if (gtComp is ShaderEffect fx)
                {     
                    // HACK (mr): Problem with null vs string comparison. Should be re-enabled after <nullable> is enabled for F.E.Core & Serialization
                    //Assert.Equal(fx.Name, ((ShaderEffect)sceneFileComp).Name);

                    if (fx.GetEffectParam(UniformNameDeclarations.NormalMapIntensity) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.NormalMapIntensity), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.NormalMapIntensity));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.NormalMap), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.NormalMap));

                    }

                    Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoColor), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.AlbedoColor));

                    if (fx.GetEffectParam(UniformNameDeclarations.AlbedoMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoMix), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.AlbedoMix));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.AlbedoTexture), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.AlbedoTexture));
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.SpecularColor) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularColor), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.SpecularColor));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularShininess), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.SpecularShininess));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularIntensity), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.SpecularIntensity));
                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.SpecularMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularMix), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.SpecularMix));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.SpecularTexture), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.SpecularTexture));
                    }

              
                    if (fx.GetEffectParam(UniformNameDeclarations.EmissiveColor) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveColor), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.EmissiveColor));

                    }

                    if (fx.GetEffectParam(UniformNameDeclarations.EmissiveMix) != null)
                    {
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveMix), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.EmissiveMix));
                        Assert.Equal(fx.GetEffectParam(UniformNameDeclarations.EmissiveTexture), ((ShaderEffect)sceneFileComp).GetEffectParam(UniformNameDeclarations.EmissiveTexture));
                    }
                }

                if (gtComp is Mesh mesh)
                {
                    Assert.Equal(mesh.Name, ((Mesh)sceneFileComp).Name);
                    Assert.Equal(mesh.BoundingBox, ((Mesh)sceneFileComp).BoundingBox);
                    Assert.Equal(mesh.Colors, ((Mesh)sceneFileComp).Colors);
                    Assert.Equal(mesh.Vertices, ((Mesh)sceneFileComp).Vertices);
                    Assert.Equal(mesh.Triangles, ((Mesh)sceneFileComp).Triangles);
                    Assert.Equal(mesh.UVs, ((Mesh)sceneFileComp).UVs);
                    Assert.Equal(mesh.MeshType.ToString(), ((Mesh)sceneFileComp).MeshType.ToString());
                    Assert.Equal(mesh.Tangents, ((Mesh)sceneFileComp).Tangents);
                    Assert.Equal(mesh.BiTangents, ((Mesh)sceneFileComp).BiTangents);
                }

                if (gtComp is Octant octant)
                {
                    Assert.Equal(octant.Name, ((Octant)sceneFileComp).Name);
                    Assert.Equal(octant.Center, ((Octant)sceneFileComp).Center);
                    Assert.Equal(octant.Guid, ((Octant)sceneFileComp).Guid);
                    Assert.Equal(octant.IsLeaf, ((Octant)sceneFileComp).IsLeaf);
                    Assert.Equal(octant.Level, ((Octant)sceneFileComp).Level);
                    Assert.Equal(octant.NumberOfPointsInNode, ((Octant)sceneFileComp).NumberOfPointsInNode);
                    Assert.Equal(octant.PosInHierarchyTex, ((Octant)sceneFileComp).PosInHierarchyTex);
                    Assert.Equal(octant.PosInParent, ((Octant)sceneFileComp).PosInParent);
                    Assert.Equal(octant.Size, ((Octant)sceneFileComp).Size);
                    Assert.Equal(octant.VisibleChildIndices, ((Octant)sceneFileComp).VisibleChildIndices);
                    Assert.Equal(octant.WasLoaded, ((Octant)sceneFileComp).WasLoaded);
                }

                if (gtComp is Weight weight)
                {
                    Assert.Equal(weight.Name, ((Weight)sceneFileComp).Name);
                    Assert.Equal(weight.BindingMatrices, ((Weight)sceneFileComp).BindingMatrices);

                    for (var j = 0; j < weight.Joints.Count; j++)
                    {
                        Assert.Equal(weight.Joints[j].Name, ((Weight)sceneFileComp).Joints[j].Name);
                    }

                    for (var k = 0; k < weight.WeightMap.Count; k++)
                    {
                        for (var l = 0; l < weight.WeightMap[k].VertexWeights.Count; l++)
                        {
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].JointIndex, ((Weight)sceneFileComp).WeightMap[k].VertexWeights[l].JointIndex);
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].Weight, ((Weight)sceneFileComp).WeightMap[k].VertexWeights[l].Weight);

                        }
                    }
                }

                if (gtComp is RectTransform rt)
                {
                    Assert.Equal(rt.Name, ((RectTransform)sceneFileComp).Name);
                    Assert.Equal(rt.Offsets.Min, ((RectTransform)sceneFileComp).Offsets.Min);
                    Assert.Equal(rt.Offsets.Max, ((RectTransform)sceneFileComp).Offsets.Max);
                    Assert.Equal(rt.Anchors.Min, ((RectTransform)sceneFileComp).Anchors.Min);
                    Assert.Equal(rt.Anchors.Max, ((RectTransform)sceneFileComp).Anchors.Max);
                }

                if (gtComp is XForm xf)
                {
                    Assert.Equal(xf.Name, ((XForm)sceneFileComp).Name);
                }

                if (gtComp is XFormText xft)
                {
                    Assert.Equal(xft.Name, ((XFormText)sceneFileComp).Name);
                    Assert.Equal(xft.Height, ((XFormText)sceneFileComp).Height);
                    Assert.Equal(xft.Width, ((XFormText)sceneFileComp).Width);
                    Assert.Equal(xft.HorizontalAlignment.ToString(), ((XFormText)sceneFileComp).HorizontalAlignment.ToString());
                    Assert.Equal(xft.VerticalAlignment.ToString(), ((XFormText)sceneFileComp).VerticalAlignment.ToString());
                }

                if (gtComp is CanvasTransform ct)
                {
                    Assert.Equal(ct.Name, ((CanvasTransform)sceneFileComp).Name);
                    Assert.Equal(ct.Scale, ((CanvasTransform)sceneFileComp).Scale);
                    Assert.Equal(ct.ScreenSpaceSize, ((CanvasTransform)sceneFileComp).ScreenSpaceSize);
                    Assert.Equal(ct.Size, ((CanvasTransform)sceneFileComp).Size);
                    Assert.Equal(ct.CanvasRenderMode.ToString(), ((CanvasTransform)sceneFileComp).CanvasRenderMode.ToString());
                }
            }
        }

        private static void FlattenScene(List<Xene.IComponent> components, Xene.INode scene)
        {
            components.AddRange(scene.EnumComponents.ToList());

            if (scene.EnumChildren == null) return;

            foreach (var c in scene.EnumChildren)
            {
                if (c != null)
                    FlattenScene(components, c);
            }
        }

        public static async Task<SceneContainer> SceneShouldGT()
        {
            return new SceneContainer
            {
                Header = new SceneHeader
                {
                    CreationDate = DateTime.Today.ToString(),
                    Generator = "SerializationTest",
                    CreatedBy = "Fusee Test Code"
                },

                Children = new List<SceneNode>
                {
                new SceneNode
                {
                    Name = "Base",
                    Components = new List<SceneComponent>
                    {
                       new CanvasTransform(Engine.Core.Scene.CanvasRenderMode.SCREEN)
                       {
                           Name = "CanvasTransform",
                           Scale = float2.One * 2,
                           ScreenSpaceSize = new MinMaxRect
                           {
                               Max = float2.One * 22,
                               Min = float2.One * -1

                           },
                           Size = new MinMaxRect
                           {
                               Min = float2.One * 22,
                               Max = float2.One * -1

                           }
                       },
                       new XFormText
                        {
                            Name = "XFormText",
                            Height = 10,
                            HorizontalAlignment = Engine.Core.Scene.HorizontalTextAlignment.CENTER,
                            VerticalAlignment = Engine.Core.Scene.VerticalTextAlignment.TOP,
                            Width = 200
                        },
                       new XForm
                        {
                            Name = "XForm"
                        },
                       new RectTransform
                       {
                           Anchors =  new MinMaxRect
                           {
                               Max = float2.Zero,
                               Min = float2.One
                           },
                           Name = "Rect",
                           Offsets = new MinMaxRect
                           {
                                Max = float2.Zero,
                               Min = float2.One
                           }
                       },
                       new Transform { Scale = new float3(100, 20, 100) },
                       new Bone
                       {
                           Name = "MyBone"
                       },
                       new Weight
                       {
                           BindingMatrices = new List<float4x4>(),
                           Joints = new List<SceneNode>(),
                           Name = "MyWeight",
                           WeightMap = new List<Engine.Core.Scene.VertexWeightList>
                           {
                               new Engine.Core.Scene.VertexWeightList
                               {
                                   VertexWeights = new List<Engine.Core.Scene.VertexWeight>
                                   {
                                       new Engine.Core.Scene.VertexWeight
                                       {
                                           Weight = 20,
                                           JointIndex = 0
                                       },
                                        new Engine.Core.Scene.VertexWeight
                                       {
                                           Weight = 30,
                                           JointIndex = 1
                                       },
                                   }
                               }
                           }
                       },
                       await ShaderCodeBuilder.MakeShaderEffectProto(
                           albedoColor: ColorUint.Tofloat4(ColorUint.Red),
                           specularColor: ColorUint.Tofloat4(ColorUint.White),
                           shininess: 4.0f,
                           specularIntensity: 1.0f),

                       new Light
                       {
                           Name = "MyLight",
                           Bias = 0,
                           MaxDistance = 100,
                           Active = true,
                           Color = float4.One,
                           InnerConeAngle = 20,
                           IsCastingShadows = true,
                           OuterConeAngle = 20,
                           Strength = 100,
                           Type = LightType.Point

                       },
                       new Octant
                       {
                           Center = double3.One,
                           Guid = new Guid(1, 2, 3, new byte[] { 4, 4, 4, 4, 4, 4, 4, 4 }),
                           IsLeaf = false,
                           Level = 10,
                           Name = "MyOctant",
                           NumberOfPointsInNode = 2,
                           PosInHierarchyTex = 0,
                           PosInParent = 5,
                           Size = 20,
                           VisibleChildIndices = 1,
                           WasLoaded = true
                       },
                       new Camera(Engine.Core.Scene.ProjectionMethod.Orthographic, 0, 500, 2000),
                       await ShaderCodeBuilder.MakeShaderEffectFromShaderEffectPropsProto(new Engine.Core.ShaderShards.ShaderEffectProps
                       {
                           MatValues =
                           {
                                FresnelReflectance = 100,
                                DiffuseFraction = 200,
                                RoughnessValue = 1
                           }
                       }),
                       new Cube()
                    },
                    Children = new ChildList
                    {
                        new SceneNode
                        {
                            Name = "Arm01",
                            Components = new List<SceneComponent>
                            {
                                new Transform {Translation=new float3(0, 60, 0),  Scale = new float3(20, 100, 20) },
                                await ShaderCodeBuilder.MakeShaderEffectProto(
                                    albedoColor: ColorUint.Tofloat4(ColorUint.Green),
                                    specularColor: ColorUint.Tofloat4(ColorUint.White),
                                    specularIntensity: 1.0f,
                                    shininess: 4.0f),
                                new Cube()
                            },
                            Children = new ChildList
                            {
                                new SceneNode
                                {
                                    Name = "Arm02Rot",
                                    Components = new List<SceneComponent>
                                    {
                                        new Bone
                                        {
                                            Name = "MyBone2"
                                        },
                                        new Transform {Translation=new float3(-20, 40, 0),  Rotation = new float3(0.35f, 0, 0), Scale = float3.One},
                                    },
                                    Children = new ChildList
                                    {
                                        new SceneNode
                                        {
                                            Name = "Arm02",
                                            Components = new List<SceneComponent>
                                            {
                                                new Transform {Translation=new float3(0, 40, 0),  Scale = new float3(20, 100, 20) },
                                                await ShaderCodeBuilder.MakeShaderEffectProto(
                                                    albedoColor: ColorUint.Tofloat4(ColorUint.Yellow),
                                                    specularColor: ColorUint.Tofloat4(ColorUint.White),
                                                    specularIntensity: 1.0f,
                                                    shininess: 4.0f),
                                                new Cube()
                                            },
                                            Children = new ChildList
                                            {
                                                new SceneNode
                                                {
                                                    Name = "Arm03Rot",
                                                    Components = new List<SceneComponent>
                                                    {
                                                        new Transform {Translation=new float3(20, 40, 0),  Rotation = new float3(0.25f, 0, 0), Scale = float3.One},
                                                    },
                                                    Children = new ChildList
                                                    {
                                                        new SceneNode
                                                        {
                                                            Name = "Arm03",
                                                            Components = new List<SceneComponent>
                                                            {
                                                                new Transform {Translation=new float3(0, 40, 0),  Scale = new float3(20, 100, 20) },
                                                                await ShaderCodeBuilder.MakeShaderEffectProto(
                                                                                    albedoColor: ColorUint.Tofloat4(ColorUint.Blue),
                                                                                    specularColor: ColorUint.Tofloat4(ColorUint.White),
                                                                                    specularIntensity: 1.0f,
                                                                                    shininess: 4.0f),
                                                                new Cube()
                                                            }
                                                        },
                                                    }
                                                }
                                            }
                                        },
                                    }
                                }
                            }
                        },
                    }
                },
            }
            };
        }
    }
}
