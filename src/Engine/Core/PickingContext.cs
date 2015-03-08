using System;
using System.Collections.Generic;
using System.Drawing;
using Fusee.Math;

namespace Fusee.Engine
{
    public class PickingContext
    {
        private readonly RenderContext _rc;

        private bool _doPick;
        private int _x;
        private int _y;
        private int _w;
        private int _h;
        private float _screenX;
        private float _screenY;

        private int _height;
        private int _width;

        private PickType _pickType;

        public List<RayPickSet> RayPicklist { get; private set; }
        public List<ColorPickSet> ColorPicklist { get; private set; }

        /// <summary>
        /// Gets the pick results.
        /// </summary>
        /// <value>
        /// The pick results.
        /// </value>
        public List<PickResultSet> PickResults { get; private set; }

        #region Shaders

        private List<float4> _colorsFloat;
        private List<Color> _colorsInt;
        private int _colorIndex = 0;

        private ShaderProgram _spPlainColor;
        private IShaderParam _colorParam;

        private void PrepareShaderColors()
        {
            _colorsFloat.Clear();
            for (float b = 0; b < 1f; b = b + 0.07f)
            {
                for (float g = 0; g < 1f; g = g + 0.07f)
                {
                    for (float r = 0; r < 1f; r = r + 0.07f)
                    {
                        var _colorFloat = new float4(r, g, b, 1);

                        _colorsFloat.Add(new float4(_colorFloat));
                        _colorsInt.Add(Color.FromArgb((int)System.Math.Floor(r * 255), (int)System.Math.Floor(g * 255), (int)System.Math.Floor(b * 255)));
                        _colorsInt.Add(Color.FromArgb((int)System.Math.Ceiling(r * 255), (int)System.Math.Ceiling(g * 255), (int)System.Math.Ceiling(b * 255)));
                    }
                }
            }
        }

        private const string VsPlainColor = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;       
        
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
            }";

        private const string PsPlainColor = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            uniform vec4 color;

            void main()
            {             
                gl_FragColor = color;
            }";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PickingContext"/> class.
        /// </summary>
        /// <param name="rc">RenderContext</param>
        /// <param name="pt">The PickType for selecting the picking method.</param>
        /// <param name="autoTick">if set to <c>true</c>, attaches the PickingContext to the RenderContext and automatically calls Tick() after Present().</param>
        public PickingContext(RenderContext rc, PickType pt = PickType.Color, bool autoTick = true)
        {
            _rc = rc;

            if (autoTick)
            {
                _rc.AttachPickingContext(this);
            }

            _height = _rc.ViewportHeight;
            _width = _rc.ViewportWidth;

            _pickType = pt;

            RayPicklist = new List<RayPickSet>();
            ColorPicklist = new List<ColorPickSet>();
            PickResults = new List<PickResultSet>();

            if (_pickType == PickType.Color || _pickType == PickType.Mix)
            {
                _colorsFloat = new List<float4>();
                _colorsInt = new List<Color>();

                _spPlainColor = _rc.CreateShader(VsPlainColor, PsPlainColor);
                _colorParam = _spPlainColor.GetShaderParam("color");
                PrepareShaderColors();
            }
        }

        /// <summary>
        /// Adds a pickable object. The transformation will be taken from the attached RenderContext, make sure to set them propperly befor using this method.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="id">The identifier.</param>
        public void AddPickableObject(Mesh mesh, string id = "")
        {
            if (_pickType == PickType.Ray)
            {
                RayPicklist.Add(new RayPickSet(id, mesh, _rc));
            }
            else if (_pickType == PickType.Color || _pickType == PickType.Mix)
            {
                ColorPicklist.Add(new ColorPickSet(id, mesh, _rc, _colorsFloat[_colorIndex]));
                _colorIndex++;
            }
        }

        /// <summary>
        /// Adds a pickable object.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="viewMatrix">The view matrix.</param>
        public void AddPickableObject(Mesh mesh, string id, float4x4 modelMatrix, float4x4 viewMatrix)
        {
            if (_pickType == PickType.Ray)
            {
                RayPicklist.Add(new RayPickSet(id, mesh, modelMatrix, viewMatrix, _rc.Projection));
            }
            else if (_pickType == PickType.Color || _pickType == PickType.Mix)
            {
                ColorPicklist.Add(new ColorPickSet(id, mesh, modelMatrix, viewMatrix, _rc.Projection, _colorsFloat[_colorIndex]));
                _colorIndex++;
            }
        }


