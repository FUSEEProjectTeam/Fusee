using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Math;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
    public class AABBCalculator : SceneVisitor
    {
        public class AABBState : VisitorState
        {
            private CollapsingStateStack<float4x4> _modelView = new CollapsingStateStack<float4x4>();

            public float4x4 ModelView
            {
                set { _modelView.Tos = value; }
                get { return _modelView.Tos; }
            }
            public AABBState()
            {
                RegisterState(_modelView);
            }
        }

        private SceneContainer _sc;
        private AABBState _state = new AABBState();
        private bool _boxValid;
        private AABBf _result;

        public AABBCalculator(SceneContainer sc)
        {
            _sc = sc;
        }

        public AABBf? GetBox()
        {
            Traverse(_sc.Children);
            if (_boxValid)
                return _result;
            return null;
        }

        #region Visitors
        [VisitMethod]
        public void OnTransform(TransformComponent transform)
        {
            _state.ModelView *= transform.Matrix();
        }
 
        [VisitMethod]
        public void OnMesh(MeshComponent meshComponent)
        {
            AABBf box = _state.ModelView * meshComponent.BoundingBox;
            if (!_boxValid)
            {
                _result = box;
                _boxValid = true;
            }
            else
            {
                _result = AABBf.Union((AABBf) _result, box);
            }
        }
        #endregion

        #region HierarchyLevel
        protected override void InitState()
        {
            _boxValid = false;
            _state.Clear();
            _state.ModelView = float4x4.Identity;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
        }
        #endregion
    }


    class LightInfo // Todo: TBD...
    {
    }

    
    public class SceneRenderer : SceneVisitor
    {
        #region Traversal information
        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private SceneContainer _sc;

        private RenderContext _rc;
        private List<LightInfo> _lights;

        private string _scenePathDirectory;
        private ShaderEffect _defaultEffect;
        #endregion

        #region State
        public class RendererState : VisitorState
        {
            private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
            public float4x4 Model
            {
                set { _model.Tos = value; }
                get { return _model.Tos; }
            }

            private StateStack<ShaderEffect> _effect = new StateStack<ShaderEffect>();
            public ShaderEffect Effect
            {
                set { _effect.Tos = value; }
                get { return _effect.Tos; }
            }

            public RendererState()
            {
                RegisterState(_model);
                RegisterState(_effect);
            }
        };

        private RendererState _state;
        private float4x4 _view;

        #endregion

        #region Initialization Construction Startup
        public SceneRenderer(SceneContainer sc, string scenePathDirectory)
        {
            _lights = new List<LightInfo>();
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
            _state = new RendererState();
        }

        public void SetContext(RenderContext rc)
        {
            if (rc == null)
                throw new ArgumentNullException("rc");
            
            if (rc != _rc)
            {
                _rc = rc;
                _meshMap = new Dictionary<MeshComponent, Mesh>();
                _matMap = new Dictionary<MaterialComponent, ShaderEffect>();
                _defaultEffect = MakeMaterial(new MaterialComponent
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
                _defaultEffect.AttachToContext(_rc);
            }
        }
        #endregion

        public void Render(RenderContext rc)
        {
            SetContext(rc);
            Traverse(_sc.Children);
        }

        #region Visitors

        [VisitMethod]
        public void RenderTransform(TransformComponent transform)
        {
            _state.Model *= transform.Matrix();
            _rc.Model = _view * _state.Model;
        }

        [VisitMethod]
        public void RenderMaterial(MaterialComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _state.Effect = effect;
        }

        
        [VisitMethod]
        public void RenderMesh(MeshComponent meshComponent)
        {
            Mesh rm;
            if (!_meshMap.TryGetValue(meshComponent, out rm))
            {
                rm = MakeMesh(meshComponent);
                _meshMap.Add(meshComponent, rm);
            }

            if (null != _state.Effect.GetEffectParam(ShaderCodeBuilder.LightDirectionName))
            {
                RenderWithLights(rm, _state.Effect);
            }
            else
            {
                _state.Effect.RenderMesh(rm);
            }
        }
        #endregion

        #region HierarchyLevel
        protected override void InitState()
        {
            _state.Clear();
            _state.Model = float4x4.Identity;
            _view = _rc.ModelView;

            _state.Effect = _defaultEffect;
        }

        protected override void PushState()
        {
            _state.Push();
        }

        protected override void PopState()
        {
            _state.Pop();
            _rc.ModelView = _view * _state.Model;
        }
        #endregion


        private void RenderWithLights(Mesh rm, ShaderEffect effect)
        {
            if (_lights.Count > 0)
            {
                foreach (LightInfo li in _lights)
                {
                    // SetupLight(li);
                    effect.RenderMesh(rm);
                }
            }
            else
            {
                // No light present - switch on standard light
                effect.SetEffectParam(ShaderCodeBuilder.LightColorName, new float3(1, 1, 1));
                // float4 lightDirHom = new float4(0, 0, -1, 0);
                float4 lightDirHom = _rc.InvModelView * new float4(0, 0, -1, 0);
                // float4 lightDirHom = _rc.TransModelView * new float4(0, 0, -1, 0);
                float3 lightDir = lightDirHom.xyz;
                lightDir.Normalize();
                effect.SetEffectParam(ShaderCodeBuilder.LightDirectionName, lightDir);
                effect.SetEffectParam(ShaderCodeBuilder.LightIntensityName, (float)1);
                effect.RenderMesh(rm);
            }
        }




        #region RenderContext/Asset Setup
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

        public static Mesh MakeMesh(MeshComponent mc)
        {
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

            ShaderEffect ret = new ShaderEffect(new []
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
                    Value = (object) mc.Diffuse.Color
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
                    Value = (object) mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularShininessName,
                    Value = (object) mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularIntensityName,
                    Value = (object) mc.Specular.Intensity
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
                    Value = (object) mc.Emissive.Color
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
                    Value = (float) 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = ShaderCodeBuilder.LightDirectionName,
                    Value = new float3(0, 0, 1)
                });
            }

            return effectParameters;
        }
        #endregion
    }


    public class SceneRendererOld
    {
        private Dictionary<MeshComponent, Mesh> _meshMap;
        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private SceneContainer _sc;

        private RenderContext _rc;
        private float4x4 _AABBXForm;

        private List<LightInfo> _lights;

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
            set { _curMat = value;}
            get { return _curMat; }
        }

        public SceneRendererOld(SceneContainer sc, string scenePathDirectory)
        {
            // Todo: scan for lights...
            _lights = new List<LightInfo>();
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
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

        protected AABBf? VisitNodeAABB(SceneNodeContainer node)
        {
            AABBf? ret = null;
            float4x4 origMV = _AABBXForm;

            // _AABBXForm = _AABBXForm * node.Transform.Matrix();
            throw new NotImplementedException("correctly handle transform");
            if (node.GetMesh() != null)
            {
                ret = _AABBXForm * node.GetMesh().BoundingBox;
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
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

        protected void VisitNodeRender(SceneNodeContainer node)
        {
            float4x4 origMV = _rc.ModelView;
            ShaderEffect origMat = CurMat;

            throw new NotImplementedException("correctly handle transform");
            // _rc.ModelView = _rc.ModelView * node.Transform.Matrix();

            if (node.GetMaterial() != null)
            {
                var mat = LookupMaterial(node.GetMaterial());
                CurMat = mat;
            }

            if (node.GetMesh() != null)
            {
                Mesh rm;
                if (!_meshMap.TryGetValue(node.GetMesh(), out rm))
                {
                    rm = MakeMesh(node);
                    _meshMap.Add(node.GetMesh(), rm);
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

            if (node.Children != null)
            {
                foreach (var child in node.Children)
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

            ShaderEffect ret = new ShaderEffect(new []
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
                    Value = (object) mc.Diffuse.Color
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
                    Value = (object) mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularShininessName,
                    Value = (object) mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = scb.SpecularIntensityName,
                    Value = (object) mc.Specular.Intensity
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
                    Value = (object) mc.Emissive.Color
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
                    Value = (float) 1
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
