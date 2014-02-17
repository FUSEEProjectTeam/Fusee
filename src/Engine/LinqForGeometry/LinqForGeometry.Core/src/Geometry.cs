/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachelor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Fusee.Math;
using Fusee.Engine;
using LinqForGeometry.Core.Exceptions;
using LinqForGeometry.Core.Handles;
using LinqForGeometry.Core.Importer;
using LinqForGeometry.Core.PtrContainer;

namespace LinqForGeometry.Core
{
    /// <summary>
    /// This is the main object for the LINQForGeometry project.
    /// This object contains a complete model as a mesh and the basic iterators.
    /// </summary>
    public class Geometry
    {
        private WavefrontImporter<float3> _objImporter;

        // Boolean helpers
        /// <summary>
        /// Bool. Should be set to true, if geometrical changes have been done to the mesh.
        /// </summary>
        public bool _Changes = false;
        private bool _VertexNormalActive = false;
        /// <summary>
        /// Accessor for the _VertexNormalActive Var. Set this to true to enable vertex normal calculation and false to not do the calculation.
        /// </summary>
        public bool _DoCalcVertexNormals
        {
            set { _VertexNormalActive = value; }
            get { return _VertexNormalActive; }
        }
        private bool _UsesTriangles = false;

        // Handles to pointer containers
        /// <summary>
        /// This list contains the handles to vertices.
        /// </summary>
        public List<HandleVertex> _LverticeHndl;
        /// <summary>
        /// This list contains handles to edges
        /// </summary>
        public List<HandleEdge> _LedgeHndl;
        /// <summary>
        /// This list contains handles to faces
        /// </summary>
        public List<HandleFace> _LfaceHndl;

        // Pointer containers
        private List<VertexPtrCont> _LvertexPtrCont;
        private List<HEdgePtrCont> _LhedgePtrCont;
        private List<EdgePtrCont> _LedgePtrCont;
        private List<FacePtrCont> _LfacePtrCont;

        // Real data
        /// <summary>
        /// Contains real vertex data as float3.
        /// </summary>
        public List<float3> _LvertexVal;
        /// <summary>
        /// Contains real face normal data as float3.
        /// </summary>
        public List<float3> _LfaceNormals;
        /// <summary>
        /// Contains real vertex normal data as float3
        /// </summary>
        public List<float3> _LVertexNormals;
        private List<float2> _LuvCoordinates;
        private List<float3> _LvertexValDefault;

        // Various runtime constants
        /// <summary>
        /// The smoothing angle for the edged based vertex normal calculation.
        /// Default is 89.9degrees.
        /// </summary>
        public double _SmoothingAngle = 89.9;
        private const double _constPiFactor = 180 / 3.141592;

        // For mesh conversion
        private List<ushort> _LtrianglesFuseeMesh;
        private List<float3> _LvertDataFuseeMesh;
        private List<float3> _LvertNormalsFuseeMesh;
        private List<float2> _LvertuvFuseeMesh;

        // Performance optimizing tools
        private Stopwatch _NormalCalcStopWatch;

        /// <summary>
        /// Constructor for the GeometryData class.
        /// </summary>
        public Geometry()
        {
            _objImporter = new WavefrontImporter<float3>();

            _LverticeHndl = new List<HandleVertex>();
            _LedgeHndl = new List<HandleEdge>();
            _LfaceHndl = new List<HandleFace>();

            _LvertexPtrCont = new List<VertexPtrCont>();
            _LhedgePtrCont = new List<HEdgePtrCont>();
            _LedgePtrCont = new List<EdgePtrCont>();
            _LfacePtrCont = new List<FacePtrCont>();

            _LvertexVal = new List<float3>();
            _LfaceNormals = new List<float3>();
            _LVertexNormals = new List<float3>();
            _LuvCoordinates = new List<float2>();

            // For mesh conversion
            _LtrianglesFuseeMesh = new List<ushort>();
            _LvertDataFuseeMesh = new List<float3>();
            _LvertNormalsFuseeMesh = new List<float3>();
            _LvertuvFuseeMesh = new List<float2>();

            // Performance stuff
            _NormalCalcStopWatch = new Stopwatch();
        }

        /// <summary>
        /// Loads an asset specified by the path string.
        /// </summary>
        /// <param name="path">Path to the wavefront.obj.model file.</param>
        public void LoadAsset(String path)
        {
            /*
                Stopwatch stopWatch = new Stopwatch();
                TimeSpan timeSpan = new TimeSpan();
                String timeDone;

                if (Debugger.IsAttached)
                    stopWatch.Start();
             */

            List<GeoFace> faceList = _objImporter.LoadAsset(path);

            /*
                timeSpan = stopWatch.Elapsed;
                timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
                Debug.WriteLine("\n\n     Time needed to import the .obj file: " + timeDone);
                stopWatch.Restart();
                Debug.WriteLine(LFGMessages.INFO_PROCESSINGDS);
             */

            // Work on the facelist and transform the data structure to the 'half-edge' data structure.
            foreach (GeoFace gf in faceList)
            {
                AddFace(gf);
            }

            /*
                if (LFGMessages.FLAG_FUSEE_TRIANGLES)
                {
                    stopWatch.Stop();
                    timeSpan = stopWatch.Elapsed;
                    timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
                    Debug.WriteLine("\n\n     Time needed to convert the object to the HES: " + timeDone);
                }
             */

            _LfaceNormals.Clear();
            foreach (HandleFace face in _LfaceHndl)
            {
                CalcFaceNormal(face);
            }

            foreach (HandleVertex vertex in _LverticeHndl)
            {
                CalcVertexNormal(vertex);
            }

            // This is just for now for debugging
            SetVertexDefaults();
        }

