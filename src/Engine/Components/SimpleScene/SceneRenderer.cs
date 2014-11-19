
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            _animation = new Animation();
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
            float4x4 origMV = _rc.ModelView;
            ShaderEffect origMat = CurMat;
            _rc.ModelView = _rc.ModelView * sbc.Transform.Matrix();
            SceneNodeContainer soc = sbc as SceneNodeContainer;
            if (soc != null)
            {
                if (soc.GetMaterial() != null)
                {
                    var mat = LookupMaterial(soc.GetMaterial());
                    CurMat = mat;
                }
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
                        //WeightComponent wc = soc.GetWeights();
                        //if (wc != null)
                        //    for (int i = 0; i < rm.Vertices.Length; i++)
                        //    {
                                
                        //        float4x4 moveMatrix = float4x4.Identity;
                        //        for (int j = 0; j < wc.Joints.Count; j++)
                        //        {
                        //            Debug.Write((float)wc.Weights[j].JointWeights[i]);
                        //            //moveMatrix *= float4x4.CreateTranslation(wc.Joints[j].Transform.Translation);
                        //            //Debug.WriteLine(soc.Transform.Rotation);
                        //            //soc.Transform.Rotation = wc.Joints[j].Transform.Rotation;//*float4x4.CreateRotationY(45);
                        //            //float3 rot = soc.Transform.Rotation;//* (float) wc.Weights[j].JointWeights[i];
                        //            float3 rot = wc.Joints[j].Transform.Rotation;
                        //            moveMatrix *= float4x4.CreateRotationY(rot.y*(float)wc.Weights[j].JointWeights[i]) *
                        //                           float4x4.CreateRotationX(rot.x * (float)wc.Weights[j].JointWeights[i]) *
                        //                           float4x4.CreateRotationZ(rot.z * (float)wc.Weights[j].JointWeights[i]);

                        //            //moveMatrix *= float4x4.CreateTranslation(-wc.Joints[j].Transform.Translation);
                        //        }
                        //        Debug.WriteLine("");

                        //        rm.Vertices[i] = soc.GetMesh().Vertices[i] * moveMatrix;
                        //    }
                        RenderWithLights(rm, CurMat);
                    }
                    else
                    {
                        CurMat.RenderMesh(rm);
                    }
                    //else
                    //{
                    //    CurMat.RenderMesh(rm);
                    //}
                }
            }
            if (sbc.Children != null)
            {
                foreach (var child in sbc.Children)
                {
                    VisitNodeRender(child);
                }
            }
            _rc.ModelView = origMV;
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
            Mesh rm;
            rm = new Mesh()
            {
                Colors = null,
                Normals = mc.Normals,
                UVs = mc.UVs,
                Vertices = mc.Vertices,
                Triangles = mc.Triangles
            };
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
                        VS = scb.VS,
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
    }
}