        /// <summary>
        /// This methode evaluates the picking, it is automaticaly called by Present() if autoTick isn't disabled.
        /// </summary>
        public void Tick()
        {
            if (_doPick)
            {
                _doPick = false;
                PickResults.Clear();

                if (_pickType == PickType.Ray)
                {
                    var _meshHits = new List<RayPickSet>();

                    //calculate hits with AABBs - eliminate non hits
                    foreach (RayPickSet rps in RayPicklist)
                    {
                        AABBf aabb = rps.AABB;
                        if (_screenX > aabb.min.x && _screenY > aabb.min.y && _screenX < aabb.max.x && _screenY < aabb.max.y)
                        {
                            _meshHits.Add(rps);
                        }
                    }

                    //calculate hits with triagles of AABB hits - eliminate non hits
                    if (_meshHits.Count > 0)
                    {
                        foreach (RayPickSet rps in _meshHits)
                        {
                            CalculateRayOnGeomentry(rps);
                        }
                    }
                }

                else if (_pickType == PickType.Color || _pickType == PickType.Mix)
                {
                    _rc.Clear(ClearFlags.Color | ClearFlags.Depth);

                    _rc.SetShader(_spPlainColor);
                    //render stuff
                    foreach (ColorPickSet cps in ColorPicklist)
                    {
                        _rc.Model = cps.Model;
                        _rc.View = cps.View;
                        _rc.Projection = cps.Projection;
                        _rc.SetShaderParam(_colorParam, cps.Color);
                        _rc.Render(cps.Mesh);
                    }

                    ImageData pickColor = _rc.GetPixelColor(_x, _y, _w, _h);

                    var x = pickColor.Width;
                    var y = pickColor.Height;

                    var pickedColors = new List<float3>();

                    for (int i = 0; i < x; i++)
                    {
                        for (int j = 0; j < y; j++)
                        {
                            var pickRed = pickColor.GetPixel(i, j).R;
                            var pickGreen = pickColor.GetPixel(i, j).G;
                            var pickBlue = pickColor.GetPixel(i, j).B;

                            var color = new float3(pickRed, pickGreen, pickBlue);

                            if (!pickedColors.Contains(color) && _colorsInt.Contains(Color.FromArgb(pickRed, pickGreen, pickBlue)))
                                pickedColors.Add(color);
                        }
                    }

                    foreach (var colorPickItem in ColorPicklist)
                    {
                        foreach (var pickedColor in pickedColors)
                        {
                            var upperObjectColor = new float3((int)System.Math.Ceiling(colorPickItem.Color.x * 255),
                                (int)System.Math.Ceiling(colorPickItem.Color.y * 255),
                                (int)System.Math.Ceiling(colorPickItem.Color.z * 255));
                            var lowerObjectColor = new float3((int)System.Math.Floor(colorPickItem.Color.x * 255),
                                (int)System.Math.Floor(colorPickItem.Color.y * 255),
                                (int)System.Math.Floor(colorPickItem.Color.z * 255));

                            if (pickedColor.Equals(lowerObjectColor) || pickedColor.Equals(upperObjectColor))
                            {
                                var result = new PickResultSet
                                {
                                    id = colorPickItem.Id,
                                    mesh = colorPickItem.Mesh,
                                    triangle = float3x3.Zero,
                                    point = float3.Zero
                                };

                                if (_pickType == PickType.Color)
                                {
                                    PickResults.Add(result);
                                }
                                else if (_pickType == PickType.Mix)
                                {
                                    var rps = new RayPickSet(colorPickItem.Id, colorPickItem.Mesh, colorPickItem.Model, colorPickItem.View, colorPickItem.Projection);
                                    CalculateRayOnGeomentry(rps);
                                }
                            }
                        }
                    }
                }
            }

            _colorIndex = 0;

            Clear();
        }