        /// <summary>
        /// This method converts the data structure to a fusee readable mesh structure
        /// </summary>
        /// <param name="shouldUseTriangulation">Boolean. True if the mesh should be triangulated.</param>
        /// <returns>A fusee readable Mesh object</returns>
        public Mesh ToMesh(bool shouldUseTriangulation = true)
        {
            if (shouldUseTriangulation && !_UsesTriangles)
            {
                TriangulateGeometry();
                _UsesTriangles = true;
            }

            _LfaceNormals.Clear();
            foreach (HandleFace faceHandle in _LfaceHndl)
            {
                CalcFaceNormal(faceHandle);
            }


            if (_VertexNormalActive)
            {
                /*
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    _NormalCalcStopWatch.Reset();
                    _NormalCalcStopWatch.Start();
                }
                 */

                _LVertexNormals.Clear();
                _LverticeHndl.ForEach(CalcVertexNormal);

                /*
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    _NormalCalcStopWatch.Stop();
                    Debug.WriteLine("Time taken to compute vertex normals: " + _NormalCalcStopWatch.ElapsedMilliseconds + " ms");
                }
                 */
            }

            _LtrianglesFuseeMesh.Clear();
            _LvertDataFuseeMesh.Clear();
            _LvertNormalsFuseeMesh.Clear();
            _LvertuvFuseeMesh.Clear();

            foreach (HandleFace faceHandle in _LfaceHndl)
            {
                foreach (HEdgePtrCont currentContainer in EnFaceAdjacentHalfEdges(faceHandle).Select(handleHalfEdge => _LhedgePtrCont[handleHalfEdge]))
                {
                    _LvertDataFuseeMesh.Add(_LvertexVal[currentContainer._v]);
                    if (_VertexNormalActive || _LVertexNormals != null)
                    {
                        if (currentContainer._vn.isValid)
                            _LvertNormalsFuseeMesh.Add(_LVertexNormals[currentContainer._vn]);
                    }
                    if (_LuvCoordinates.Count > 0)
                    {
                        _LvertuvFuseeMesh.Add(_LuvCoordinates[currentContainer._vuv]);
                    }
                    _LtrianglesFuseeMesh.Add((ushort)(_LvertDataFuseeMesh.Count - 1));
                }
            }

            Mesh fuseeMesh = new Mesh();
            fuseeMesh.Vertices = _LvertDataFuseeMesh.ToArray();

            if (_VertexNormalActive || _LvertNormalsFuseeMesh != null)
                fuseeMesh.Normals = _LvertNormalsFuseeMesh.ToArray();

            fuseeMesh.UVs = _LvertuvFuseeMesh.ToArray();
            fuseeMesh.Triangles = _LtrianglesFuseeMesh.ToArray();

            return fuseeMesh;
        }

