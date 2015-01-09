
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Fusee.Engine;
using Fusee.KeyFrameAnimation;
using Fusee.Math;
using Fusee.Serialization;
namespace Fusee.Engine.SimpleScene
{
    class LightInfo // Todo: TBD...
    {
    }
    public static class SceneRendererExtensions
    {
        public static float4x4 Matrix(this TransformContainer tcThis)
        {
            return float4x4.CreateTranslation(tcThis.Translation) * float4x4.CreateRotationY(tcThis.Rotation.y) *
            float4x4.CreateRotationX(tcThis.Rotation.x) * float4x4.CreateRotationZ(tcThis.Rotation.z) *
            float4x4.CreateScale(tcThis.Scale);
        }
    }
    public class SceneRenderer
    {
        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<SceneNodeContainer, float4x4> _boneMap;
        private SceneContainer _sc;
        private RenderContext _rc;
        private float4x4 _AABBXForm;
        private List<LightInfo> _lights;
        private Animation _animation;
        private RenderStateSet _stateSet = new RenderStateSet()
        {
            AlphaBlendEnable = false,
            SourceBlend = Blend.One,
            DestinationBlend = Blend.Zero,
            ZEnable = true,
            ZFunc = Compare.Less
        };
        private ShaderEffect _curMat;
        private string _scenePathDirectory;
        ShaderEffect CurMat
        {
            set { _curMat = value; }
            get { return _curMat; }
        }
        public SceneRenderer(SceneContainer sc, string scenePathDirectory)
        {
            // Todo: scan for lights...
            _lights = new List<LightInfo>();
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
            InitAnimations(_sc);
        }
        public void InitShaders(RenderContext rc)
        {
            if (rc != _rc)
            {
                _rc = rc;
                _meshMap = new Dictionary<MeshComponent, Mesh>();
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _boneMap = new Dictionary<SceneNodeContainer, float4x4>();
                _curMat = null;
            }
            if (_curMat == null)
            {
                _curMat = MakeMaterial(new MaterialComponent
                {
                    Diffuse = new MatChannelContainer()
                    {
                        Color = new float3(0.5f, 0.5f, 0.5f)
                    },
                    Specular = new SpecularChannelContainer()
                    {
                        Color = new float3(1, 1, 1),
                        Intensity = 0.5f,
                        Shininess = 22
                    }
                });
                CurMat.AttachToContext(rc);
            }
        }

        public void InitAnimations(SceneContainer sc)
        {

            _animation = new Animation(1);
            if(sc.AnimationTracks != null){
                foreach (AnimationTrackContainer animTrackContainer in sc.AnimationTracks)
                {
                    Type t = animTrackContainer.KeyType;
                    if (typeof (int).IsAssignableFrom(t))
                    {
                        Channel<int> channel = new Channel<int>(Lerp.IntLerp);
                        foreach (AnimationKeyContainerInt key in animTrackContainer.KeyFrames)
                        {
                            channel.AddKeyframe(new Keyframe<int>(key.Time, key.Value));
                        }
                        _animation.AddAnimation(channel, animTrackContainer.SceneObject, animTrackContainer.Property);
                    }
                    else if (typeof (float).IsAssignableFrom(t))
                    {
                        Channel<float> channel = new Channel<float>(Lerp.FloatLerp);
                        foreach (AnimationKeyContainerFloat key in animTrackContainer.KeyFrames)
                        {
                            channel.AddKeyframe(new Keyframe<float>(key.Time, key.Value));
                        }
                        _animation.AddAnimation(channel, animTrackContainer.SceneObject, animTrackContainer.Property);
                    }
                    else if (typeof (float2).IsAssignableFrom(t))
                    {
                        Channel<float2> channel = new Channel<float2>(Lerp.Float2Lerp);
                        foreach (AnimationKeyContainerFloat2 key in animTrackContainer.KeyFrames)
                        {
                            channel.AddKeyframe(new Keyframe<float2>(key.Time, key.Value));
                        }
                        _animation.AddAnimation(channel, animTrackContainer.SceneObject, animTrackContainer.Property);
                    }
                    else if (typeof (float3).IsAssignableFrom(t))
                    {
                        Channel<float3> channel = new Channel<float3>(Lerp.Float3Lerp);
                        foreach (AnimationKeyContainerFloat3 key in animTrackContainer.KeyFrames)
                        {
                            channel.AddKeyframe(new Keyframe<float3>(key.Time, key.Value));
                        }
                        _animation.AddAnimation(channel, animTrackContainer.SceneObject, animTrackContainer.Property);
                    }
                    else if (typeof (float4).IsAssignableFrom(t))
                    {
                        Channel<float4> channel = new Channel<float4>(Lerp.Float4Lerp);
                        foreach (AnimationKeyContainerFloat4 key in animTrackContainer.KeyFrames)
                        {
                            channel.AddKeyframe(new Keyframe<float4>(key.Time, key.Value));
                        }
                        _animation.AddAnimation(channel, animTrackContainer.SceneObject, animTrackContainer.Property);
                    }
                    //TODO : Add cases for each type
                }
            }
        }