        /// <summary>
        /// Picks at the specified mousepos.
        /// </summary>
        /// <param name="mousepos">The mousepos.</param>
        public void Pick(Point mousepos, int w = 1, int h = 1)
        {
            Pick(mousepos.x, mousepos.y, w, h);
        }

        /// <summary>
        /// Picks at the specified coordinates.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        public void Pick(int x, int y, int w = 1, int h = 1)
        {
            y = _height - y;

            if (w != 1 || h != 1)
            {
                if (_pickType == PickType.Ray)
                {
                    Console.WriteLine("Values of w or h are ignored for PickType.Ray");
                    w = 1;
                    h = 1;
                }

                else if (_pickType == PickType.Color)
                {
                    x = x - (int)System.Math.Floor(w * 0.5d);
                    y = y - (int)System.Math.Floor(h * 0.5d);
                }
            }

            _x = x;
            _y = y;
            _w = w;
            _h = h;

            //Console.WriteLine(w + h);

            _screenX = (_x - _rc.ViewportWidth * 0.5f) / (_rc.ViewportWidth * 0.5f);
            _screenY = (_y - _rc.ViewportHeight * 0.5f) / (_rc.ViewportHeight * 0.5f);

            _doPick = true;
        }

        private bool IsPointInsideTriangle(float px, float py, float ax, float ay, float bx, float by, float cx, float cy)
        {
            return IsPointInsideTriangle(new float2(px, py), new float2(ax, ay), new float2(bx, by), new float2(cx, cy));
        }

        private bool IsPointInsideTriangle(float2 P, float2 A, float2 B, float2 C)
        {
            float planeAB = (A.x - P.x) * (B.y - P.y) - (B.x - P.x) * (A.y - P.y);
            float planeBC = (B.x - P.x) * (C.y - P.y) - (C.x - P.x) * (B.y - P.y);
            float planeCA = (C.x - P.x) * (A.y - P.y) - (A.x - P.x) * (C.y - P.y);
            //return false;
            return System.Math.Sign(planeAB) == System.Math.Sign(planeBC) && System.Math.Sign(planeBC) == System.Math.Sign(planeCA);
        }

        private float3 Unproject(float4x4 viewMatrix, Size viewport, Point mouse)
        {
            float depth = 0;

            depth = _rc.GetPixelDepth(mouse.x, mouse.y);

            float4 viewPosition = new float4
                (
                (((float)mouse.x + 0.5f) / (viewport.Width)) * 2.0f - 1.0f, // Map X to -1 to 1 range
                (((float)mouse.y + 0.5f) / (viewport.Height)) * 2.0f - 1.0f, // Map Y to -1 to 1 range
                1, //depth*2.0f - 1.0f, // Map Z to -1 to 1 range
                1.0f
                );

            float4 final4 = viewMatrix * viewPosition;

            final4.x /= final4.w;
            final4.y /= final4.w;
            final4.z /= final4.w;

            return new float3(final4.x, final4.y, final4.z);
        }

        private void CalculateRayOnGeomentry(RayPickSet rps)
        {
            if (rps.Mesh.TrianglesSet)
            {
                var triP = new float3[3];
                float4x4 mv = rps.View * rps.Model;
                float4x4 mvp = rps.Projection * mv;

                for (int j = 0; j < rps.Mesh.Triangles.Length; j += 3)
                {
                    triP[0] = mvp * rps.Mesh.Vertices[rps.Mesh.Triangles[j]];
                    triP[1] = mvp * rps.Mesh.Vertices[rps.Mesh.Triangles[j + 1]];
                    triP[2] = mvp * rps.Mesh.Vertices[rps.Mesh.Triangles[j + 2]];

                    float3 uP = triP[1] - triP[0];
                    float3 vP = triP[2] - triP[0];

                    float3 normV = float3.Cross(uP, vP);

                    if (IsPointInsideTriangle(_screenX, _screenY, triP[0].x, triP[0].y, triP[1].x, triP[1].y, triP[2].x, triP[2].y) && normV.z > 0)
                    {
                        //Console.WriteLine("Geomentry hit: " + j);


                        float3 A = mv * rps.Mesh.Vertices[rps.Mesh.Triangles[j]];
                        float3 B = mv * rps.Mesh.Vertices[rps.Mesh.Triangles[j + 1]];
                        float3 C = mv * rps.Mesh.Vertices[rps.Mesh.Triangles[j + 2]];

                        float3 P = new float3();

                        P = Unproject(rps.View, new Size(_rc.ViewportWidth, _rc.ViewportHeight), new Point { x = _x, y = _y });

                        float3 pickRay = rps.View.Column3.xyz - P;

                        var planeN = float3.Cross(B - A, C - A);

                        float3 point = (float3.Dot(planeN, A) /
                                        float3.Dot(planeN, pickRay)) *
                                       (pickRay);

                        var result = new PickResultSet { id = rps.Id, mesh = rps.Mesh, triangle = float3x3.Zero, point = point };
                        PickResults.Add(result);
                    }
                }
            }
        }