        /// <summary>
        /// This method converts a quad based 'Geometry' object to a triangle based one.
        /// </summary>
        private void TriangulateGeometry()
        {
            List<HandleFace> LtmpFaces = new List<HandleFace>();

            foreach (HandleFace currentFace in _LfaceHndl)
            {
                // Pruefe zuerst ob man das face triangulaten sollte oder nicht.
                if (EnFaceAdjacentHalfEdges(currentFace).Count() == 3)
                    continue;

                // Hole aktuelles face und merke den index.
                FacePtrCont currentFaceCont = _LfacePtrCont[currentFace];
                // Merke erste hedge h0.
                HandleHalfEdge h0H = currentFaceCont._h;
                HEdgePtrCont h0Cont = _LhedgePtrCont[h0H];
                // Merke ersten vert v0.
                HandleVertex v0H = _LhedgePtrCont[h0Cont._he]._v;
                // Merke die letzte hedge im face hl.
                //HandleHalfEdge hlH = RetLastHalfEdgeInFaceCw(currentFace);

                var temp = EnFaceAdjacentHalfEdges(currentFace);


                HandleHalfEdge hlH = temp.ElementAt(temp.Count() - 1);
                HEdgePtrCont hlCont = _LhedgePtrCont[hlH];
                // Lege zwei neue hedges an und fülle sie korrekt.
                int hedgeCount = _LhedgePtrCont.Count;
                HandleHalfEdge hedge0H = new HandleHalfEdge() { _DataIndex = hedgeCount };
                HandleHalfEdge hedge1H = new HandleHalfEdge() { _DataIndex = hedgeCount + 1 };
                HandleEdge edgeHNew = new HandleEdge() { _DataIndex = _LedgeHndl.Count };
                EdgePtrCont edgeContNew = new EdgePtrCont() { _he1 = hedge0H, _he2 = hedge1H };

                HEdgePtrCont newhedge0 = new HEdgePtrCont()
                {
                    _nhe = h0H,
                    _v = v0H,
                    _he = hedge1H,
                    _f = currentFace,
                    _vn = hlCont._vn,
                    _vuv = hlCont._vuv
                };
                // Hole h1 und h2 zum Merken.
                HandleHalfEdge h1H = h0Cont._nhe;
                HEdgePtrCont h1Cont = _LhedgePtrCont[h1H];
                HandleHalfEdge h2H = h1Cont._nhe;
                HEdgePtrCont h2Cont = _LhedgePtrCont[h2H];

                HEdgePtrCont newhedge1 = new HEdgePtrCont()
                {
                    _nhe = h1Cont._nhe,
                    _v = h1Cont._v,
                    _he = hedge0H,
                    _f = new HandleFace(-1),
                    _vn = h1Cont._vn,
                    _vuv = h1Cont._vuv,
                };
                // Update die jeweiligen next pointer der angrenzenden hedges.
                h1Cont._nhe = hedge0H;
                hlCont._nhe = hedge1H;
                // Lege ein neues face an für das triangle 2.
                HandleFace f1H = new HandleFace() { _DataIndex = (_LfaceHndl.Count - 1) + LtmpFaces.Count + 1 };
                FacePtrCont f1Cont = new FacePtrCont() { _fn = currentFaceCont._fn, _h = hlH };
                // Update das neue triangle bezüglich des neuen faces. Dazu erstmal h2 holen noch.
                newhedge1._f = f1H;
                h2Cont._f = f1H;
                hlCont._f = f1H;
                // Sichere die Änderungen in den listen.
                _LedgeHndl.Add(edgeHNew);
                _LedgePtrCont.Add(edgeContNew);
                _LhedgePtrCont.Add(newhedge0);
                _LhedgePtrCont.Add(newhedge1);

                // Speichere das face handle erstmal in tmp faces wegen der iteration.
                LtmpFaces.Add(f1H);
                _LfacePtrCont.Add(f1Cont);

                _LhedgePtrCont[h1H] = new HEdgePtrCont()
                {
                    _f = h1Cont._f,
                    _he = h1Cont._he,
                    _nhe = h1Cont._nhe,
                    _v = h1Cont._v,
                    _vn = h1Cont._vn,
                    _vuv = h1Cont._vuv
                };

                _LhedgePtrCont[h2H] = new HEdgePtrCont()
                {
                    _f = h2Cont._f,
                    _he = h2Cont._he,
                    _nhe = h2Cont._nhe,
                    _v = h2Cont._v,
                    _vn = h2Cont._vn,
                    _vuv = h2Cont._vuv
                };

                _LhedgePtrCont[hlH] = new HEdgePtrCont()
                {
                    _f = hlCont._f,
                    _he = hlCont._he,
                    _nhe = hlCont._nhe,
                    _v = hlCont._v,
                    _vn = hlCont._vn,
                    _vuv = hlCont._vuv
                };
            }

            foreach (HandleFace handleFace in LtmpFaces)
            {
                _LfaceHndl.Add(handleFace);
            }
            LtmpFaces.Clear();
        }

        /// <summary>
        /// Adds a vertex to the geometry container.
        /// Will return a handle to the newly inserted or still existing vertex.
        /// </summary>
        /// <param name="val">float3 value to insert</param>
        /// <returns>Returns a handle to the just inserted vertex or a handle to an existing one because the given one was already inserterd.</returns>
        public HandleVertex AddVertex(float3 val)
        {
            int index = DoesVertexExist(val);
            HandleVertex hvToAdd = new HandleVertex() { _DataIndex = -1 };

            // When vertex does not exist - insert it
            if (index == -1)
            {
                _LvertexVal.Add(val);

                _LvertexPtrCont.Add(
                    new VertexPtrCont()
                    {
                        _h = new HandleHalfEdge()
                        {
                            _DataIndex = -1
                        }
                    }
                );
                hvToAdd = new HandleVertex() { _DataIndex = _LvertexPtrCont.Count - 1 };
            }
            else
            {
                HandleVertex vHndl = new HandleVertex();
                vHndl._DataIndex = index;
                hvToAdd = vHndl;
            }

            if (!_LverticeHndl.Contains(hvToAdd))
            {
                _LverticeHndl.Add(hvToAdd);
            }
            else
            {
                if (LFGMessages._DEBUGOUTPUT)
                {
                    Debug.WriteLine("$$$ Vertex has been already inserted!");
                }
            }
            return hvToAdd;
        }

        /// <summary>
        /// Checks if a vertex already exists in the value list
        /// </summary>
        /// <param name="v">VertexType parameter (e.g. float3)</param>
        /// <returns>boolean, true if vertex does alreadyexist</returns>
        private int DoesVertexExist(float3 v)
        {
            int index = _LvertexVal.FindIndex(vert => vert.Equals(v));
            return index >= 0 ? index : -1;
        }

