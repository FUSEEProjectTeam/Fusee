using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.Serialization;

namespace Examples.SceneViewer
{

    public class SceneRenderer
    {
        internal delegate void SetParamFunc();
        internal struct SRMaterial
        {
            public ShaderProgram Shader;
            public List<SetParamFunc> ParamSetters;
            // public Dictionary<IShaderParam, object> Settings;
        };



        private Dictionary<MeshContainer, Mesh> _meshMap;
        private Dictionary<MaterialContainer, SRMaterial> _matMap;
        private SceneContainer _sc;

        private RenderContext _rc;
        private ShaderProgram _colorShader;
        private IShaderParam _colorParam;
        private ShaderProgram _textureShader;
        private IShaderParam _textureParam;
        private float4x4 _AABBXForm;

        private RenderStateSet _stateSet = new RenderStateSet()
        {
            AlphaBlendEnable = false,
            SourceBlend = Blend.One,
            DestinationBlend = Blend.Zero,
            ZEnable = true,
            ZFunc = Compare.Less
        };

        private SRMaterial _curMat;
        private string _scenePathDirectory;

        SRMaterial CurMat
        {
            set
            {
                if (_rc != null)
                {
                    _rc.SetShader(value.Shader);
                    foreach (var paramSetter in value.ParamSetters)
                    {
                        paramSetter();
                    }                    
                }
                _curMat = value;
            }
            get { return _curMat; }
        }


        public SceneRenderer(SceneContainer sc, string scenePathDirectory)
        {
            _sc = sc;
            _scenePathDirectory = scenePathDirectory;
        }

        public void InitShaders(RenderContext rc)
        {
            if (rc != _rc)
            {
                _rc = rc;
                _colorShader = null;
                _colorParam = null;
                _textureShader = null;
                _textureParam = null;
                _meshMap = new Dictionary<MeshContainer, Mesh>();
                _matMap = new Dictionary<MaterialContainer, SRMaterial>();

            }
            if (_colorShader == null)
            {
                _colorShader = MoreShaders.GetDiffuseColorShader(rc);
                _colorParam = _colorShader.GetShaderParam("color");
                
                var curMat = new SRMaterial()
                {
                    Shader = _colorShader,
                    ParamSetters = new List<SetParamFunc>(new SetParamFunc[] {
                        () => _rc.SetShaderParam(_colorParam, new float4(0.5f, 0.5f, 0.5f, 1))
                    })
                };
                CurMat = curMat;
            }
            if (_textureShader == null)
            {
                _textureShader = MoreShaders.GetTextureShader(rc);
                _textureParam = _textureShader.GetShaderParam("texture1");
            }
            rc.SetShader(_colorShader);
            rc.SetRenderState(_stateSet);
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

        protected AABBf? VisitNodeAABB(SceneObjectContainer soc)
        {
            AABBf? ret = null;
            float4x4 origMV = _AABBXForm;

            _AABBXForm = _AABBXForm * soc.Transform;
            if (soc.Mesh != null)
            {
                ret = _AABBXForm * soc.Mesh.BoundingBox;
            }

            if (soc.Children != null)
            {
                foreach (var child in soc.Children)
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

            foreach (var soc in _sc.Children)
            {
                VisitNodeRender(soc);
            }
        }

        protected void VisitNodeRender(SceneObjectContainer soc)
        {
            float4x4 origMV = _rc.ModelView;
            SRMaterial origMat = CurMat;

            if (soc.Material != null)
            {
                var srMat = LookupMaterial(soc.Material);
                CurMat = srMat;
            }
            _rc.ModelView = _rc.ModelView * soc.Transform;
            if (soc.Mesh != null)
            {
                Mesh rm;
                if (!_meshMap.TryGetValue(soc.Mesh, out rm))
                {
                    rm = MakeMesh(soc);
                    _meshMap.Add(soc.Mesh, rm);
                }
                _rc.Render(rm);
            }

            if (soc.Children != null)
            {
                foreach (var child in soc.Children)
                {
                    VisitNodeRender(child);
                }
            }

            _rc.ModelView = origMV;
            CurMat = origMat;
        }

        private SRMaterial LookupMaterial(MaterialContainer mc)
        {
            SRMaterial srMat;
            if (!_matMap.TryGetValue(mc, out srMat))
            {
                srMat = MakeMaterial(mc);
                _matMap.Add(mc, srMat);
            }
            return srMat;
        }

        private static Mesh MakeMesh(SceneObjectContainer soc)
        {
            Mesh rm;
            rm = new Mesh()
            {
                Colors = null,
                Normals = soc.Mesh.Normals,
                UVs = soc.Mesh.UVs,
                Vertices = soc.Mesh.Vertices,
                Triangles = soc.Mesh.Triangles
            };
            return rm;
        }

        private SRMaterial MakeMaterial(MaterialContainer mc)
        {
            SRMaterial ret = new SRMaterial();
            ret.ParamSetters = new List<SetParamFunc>();
            if (mc.HasDiffuse)
            {
                if (mc.Diffuse.Texture == null)
                {
                    ret.Shader = _colorShader;
                    ret.ParamSetters.Add(delegate()
                    {
                        _rc.SetShaderParam(_colorParam,
                            new float4(mc.Diffuse.Color, 1));
                    });
                }
                else
                {
                    ret.Shader = _textureShader;
                    string texturePath = Path.Combine(_scenePathDirectory, mc.Diffuse.Texture);
                    var image = _rc.LoadImage(texturePath);
                    var texHandle = _rc.CreateTexture(image);
                    ret.ParamSetters.Add(delegate()
                    {
                        _rc.SetShaderParamTexture(_textureParam, texHandle);
                    });
                }
            }
            return ret;
        }

        /*
        private SRMaterial MakeMaterial(MaterialContainer mc)
        {
            SRMaterial ret=new SRMaterial();
            ret.ParamSetters = new List<SetParamFunc>();
            if (mc.DiffuseTexure == null)
            {
                ret.Shader = _colorShader;
                ret.ParamSetters.Add(delegate()
                {
                    _rc.SetShaderParam(_colorParam, new float4(mc.DiffuseColor.x, mc.DiffuseColor.y, mc.DiffuseColor.z, 1));
                });
            }
            else
            {
                ret.Shader = _textureShader;
                string texturePath = Path.Combine(_scenePathDirectory, mc.DiffuseTexure);
                var image = _rc.LoadImage(texturePath);
                var texHandle = _rc.CreateTexture(image);
                ret.ParamSetters.Add(delegate()
                {
                    _rc.SetShaderParamTexture(_textureParam, texHandle);
                });
            }
            return ret;
        }
        */
    }
}