        public void ClearResults()
        {
            PickResults.Clear();
        }

        public void Clear()
        {
            RayPicklist.Clear();
            ColorPicklist.Clear();
        }

        #region Nested Classes
        public class RayPickSet
        {
            public string Id { get; set; }

            public Mesh Mesh { get; set; }

            public float4x4 Model { get; set; }

            public float4x4 View { get; set; }

            public float4x4 Projection { get; set; }

            private AABBf Aabb;
            private bool AabbIsSet = false;
            public AABBf AABB
            {
                get
                {
                    if (!AabbIsSet)
                        CalcAABB();

                    return Aabb;
                }
                set { Aabb = value; }
            }


            public RayPickSet(string id, Mesh mesh, float4x4 model, float4x4 view, float4x4 projection, AABBf aabb)
            {
                Id = id;
                Mesh = mesh;
                Model = model;
                View = view;
                Projection = projection;
                Aabb = aabb;
                AabbIsSet = true;
            }

            public RayPickSet(string id, Mesh mesh, float4x4 model, float4x4 view, float4x4 projection)
            {
                Id = id;
                Mesh = mesh;
                Model = model;
                View = view;
                Projection = projection;
            }

            public RayPickSet(string id, Mesh mesh, RenderContext rc)
            {
                Id = id;
                Mesh = mesh;
                Model = rc.Model;
                View = rc.View;
                Projection = rc.Projection;
            }

            private void CalcAABB()
            {
                //calculate AABBf
                var modelViewProjection = Projection * View * Model;

                var aabbfMin = modelViewProjection * Mesh.Vertices[0];
                var aabbfMax = modelViewProjection * Mesh.Vertices[0];

                for (int i = 1; i < Mesh.Vertices.Length; i++)
                {
                    var vertice = modelViewProjection * Mesh.Vertices[i];
                    if (vertice.x < aabbfMin.x)
                        aabbfMin.x = vertice.x;
                    if (vertice.y < aabbfMin.y)
                        aabbfMin.y = vertice.y;
                    if (vertice.z < aabbfMin.z)
                        aabbfMin.z = vertice.z;

                    if (vertice.x > aabbfMax.x)
                        aabbfMax.x = vertice.x;
                    if (vertice.y > aabbfMax.y)
                        aabbfMax.y = vertice.y;
                    if (vertice.z > aabbfMax.z)
                        aabbfMax.z = vertice.z;
                }
                Aabb = new AABBf(aabbfMin, aabbfMax);
                AabbIsSet = true;
            }
        }

        public class ColorPickSet
        {
            public string Id { get; set; }

            public Mesh Mesh { get; set; }

            public float4x4 Model { get; set; }

            public float4x4 View { get; set; }

            public float4x4 Projection { get; set; }

            public float4 Color { get; set; }

            public ColorPickSet(string id, Mesh mesh, float4x4 model, float4x4 view, float4x4 projection, float4 color)
            {
                Id = id;
                Mesh = mesh;
                Model = model;
                View = view;
                Projection = projection;
                Color = color;
            }

            public ColorPickSet(string id, Mesh mesh, RenderContext rc, float4 color)
            {
                Id = id;
                Mesh = mesh;
                Model = rc.Model;
                View = rc.View;
                Projection = rc.Projection;
                Color = color;
            }
        }
        #endregion
    }

    public struct PickResultSet
    {
        public string id;
        public Mesh mesh;
        public float3x3 triangle;
        public float3 point;
    }

    public enum PickType
    {
        Color,
        Ray,
        Mix
    }
}