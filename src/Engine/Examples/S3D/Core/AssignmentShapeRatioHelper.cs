

using System.Collections.Generic;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.S3D.Core
{
    public static class AssignmentShapeRatioHelper
    {
        #region Create Scene
        public static int SphereOneDistToRoot = 0;
        public static int SphereTwoDistToRoot = 3;
        
        public static SceneContainer CreateScene()
        {
            var sphere = new Icosphere(5);
            var plane = new Plane();

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
                                Name = "Sphere",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(1,1,1),
                                        Translation = new float3(0,0,SphereOneDistToRoot)
                                    },

                                    new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer{ Color = new float3(1,0.9f,0.4f)},
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
                                        Normals = sphere.Normals
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
                                        Scale = new float3(1,1,1),
                                        Translation = new float3(0,0,SphereTwoDistToRoot)
                                    },

                                    new MaterialComponent
                                    {
                                        Diffuse = new MatChannelContainer{ Color = new float3(0.1f,0.8f,0.4f)},
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
                                        Normals = sphere.Normals
                                    }

                                }
                            }

                        }
                        
                    },
                }
            };
        }
        #endregion

        #region Calculate shape ratio

        //Assumption: 1 fusee unit = 1 meter, all following varaiables are in meters
        public static float ViewingDistance;//Distance User to Display (V)
        public static float Interaxial;     //Stereo base (t)
        public static float Magnification;  //Magnification factor sensor to image (M)
        public static float FocalLength;    //Camera focal lenght - only fov for calculation... (f)
        public static float Hit;            //Image to Sensor offset (h)
        public static float EyeSeparation;    //Eye separation of the user (e)

        //distCamObject: Distance camera to object in question (Z0)
        public static float CalculateShapeRatio(float distCamObject)
        {
            return (ViewingDistance * Interaxial) / (Magnification * FocalLength * Interaxial -
                    distCamObject * (2 * Magnification * Hit - EyeSeparation));
        }

        #endregion

        #region Calculate pixel to meter conversion for given object and display width

        //distCamToObject: fusee unity == meter
        //fov: degree
        //displayWidth: meter
        public static float PixelToMeter(int hitInPx, float distCamToObject, float fov, float widthResolution, float aspectRatio, float physicalDisplayWidth)
        {

            /*var angle = M.RadiansToDegrees(fov);
            var halfHeight = (float)System.Math.Tan(angle/ 2) * distCamToObject;
            var halfWidth = halfHeight * aspectRatio;

            var q = 100 / widthResolution;
            var p = hitInPx * q;*/

            var pxSize = physicalDisplayWidth / widthResolution;

            return hitInPx * pxSize;
        }

        #endregion
    }
}
