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

        public void Render(RenderContext rc)
        {
            InitShaders(rc);

            foreach (var soc in _sc.Children)
            {
                VisitNode(soc);
            }
        }

        protected void VisitNode(SceneObjectContainer soc)
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
                    VisitNode(child);
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
    }
}