        /// <summary>
        /// Adds a face from the importer to the geometry container
        /// </summary>
        /// <param name="gf">GeoFace object from the importer</param>
        private void AddFace(GeoFace gf)
        {
            // Add a face container.
            _LfacePtrCont.Add(
                new FacePtrCont()
                {
                    _h = new HandleHalfEdge()
                    {
                        _DataIndex = -1
                    }
                }
                );
            // Add a face handle.
            _LfaceHndl.Add(
                new HandleFace() { _DataIndex = _LfacePtrCont.Count - 1 }
                );

            // Insert all the vertices for the face.
            List<HandleVertex> LHandleVertsForFace = new List<HandleVertex>();
            foreach (float3 vVal in gf._LFVertices)
            {
                LHandleVertsForFace.Add(
                    AddVertex(vVal)
                    );
            }
            // Insert all the uv coordinates for the face.
            List<HandleVertexUV> LHandleUVsForFace = new List<HandleVertexUV>();
            foreach (float2 uvVal in gf._UV)
            {
                _LuvCoordinates.Add(uvVal);
                LHandleUVsForFace.Add(new HandleVertexUV() { _DataIndex = _LuvCoordinates.Count - 1 });
            }

            // Build up the half-edge connections for the face
            List<HandleHalfEdge> LHandleHEForFace = new List<HandleHalfEdge>();
            for (int i = 0; i < LHandleVertsForFace.Count; i++)
            {
                HandleVertex fromVert = LHandleVertsForFace[i];
                HandleVertex toVert = i + 1 < LHandleVertsForFace.Count ? LHandleVertsForFace[i + 1] : LHandleVertsForFace[0];

                LHandleHEForFace.Add(
                        CreateConnection(fromVert, toVert)
                    );
            }

            // Loop over all the half-edges for the face and concat them and set the correct uv coordinates.
            for (int i = 0; i < LHandleHEForFace.Count; i++)
            {
                HandleHalfEdge currentHedge = LHandleHEForFace[i];
                HEdgePtrCont hedge = _LhedgePtrCont[currentHedge];
                HandleHalfEdge nextHedge = i + 1 < LHandleHEForFace.Count ? LHandleHEForFace[i + 1] : LHandleHEForFace[0];
                hedge._nhe = nextHedge;
                if (LHandleUVsForFace.Count > 0)
                {
                    HandleVertexUV currentUV = i + 1 < LHandleUVsForFace.Count ? LHandleUVsForFace[i + 1] : LHandleUVsForFace[0];
                    hedge._vuv = currentUV;
                }
                //_LhedgePtrCont.RemoveAt(currentHedge);
                //_LhedgePtrCont.Insert(currentHedge, hedge);
                _LhedgePtrCont[currentHedge] = new HEdgePtrCont()
                {
                    _f = hedge._f,
                    _he = hedge._he,
                    _nhe = hedge._nhe,
                    _v = hedge._v,
                    _vn = hedge._vn,
                    _vuv = hedge._vuv
                };
            }

            // Set the half-edge the face points to.
            FacePtrCont face = _LfacePtrCont[_LfacePtrCont.Count - 1];
            face._h = new HandleHalfEdge(LHandleHEForFace.First());
            _LfacePtrCont.RemoveAt(_LfacePtrCont.Count - 1);
            _LfacePtrCont.Add(face);
        }

        /// <summary>
        /// Establishes a connection between two vertices.
        /// 1) Creates two half-edges
        /// 2) Fills them with information
        /// 3) Creates an edge pointer container and adds it to the geo container.
        /// 4) returns a handle to an edge
        /// </summary>
        /// <param name="fromVert">HandleVertex from which vertex</param>
        /// <param name="toVert">Handlevertex to which vertex</param>
        /// <returns>Returns a handle to the half-edge that has just been inserted</returns>
        public HandleHalfEdge CreateConnection(HandleVertex fromVert, HandleVertex toVert)
        {
            // Check if the connection does already exist.
            HandleEdge existingEdge = DoesConnectionExist(fromVert, toVert);
            if (existingEdge != -1)
            {
                return ReuseExistingConnection(existingEdge, fromVert, toVert);
            }
            else
            {
                return CreateAllNewConnection(fromVert, toVert);
            }
        }

        /// <summary>
        /// For testing only now.
        /// </summary>
        /// <param name="existingEdge"></param>
        /// <param name="fromVert"></param>
        /// <param name="toVert"></param>
        /// <returns></returns>
        private HandleHalfEdge ReuseExistingConnection(HandleEdge existingEdge, HandleVertex fromVert, HandleVertex toVert)
        {
            // Check half-edge 1 and 2 if one points to the actual face. This is the one we use for our face then. If no one we build a new connection.
            HEdgePtrCont hedge1 = _LhedgePtrCont[_LedgePtrCont[existingEdge]._he1];
            HEdgePtrCont hedge2 = _LhedgePtrCont[_LedgePtrCont[existingEdge]._he2];

            HandleHalfEdge hedgeToUse = new HandleHalfEdge(-1);

            if (hedge2._f == -1)
            {
                // It is hedge 2 that is free. We should use it.
                hedgeToUse = _LedgePtrCont[existingEdge]._he2;

            }
            else if (hedge1._f == -1)
            {
                // It is hedge 1 that is free. We should use it. Should never happen. TODO: Exception throw?
                hedgeToUse = _LedgePtrCont[existingEdge]._he1;
            }
            else
            {
                // Neither one of the faces of the existing half-edges was free so we build a new edge.
                return CreateAllNewConnection(fromVert, toVert);
            }
            // Updating the face pointer.
            HEdgePtrCont hedge = _LhedgePtrCont[hedgeToUse];
            hedge._f = new HandleFace(_LfacePtrCont.Count - 1);

            _LhedgePtrCont[hedgeToUse] = new HEdgePtrCont()
            {
                _f = hedge._f,
                _he = hedge._he,
                _nhe = hedge._nhe,
                _v = hedge._v,
                _vn = hedge._vn,
                _vuv = hedge._vuv
            };

            return hedgeToUse;
        }

