using System.Collections.Generic;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.UI.Core
{
    public class UI : RenderCanvas
    {
        protected readonly string GUIVS = @"
            
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
            varying vec2 vUV;
            varying vec3 vMVNormal;
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;
        
            void main() {
               
               vUV = fuUV;
               vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
               gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        protected readonly string NINESLICEVS = @"#version 330            

attribute vec3 fuVertex;
attribute vec3 fuNormal;
attribute vec2 fuUV;

varying vec2 vUV;
varying vec3 vMVNormal;

uniform mat4 FUSEE_MVP;
uniform mat4 FUSEE_ITMV;
uniform mat4 FUSEE_IMV;
uniform mat4 FUSEE_M;
uniform mat4 FUSEE_V;
uniform mat4 FUSEE_P;
uniform vec4 borders;
uniform float borderThickness;

bool isFloatEqual(float a, float b)
{
	return (a + 0.000001 >= b) && (a - 0.000001 <= b);
}

vec4 calculateTranslationVector(vec2 scale, float borderX, bool isXnegative, float borderY, bool isYnegative, vec3 coordinateSysVecX, vec3 coordinateSysVecY)
{                
	vec4 translateXVec = vec4(0.0,0.0,0.0,0.0);
	vec4 translateYVec = vec4(0.0,0.0,0.0,0.0);
	float translateX = 0.0;
	float translateY = 0.0;
                
	if( borderX > 0.00001)
	{
		float isX = abs(fuVertex.x * (scale.x));
		float translateToX = (((scale.x/2.0) - (borderThickness * borderX)) - isX);
		translateXVec = (isXnegative) ? vec4(normalize(coordinateSysVecX) * -translateToX,0.0) : vec4(coordinateSysVecX * translateToX,0.0);                    
	}
                
	if( borderY  > 0.00001 )
	{
		float isY = abs(fuVertex.y * (scale.y));
		float translateToY = (((scale.y/2.0) - (borderThickness * borderY)) - isY);
                    
		translateYVec = (isYnegative) ? vec4(normalize(coordinateSysVecY) * -translateToY,0.0) : vec4(coordinateSysVecY * translateToY,0.0);                    
	} 
	return (translateXVec + translateYVec);
}

vec4 calculateGlPosAccordingToUvs()
{
	vec2 scale =  vec2(length(FUSEE_M[0]),length(FUSEE_M[1]));

	mat4 origPlaneCoord = mat4(1.0);
	origPlaneCoord[2][2] = -1;

    mat4 planeCoord =  FUSEE_M * origPlaneCoord;

    vec3 xVec = normalize(planeCoord[0].xyz);
    vec3 yVec = normalize(planeCoord[1].xyz);
               
    float offsetL = borders.x;
    float offsetR = borders.y;
    float offsetT = borders.z;
    float offsetB = borders.w;                

	//left bottom corner
    if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 1.0/3.0))	
    {
         //Set Vertex and UV in unit plane, according to given border. 
		 gl_Position = vec4(-(0.5f-offsetL), -(0.5f-offsetB), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		 //Translate the Vertex according to the scaling of the plane.
		 vec4 translateVec = calculateTranslationVector(scale, offsetL, true, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4(-(0.5f-offsetL), -0.5f, 0.0, 1.0);
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale, offsetL, true, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5f, -(0.5f-offsetB), 0.0, 1.0);			
		vUV = vec2(gl_Position.x,gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//left top corner
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
         gl_Position = vec4(-(0.5f-offsetL), (0.5f-offsetT), 0.0, 1.0);		 
		 vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		 
		 vec4 translateVec = calculateTranslationVector(scale, offsetL, true, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 1.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4(-(0.5f-offsetL), 0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

		vec4 translateVec = calculateTranslationVector(scale, offsetL, true, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 0.0))
    {
		gl_Position = vec4(-0.5f, (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);		

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetT, false, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right bottom corner
    if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0/3.0))	
    {
		gl_Position = vec4((0.5f-offsetR), -(0.5f-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

         vec4 translateVec = calculateTranslationVector(scale, offsetR, false, offsetB, true, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 0.0))
    {
		gl_Position = vec4((0.5f-offsetR), -0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
		vec4 translateVec = calculateTranslationVector(scale, offsetR, false, 0.0, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 1.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5f, -(0.5f-offsetB), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

        vec4 translateVec = calculateTranslationVector(scale, 0.0, true, offsetB, true, xVec, yVec);
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//right top corner
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 2.0/3.0))
    {
		gl_Position = vec4((0.5f-offsetR), (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);	
			
         vec4 translateVec = calculateTranslationVector(scale, offsetR, false, offsetT, false, xVec, yVec);
         return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));
    }
	if(isFloatEqual(vUV.x, 2.0/3.0) && isFloatEqual(vUV.y, 1.0))
    {
		gl_Position = vec4((0.5f-offsetR),  0.5f, 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);

		vec4 translateVec = calculateTranslationVector(scale, offsetR, false, 0.0, true, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	if(isFloatEqual(vUV.y, 2.0/3.0) && isFloatEqual(vUV.x, 1.0))
    {
		gl_Position = vec4(0.5f, (0.5f-offsetT), 0.0, 1.0);		 
		vUV = vec2(gl_Position.x, gl_Position.y) + vec2(0.5,0.5);
		
        vec4 translateVec = calculateTranslationVector(scale, 0.0, false, offsetT, false, xVec, yVec); 
        return(FUSEE_P * FUSEE_V * ((FUSEE_M *  vec4(fuVertex, 1.0)) + translateVec));                    
    }
	//corner vertices
    if((isFloatEqual(vUV.x, 1.0) && isFloatEqual(vUV.y, 0.0)) || (isFloatEqual(vUV.x, 1.0) && isFloatEqual(vUV.y, 1.0)) || (isFloatEqual(vUV.x,0.0) && isFloatEqual(vUV.y, 1.0)) || (isFloatEqual(vUV.x, 0.0) && isFloatEqual(vUV.y, 0.0)))
	{ 
		return(FUSEE_P * FUSEE_V * FUSEE_M * vec4(fuVertex, 1.0));
	}	
}
                    
void main() 
{
	vUV = fuUV;	

	vMVNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);
	//gl_Position = FUSEE_P * FUSEE_V * FUSEE_M * vec4(fuVertex, 1.0);
	gl_Position = calculateGlPosAccordingToUvs();
}";//AssetStorage.Get<string>("nineSlice.vert");

        protected readonly string TEXTUREPS = @"
            #version 330

            #ifdef GL_ES
                precision highp float;
            #endif
            
            varying vec3 vMVNormal;            
            varying vec2 vUV;            
            uniform mat4 FUSEE_MV;
            uniform sampler2D DiffuseTexture;
            uniform vec4 DiffuseColor;
            uniform float DiffuseMix;
            
            void main()
            {
                vec3 N = normalize(vMVNormal);
                vec3 L = normalize(vec3(0.0,0.0,-1.0));
                gl_FragColor = vec4(texture2D(DiffuseTexture, vUV) * DiffuseMix) * DiffuseColor *  max(dot(N, L), 0.0) ;
            }";

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;

        private bool _keys;

        private SceneContainer CreateAnchorTestScene()
        {
            return new SceneContainer
            {
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
                                Translation = new float3(0,5,0),
                                Rotation = new float3(0,45,0)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                                new SceneNodeContainer
                                {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-5,0),
                                            Max = new float2(5,3)
                                        }
                                    }
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Canvas_XForm",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new XFormComponent
                                            {
                                                Name = "Canvas_XForm"
                                            },
                                            new ShaderEffectComponent
                                            {
                                                Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                {
                                                    Diffuse = new MatChannelContainer {Color = new float3(0,1,0)},
                                                })
                                            },
                                            new Cube()
                                        }
                                    },

                                    new SceneNodeContainer
                                    {
                                        Name = "Child",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(1,0),
                                                    Max = new float2(-1,2)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent
                                                    {
                                                        Name = "Child_XForm"
                                                    },
                                                    new ShaderEffectComponent
                                                    {
                                                        Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                        {
                                                            Diffuse = new MatChannelContainer {Color = new float3(1,0,0)},
                                                        })
                                                    },
                                                    new Cube()
                                                },
                                                Children = new List<SceneNodeContainer>
                                                {
                                                    new SceneNodeContainer
                                                    {
                                                        Name = "GrandChild_RectTransform",
                                                        Components = new List<SceneComponentContainer>
                                                        {
                                                            new RectTransformComponent
                                                            {
                                                                Name = "GrandChild_RectTransform",
                                                                Anchors = new MinMaxRect
                                                                {
                                                                    Min = new float2(1,1),
                                                                    Max = new float2(1,1)
                                                                },
                                                                Offsets = new MinMaxRect
                                                                {
                                                                    Min = new float2(-1,-1),
                                                                    Max = new float2(0,0)
                                                                }
                                                            }
                                                        },
                                                       Children = new List<SceneNodeContainer>
                                                       {
                                                           new SceneNodeContainer
                                                           {
                                                               Name = "GrandChild_XForm",
                                                               Components = new List<SceneComponentContainer>
                                                               {
                                                                   new XFormComponent
                                                                   {
                                                                       Name = "GrandChild_XForm"
                                                                   },
                                                                   new ShaderEffectComponent
                                                                   {
                                                                       Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                                       {
                                                                           Diffuse = new MatChannelContainer {Color = ColorUint.Yellow.Tofloat3()},
                                                                       })
                                                                   },
                                                                   new Cube()
                                                               }
                                                           }
                                                       }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        private SceneContainer CreateImageTestScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-5,0),
                                            Max = new float2(5,3)
                                        }
                                    }
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Canvas_XForm",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new XFormComponent
                                            {
                                                Name = "Canvas_XForm"
                                            },
                                            new ShaderEffectComponent
                                            {
                                                Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                {
                                                    Diffuse = new MatChannelContainer {Color = new float3(1,0,0)},
                                                })
                                            },
                                            new Plane()
                                        }
                                    },
                                    new SceneNodeContainer
                                    {
                                        Name = "Child",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(1,0),
                                                    Max = new float2(-1,2)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent
                                                    {
                                                        Name = "Child_XForm"
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
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
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f}
                                                        })},
                                                    new Plane()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private SceneContainer CreateNineSliceTestScene()
        {
            return new SceneContainer
            {
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = "Null_Transform",
                        Components = new List<SceneComponentContainer>()
                        {
                            new TransformComponent()
                            {
                                Translation = new float3(0,0,0),
                                Rotation = new float3(0,45,0),
                                Scale = new float3(1,1,1)
                            }
                        },
                        Children = new List<SceneNodeContainer>
                        {
                            new SceneNodeContainer
                            {
                                Name = "Canvas",
                                Components = new List<SceneComponentContainer>
                                {
                                    new CanvasTransformComponent
                                    {
                                        Name = "Canvas_CanvasTransform",
                                        CanvasRenderMode = CanvasRenderMode.WORLD,
                                        Size = new MinMaxRect
                                        {
                                            Min = new float2(-10,-5),
                                            Max = new float2(10,5)
                                        }
                                    }
                                },
                                Children = new List<SceneNodeContainer>
                                {
                                    new SceneNodeContainer
                                    {
                                        Name = "Canvas_XForm",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new XFormComponent
                                            {
                                                Name = "Canvas_XForm"
                                            },
                                            new ShaderEffectComponent
                                            {
                                                Effect = ShaderCodeBuilder.MakeShaderEffectFromMatComp(new MaterialComponent
                                                {
                                                    Diffuse = new MatChannelContainer {Color = new float3(1,0,0)},
                                                })
                                            },
                                            new Plane()
                                        }
                                    },
                                    new SceneNodeContainer
                                    {
                                        Name = "Child1",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child1_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(0,0),
                                                    Max = new float2(1,0)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(7.5f,0),
                                                    Max = new float2(-7.5f,5)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child1_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent()
                                                    {
                                                        Name = "Child1_XForm",
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = NINESLICEVS,
                                                                PS = TEXTUREPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("testTex.jpg"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "borders", Value = new float4(0.1f,0.1f,0.1f,0.1f)},
                                                            new EffectParameterDeclaration {Name = "borderThickness", Value = 5f}
                                                        })},
                                                    new NineSlicePlane()
                                                }
                                            }
                                        }
                                    },
                                    new SceneNodeContainer
                                    {
                                        Name = "Child2",
                                        Components = new List<SceneComponentContainer>
                                        {
                                            new RectTransformComponent
                                            {
                                                Name = "Child2_RectTransform",
                                                Anchors = new MinMaxRect
                                                {
                                                    Min = new float2(1,1),
                                                    Max = new float2(1,1)
                                                },
                                                Offsets = new MinMaxRect
                                                {
                                                    Min = new float2(-8,-4),
                                                    Max = new float2(0,0)
                                                }

                                             }
                                        },
                                        Children =  new List<SceneNodeContainer>
                                        {
                                            new SceneNodeContainer
                                            {
                                                Name = "Child2_XForm",
                                                Components = new List<SceneComponentContainer>
                                                {
                                                    new XFormComponent()
                                                    {
                                                        Name = "Child2_XForm",
                                                    },
                                                    new ShaderEffectComponent{Effect = new ShaderEffect(new[]
                                                        {
                                                            new EffectPassDeclaration
                                                            {
                                                                VS = NINESLICEVS,
                                                                PS = TEXTUREPS,
                                                                StateSet = new RenderStateSet
                                                                {
                                                                    AlphaBlendEnable = true,
                                                                    SourceBlend = Blend.SourceAlpha,
                                                                    DestinationBlend = Blend.InverseSourceAlpha,
                                                                    BlendOperation = BlendOperation.Add,
                                                                    ZEnable = false
                                                                }
                                                            }
                                                        },
                                                        new[]
                                                        {
                                                            new EffectParameterDeclaration {Name = "DiffuseTexture", Value = new Texture(AssetStorage.Get<ImageData>("9SliceSprites-4.png"))},
                                                            new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                                            new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                                            new EffectParameterDeclaration {Name = "borders", Value = new float4(0.1f,0.1f,0.1f,0.1f)},
                                                            new EffectParameterDeclaration {Name = "borderThickness", Value = 1f}
                                                        })},
                                                    new NineSlicePlane()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            // Load the rocket model
            //_scene = CreateAnchorTestScene();
            _scene = CreateNineSliceTestScene();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRenderer(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Mouse and keyboard movement
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Input.Mouse.XVel * Time.DeltaTime * 0.0005f;
                _angleVelVert = -RotationSpeed * Input.Mouse.YVel * Time.DeltaTime * 0.0005f;
            }
            else if (Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                // _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                // _angleVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTime;
                    _angleVelVert = -RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTime);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;


            //Viewport transformation
            // Create the camera matrix and set it as the current ModelView transformation 

            //Worldspace


            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 0, -15, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot;

            //View Space

            // Pick it !
            /*
            if (Mouse.LeftButton)
            {
                PickAtPosition(_scene.Children,
                    Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1),
                    RC.Projection * RC.ModelView);
            }
            */

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}