        public void Animate()
        {
            _animation.Animate();
        }
        public AABBf? GetAABB()
        {
            AABBf? ret = null;
            _AABBXForm = float4x4.Identity;
            foreach (var soc in _sc.Children)
            {
                AABBf? nodeBox = VisitNodeAABB(soc);
                if (nodeBox != null)
                {
                    if (ret == null)
                    {
                        ret = nodeBox;
                    }
                    else
                    {
                        ret = AABBf.Union((AABBf)ret, (AABBf)nodeBox);
                    }
                }
            }
            return ret;
        }
        protected AABBf? VisitNodeAABB(SceneNodeContainer sbc)
        {
            AABBf? ret = null;
            float4x4 origMV = _AABBXForm;
            _AABBXForm = _AABBXForm * sbc.Transform.Matrix();
            SceneNodeContainer snc = sbc as SceneNodeContainer;
            if (snc != null && snc.GetMesh() != null)
            {
                ret = _AABBXForm * snc.GetMesh().BoundingBox;
            }
            if (sbc.Children != null)
            {
                foreach (var child in sbc.Children)
                {
                    AABBf? nodeBox = VisitNodeAABB(child);
                    if (nodeBox != null)
                    {
                        if (ret == null)
                        {
                            ret = nodeBox;
                        }
                        else
                        {
                            ret = AABBf.Union((AABBf)ret, (AABBf)nodeBox);
                        }
                    }
                }
            }
            _AABBXForm = origMV;
            return ret;
        }
        public void Render(RenderContext rc)
        {
            InitShaders(rc);
            foreach (var sbc in _sc.Children)
            {
                VisitNodeRender(sbc);
            }
        }

        protected void VisitNodeRender(SceneNodeContainer sbc)
        {
            float4x4 origMV = _rc.Model;
            ShaderEffect origMat = CurMat;
            _rc.Model = _rc.Model * sbc.Transform.Matrix();

            if (sbc.IsBone)
            {
                SceneNodeContainer bone = sbc as SceneNodeContainer;
                float4x4 transform;

                if (!_boneMap.TryGetValue(bone, out transform))
                    _boneMap.Add(bone, _rc.Model);
                else
                    _boneMap[bone] = _rc.Model;
            }


            SceneNodeContainer soc = sbc as SceneNodeContainer;
            if (soc != null)
            {
                if (soc.GetMaterial() != null)
                {
                    var mat = LookupMaterial(soc.GetMaterial());
                    CurMat = mat;
                }
                ////new
                if (soc.GetWeights() != null)
                {
                    float4x4[] boneArray = new float4x4[soc.GetWeights().Joints.Count()];
                    for (int i = 0; i < soc.GetWeights().Joints.Count(); i++)
                    {
                        float4x4 tmp = soc.GetWeights().BindingMatrices[i];
                        boneArray[i] = _boneMap[soc.GetWeights().Joints[i]] * tmp;
                    }

                    _rc.Bones = boneArray;
                }

                //if (soc.GetWeights() != null)
                //{
                //    float4x4[] boneArray = new float4x4[soc.GetWeights().Joints.Count()];
                //    for (int i = 0; i < soc.GetWeights().Joints.Count(); i++)
                //    {
                //        if (boneArray[i].Column3.w == 0)
                //            boneArray[i] = _rc.ModelView;
                        
                //        boneArray[i] *= soc.GetWeights().Joints[i].Transform.Matrix();
                //        if (soc.GetWeights().Joints[i].Children != null)
                //            foreach (var child in soc.GetWeights().Joints[i].Children)
                //            {
                //                if (boneArray[soc.GetWeights().Joints.IndexOf(child)].Column3.w == 0)
                //                    boneArray[soc.GetWeights().Joints.IndexOf(child)] = _rc.ModelView;
                //                boneArray[soc.GetWeights().Joints.IndexOf(child)] *= boneArray[i];
                //            }

                //    }
                //    _rc.Bones = boneArray;
                //}

                if (soc.GetMesh() != null)
                {
                    Mesh rm;
                    if (!_meshMap.TryGetValue(soc.GetMesh(), out rm))
                    {
                        rm = MakeMesh(soc);
                        _meshMap.Add(soc.GetMesh(), rm);
                    }
                    if (null != CurMat.GetEffectParam(ShaderCodeBuilder.LightDirectionName))
                    {
                        RenderWithLights(rm, CurMat);
                    }
                    else
                    {
                        CurMat.RenderMesh(rm);
                    }
                }
            }
            if (sbc.Children != null)
            {
                foreach (var child in sbc.Children)
                {
                    VisitNodeRender(child);
                }
            }

            _rc.Model = origMV;
            CurMat = origMat;
        }