        /// <summary>
        /// For testing now only.
        /// </summary>
        /// <returns></returns>
        private HandleHalfEdge CreateAllNewConnection(HandleVertex fromVert, HandleVertex toVert)
        {
            HEdgePtrCont hedge1 = new HEdgePtrCont()
            {
                _f = new HandleFace(_LfacePtrCont.Count - 1),
                _he = new HandleHalfEdge(_LedgePtrCont.Count == 0 ? 1 : _LhedgePtrCont.Count + 1),
                _v = new HandleVertex(toVert),
                _vn = new HandleVertexNormal(-1),
                _vuv = new HandleVertexUV(-1),
                _nhe = new HandleHalfEdge(-1)
            };

            HEdgePtrCont hedge2 = new HEdgePtrCont()
            {
                _f = new HandleFace(-1),
                _he = new HandleHalfEdge(_LedgePtrCont.Count == 0 ? 0 : _LhedgePtrCont.Count),
                _v = new HandleVertex(fromVert),
                _vn = new HandleVertexNormal(-1),
                _vuv = new HandleVertexUV(-1),
                _nhe = new HandleHalfEdge(-1)
            };

            _LhedgePtrCont.Add(hedge1);
            _LhedgePtrCont.Add(hedge2);

            _LedgePtrCont.Add(
                new EdgePtrCont()
                {
                    _he1 = new HandleHalfEdge(_LhedgePtrCont.Count - 2),
                    _he2 = new HandleHalfEdge(_LhedgePtrCont.Count - 1)
                }
                );
            _LedgeHndl.Add(
                new HandleEdge() { _DataIndex = _LedgePtrCont.Count - 1 }
                );

            // Update the vertices.
            VertexPtrCont vertFrom = _LvertexPtrCont[fromVert._DataIndex];
            VertexPtrCont vertTo = _LvertexPtrCont[toVert._DataIndex];

            if (!vertFrom._h.isValid)
            {
                vertFrom._h = _LedgePtrCont[_LedgePtrCont.Count - 1]._he1;
                _LvertexPtrCont[fromVert] = new VertexPtrCont() { _h = vertFrom._h };
            }
            if (!vertTo._h.isValid)
            {
                vertTo._h = _LedgePtrCont[_LedgePtrCont.Count - 1]._he2;
                _LvertexPtrCont[toVert] = new VertexPtrCont() { _h = vertTo._h };
            }

            return _LedgePtrCont[_LedgePtrCont.Count - 1]._he1;
        }

        /// <summary>
        /// Only for testing now.
        /// </summary>
        /// <param name="fromVert"></param>
        /// <param name="toVert"></param>
        /// <returns></returns>
        private HandleEdge DoesConnectionExist(HandleVertex fromVert, HandleVertex toVert)
        {
            return new HandleEdge(
                _LedgePtrCont.FindIndex(
                    edgePtrCont => _LhedgePtrCont[edgePtrCont._he1]._v == fromVert && _LhedgePtrCont[edgePtrCont._he2]._v == toVert || _LhedgePtrCont[edgePtrCont._he1]._v._DataIndex == toVert && _LhedgePtrCont[edgePtrCont._he2]._v == fromVert)
                );
        }

        /// <summary>
        /// This method adds a face normal vector to a list.
        /// The vector is calculated for the face which handle the method expects.
        /// </summary>
        /// <param name="faceHandle">Handle to a face to calculate the normal for.</param>
        public void CalcFaceNormal(HandleFace faceHandle)
        {
            List<HandleVertex> tmpList = EnFaceAdjacentVertices(faceHandle).ToList();
            if (tmpList.Count < 3)
                return;

            var v0 = _LvertexVal[tmpList[0]];
            var v1 = _LvertexVal[tmpList[1]];
            var v2 = _LvertexVal[tmpList[2]];

            float3 c1 = float3.Subtract(v0, v1);
            float3 c2 = float3.Subtract(v0, v2);
            float3 n = float3.Cross(c1, c2);

            _LfaceNormals.Add(float3.Normalize(n));

            FacePtrCont fc = _LfacePtrCont[faceHandle];
            _LfacePtrCont[faceHandle] = new FacePtrCont()
            {
                _fn = new HandleFaceNormal(_LfaceNormals.Count - 1),
                _h = fc._h
            };
        }

