using System.Collections.Generic;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.S3D.Core
{
    public static class AssignmentShapeRatioHelper
    {
        #region Create Scene
        public static int ObjOneDistToRoot = 0;
        public static int ObjTwoDistToRoot = 2;

        public static SceneContainer CreateScene()
        {
            var sphere = new Icosphere(6);
            var plane = new Plane();
            var cube = new Cube();

            return new SceneContainer
            {
                Header = new SceneHeader
                {

                },
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Components = new List<SceneComponentContainer>
                        {
                            new TransformComponent
                            {
                                Scale = new float3(1,1,1),
                                Translation = new float3(0,0,0)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            //new SceneNodeContainer
                            //{
                            //    Name = "Floor",
                            //    Components = new List<SceneComponentContainer>
                            //    {
                            //        new TransformComponent
                            //        {
                            //            Scale = new float3(50,50,1),
                            //            Translation = new float3(0,-4,0),
                            //            Rotation = new float3(90,0,0)
                            //        },
                            //        new MaterialComponent
                            //        {
                            //            Diffuse = new MatChannelContainer{ Color = new float3(0.5f,0.5f,0.5f)},
                            //            Specular =  new SpecularChannelContainer
                            //            {
                            //                Color = new float3(1,1,1),
                            //                Intensity = 0.5f,
                            //                Shininess = 100f
                            //            }
                            //        },
                            //        new MeshComponent
                            //        {
                            //            Vertices = plane.Vertices,
                            //            Triangles = plane.Triangles,
                            //            Normals = plane.Normals
                            //        }
                            //    }
                            //},

                            new SceneNodeContainer
                            {
                                Name = "Cube",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(1,1,1),
                                        Translation = new float3(0,0,ObjOneDistToRoot)
                                    },

                                    new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer
                                        {
                                            Color = new float3(1,0.9f,0.4f),
                                            Texture = "grid.jpg",
                                            Mix = 0.1f
                                        },
                                        Specular =  new SpecularChannelContainer
                                        {
                                            Color = new float3(1,1,1),
                                            Intensity = 0.5f,
                                            Shininess = 100f
                                        }
                                    },
                                    
                                    new MeshComponent
                                    {
                                        Vertices = cube.Vertices,
                                        Triangles = cube.Triangles,
                                        Normals = cube.Normals,
                                        UVs = cube.UVs
                                    }

                                }
                            },
                            new SceneNodeContainer
                            {
                                Name = "Sphere",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(0.5f,0.5f,0.5f),
                                        Translation = new float3(0,0,ObjTwoDistToRoot)
                                    },

                                    new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer
                                        {
                                            Color = new float3(0.1f,0.8f,0.4f),
                                            Texture = "grid.jpg",
                                            Mix = 0.1f

                                        },
                                        Specular =  new SpecularChannelContainer
                                        {
                                            Color = new float3(1,1,1),
                                            Intensity = 0.5f,
                                            Shininess = 100f
                                        }
                                    },
                                    new MeshComponent
                                    {
                                        Vertices = sphere.Vertices,
                                        Triangles = sphere.Triangles,
                                        Normals = sphere.Normals,
                                        UVs = sphere.UVs
                                    }

                                }
                            }

                        }

                    }
                }
            };
        }
        #endregion

        #region Calculate shape ratio (Smith, Collar)

        //distCamObject: Distance camera to object in question (Z0)
        public static float CalculateShapeRatio(float distCamObject) =>
            S3D.ViewingDistance * S3D.Interaxial /
            (S3D.Magnification * S3D.FocalLength * S3D.Interaxial - distCamObject * (2 * S3D.Magnification * S3D.Hit - S3D.EyeSeparation));
        #endregion

        #region Calculate pixel to meter conversion hit value
        //distCamToObject: fusee unity == meter
        //fov: degree
        //displayWidth: meter
        public static float PixelToMeter(int hitInPx, float widthResolution, float physicalDisplayWidth) =>
            (physicalDisplayWidth / widthResolution) * hitInPx;
        #endregion

        public static float2 WorldToScreenCoord(float3 posInWorldSpace, RenderContext ctx, int canvasHeight, int canvasWidth)
        {
            var clipSpace = posInWorldSpace.TransformPerspective(ctx.ModelViewProjection);
            var zwerg = new float2(clipSpace.x, clipSpace.y);
            return (zwerg * new float2(0.5f, -0.5f) + new float2(0.5f, 0.5f)) * new float2(canvasWidth, canvasHeight);
        }

        public static float CalcParallaxFromModelCoord(float3 pointInModelCoord, float4x4 mvpR, float4x4 mvpL, int resW,float pixelWidth) =>
            ((mvpR * pointInModelCoord - mvpL * pointInModelCoord) * resW).x * pixelWidth;
        

        public static float CalcXi(float3 pointInModelCoord,float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth)
        {
            eyeSep = eyeSep * 1000;

            //var xsL = ((mvpL * pointInModelCoord * new float3(0.5f, -0.5f,0) + new float3(0.5f, 0.5f, 0))*resW).x * pixelWidth;
            //var xsR = ((mvpR * pointInModelCoord * new float3(0.5f, -0.5f,0) + new float3(0.5f, 0.5f, 0))*resW).x * pixelWidth;

            //var zwerg = xsL + xsR;

            var zwerg = ((mvpL * pointInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0)) +
                       (mvpR * pointInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;

            zwerg = zwerg * resW * pixelWidth;

            
            var parallax = CalcParallaxFromModelCoord(new float3(-0.5f, 0.5f, -0.5f), mvpR, mvpL, resW/2 , pixelWidth);

            var nominator = eyeSep * zwerg;
            var denominator = 2*(eyeSep- parallax);
            return nominator / denominator;
        }

        public static float CalcZi(float3 pointInModelCoord, float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW,float pixelWidth, float viewingDistInMm)
        {
            eyeSep = eyeSep * 1000;
            var nominator = eyeSep * viewingDistInMm;
            var denominator = eyeSep - (CalcParallaxFromModelCoord(pointInModelCoord, mvpR, mvpL, resW/2, pixelWidth));
            return nominator / denominator;
        }


        public static float CalcWidthMag3D(float3 pointOneInModelCoord, float3 pointTwoInModelCoord, float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth)
        {
            eyeSep = eyeSep * 1000;
            var zwerg1 = ((mvpL * pointOneInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0)) +
                         (mvpR * pointOneInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;
            
            var zwerg2 = ((mvpL * pointTwoInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0)) +
                          (mvpR * pointTwoInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;

            var prallax = CalcParallaxFromModelCoord(pointOneInModelCoord, mvpR, mvpL, resW / 2, pixelWidth);
            
            var nominator = eyeSep * ((zwerg2 - zwerg1)* resW * pixelWidth);
            var denominatro = 2 * (eyeSep - prallax);

            return nominator / denominatro;

        }

        public static float CalcWidthMag3D(float xiOne, float xiTwo) => xiTwo - xiOne;


    }
}
