using Fusee.Base.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fusee.Tests.Serialization.V1
{
    public class SimpleConvertSceneGraphV1
    {
        [Fact]
        public void V1_SimpleScene_Convert()
        {
            var groundTruth = BuildGroundTruthSceneContainer();
            var gtFlattened = new List<Xene.IComponent>();
            FlattenScene(gtFlattened, groundTruth.Children[0]);

            var fusFileFromGroundTruth = FusSceneConverter.ConvertTo(BuildGroundTruthSceneContainer());
            var fusFileFlattened = new List<Xene.IComponent>();
            FlattenScene(fusFileFlattened, ((FusScene)fusFileFromGroundTruth.Contents).Children[0]);

            //Asserts: expected Scene Component values, actual: Serialization (Fus) Component values
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

                if (gtComp is SurfaceEffect fx)
                {
                    Assert.Equal(fx.Name, ((FusMaterialBase)fusFileComp).Name);

                    if (fx.SurfaceInput.ShadingModel == ShadingModel.DiffuseSpecular)
                    {
                        if (fx.SurfaceInput.ShadingModel != ShadingModel.Unlit)
                        {
                            Assert.Equal(fx.SurfaceInput.Albedo, ((FusMaterialStandard)fusFileComp).Albedo.Color);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.NormalMap))
                        {
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).NormalMappingStrength, ((FusMaterialStandard)fusFileComp).NormalMap?.Intensity);
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).NormalTex.PathAndName, ((FusMaterialStandard)fusFileComp).NormalMap.Texture);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.AlbedoTex))
                        {
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).AlbedoMix, ((FusMaterialStandard)fusFileComp).Albedo?.Mix);
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).AlbedoTex.PathAndName, ((FusMaterialStandard)fusFileComp).Albedo.Texture);
                        }
                        Assert.Equal(((SpecularInput)fx.SurfaceInput).Shininess, ((FusMaterialStandard)fusFileComp).Specular.Shininess);
                        Assert.Equal(((SpecularInput)fx.SurfaceInput).SpecularStrength, ((FusMaterialStandard)fusFileComp).Specular.Strength);

                    }
                    else if (fx.SurfaceInput.ShadingModel == ShadingModel.DiffuseOnly)
                    {
                        Assert.Equal(fx.SurfaceInput.Albedo, ((FusMaterialStandard)fusFileComp).Albedo.Color);
                    }
                    else if (fx.SurfaceInput.ShadingModel == ShadingModel.BRDF)
                    {
                        Assert.Equal(fx.SurfaceInput.Albedo, ((FusMaterialBRDF)fusFileComp).Albedo.Color);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).IOR, ((FusMaterialBRDF)fusFileComp).BRDF.IOR);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Metallic, ((FusMaterialBRDF)fusFileComp).BRDF.Metallic);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Roughness, ((FusMaterialBRDF)fusFileComp).BRDF.Roughness);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Specular, ((FusMaterialBRDF)fusFileComp).BRDF.Specular);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Subsurface, ((FusMaterialBRDF)fusFileComp).BRDF.Subsurface);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).SubsurfaceColor, ((FusMaterialBRDF)fusFileComp).BRDF.SubsurfaceColor);

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.NormalMap))
                        {
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).NormalMappingStrength, ((FusMaterialStandard)fusFileComp).NormalMap?.Intensity);
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).NormalTex.PathAndName, ((FusMaterialStandard)fusFileComp).NormalMap.Texture);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.AlbedoTex))
                        {
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).AlbedoMix, ((FusMaterialStandard)fusFileComp).Albedo?.Mix);
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).AlbedoTex.PathAndName, ((FusMaterialStandard)fusFileComp).Albedo.Texture);
                        }
                    }

                    #region NOT IMPLEMENTED
                    //if (fx.GetFxParam<float>(UniformNameDeclarations.SpecularMix) != null)
                    //{
                    //    Assert.Equal(fx.GetFxParam<float>(UniformNameDeclarations.SpecularMix), ((FusMaterialStandard)fusFileComp).Specular.Mix);
                    //    Assert.Equal(fx.GetFxParam<Texture>(UniformNameDeclarations.SpecularTexture).PathAndName, ((FusMaterialStandard)fusFileComp).Specular.Texture);
                    //}

                    //if (fx.GetFxParam<float4>(UniformNameDeclarations.EmissiveColor) != null)
                    //{
                    //    Assert.Equal(fx.GetFxParam<float4>(UniformNameDeclarations.EmissiveColor), ((FusMaterialStandard)fusFileComp).Emissive.Color);
                    //}

                    //if (fx.GetFxParam<float>(UniformNameDeclarations.EmissiveMix) != null)
                    //{
                    //    Assert.Equal(fx.GetFxParam<float>(UniformNameDeclarations.EmissiveMix), ((FusMaterialStandard)fusFileComp).Emissive.Mix);
                    //    Assert.Equal(fx.GetFxParam<Texture>(UniformNameDeclarations.EmissiveTexture).PathAndName, ((FusMaterialStandard)fusFileComp).Emissive.Texture);
                    //}
                    #endregion
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

                if (gtComp is OctantD octantD)
                {
                    Assert.Equal(octantD.Name, ((FusOctantD)fusFileComp).Name);
                    Assert.Equal(octantD.Center, ((FusOctantD)fusFileComp).Center);
                    Assert.Equal(octantD.Guid, ((FusOctantD)fusFileComp).Guid);
                    Assert.Equal(octantD.IsLeaf, ((FusOctantD)fusFileComp).IsLeaf);
                    Assert.Equal(octantD.Level, ((FusOctantD)fusFileComp).Level);
                    Assert.Equal(octantD.NumberOfPointsInNode, ((FusOctantD)fusFileComp).NumberOfPointsInNode);
                    Assert.Equal(octantD.PosInHierarchyTex, ((FusOctantD)fusFileComp).PosInHierarchyTex);
                    Assert.Equal(octantD.PosInParent, ((FusOctantD)fusFileComp).PosInParent);
                    Assert.Equal(octantD.Size, ((FusOctantD)fusFileComp).Size);
                    Assert.Equal(octantD.VisibleChildIndices, ((FusOctantD)fusFileComp).VisibleChildIndices);
                    Assert.Equal(octantD.WasLoaded, ((FusOctantD)fusFileComp).WasLoaded);
                }

                if (gtComp is OctantF octantF)
                {
                    Assert.Equal(octantF.Name, ((FusOctantF)fusFileComp).Name);
                    Assert.Equal(octantF.Center, ((FusOctantF)fusFileComp).Center);
                    Assert.Equal(octantF.Guid, ((FusOctantF)fusFileComp).Guid);
                    Assert.Equal(octantF.IsLeaf, ((FusOctantF)fusFileComp).IsLeaf);
                    Assert.Equal(octantF.Level, ((FusOctantF)fusFileComp).Level);
                    Assert.Equal(octantF.NumberOfPointsInNode, ((FusOctantF)fusFileComp).NumberOfPointsInNode);
                    Assert.Equal(octantF.PosInHierarchyTex, ((FusOctantF)fusFileComp).PosInHierarchyTex);
                    Assert.Equal(octantF.PosInParent, ((FusOctantF)fusFileComp).PosInParent);
                    Assert.Equal(octantF.Size, ((FusOctantF)fusFileComp).Size);
                    Assert.Equal(octantF.VisibleChildIndices, ((FusOctantF)fusFileComp).VisibleChildIndices);
                    Assert.Equal(octantF.WasLoaded, ((FusOctantF)fusFileComp).WasLoaded);
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

            // Now we are sure the converted FusFile resembles the ground truth SceneContainer. We can deserialize it to test those methods.
            var FusFileConvertedToGT = FusSceneConverter.ConvertFrom(fusFileFromGroundTruth, "Test.fus");
            var sceneFileFlattenedAgain = new List<Xene.IComponent>();
            FlattenScene(sceneFileFlattenedAgain, (FusFileConvertedToGT.Children[0]));

            Assert.Equal(sceneFileFlattenedAgain.Count, gtFlattened.Count);

            //Asserts: expected Scene Component values, actual: expected Scene Component values of deserialized FusScene
            // Check against gt, they should be equal in every manner (expect mesh!)
            for (var i = 0; i < sceneFileFlattenedAgain.Count; i++)
            {
                var gtComp = gtFlattened[i];
                var sceneFileComp = sceneFileFlattenedAgain[i];

                if (gtComp is Transform t)
                {
                    Assert.Equal(t.Name, ((Transform)sceneFileComp).Name);
                    Assert.Equal(t.Rotation, ((Transform)sceneFileComp).Rotation);
                    Assert.Equal(t.Scale, ((Transform)sceneFileComp).Scale);
                    Assert.Equal(t.Translation, ((Transform)sceneFileComp).Translation);
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

                if (gtComp is SurfaceEffect fx)
                {
                    if (fx.SurfaceInput.ShadingModel == ShadingModel.DiffuseSpecular)
                    {
                        if (fx.SurfaceInput.ShadingModel != ShadingModel.Unlit)
                        {
                            Assert.Equal(fx.SurfaceInput.Albedo, ((SurfaceEffect)sceneFileComp).SurfaceInput.Albedo);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.NormalMap))
                        {
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).NormalMappingStrength, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).NormalMappingStrength);
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).NormalTex.PathAndName, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).NormalTex.PathAndName);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.AlbedoTex))
                        {
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).AlbedoMix, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).AlbedoMix);
                            Assert.Equal(((SpecularInput)fx.SurfaceInput).AlbedoTex.PathAndName, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).AlbedoTex.PathAndName);
                        }
                        Assert.Equal(((SpecularInput)fx.SurfaceInput).Shininess, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Shininess);
                        Assert.Equal(((SpecularInput)fx.SurfaceInput).SpecularStrength, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).SpecularStrength);

                    }
                    else if (fx.SurfaceInput.ShadingModel == ShadingModel.DiffuseOnly)
                    {
                        Assert.Equal(fx.SurfaceInput.Albedo, ((SurfaceEffect)sceneFileComp).SurfaceInput.Albedo);
                    }
                    else if (fx.SurfaceInput.ShadingModel == ShadingModel.BRDF)
                    {
                        Assert.Equal(fx.SurfaceInput.Albedo, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Albedo);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).IOR, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).IOR);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Metallic, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Metallic);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Roughness, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Roughness);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Specular, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Specular);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).Subsurface, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).Subsurface);
                        Assert.Equal(((BRDFInput)fx.SurfaceInput).SubsurfaceColor, ((BRDFInput)((SurfaceEffect)sceneFileComp).SurfaceInput).SubsurfaceColor);

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.NormalMap))
                        {
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).NormalMappingStrength, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).NormalMappingStrength);
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).NormalTex.PathAndName, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).NormalTex.PathAndName);
                        }

                        if (fx.SurfaceInput.TextureSetup.HasFlag(TextureSetup.AlbedoTex))
                        {
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).AlbedoMix, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).AlbedoMix);
                            Assert.Equal(((BRDFInput)fx.SurfaceInput).AlbedoTex.PathAndName, ((SpecularInput)((SurfaceEffect)sceneFileComp).SurfaceInput).AlbedoTex.PathAndName);
                        }
                    }

                    #region NOT IMPLEMENTED
                    //Assert.Equal(fx.GetFxParam<float4>(UniformNameDeclarations.SpecularColor), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<float4>(UniformNameDeclarations.SpecularColor));
                    //Assert.Equal(fx.GetFxParam<float>(UniformNameDeclarations.SpecularMix), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<float>(UniformNameDeclarations.SpecularMix));
                    //Assert.Equal(fx.GetFxParam<Texture>(UniformNameDeclarations.SpecularTexture), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<Texture>(UniformNameDeclarations.SpecularTexture));

                    //Assert.Equal(fx.GetFxParam<float4>(UniformNameDeclarations.EmissiveColor), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<float4>(UniformNameDeclarations.EmissiveColor));

                    //Assert.Equal(fx.GetFxParam<float>(UniformNameDeclarations.EmissiveMix), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<float>(UniformNameDeclarations.EmissiveMix));
                    //Assert.Equal(fx.GetFxParam<Texture>(UniformNameDeclarations.EmissiveTexture), ((DefaultSurfaceEffect)sceneFileComp).GetFxParam<Texture>(UniformNameDeclarations.EmissiveTexture));
                    #endregion
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

                if (gtComp is OctantD octantD)
                {
                    Assert.Equal(octantD.Name, ((OctantD)sceneFileComp).Name);
                    Assert.Equal(octantD.Center, ((OctantD)sceneFileComp).Center);
                    Assert.Equal(octantD.Guid, ((OctantD)sceneFileComp).Guid);
                    Assert.Equal(octantD.IsLeaf, ((OctantD)sceneFileComp).IsLeaf);
                    Assert.Equal(octantD.Level, ((OctantD)sceneFileComp).Level);
                    Assert.Equal(octantD.NumberOfPointsInNode, ((OctantD)sceneFileComp).NumberOfPointsInNode);
                    Assert.Equal(octantD.PosInHierarchyTex, ((OctantD)sceneFileComp).PosInHierarchyTex);
                    Assert.Equal(octantD.PosInParent, ((OctantD)sceneFileComp).PosInParent);
                    Assert.Equal(octantD.Size, ((OctantD)sceneFileComp).Size);
                    Assert.Equal(octantD.VisibleChildIndices, ((OctantD)sceneFileComp).VisibleChildIndices);
                    Assert.Equal(octantD.WasLoaded, ((OctantD)sceneFileComp).WasLoaded);
                }

                if (gtComp is OctantF octantF)
                {
                    Assert.Equal(octantF.Name, ((OctantF)sceneFileComp).Name);
                    Assert.Equal(octantF.Center, ((OctantF)sceneFileComp).Center);
                    Assert.Equal(octantF.Guid, ((OctantF)sceneFileComp).Guid);
                    Assert.Equal(octantF.IsLeaf, ((OctantF)sceneFileComp).IsLeaf);
                    Assert.Equal(octantF.Level, ((OctantF)sceneFileComp).Level);
                    Assert.Equal(octantF.NumberOfPointsInNode, ((OctantF)sceneFileComp).NumberOfPointsInNode);
                    Assert.Equal(octantF.PosInHierarchyTex, ((OctantF)sceneFileComp).PosInHierarchyTex);
                    Assert.Equal(octantF.PosInParent, ((OctantF)sceneFileComp).PosInParent);
                    Assert.Equal(octantF.Size, ((OctantF)sceneFileComp).Size);
                    Assert.Equal(octantF.VisibleChildIndices, ((OctantF)sceneFileComp).VisibleChildIndices);
                    Assert.Equal(octantF.WasLoaded, ((OctantF)sceneFileComp).WasLoaded);
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

        public static SceneContainer BuildGroundTruthSceneContainer()
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
                       new CanvasTransform(Engine.Core.Scene.CanvasRenderMode.Screen)
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
                            HorizontalAlignment = Engine.Common.HorizontalTextAlignment.Center,
                            VerticalAlignment = Engine.Common.VerticalTextAlignment.Top,
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
                       MakeEffect.FromDiffuseSpecular(
                           albedoColor: (float4)ColorUint.Red,
                           emissionColor: float4.Zero,
                           shininess: 4.0f,
                           specularStrength: 1.0f),

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
                       new OctantD(double3.One, 20)
                       {
                            IsLeaf = false,
                            Level = 10,
                            PosInParent = 5,

                           Guid = new Guid(1, 2, 3, new byte[] { 4, 4, 4, 4, 4, 4, 4, 4 }),
                           Name = "MyOctantD",
                           NumberOfPointsInNode = 2,
                           PosInHierarchyTex = 0,
                           VisibleChildIndices = 1,
                           WasLoaded = true
                       },

                       new OctantF(float3.One, 20)
                       {
                            IsLeaf = false,
                            Level = 10,
                            PosInParent = 5,

                           Guid = new Guid(1, 2, 3, new byte[] { 4, 4, 4, 4, 4, 4, 4, 4 }),
                           Name = "MyOctantF",
                           NumberOfPointsInNode = 2,
                           PosInHierarchyTex = 0,
                           VisibleChildIndices = 1,
                           WasLoaded = true
                       },
                       new Camera(Engine.Core.Scene.ProjectionMethod.Orthographic, 0, 500, 2000),
                       MakeEffect.FromBRDF((float4)ColorUint.Green, 0.2f, 0, 0.5f, 1.46f),
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
                                MakeEffect.FromDiffuseSpecular(
                                    albedoColor: (float4)ColorUint.Green,
                                    emissionColor: float4.Zero,
                                    specularStrength: 1.0f,
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
                                                MakeEffect.FromDiffuseSpecular(
                                                    albedoColor: (float4)ColorUint.Yellow,
                                                    emissionColor: float4.Zero,
                                                    specularStrength: 1.0f,
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
                                                                MakeEffect.FromDiffuseSpecular(
                                                                                    albedoColor: (float4)ColorUint.Blue,
                                                                                    emissionColor: float4.Zero,
                                                                                    specularStrength: 1.0f,
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