        /// <summary>
        /// This method calculates vertex normals for a specific vertex in the geometry and inserts them at the corresponding half-edges on the correct faces.
        /// This method uses an angle based algorithm to determine whether to calculate with another faces normal or not.
        /// </summary>
        /// <param name="vertexHandle">A handle for the vertex to calc the normals for.</param>
        public void CalcVertexNormal(HandleVertex vertexHandle)
        {
            List<HandleHalfEdge> EincomingHEdges = EnVertexIncomingHalfEdge(vertexHandle).ToList();

            // Loop over every incoming half-edge.
            foreach (HandleHalfEdge handleHedge in EincomingHEdges)
            {
                int hedgeIndex = handleHedge;
                // Check if the half-edge is pointing to a face.
                int faceIndex = _LhedgePtrCont[hedgeIndex]._f;
                if (faceIndex == -1)
                    return;

                float3 currentFaceNormal = _LfaceNormals[_LfacePtrCont[faceIndex]._fn];
                float3 normalAggregate = new float3();
                // Loop over every incoming half-edge again, so we can compare the angles between the current one and all the others.
                // We do this to decide which normal should be added to the sum and which not.
                foreach (int eincomingHEdge in EincomingHEdges)
                {
                    // Add the current normal if the index is on it and do not compare any angles etc.
                    if (eincomingHEdge == hedgeIndex)
                    {
                        normalAggregate += currentFaceNormal;
                        continue;
                    }
                    // Stop when the current half-edge is not pointing to a face.
                    int faceIndex2 = _LhedgePtrCont[eincomingHEdge]._f;
                    if (faceIndex2 == -1)
                        continue;

                    float3 normalToCompare = _LfaceNormals[_LfacePtrCont[faceIndex2]._fn];

                    float dot = float3.Dot(currentFaceNormal, normalToCompare);
                    if (System.Math.Acos(dot) * _constPiFactor < _SmoothingAngle)
                        normalAggregate += float3.Add(normalAggregate, normalToCompare);
                }

                _LVertexNormals.Add(float3.Normalize(normalAggregate));

                HEdgePtrCont currentHedge = _LhedgePtrCont[hedgeIndex];
                _LhedgePtrCont[hedgeIndex] = new HEdgePtrCont()
                {
                    _f = currentHedge._f,
                    _he = currentHedge._he,
                    _nhe = currentHedge._nhe,
                    _v = currentHedge._v,
                    _vn = new HandleVertexNormal(_LVertexNormals.Count - 1),
                    _vuv = currentHedge._vuv
                };
            }
        }

