using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.S3D.Core
{
    public static class UtilityBc
    {

        public static int ObjOneDistToRoot = 0;         //Fusee units    
        public static int ObjTwoDistToRoot = 2;         //Fusee units
        public static float ConvergenceDist = 10;       //Fusee units
        public static float CamOffset = 10;             //Fusee units

        public const float PhysicalDisplayWidth = 2500; //mm
        public const float Interaxial = 0.2f;           //Fusee units
        public const int HitInPx = 0;                   //px
        public const int ResolutionW = 1920;            //px
        public const int ResolutonH = 1080;             //px
        public const int EyeSeparation = 65;            //mm
        public const int ViewingDistance = 2500;        //mm
        public const int Magnification = 1;             //ratio
        public static float3 CamPosBc;

        #region Shader
        static readonly string GUIVS = @"
            uniform mat4 guiXForm;
            attribute vec3 fuVertex;
            attribute vec2 fuUV;
            attribute vec4 fuColor;
            uniform mat4 FUSEE_MVP;                 
            varying vec2 vUV;
            varying vec4 vColor;
            void main()
            {
                vUV = fuUV;
                vColor = fuColor;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1);
            }";

        static readonly string TEXTUREPS = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            uniform vec4 blendColor;
            varying vec2 vUV;
            varying vec4 vColor;
            uniform sampler2D tex;
            
            void main(void) {
                gl_FragColor = vec4(vec3(0.5,0.5,0.5), 0.7);   
            }";

        #endregion

        #region Create Scene    

        public static SceneContainer CreateScene(RenderContext rc)
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "RootNull_Transform",
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

                            new SceneNodeContainer
                            {
                                Name = "Cube",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(1,1,1),
                                        Translation = new float3(0,0,ObjOneDistToRoot),
                                        Rotation = new float3(S3D.AngleVert, S3D.AngleHorz, 0)

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

                                    Cube.CreateCube()

                                },
                                Children = new List<SceneNodeContainer>()
                                {
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
                                            Icosphere.CreateIcosphere(6)

                                        }
                                    },
                                }
                            },

                            new SceneNodeContainer
                            {
                                Name = "ConvergencePlane",
                                Components = new List<SceneComponentContainer>
                                {
                                    new TransformComponent
                                    {
                                        Scale = new float3(5* (16/9),5,1),
                                        Translation = new float3(0,0,-CamOffset+ConvergenceDist),
                                    },
                                    new ShaderEffectComponent(rc, new ShaderEffect(new[]
                                        {
                                            new EffectPassDeclaration
                                            {
                                                VS = GUIVS,
                                                PS = TEXTUREPS,
                                                StateSet = new RenderStateSet
                                                {
                                                    AlphaBlendEnable = true,
                                                    SourceBlend = Blend.SourceAlpha,
                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                    BlendOperation = BlendOperation.Add,
                                                    ZEnable = true
                                                }
                                            }
                                        },
                                        new[]
                                        {
                                            new EffectParameterDeclaration {Name = "tex", Value = rc.CreateTexture(AssetStorage.Get<ImageData>("grid.jpg"))},
                                            new EffectParameterDeclaration {Name = "blendColor", Value = new float4(0.5f,0.5f,0.5f,0.5f)},
                                        })),
                                   Plane.CreatePlane()
                                }
                            },

                        }

                    }
                }
            };
        }
        #endregion

        #region S3D Calculation

        //Returns the screen coordinates in pixel for a given point in world space.
        public static float2 WorldToScreenCoord(float3 posInWorldSpace, RenderContext ctx, int canvasHeight, int canvasWidth)
        {
            var clipSpace = posInWorldSpace.TransformPerspective(ctx.ModelViewProjection);
            var zwerg = new float2(clipSpace.x, clipSpace.y);
            return (zwerg * new float2(0.5f, -0.5f) + new float2(0.5f, 0.5f)) * new float2(canvasWidth, canvasHeight);
        }

        //Calculates the absolute parallaxe for a point in model/ local space in millimeters.
        //mvpR:         Modelviewprojection matrix for the right camera.
        //mvpL:         Modelviewprojection matrix for the left camera.
        //resW:         Target screen resolution (width).
        //pixelWidth:   Width of one pixel of the target screen  in millimeters.
        public static float CalcParallaxFromModelCoord(float3 pointInModelCoord, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth) =>
            ((mvpR * pointInModelCoord - mvpL * pointInModelCoord) * resW).x * pixelWidth;

        //Calculates the x coordinate of the perceived position of a point in model/ local space. Returns the value in millimeters.
        //eyeSep:       Eye separation of the viewer  in millimeters.
        //mvpR:         Modelviewprojection matrix for the right camera.
        //mvpL:         Modelviewprojection matrix for the left camera.
        //resW:         Target screen resolution (width).
        //pixelWidth:   Width of one pixel of the target screen in millimeters.
        public static float CalcXi(float3 pointInModelCoord, float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth)
        {
            var zwerg = ((mvpL * pointInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0)) +
                       (mvpR * pointInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;

            zwerg = zwerg * resW * pixelWidth;


            var parallax = CalcParallaxFromModelCoord(new float3(-0.5f, 0.5f, -0.5f), mvpR, mvpL, resW / 2, pixelWidth);

            var nominator = eyeSep * zwerg;
            var denominator = 2 * (eyeSep - parallax);
            return nominator / denominator;
        }


        //Calculates the z coordinate of the perceived position of a point in model/ local space. Returns the value in millimeters.
        //eyeSep:               Eye separation of the viewer in millimeters.
        //mvpR:                 Modelviewprojection matrix for the right camera.
        //mvpL:                 Modelviewprojection matrix for the left camera.
        //resW:                 Target screen resolution (width).
        //pixelWidth:           Width of one pixel of the target screen in millimeters.
        //viewingDistanceInMm:  Viewing distance - viewer to screen.
        public static float CalcZi(float3 pointInModelCoord, float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth, float viewingDistInMm)
        {
            var nominator = eyeSep * viewingDistInMm;
            var denominator = eyeSep - (CalcParallaxFromModelCoord(pointInModelCoord, mvpR, mvpL, resW / 2, pixelWidth));
            return nominator / denominator;
        }

        //Calculates the perceived width of a object from two points in model / local space. Returns the value in millimeters.
        //eyeSep:       Eye separation of the viewer  in millimeters.
        //mvpR:         Modelviewprojection matrix for the right camera.
        //mvpL:         Modelviewprojection matrix for the left camera.
        //resW:         Target screen resolution (width).
        //pixelWidth:   Width of one pixel of the target screen.
        public static float CalcWidth3D(float3 pointOneInModelCoord, float3 pointTwoInModelCoord, float eyeSep, float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth)
        {
            var zwerg1 = (mvpL * pointOneInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0) +
                         (mvpR * pointOneInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;

            var zwerg2 = (mvpL * pointTwoInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0) +
                          (mvpR * pointTwoInModelCoord * new float3(0.5f, -0.5f, 0) + new float3(0.5f, 0.5f, 0))).x;

            var prallax = CalcParallaxFromModelCoord(pointOneInModelCoord, mvpR, mvpL, resW / 2, pixelWidth);

            var nominator = eyeSep * ((zwerg2 - zwerg1) * resW * pixelWidth);
            var denominatro = 2 * (eyeSep - prallax);

            return nominator / denominatro;

        }

        //Calculates the perceived width of a object from the x coordinates of the perceived position of two points. Returns the value in millimeters.
        public static float CalcWidth3D(float xiOne, float xiTwo) => xiTwo - xiOne;

        //Ratio of the perveived width of an object to the "real" object width (e.g a unit cube would have a "real" width of one fusee unit).
        //The ratio is calculated by two points given in model / local space.
        //eyeSep:       Eye separation of the viewer in millimeters.
        //mvpR:         Modelviewprojection matrix for the right camera.
        //mvpL:         Modelviewprojection matrix for the left camera.
        //resW:         Target screen resolution (width).
        //pixelWidth:   Width of one pixel of the target screen.
        //objWidthInMm: "Real" Object width, given in millimeters.
        public static float CalcWidthMag3D(float3 pointOneInModelCoord, float3 pointTwoInModelCoord, float eyeSep,
            float4x4 mvpR, float4x4 mvpL, int resW, float pixelWidth, int objWidthInMm)
            => CalcWidth3D(pointOneInModelCoord, pointTwoInModelCoord, eyeSep, mvpR, mvpL, resW, pixelWidth) /
               objWidthInMm;

        //Ratio of the perveived width of an object to the "real" object width (e.g a unit cube would have a "real" width of one fusee unit).
        //The ratio is calculated from the x coordinates of the perceived position of two points.
        //objWidthInMm: "Real" Object width, given in millimeters.
        public static float CalcWidthMag3D(float xiOne, float xiTwo, int objWidthInMm)
            => CalcWidth3D(xiOne, xiTwo) /
               objWidthInMm;

        //Calculates the roundness facator for an object.
        //All parameters need to be given in millimeters.
        //interaxial:       Distance between the two rendering cameras.
        //viewingDistance:  Distance between viewer and screen.
        //cWidth:           Width of the covergence plane.
        //zo:               Distance from camera to object.
        //eyeSep:           Eye separation of the viewer in.
        //screenWidth:      Width of the target screen.
        //c:                Distance from the camera to the convergence plane.
        public static float CalcRoundnessFactor(float interaxial, float viewingDistance, float cWidth, float zo, float eyeSep, float screenWidth, float c)
        {
            var nominator = interaxial * viewingDistance * cWidth;
            var denominator = zo * (eyeSep * cWidth - interaxial * screenWidth) + interaxial * c * screenWidth;

            return nominator / denominator;
        }
        #endregion
    }
}