        private void RenderWithLights(Mesh rm, ShaderEffect CurMat)
        {
            if (_lights.Count > 0)
            {
                foreach (LightInfo li in _lights)
                {
                    // SetupLight(li);
                    CurMat.RenderMesh(rm);
                }
            }
            else
            {
                // No light present - switch on standard light
                CurMat.SetEffectParam(ShaderCodeBuilder.LightColorName, new float3(1, 1, 1));
                // float4 lightDirHom = new float4(0, 0, -1, 0);
                float4 lightDirHom = _rc.InvModelView * new float4(0, 0, -1, 0);
                // float4 lightDirHom = _rc.TransModelView * new float4(0, 0, -1, 0);
                float3 lightDir = lightDirHom.xyz;
                lightDir.Normalize();
                CurMat.SetEffectParam(ShaderCodeBuilder.LightDirectionName, lightDir);
                CurMat.SetEffectParam(ShaderCodeBuilder.LightIntensityName, (float)1);
                CurMat.RenderMesh(rm);
            }
        }
        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            ShaderEffect mat;
            if (!_matMap.TryGetValue(mc, out mat))
            {
                mat = MakeMaterial(mc);
                mat.AttachToContext(_rc);
                _matMap.Add(mc, mat);
            }
            return mat;
        }
        public static Mesh MakeMesh(SceneNodeContainer soc)
        {
            MeshComponent mc = soc.GetMesh();
            WeightComponent wc = soc.GetWeights();
            Mesh rm;

            if (wc == null)
                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                };
            else // Create Mesh with weightdata
            {

                // invert weightmap to handle it easier
                float[,] invertedWeightMap = new float[wc.WeightMap[0].JointWeights.Count, wc.Joints.Count];
                for (int i = 0; i < wc.WeightMap.Count; i++)
                {
                    for (int j = 0; j < wc.WeightMap[i].JointWeights.Count; j++)
                    {
                        invertedWeightMap[j, i] = (float) wc.WeightMap[i].JointWeights[j];
                    }
                }

                float4[] boneWeights = new float4[invertedWeightMap.GetLength(0)];
                float4[] boneIndices = new float4[invertedWeightMap.GetLength(0)];

                for (int i = 0; i < invertedWeightMap.GetLength(0); i++)
                {
                    boneWeights[i] = new float4(0,0,0,0);
                    boneIndices[i] = new float4(0,0,0,0);

                    var tempDictionary = new Dictionary<int, float>();

                    for (int j = 0; j < invertedWeightMap.GetLength(1); j++)
                    {
                        if (j < 4)
                        {
                            tempDictionary.Add(j, invertedWeightMap[i, j]);
                        }
                        else
                        {
                            float tmpWeight = invertedWeightMap[i, j];
                            var keyAndValue = tempDictionary.OrderBy(kvp => kvp.Value).First();
                            if (tmpWeight > keyAndValue.Value)
                            {
                                tempDictionary.Remove(keyAndValue.Key);
                                tempDictionary.Add(j, tmpWeight);
                            }
                        }
                    }

                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].x = keyValuePair.Key;
                        boneWeights[i].x = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].y = keyValuePair.Key;
                        boneWeights[i].y = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].z = keyValuePair.Key;
                        boneWeights[i].z = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }
                    if (tempDictionary.Count != 0)
                    {
                        var keyValuePair = tempDictionary.First();
                        boneIndices[i].w = keyValuePair.Key;
                        boneWeights[i].w = keyValuePair.Value;
                        tempDictionary.Remove(keyValuePair.Key);
                    }

                    boneWeights[i].Normalize1();
                }

                rm = new Mesh()
                {
                    Colors = null,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    BoneIndices = boneIndices,
                    BoneWeights = boneWeights,
                    Vertices = mc.Vertices,
                    Triangles = mc.Triangles
                }; 
            }


            return rm;
        }
        private ITexture LoadTexture(string path)
        {
            string texturePath = Path.Combine(_scenePathDirectory, path);
            var image = _rc.LoadImage(texturePath);
            return _rc.CreateTexture(image);
        }
        private ShaderEffect MakeMaterial(MaterialComponent mc)
        {
            ShaderCodeBuilder scb = new ShaderCodeBuilder(mc, null);
            var effectParameters = AssembleEffectParamers(mc, scb);

            ShaderEffect ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration()
                    {


                        //VS = scb.VS,
                        VS = VsBones,
                        PS = scb.PS,
                        StateSet = new RenderStateSet()
                        {
                            ZEnable = true,
                            AlphaBlendEnable = false
                        }
                    }
                },
                effectParameters
            );
            return ret;
        }

        private List<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc, ShaderCodeBuilder scb)
        {
            List<EffectParameterDeclaration> effectParameters = new List<EffectParameterDeclaration>();
            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.DiffuseColorName,
                    Value = (object)mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }
            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularColorName,
                    Value = (object)mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularShininessName,
                    Value = (object)mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularIntensityName,
                    Value = (object)mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.SpecularTextureName,
                        Value = LoadTexture(mc.Specular.Texture)
                    });
                }
            }
            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.EmissiveColorName,
                    Value = (object)mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = scb.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }
            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }
            // Any light calculation needed at all?
            if (mc.HasDiffuse || mc.HasSpecular)
            {
                // Light calculation parameters
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightColorName,
                    Value = new float3(1, 1, 1)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightIntensityName,
                    Value = (float)1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightDirectionName,
                    Value = new float3(0, 0, 1)
                });
            }
            return effectParameters;
        }


        public string VsBones =
            "attribute vec3 fuVertex; " +
            "attribute vec3 fuNormal;" +
            "attribute vec2 fuUV;  " +
            "attribute vec4 fuBoneIndex;" +
            "attribute vec4 fuBoneWeight;" +
            "uniform mat4 FUSEE_IMV; " +
            "uniform mat4 FUSEE_P;" +
            "uniform mat4 FUSEE_V;" +
            "uniform mat4 FUSEE_M;" +
            "uniform mat4 FUSEE_IV;" +
            "uniform vec4 FUSEE_BONES[100];" +
            "varying vec3 vViewDir; " +
            "varying vec3 vNormal; " +
            "varying vec2 vUV;  " +

            "void CalcBoneMatrix(in float ind, in vec4[100] Bones, inout mat4 result){" +
            //    "mat4 ret;" + 
            "int index = int(ind);" +
            "result[0] = Bones[index*4];" +
            "result[1] = Bones[index*4+1];" +
            "result[2] = Bones[index*4+2];" +
            "result[3] = Bones[index*4+3];" +
            //"result = ret;" +
            "}" +

            "void main() " +
            "{ " +
            "vec4 newVertex;" +
            "vec4 newNormal;" +
            "int index;" +

            
            "mat4 boneMatrix;" +
            "CalcBoneMatrix(fuBoneIndex.x, FUSEE_BONES, boneMatrix);" +
            "vec3 ver = fuVertex + vec3(0,0,0);" +
            "newVertex = (boneMatrix *  vec4(ver, 1.0) ) * fuBoneWeight.x ;" +
            "newNormal = (boneMatrix * vec4(fuNormal, 0.0)) * fuBoneWeight.x;" +

            //"ver = fuVertex + vec3(0,100,0);" +
            "CalcBoneMatrix(fuBoneIndex.y, FUSEE_BONES, boneMatrix);" +
            "newVertex = (boneMatrix * vec4(ver, 1.0)) * fuBoneWeight.y + newVertex;" +
            "newNormal = (boneMatrix * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;" +

            //"ver = fuVertex + vec3(0,0,0);" +
            "CalcBoneMatrix(fuBoneIndex.z, FUSEE_BONES, boneMatrix);" +
            "newVertex = (boneMatrix * vec4(ver, 1.0)) * fuBoneWeight.z + newVertex;" +
            "newNormal = (boneMatrix * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;" +

            //"ver = fuVertex + vec3(0,-100,0);" +
            "CalcBoneMatrix(fuBoneIndex.w, FUSEE_BONES, boneMatrix);" +
            "newVertex = (boneMatrix * vec4(ver, 1.0)) * fuBoneWeight.w + newVertex;" +
            "newNormal = (boneMatrix * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;" +


            "vNormal = normalize(vec3(newNormal)); " +
            "vec3 viewPos = FUSEE_IMV[3].xyz; " +
            "vViewDir = normalize(viewPos - vec3(newVertex)); " +
            "gl_Position = FUSEE_P *FUSEE_V* vec4(vec3(newVertex), 1.0); " +
            "vUV = fuUV;" +
            " } ";

    }
}