        /// <summary>
        /// Updates the "inner" half edges clockwise so the next pointers are correct.
        /// Is called after a face is inserted.
        /// </summary>
        /// <param name="edgeList">A list of edges that belong to a specific face</param>
        private void UpdateCWHedges(List<HandleEdge> edgeList)
        {
            // Proceed the loop for every edge and connect "hedge1" to the next hedge.
            for (int i = 0; i < edgeList.Count; i++)
            {
                // Test if the hedge1 or hedge2 is used. Decide by looking at their face pointers.
                int indexhedge1 = _LedgePtrCont[edgeList[i]._DataIndex]._he1;
                int indexhedge2 = _LedgePtrCont[edgeList[i]._DataIndex]._he2;
                HEdgePtrCont hedgePtrCont1 = _LhedgePtrCont[indexhedge1];
                HEdgePtrCont hedgePtrCont2 = _LhedgePtrCont[indexhedge2];

                if (hedgePtrCont1._f == _LfacePtrCont.Count - 1)
                {
                    #region UseHEdge1
                    // The face the edge1 points to is the active face - hurray! use this edge and move on.
                    if (i + 1 < edgeList.Count)
                    {
                        // Just use the next hedge
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[i + 1]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont1._nhe._DataIndex;
                        }
                        else
                        {
                            // use second
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont1._nhe._DataIndex;
                        }
                        _LhedgePtrCont[indexhedge1] = new HEdgePtrCont()
                        {
                            _f = hedgePtrCont1._f,
                            _he = hedgePtrCont1._he,
                            _nhe = hedgePtrCont1._nhe,
                            _v = hedgePtrCont1._v,
                            _vn = hedgePtrCont1._vn,
                            _vuv = hedgePtrCont1._vuv
                        };
                    }
                    else
                    {
                        // Connect to the first hedge in the list because the current is the last one in the face
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont1._nhe._DataIndex;
                        }
                        else
                        {
                            // use second
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont1._nhe._DataIndex;
                        }
                        _LhedgePtrCont[indexhedge1] = new HEdgePtrCont()
                        {
                            _f = hedgePtrCont1._f,
                            _he = hedgePtrCont1._he,
                            _nhe = hedgePtrCont1._nhe,
                            _v = hedgePtrCont1._v,
                            _vn = hedgePtrCont1._vn,
                            _vuv = hedgePtrCont1._vuv
                        };
                    }
                    #endregion UseHEdge1
                }
                else if (hedgePtrCont2._f == _LfacePtrCont.Count - 1 || hedgePtrCont2._f == -1)
                {
                    #region UseHEdge2
                    hedgePtrCont2._f._DataIndex = _LfacePtrCont.Count - 1;
                    // The face the edge2 points to is the current face - let's use the second one then
                    if (i + 1 < edgeList.Count)
                    {
                        // Just use the next hedge
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[i + 1]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont2._nhe._DataIndex;
                        }
                        else
                        {
                            // use second
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont2._nhe._DataIndex;
                        }
                        _LhedgePtrCont[indexhedge2] = new HEdgePtrCont()
                        {
                            _f = hedgePtrCont2._f,
                            _he = hedgePtrCont2._he,
                            _nhe = hedgePtrCont2._nhe,
                            _v = hedgePtrCont2._v,
                            _vn = hedgePtrCont2._vn,
                            _vuv = hedgePtrCont2._vuv
                        };
                    }
                    else
                    {
                        // Connect to the first hedge in the list because the current is the last one in the face
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont2._nhe._DataIndex;
                        }
                        else
                        {
                            // use second
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont2._nhe._DataIndex;
                        }
                        _LhedgePtrCont[indexhedge2] = new HEdgePtrCont()
                        {
                            _f = hedgePtrCont2._f,
                            _he = hedgePtrCont2._he,
                            _nhe = hedgePtrCont2._nhe,
                            _v = hedgePtrCont2._v,
                            _vn = hedgePtrCont2._vn,
                            _vuv = hedgePtrCont2._vuv
                        };
                    }
                    #endregion UseHEdge2
                }
                else
                {
                    /* It's an edge shared by more than two faces. So we need to do something.
                     * TODO:
                     * 
                     * Insert new edge between the two vertices.
                     * Update the pointers of the edge
                     */

                    Debug.WriteLine("Both edges are already in use and don't point to the current face. Do something different.");
                    HEdgePtrCont hedge1 = hedgePtrCont1;
                    HEdgePtrCont hedge2 = hedgePtrCont2;

                    hedge1._f._DataIndex = _LfacePtrCont.Count - 1;
                    hedge1._he._DataIndex = _LhedgePtrCont.Count + 1;
                    hedge2._he._DataIndex = _LhedgePtrCont.Count;

                    if (i + 1 < edgeList.Count)
                    {
                        int nextIndex = _LedgePtrCont[edgeList[i + 1]._DataIndex]._he1._DataIndex;
                        hedge1._nhe._DataIndex = nextIndex;
                    }
                    else
                    {
                        int firstHEdgeIndex = _LedgePtrCont[edgeList[0]._DataIndex]._he1._DataIndex;
                        hedge1._nhe._DataIndex = firstHEdgeIndex;
                    }

                    // Add the hedges to the global list
                    _LhedgePtrCont.Add(hedge1);
                    _LhedgePtrCont.Add(hedge2);

                    // Add an edge to the global list
                    _LedgePtrCont.Add(
                            new EdgePtrCont()
                            {
                                _he1 = new HandleHalfEdge() { _DataIndex = _LhedgePtrCont.Count - 2 },
                                _he2 = new HandleHalfEdge() { _DataIndex = _LhedgePtrCont.Count - 1 }
                            }
                        );

                    _LedgeHndl.Add(new HandleEdge() { _DataIndex = _LedgePtrCont.Count - 1 }
                        );

                    // Change the current edgelist
                    edgeList.RemoveAt(i);
                    edgeList.Insert(i, _LedgeHndl[_LedgeHndl.Count - 1]);

                    // Change the next pointer for the edge before the current one
                    int indexPrevhedge1 = i == 0 ? _LedgePtrCont[edgeList[i]._DataIndex]._he1 : _LedgePtrCont[edgeList[i - 1]._DataIndex]._he1;
                    int indexPrevhedge2 = i == 0 ? _LedgePtrCont[edgeList[i]._DataIndex]._he2 : _LedgePtrCont[edgeList[i - 1]._DataIndex]._he2;
                    HEdgePtrCont prevhedge1 = _LhedgePtrCont[indexhedge1];
                    HEdgePtrCont prevhedge2 = _LhedgePtrCont[indexhedge2];

                    if (prevhedge1._f._DataIndex == hedge1._f._DataIndex)
                    {
                        // use the first hedge
                        prevhedge1._nhe._DataIndex = _LhedgePtrCont.Count - 2;
                        _LhedgePtrCont[indexPrevhedge1] = new HEdgePtrCont()
                        {
                            _f = prevhedge1._f,
                            _he = prevhedge1._he,
                            _nhe = prevhedge1._nhe,
                            _v = prevhedge1._v,
                            _vn = prevhedge1._vn,
                            _vuv = prevhedge1._vuv
                        };
                    }
                    else
                    {
                        // use the second hedge
                        prevhedge2._nhe._DataIndex = _LhedgePtrCont.Count - 2;
                        _LhedgePtrCont[indexPrevhedge2] = new HEdgePtrCont()
                        {
                            _f = prevhedge2._f,
                            _he = prevhedge2._he,
                            _nhe = prevhedge2._nhe,
                            _v = prevhedge2._v,
                            _vn = prevhedge2._vn,
                            _vuv = prevhedge2._vuv
                        };
                    }

                }

            }
        }

        /// <summary>
        /// Returns an enumerable of all vertices handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> EnAllVertices()
        {
            return _LverticeHndl;
        }

        /// <summary>
        /// Returns an enumerable of all edge handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleEdge</returns>
        public IEnumerable<HandleEdge> EnAllEdges()
        {
            return _LedgeHndl;
        }

        /// <summary>
        /// Returns an enumerable of all face handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleFace</returns>
        public IEnumerable<HandleFace> EnAllFaces()
        {
            return _LfaceHndl;
        }

        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all vertices connected by a direct edge.
        /// </summary>
        /// <param name="vertexHandle">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of VertexHandles to be used in loops, etc.</returns>
        public IEnumerable<HandleVertex> EnStarVertexVertex(HandleVertex vertexHandle)
        {
            return EnVertexIncomingHalfEdge(vertexHandle).Select(handleHalfEdge => _LhedgePtrCont[_LhedgePtrCont[handleHalfEdge]._he]._v);
        }

        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all incoming halfedges.
        /// </summary>
        /// <param name="vertexHandle">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleHalfEdge> EnVertexIncomingHalfEdge(HandleVertex vertexHandle)
        {
            
            List<HandleHalfEdge> LTmpIncomingHedges = new List<HandleHalfEdge>();

            //Get the one outgoing half-edge for the vertex.
            HandleHalfEdge currentHedge = _LvertexPtrCont[vertexHandle]._h;
            //Remember the index of the first half-edge
            int startHedgeIndex = currentHedge;
            do
            {
                if (currentHedge == -1)
                    break;

                HEdgePtrCont currentHedgeContainer = _LhedgePtrCont[currentHedge];
                if (vertexHandle == _LhedgePtrCont[currentHedgeContainer._he]._v)
                {
                    LTmpIncomingHedges.Add(currentHedgeContainer._he);
                }
                currentHedge = _LhedgePtrCont[currentHedgeContainer._he]._nhe;
            } while (currentHedge != startHedgeIndex);

            return LTmpIncomingHedges;
            
            //return _LhedgePtrCont.Where(e => e._v == vertexHandle).AsParallel().Select(e => _LhedgePtrCont[e._he._DataIndex]._he).AsParallel().ToList();
            //return (from e in _LhedgePtrCont where e._v == vertexHandle select _LhedgePtrCont[e._he]._he).AsParallel().ToList();
        }

        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all outgoing halfedges.
        /// </summary>
        /// <param name="vertexHandle">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleHalfEdge> EnVertexOutgoingHalfEdge(HandleVertex vertexHandle)
        {
            return EnVertexIncomingHalfEdge(vertexHandle).Select(handleHalfEdge => _LhedgePtrCont[handleHalfEdge]._he);
        }

        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all faces adjacent to the center vertex.
        /// </summary>
        /// <param name="hv">A handle to a vertex to use as a 'center' vertex.</param>
        /// <param name="vertexHandle">Handle to the vertex to do this operation on.</param>
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleFace> EnVertexAdjacentFaces(HandleVertex vertexHandle)
        {
            return EnVertexIncomingHalfEdge(vertexHandle).Select(handleHalfEdge => _LhedgePtrCont[handleHalfEdge]._f);
        }

        /// <summary>
        /// Iterator.
        /// This is a method that retrieves all halfedge handles which belong to a specific face handle.
        /// </summary>
        /// <param name="faceHandle">A handle to the face to get the half-edges from.</param>
        /// <returns>An Enumerable of haldedge pointer containers.</returns>
        public IEnumerable<HandleHalfEdge> EnFaceAdjacentHalfEdges(HandleFace faceHandle)
        {
            int startHedgeIndex = _LfacePtrCont[faceHandle]._h;
            int currentIndex = startHedgeIndex;
            List<HandleHalfEdge> LHedgeHandles = new List<HandleHalfEdge>();
            do
            {
                LHedgeHandles.Add(new HandleHalfEdge(currentIndex));
                if (_LhedgePtrCont[_LhedgePtrCont[currentIndex]._nhe]._f != faceHandle)
                    break;

                currentIndex = _LhedgePtrCont[currentIndex]._nhe;
            } while (currentIndex != startHedgeIndex);
            return LHedgeHandles;
        }

        /// <summary>
        /// Iterator.
        /// Circulate around all the vertices of a given face handle.
        /// </summary>
        /// <param name="faceHandle">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of vertex handles to be used in loops, etc.</returns>
        public IEnumerable<HandleVertex> EnFaceAdjacentVertices(HandleFace faceHandle)
        {
            return EnFaceAdjacentHalfEdges(faceHandle).Select(handleHalfEdge => _LhedgePtrCont[handleHalfEdge]._v);
        }

        /// <summary>
        /// Iterator.
        /// Circulate around all the faces surrounding a specific face.
        /// </summary>
        /// <param name="faceHandle">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of face handles to be used in loops, etc.</returns>
        public IEnumerable<HandleFace> EnFaceAdjacentFaces(HandleFace faceHandle)
        {
            return EnFaceAdjacentHalfEdges(faceHandle).Select(handleHalfEdge => _LhedgePtrCont[_LhedgePtrCont[handleHalfEdge]._he]._f);
        }

        /* Developing Methods only for testing the data structure algorithms.*/
        /// <summary>
        /// Resets the geometry object to default scaling etc.
        /// </summary>
        /// <returns>Boolean. True if succesful.</returns>
        public bool ResetGeometryToDefault()
        {
            try
            {
                ResetVerticesToDefault();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Set the vertex defaults by using the actual vertices.
        /// This is called before the first change to a model is done.
        /// </summary>
        public void SetVertexDefaults()
        {
            if (this._LvertexValDefault == null)
            {
                this._LvertexValDefault = new List<float3>(this._LvertexVal);
            }
        }

        /// <summary>
        /// Used for developing and debugging.
        /// Resets geometry object to the data it had when initially loaded.
        /// </summary>
        public void ResetVerticesToDefault()
        {
            this._LvertexVal.Clear();
            this._LvertexVal = null;

            this._LvertexVal = new List<float3>(this._LvertexValDefault);
        }

    }
}