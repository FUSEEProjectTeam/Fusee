using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fusee.Serialization.Test
{
    public class SimpleConvertSceneGraphV1
    {
        [Fact]
        public void V1_SimpleScene_Convert()
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
                WeightMap = new List<V1.VertexWeightList>
                           {
                               new V1.VertexWeightList
                               {
                                   VertexWeights = new List<V1.VertexWeight>
                                   {
                                       new V1.VertexWeight
                                       {
                                           Weight = 20,
                                           JointIndex = 0
                                       },
                                        new V1.VertexWeight
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
                Diffuse = new V1.MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Red) },
                Specular = new V1.SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
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
                ProjectionMethod = V1.ProjectionMethod.ORTHOGRAPHIC,
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
                Diffuse = new V1.MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Green) },
                Specular = new V1.SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
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
                Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Yellow) },
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
                Diffuse = new MatChannelContainer { Color = ColorUint.Tofloat4(ColorUint.Blue) },
                Specular = new SpecularChannelContainer { Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f }
            });

            ((FusScene)scene.Contents).Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].AddComponent(daMesh);

            #endregion


            var groundTruth = SceneShouldGT();
            var gtFlattened = new List<Xene.IComponent>();
            FlattenScene(gtFlattened, groundTruth.Children[0]);

            var GTConvertedToFusFile = FusSceneConverter.ConvertTo(SceneShouldGT());
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
                    Assert.Equal(camera.ProjectionMethod, (((FusCamera)fusFileComp).ProjectionMethod == V1.ProjectionMethod.ORTHOGRAPHIC ?
                        Engine.Common.ProjectionMethod.ORTHOGRAPHIC : Engine.Common.ProjectionMethod.PERSPECTIVE));
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

                if (gtComp is Material material)
                {
                    Assert.Equal(material.Name, ((FusMaterial)fusFileComp).Name);
                    Assert.Equal(material.Bump.Intensity, ((FusMaterial)fusFileComp).Bump.Intensity);
                    Assert.Equal(material.Bump.Texture, ((FusMaterial)fusFileComp).Bump.Texture);
                    
                    Assert.Equal(material.Diffuse.Color, ((FusMaterial)fusFileComp).Diffuse.Color);
                    Assert.Equal(material.Diffuse.Mix, ((FusMaterial)fusFileComp).Diffuse.Mix);
                    Assert.Equal(material.Diffuse.Texture, ((FusMaterial)fusFileComp).Diffuse.Texture);

                    Assert.Equal(material.Specular.Color, ((FusMaterial)fusFileComp).Specular.Color);
                    Assert.Equal(material.Specular.Mix, ((FusMaterial)fusFileComp).Specular.Mix);
                    Assert.Equal(material.Specular.Texture, ((FusMaterial)fusFileComp).Specular.Texture);
                    Assert.Equal(material.Specular.Shininess, ((FusMaterial)fusFileComp).Specular.Shininess);
                    Assert.Equal(material.Specular.Intensity, ((FusMaterial)fusFileComp).Specular.Intensity);


                    Assert.Equal(material.Emissive.Color, ((FusMaterial)fusFileComp).Emissive.Color);
                    Assert.Equal(material.Emissive.Mix, ((FusMaterial)fusFileComp).Emissive.Mix);
                    Assert.Equal(material.Emissive.Texture, ((FusMaterial)fusFileComp).Emissive.Texture);
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
                        for(var l = 0; l < weight.WeightMap[k].VertexWeights.Count; l++)
                        {
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].JointIndex, ((FusWeight)fusFileComp).WeightMap[k].VertexWeights[l].JointIndex);
                            Assert.Equal(weight.WeightMap[k].VertexWeights[l].Weight, ((FusWeight)fusFileComp).WeightMap[k].VertexWeights[l].Weight);

                        }
                    }
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

                if (gtComp is Material material)
                {
                    Assert.Equal(material.Name, ((Material)sceneFileComp).Name);
                    Assert.Equal(material.Bump.Intensity, ((Material)sceneFileComp).Bump.Intensity);
                    Assert.Equal(material.Bump.Texture, ((Material)sceneFileComp).Bump.Texture);

                    Assert.Equal(material.Diffuse.Color, ((Material)sceneFileComp).Diffuse.Color);
                    Assert.Equal(material.Diffuse.Mix, ((Material)sceneFileComp).Diffuse.Mix);
                    Assert.Equal(material.Diffuse.Texture, ((Material)sceneFileComp).Diffuse.Texture);

                    Assert.Equal(material.Specular.Color, ((Material)sceneFileComp).Specular.Color);
                    Assert.Equal(material.Specular.Mix, ((Material)sceneFileComp).Specular.Mix);
                    Assert.Equal(material.Specular.Texture, ((Material)sceneFileComp).Specular.Texture);
                    Assert.Equal(material.Specular.Shininess, ((Material)sceneFileComp).Specular.Shininess);
                    Assert.Equal(material.Specular.Intensity, ((Material)sceneFileComp).Specular.Intensity);


                    Assert.Equal(material.Emissive.Color, ((Material)sceneFileComp).Emissive.Color);
                    Assert.Equal(material.Emissive.Mix, ((Material)sceneFileComp).Emissive.Mix);
                    Assert.Equal(material.Emissive.Texture, ((Material)sceneFileComp).Emissive.Texture);
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

        public static Scene SceneShouldGT()
        {
            return new Scene
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
                           WeightMap = new List<Engine.Common.VertexWeightList>
                           {
                               new Engine.Common.VertexWeightList
                               {
                                   VertexWeights = new List<Engine.Common.VertexWeight>
                                   {
                                       new Engine.Common.VertexWeight
                                       {
                                           Weight = 20,
                                           JointIndex = 0
                                       },
                                        new Engine.Common.VertexWeight
                                       {
                                           Weight = 30,
                                           JointIndex = 1
                                       },
                                   }
                               }
                           }
                       },
                       ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(new Material
                       {
                            Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Red) },
                            Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                       }),
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
                       new Camera(Engine.Common.ProjectionMethod.ORTHOGRAPHIC, 0, 500, 2000),
                       ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(new MaterialPBR
                       {
                           FresnelReflectance = 100,
                           DiffuseFraction = 200,
                           RoughnessValue = 1
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
                                 ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(new Material
                                {
                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Green) },
                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                }),
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
                                                ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(new Material
                                                {
                                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Yellow) },
                                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                }),
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
                                                                ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(new Material
                                                                {
                                                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Blue) },
                                                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                                }),
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
