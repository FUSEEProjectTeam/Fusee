using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.Serialization;

namespace Examples.SceneViewer
{
    public class SceneRenderer
    {
        private Dictionary<MeshContainer, Mesh> _meshMap;
        private SceneContainer _sc;

        private RenderContext _rc;
        private ShaderProgram _shader;
        private IShaderParam _colorParam;

        private RenderStateSet _stateSet = new RenderStateSet()
        {
            AlphaBlendEnable = false,
            SourceBlend = Blend.One,
            DestinationBlend = Blend.Zero,
            ZEnable = true,
            ZFunc = Compare.Less
        };

        private float3 _curCol;
        float3 CurCol
        {
            set
            {
                if (_rc != null)
                    _rc.SetShaderParam(_colorParam, new float4(value.x, value.y, value.z, 1));
                _curCol = value;
            }
            get { return _curCol; }
        }


        public SceneRenderer(SceneContainer sc)
        {
            _sc = sc;
            _meshMap = new Dictionary<MeshContainer, Mesh>();
        }

        public void Render(RenderContext rc)
        {
            if (rc != _rc)
            {
                _rc = rc;
                _shader = null;
                _colorParam = null;
                _curCol = new float3(0.5f, 0.5f, 0.5f);
            }
            if (_shader == null)
            {
                _shader = MoreShaders.GetDiffuseColorShader(rc);
                _colorParam = _shader.GetShaderParam("color");
            }
            rc.SetShader(_shader);
            rc.SetRenderState(_stateSet);
            rc.SetShaderParam(_colorParam, new float4(_curCol.x, _curCol.y, _curCol.z, 1));

            foreach (var soc in _sc.Children)
            {
                VisitNode(soc);
            }
        }

        protected void VisitNode(SceneObjectContainer soc)
        {
            float4x4 origMV = _rc.ModelView;
            float3 origCol = CurCol;

            if (soc.Material != null)
                CurCol = soc.Material.DiffuseColor;
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
            CurCol = origCol;
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
    }
}
