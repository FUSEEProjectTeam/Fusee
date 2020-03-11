using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization.V1;
using System;
using System.Collections.Generic;
using System.Text;
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
                Guid = Guid.NewGuid(),
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

            var convertedScene = new ConvertSceneGraphV1().Convert(scene);

            // TODO: Here we need a sophisticated test against method
            Assert.Equal(SceneShould(), convertedScene);

        }


        public static Scene SceneShould()
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
                       new Material
                       {
                            Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Red) },
                            Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                        },
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
                           Guid = Guid.NewGuid(),
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
                       new Camera(Engine.Core.ProjectionMethod.ORTHOGRAPHIC, 0, 500, 2000),
                       new MaterialPBR
                       {
                           FresnelReflectance = 100,
                           DiffuseFraction = 200,
                           RoughnessValue = 1
                       },
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
                                new Material
                                {
                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Green) },
                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                },
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
                                                new Material
                                                {
                                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Yellow) },
                                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                },
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
                                                                new Material
                                                                {
                                                                    Diffuse = new MatChannel { Color = ColorUint.Tofloat4(ColorUint.Blue) },
                                                                    Specular = new SpecularChannel {Color = ColorUint.Tofloat4(ColorUint.White), Intensity = 1.0f, Shininess = 4.0f}
                                                                },
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
