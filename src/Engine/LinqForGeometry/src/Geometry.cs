using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Math;
using Fusee.Engine;
using LinqForGeometry.Exceptions;
using LinqForGeometry.Importer;
using LinqForGeometry.Structs.Handles;
using LinqForGeometry.Structs.PtrContainer;

namespace LinqForGeometry
{
    public class Geometry
    {
        private WavefrontImporter<float3> _objImporter;
        private List<GeoFace> _GeoFaces;

        public bool _Changes = false;

        // Handles to pointer containers
        private List<HandleVertex> _LverticeHndl;
        private List<HandleEdge> _LedgeHndl;
        private List<HandleFace> _LfaceHndl;
        private List<short> _LtriangleList;

        // Boolean helpers
        private bool _triangleListSet = false;
        private bool _ChangesOnFaces = false;

        // Real data
        private List<float3> _LvertexVal;
        private List<float3> _LfaceNormals;
        private List<float3> _LVertexNormals;
        private List<float2> _LuvCoordinates;
        private List<float3> _LvertexValDefault;
        
        // Pointer containers
        private List<VertexPtrCont> _LvertexPtrCont;
        private List<HEdgePtrCont> _LhedgePtrCont;
        private List<EdgePtrCont> _LedgePtrCont;
        private List<FacePtrCont> _LfacePtrCont;

        // Various runtime constants
        private static double _SmoothingAngle = 89;
        Mesh _FuseeMesh = new Mesh();

        // For mesh conversion
        private List<short> LtrianglesTMP;
        private List<float3> LvertDataTMP;
        private List<float3> LvertNormalsTMP;
        private List<float2> LvertuvTMP;

        /// <summary>
        /// Constructor for the GeometryData class.
        /// </summary>
        public Geometry()
        {
            _objImporter = new WavefrontImporter<float3>();
            _GeoFaces = new List<GeoFace>();

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

            _LtriangleList = new List<short>();

            // For mesh conversion
            LtrianglesTMP = new List<short>();
            LvertDataTMP = new List<float3>();
            LvertNormalsTMP = new List<float3>();
            LvertuvTMP = new List<float2>();
        }


        /// <summary>
        /// Loads an asset specified by the path string.
        /// </summary>
        /// <param name="path">Path to the wavefront.obj.model file.</param>
        public void LoadAsset(String path)
        {
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan timeSpan = new TimeSpan();
            String timeDone;

            if (LFGMessages._DEBUGOUTPUT)
            {
                stopWatch.Start();
            }

            List<GeoFace> faceList = _objImporter.LoadAsset(path);

            // Convert a x-poly model to a triangular poly model because FUSEE only can handle triangular polys for now.
            if (LFGMessages.FLAG_FUSEE_TRIANGLES)
            {
                List<GeoFace> newFaces = ConvertFacesToTriangular(faceList);
                faceList.Clear();
                faceList = newFaces;
            }

            if (LFGMessages._DEBUGOUTPUT)
            {
                timeSpan = stopWatch.Elapsed;
                timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
                Debug.WriteLine("\n\n     Time needed to import the .obj file: " + timeDone);
                stopWatch.Restart();
            }

            if (LFGMessages._DEBUGOUTPUT)
            {
                Debug.WriteLine(LFGMessages.INFO_PROCESSINGDS);
            }

            // Work on the facelist and transform the data structure to the 'half-edge' data structure.
            foreach (GeoFace gf in faceList)
            {
                AddFace(gf);
            }

            if (LFGMessages.FLAG_FUSEE_TRIANGLES)
            {
                stopWatch.Stop();
                timeSpan = stopWatch.Elapsed;
                timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
                Debug.WriteLine("\n\n     Time needed to convert the object to the HES: " + timeDone);
            }

            _LfaceNormals.Clear();
            foreach (HandleFace face in EnAllFaces())
            {
                CalcFaceNormalForFace(face);
            }

            // This is just for now for debugging
            SetVertexDefaults();
        }


        /// <summary>
        /// This method converts a quadrangular polygon mesh to a triangular polygon mesh
        /// </summary>
        /// <param name="faces">List of GeoFace</param>
        /// <returns>List of GeoFaces</returns>
        private List<GeoFace> ConvertFacesToTriangular(List<GeoFace> faces)
        {
            int secondVert = 0;
            List<GeoFace> triangleFaces = new List<GeoFace>();

            foreach (GeoFace face in faces)
            {
                int faceVertCount = face._LFVertices.Count;

                if (faceVertCount == 3)
                {
                    triangleFaces.Add(face);
                }
                else if (faceVertCount > 3)
                {
                    secondVert++;
                    while (secondVert != faceVertCount - 1)
                    {
                        GeoFace newFace = new GeoFace() { _LFVertices = new List<float3>(), _UV = new List<float2>() };
                        newFace._LFVertices.Add(face._LFVertices[0]);
                        newFace._LFVertices.Add(face._LFVertices[secondVert]);
                        newFace._LFVertices.Add(face._LFVertices[secondVert + 1]);

                        newFace._UV.Add(face._UV[0]);
                        newFace._UV.Add(face._UV[secondVert]);
                        newFace._UV.Add(face._UV[secondVert + 1]);

                        triangleFaces.Add(newFace);
                        secondVert++;
                    }
                    secondVert = 0;
                }
                else if (faceVertCount < 3)
                {
                    // Error. Faces with less than 3 vertices does not exist.
                    throw new MeshLeakException();
                }
            }
            return triangleFaces;
        }

        /// <summary>
        /// This method converts the data structure to a fusee readable mesh structure
        /// </summary>
        /// <returns>A fusee readable Mesh object</returns>
        public Mesh ToMesh()
        {
            _LfaceNormals.Clear();
            foreach (HandleFace faceHandle in EnAllFaces())
            {
                CalcFaceNormalForFace(faceHandle);
            }

            _LVertexNormals.Clear();
            foreach (HandleVertex handleVertex in _LverticeHndl)
            {
                CalcVertexNormalTest(handleVertex);
            }

            LtrianglesTMP.Clear();
            LvertDataTMP.Clear();
            LvertNormalsTMP.Clear();
            LvertuvTMP.Clear();

            foreach (HandleFace face in _LfaceHndl)
            {
                GrabFaceDataForMesh(face);
            }

            Mesh mesh = new Mesh();
            mesh.Vertices = LvertDataTMP.ToArray();
            mesh.Normals = LvertNormalsTMP.ToArray();
            mesh.UVs = LvertuvTMP.ToArray();
            mesh.Triangles = LtrianglesTMP.ToArray();

            return mesh;
        }

        /// <summary>
        /// Adds a vertex to the geometry container.
        /// </summary>
        /// <param name="val"></param>
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
            _LfacePtrCont.Add(
               new FacePtrCont()
               {
                   _h = new HandleHalfEdge()
                   {
                       _DataIndex = -1
                   }
               }
            );
            HandleFace fHndl = new HandleFace();
            fHndl._DataIndex = _LfacePtrCont.Count - 1;
            _LfaceHndl.Add(fHndl);

            Debug.WriteLine("Current Face -> AddFace: " + _LfaceHndl[_LfaceHndl.Count - 1]._DataIndex);

            List<HandleVertex> LhFaceVerts = new List<HandleVertex>();
            foreach (float3 vVal in gf._LFVertices)
            {
                LhFaceVerts.Add(
                        AddVertex(vVal)
                    );
            }

            List<HandleEdge> LtmpEdgesForFace = new List<HandleEdge>();
            int vertsCount = LhFaceVerts.Count;
            for (int i = 0; i < vertsCount; i++)
            {
                HandleVertex hvFrom = LhFaceVerts[i];
                float2 uvFrom = gf._UV[i];
                if (i + 1 < vertsCount)
                {
                    HandleVertex hvTo = LhFaceVerts[i + 1];
                    float2 uvTo = gf._UV[i + 1];

                    HandleEdge handleEdge = AddEdge(hvFrom, hvTo);
                    if (!_LedgeHndl.Contains(handleEdge))
                    {
                        _LedgeHndl.Add(handleEdge);
                    }
                    else
                    {
                        if (LFGMessages._DEBUGOUTPUT)
                        {
                            Debug.WriteLine("$$$ Edge has been already inserted!");
                        }
                    }
                    LtmpEdgesForFace.Add(handleEdge);
                }
                else
                {
                    HandleVertex hvTo = LhFaceVerts[0];
                    float2 uvTo = gf._UV[0];
                    HandleEdge handleEdge = AddEdge(hvFrom, hvTo);
                    if (!_LedgeHndl.Contains(handleEdge))
                    {
                        _LedgeHndl.Add(handleEdge);
                    }
                    else
                    {
                        if (LFGMessages._DEBUGOUTPUT)
                        {
                            Debug.WriteLine("$$$ Edge has been already inserted!");
                        }
                    }
                    LtmpEdgesForFace.Add(handleEdge);
                }
            }

            // Update the face handle, so that it points to the first half edge the face consists of.
            UpdateFaceToHedgePtr(LtmpEdgesForFace[0]);

            // Hand over the list of edges that are used for this face. Now build up the connections.
            UpdateCWHedges(LtmpEdgesForFace);

            InsertUVCoordinatesForFace(new HandleFace() { _DataIndex = _LfaceHndl.Count - 1 }, gf._UV);
        }


        /// <summary>
        /// This method adds a edge to the container. The edge is 'drawn' between two vertices
        /// It first checks if a connection is already present. If so it returns a handle to this connection
        /// If not it will establish a connection between the two input vertices.
        /// </summary>
        /// <param name="hvFrom">Vertex From</param>
        /// <param name="hvTo">Vertex To</param>
        /// <param name="uvFrom">uv coordinates for the from vertex</param>
        /// <param name="uvTo">uv coordinates for the to vertex</param>
        public HandleEdge AddEdge(HandleVertex hvFrom, HandleVertex hvTo)
        {
            HandleEdge hndlEdge;
            GetOrAddConnection(hvFrom, hvTo, out hndlEdge);
            return new HandleEdge() { _DataIndex = hndlEdge._DataIndex };
        }


        /// <summary>
        /// Returns true if a connection already exists and fills the out parameter with a handle to the edge
        /// </summary>
        /// <param name="hv1">HandleVertex From vertex</param>
        /// <param name="hv2">HandleVertex To vertex</param>
        /// <param name="he">HandleEdge is filled when connection already exists with valid index otherwise with -1</param>
        /// <param name="uvFrom">Uv coordinates for the from vertex</param>
        /// <param name="uvTo">Uv coordinates for the to vertex</param>
        /// <returns></returns>
        public bool GetOrAddConnection(HandleVertex hv1, HandleVertex hv2, out HandleEdge he)
        {
            int index = -1;
            bool returnVal = false;
            if (_LedgePtrCont.Count != 0 && _LhedgePtrCont.Count != 0)
            {
                index = _LedgePtrCont.FindIndex(
                    edgePtrCont => _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv1._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv2._DataIndex || _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv2._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv1._DataIndex
                    );
            }
            if (index >= 0)
            {
                if (LFGMessages._DEBUGOUTPUT)
                {
                    Debug.WriteLine("     Existing edge found - Not creating a new one.");
                }
                he = new HandleEdge() { _DataIndex = index };
                returnVal = true;
            }
            else
            {
                if (LFGMessages._DEBUGOUTPUT)
                {
                    Debug.WriteLine("     Edge not found - creating new one.");
                }
                he._DataIndex = CreateConnection(hv1, hv2)._DataIndex;
                returnVal = false;
            }
            return returnVal;
        }


        /// <summary>
        /// Establishes a connection between two vertices.
        /// 1) Creates two half-edges
        /// 2) Fills them with information
        /// 3) Creates an edge pointer container and adds it to the geo container.
        /// 4) returns a handle to an edge
        /// </summary>
        /// <param name="hv1">HandleVertex from which vertex</param>
        /// <param name="hv2">Handlevertex to which vertex</param>
        public HandleEdge CreateConnection(HandleVertex hvFrom, HandleVertex hvTo)
        {
            HEdgePtrCont hedge1 = new HEdgePtrCont();
            HEdgePtrCont hedge2 = new HEdgePtrCont();

            hedge1._he._DataIndex = _LedgePtrCont.Count == 0 ? 1 : _LhedgePtrCont.Count + 1;
            hedge1._v._DataIndex = hvTo._DataIndex;
            hedge1._f._DataIndex = _LfacePtrCont.Count - 1;
            hedge1._nhe._DataIndex = -1;

            hedge2._he._DataIndex = _LedgePtrCont.Count == 0 ? 0 : _LhedgePtrCont.Count;
            hedge2._v._DataIndex = hvFrom._DataIndex;
            hedge2._f._DataIndex = -1;
            hedge2._nhe._DataIndex = -1;

            _LhedgePtrCont.Add(hedge1);
            _LhedgePtrCont.Add(hedge2);

            _LedgePtrCont.Add(
                new EdgePtrCont()
                {
                    _he1 = new HandleHalfEdge() { _DataIndex = _LedgePtrCont.Count > 0 ? _LedgePtrCont.Count * 2 : 0 },
                    _he2 = new HandleHalfEdge() { _DataIndex = _LedgePtrCont.Count > 0 ? _LedgePtrCont.Count * 2 + 1 : 1 }
                }
            );

            // Update the vertices so they point to the correct hedges.
            VertexPtrCont vertFrom = _LvertexPtrCont[hvFrom._DataIndex];
            VertexPtrCont vertTo = _LvertexPtrCont[hvTo._DataIndex];

            if (vertFrom._h._DataIndex == -1)
            {
                vertFrom._h._DataIndex = _LedgePtrCont[_LedgePtrCont.Count - 1]._he1._DataIndex;
                _LvertexPtrCont.RemoveAt(hvFrom._DataIndex);
                _LvertexPtrCont.Insert(hvFrom._DataIndex, vertFrom);
            }
            if (vertTo._h._DataIndex == -1)
            {
                vertTo._h._DataIndex = _LedgePtrCont[_LedgePtrCont.Count - 1]._he2._DataIndex;
                _LvertexPtrCont.RemoveAt(hvTo._DataIndex);
                _LvertexPtrCont.Insert(hvTo._DataIndex, vertTo);
            }

            return new HandleEdge() { _DataIndex = _LedgePtrCont.Count - 1 };
        }


        /// <summary>
        /// Adds the uv coordiantes that correspond to a vertex to his corresponding half-edge.
        /// </summary>
        /// <param name="faceHandle">A handle to the face to work on.</param>
        /// <param name="LuvCoord">A List of uv coordinate pairs.</param>
        public void InsertUVCoordinatesForFace(HandleFace faceHandle, List<float2> LuvCoord)
        {
            int startHEIndex = _LfacePtrCont[faceHandle]._h._DataIndex;
            int currentHEIndex = startHEIndex;

            List<HEdgePtrCont> LtmpHedges = new List<HEdgePtrCont>();

            do
            {
                if (_LhedgePtrCont[currentHEIndex]._f._DataIndex != faceHandle._DataIndex)
                    break;

                LtmpHedges.Add(
                        _LhedgePtrCont[currentHEIndex]
                        );

                currentHEIndex = _LhedgePtrCont[currentHEIndex]._nhe._DataIndex;
            } while (currentHEIndex != startHEIndex);

            for (int i = 0; i < LtmpHedges.Count; i++)
            {
                HEdgePtrCont currentHedge = LtmpHedges[i];

                if (i == LtmpHedges.Count - 1)
                {
                    _LuvCoordinates.Add(LuvCoord[0]);
                    currentHedge._vuv._DataIndex = _LuvCoordinates.Count - 1;
                }
                else
                {
                    _LuvCoordinates.Add(LuvCoord[i + 1]);
                    currentHedge._vuv._DataIndex = _LuvCoordinates.Count - 1;
                }

                int currentIndex = _LhedgePtrCont[currentHedge._he._DataIndex]._he._DataIndex;
                _LhedgePtrCont.RemoveAt(currentIndex);
                _LhedgePtrCont.Insert(currentIndex, currentHedge);
            }

            LtmpHedges.Clear();
        }


        /// <summary>
        /// This method adds a face normal vector to a list.
        /// The vector is calculated for the face which handle the method expects.
        /// </summary>
        /// <param name="handleFace">Handle to a face</param>
        public void CalcFaceNormalForFace(HandleFace faceHandle)
        {
            List<HandleVertex> tmpList = IteratorVerticesAroundFace(faceHandle).ToList();

            var v0 = _LvertexVal[tmpList[0]];
            var v1 = _LvertexVal[tmpList[1]];
            var v2 = _LvertexVal[tmpList[2]];

            float3 c1 = float3.Subtract(v0, v1);
            float3 c2 = float3.Subtract(v0, v2);
            float3 n = float3.Cross(c1, c2);

            _LfaceNormals.Add(float3.Normalize(n));

            FacePtrCont fh = new FacePtrCont();
            fh = _LfacePtrCont[faceHandle._DataIndex];
            fh._fn._DataIndex = _LfaceNormals.Count - 1;
            _LfacePtrCont.RemoveAt(faceHandle._DataIndex);
            _LfacePtrCont.Insert(faceHandle._DataIndex, fh);
        }


        /// <summary>
        /// This method calculates vertex normals for a specific vertex in the geometry and inserts them at the corresponding half-edges.
        /// </summary>
        public void CalcVertexNormalTest(HandleVertex vertexHandle)
        {
            IEnumerable<int> EincomingHEdges = EnStarVertexIncomingHalfEdge(vertexHandle);
            foreach (int hedgeIndex in EincomingHEdges)
            {
                List<float3> LfaceNormals = new List<float3>();
                int faceIndex = _LhedgePtrCont[hedgeIndex]._f._DataIndex;
                if (faceIndex == -1)
                    continue;

                float3 currentNormal = _LfaceNormals[_LfacePtrCont[faceIndex]._fn._DataIndex];
                float3 normalAggregate = new float3();
                foreach (int eincomingHEdge in EincomingHEdges)
                {
                    if (eincomingHEdge == hedgeIndex)
                    {
                        normalAggregate += currentNormal;
                        continue;
                    }
                    int faceIndex2 = _LhedgePtrCont[eincomingHEdge]._f._DataIndex;
                    if (faceIndex2 == -1)
                        continue;

                    float3 normalToCompare = _LfaceNormals[_LfacePtrCont[faceIndex2]._fn._DataIndex];
                    float dot = float3.Dot(currentNormal, normalToCompare);
                    double angle = _SmoothingAngle;
                    double acos = System.Math.Acos(dot) * (180 / 3.141592);

                    if (acos < angle)
                    {
                        normalAggregate += float3.Add(normalAggregate, normalToCompare);
                    }
                }

                _LVertexNormals.Add(float3.Normalize(normalAggregate));
                HEdgePtrCont currentHedge = _LhedgePtrCont[hedgeIndex];
                currentHedge._vn._DataIndex = _LVertexNormals.Count - 1;

                _LhedgePtrCont.RemoveAt(hedgeIndex);
                _LhedgePtrCont.Insert(hedgeIndex, currentHedge);
            }
        }


        /// <summary>
        /// Aggregates an array of information that contains 'vertex pointer, vertex normal, vertex uv coordinates' for every half-edge in the geometry.
        /// </summary>
        /// <param name="face">A handle to a face to perform on.</param>
        /// <returns>An array of handle indexes in the following order: vertex id, vertex normal id, vertex uv id</returns>
        public void GrabFaceDataForMesh(HandleFace face)
        {
            HEdgePtrCont currentHedge = _LhedgePtrCont[_LfacePtrCont[face]._h];
            int startHedgeVertexIndex = currentHedge._v._DataIndex;

            do
            {
                LvertDataTMP.Add(
                    _LvertexVal[currentHedge._v._DataIndex]
                    );

                LvertNormalsTMP.Add(
                    _LVertexNormals[currentHedge._vn._DataIndex]
                    );

                LvertuvTMP.Add(
                    _LuvCoordinates[currentHedge._vuv]
                    );

                int idx = LvertDataTMP.Count - 1;
                LtrianglesTMP.Add((short)idx);
                currentHedge = _LhedgePtrCont[currentHedge._nhe._DataIndex];
            } while (currentHedge._v._DataIndex != startHedgeVertexIndex);
        }


        /// <summary>
        /// This updates the half-edge a face points to.
        /// Is called directly after inserting a face and it's vertices, edges to the container is done
        /// </summary>
        /// <param name="handleEdge">the Edge Handle "containing" the half-edge the face should point to</param>
        public void UpdateFaceToHedgePtr(HandleEdge handleEdge)
        {
            FacePtrCont faceCont = _LfacePtrCont.Count - 1 < 0 ? _LfacePtrCont[0] : _LfacePtrCont[_LfacePtrCont.Count - 1];
            HEdgePtrCont hEdgePtrCont1 = _LhedgePtrCont[_LedgePtrCont[handleEdge._DataIndex]._he1._DataIndex];
            HEdgePtrCont hEdgePtrCont2 = _LhedgePtrCont[_LedgePtrCont[handleEdge._DataIndex]._he2._DataIndex];
            if (hEdgePtrCont1._f._DataIndex == (_LfacePtrCont.Count - 1))
            {
                faceCont._h._DataIndex = hEdgePtrCont1._he._DataIndex - 1;
            }
            else
            {
                // The first hedge does not point to this face - so you can assume it points to the neighbour face. So the second is the hedge to go with.
                faceCont._h._DataIndex = hEdgePtrCont1._he._DataIndex;
            }
            _LfacePtrCont.RemoveAt(_LfacePtrCont.Count - 1);
            _LfacePtrCont.Add(faceCont);
        }


        /// <summary>
        /// Updates the "inner" half edges clockwise so the next pointers are correct.
        /// Is called after a face is inserted.
        /// </summary>
        /// <param name="edgeList">A list of edges that belong to a specific face</param>
        public void UpdateCWHedges(List<HandleEdge> edgeList)
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
                            _LhedgePtrCont.RemoveAt(indexhedge1);
                            _LhedgePtrCont.Insert(indexhedge1, hedgePtrCont1);
                        }
                        else
                        {
                            // use second
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont1._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge1);
                            _LhedgePtrCont.Insert(indexhedge1, hedgePtrCont1);
                        }
                    }
                    else
                    {
                        // Connect to the first hedge in the list because the current is the last one in the face
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont1._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge1);
                            _LhedgePtrCont.Insert(indexhedge1, hedgePtrCont1);
                        }
                        else
                        {
                            // use second
                            hedgePtrCont1._nhe._DataIndex = hedgePtrCont1._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont1._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge1);
                            _LhedgePtrCont.Insert(indexhedge1, hedgePtrCont1);
                        }
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
                            _LhedgePtrCont.RemoveAt(indexhedge2);
                            _LhedgePtrCont.Insert(indexhedge2, hedgePtrCont2);
                        }
                        else
                        {
                            // use second
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont2._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge2);
                            _LhedgePtrCont.Insert(indexhedge2, hedgePtrCont2);
                        }
                    }
                    else
                    {
                        // Connect to the first hedge in the list because the current is the last one in the face
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1];
                        if (nextHedgePtrCont._f == _LfacePtrCont.Count - 1)
                        {
                            // use first
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedgePtrCont2._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge2);
                            _LhedgePtrCont.Insert(indexhedge2, hedgePtrCont2);
                        }
                        else
                        {
                            // use second
                            hedgePtrCont2._nhe._DataIndex = hedgePtrCont2._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex : hedgePtrCont2._nhe._DataIndex;
                            _LhedgePtrCont.RemoveAt(indexhedge2);
                            _LhedgePtrCont.Insert(indexhedge2, hedgePtrCont2);
                        }
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
                    /*

                    HandleVertex v1Handle = hedgePtrCont1._v;
                    HandleVertex v2Handle = hedgePtrCont2._v;

                    HandleEdge newEdge = CreateConnection(v1Handle, v2Handle);

                    HandleHalfEdge hedge1 = _LedgePtrCont[newEdge._DataIndex]._he1;
                    //HandleHalfEdge hedge2 = _LedgePtrCont[newEdge._DataIndex]._he2;

                    HEdgePtrCont hedge1Ptr = _LhedgePtrCont[hedge1._DataIndex];
                    //HEdgePtrCont hedge2Ptr = _LhedgePtrCont[hedge2._DataIndex];

                    hedge1Ptr._f._DataIndex = _LfacePtrCont.Count - 1;
                    //hedge2Ptr._f._DataIndex = -1;

                    if (i + 1 < edgeList.Count)
                    {
                        // Just use the next edge
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[i + 1]._DataIndex]._he1];
                        hedge1Ptr._nhe._DataIndex = hedge1Ptr._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedge1Ptr._nhe._DataIndex;

                        // sure?
                        _LhedgePtrCont.Add(hedge1Ptr);
                    }
                    else
                    {
                        // Use the first of the triangle again, because the current is the last one.
                        HEdgePtrCont nextHedgePtrCont = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1];
                        hedge1Ptr._nhe._DataIndex = hedge1Ptr._nhe._DataIndex == -1 ? nextHedgePtrCont._he._DataIndex - 1 : hedge1Ptr._nhe._DataIndex;

                        // sure?
                        _LhedgePtrCont.Add(hedge1Ptr);
                    }
                     * */

                }
            }
        }


        /* Standard circle iterators over all elemets of the geometry object */
        /// <summary>
        /// Returns an enumerable of all vertices handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> EnAllVertices()
        {
            return _LverticeHndl.AsEnumerable();
        }


        /// <summary>
        /// Returns an enumerable of all edge handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleEdge</returns>
        public IEnumerable<HandleEdge> EnAllEdges()
        {
            return _LedgeHndl.AsEnumerable();
        }


        /// <summary>
        /// Returns an enumerable of all face handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleFace</returns>
        public IEnumerable<HandleFace> EnAllFaces()
        {
            return _LfaceHndl.AsEnumerable();
        }


        /// <summary>
        /// Iterator.
        /// This is a private method that retrieves all halfedge pointer containers pointing to a vertex.
        /// </summary>
        /// <param name="hv">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of HalfEdgePointerContainers to be used in other iterators.</returns>
        private IEnumerable<HEdgePtrCont> VertexCenterHalfEdges(HandleVertex hv)
        {
            return _LhedgePtrCont.FindAll(hedges => hedges._v == hv);
        }


        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all vertices connected by a direct edge.
        /// </summary>
        /// <param name="hv">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of VertexHandles to be used in loops, etc.</returns>
        public IEnumerable<HandleVertex> EnStarVertexVertex(HandleVertex hv)
        {
            return VertexCenterHalfEdges(hv).Select(val => _LhedgePtrCont[val._he]._v);
        }


        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all incoming halfedges.
        /// </summary>
        /// <param name="hv">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<int> EnStarVertexIncomingHalfEdge(HandleVertex hv)
        {
            List<int> LTmpIncomingHedges = new List<int>();

            //Get the one outgoing half-edge for the vertex.
            int currentHedge = _LvertexPtrCont[hv._DataIndex]._h._DataIndex;
            //Remember the index of the first half-edge
            int startHedgeIndex = currentHedge;

            do
            {
                if (currentHedge == -1)
                    break;

                // Get the neighbour of the current edge
                currentHedge = _LhedgePtrCont[currentHedge]._he._DataIndex;
                LTmpIncomingHedges.Add(currentHedge);
                currentHedge = _LhedgePtrCont[currentHedge]._nhe._DataIndex;
            } while (currentHedge != startHedgeIndex);

            return LTmpIncomingHedges.AsEnumerable();
        }


        /// <summary>
        /// Iterator.
        /// Circulate around a given vertex and enumerate all outgoing halfedges.
        /// </summary>
        /// <param name="hv">A handle to a vertex to use as a 'center' vertex.</param>
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleHalfEdge> EnStarVertexOutgoingHalfEdge(HandleVertex hv)
        {
            return VertexCenterHalfEdges(hv).Select(val => _LhedgePtrCont[val._he]._he);
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
            List<HandleFace> LtmpFaceHandles = new List<HandleFace>();
            int hedgeIndx = _LvertexPtrCont[vertexHandle._DataIndex]._h._DataIndex;
            int startHedgeIndex = hedgeIndx;

            do
            {
                if (hedgeIndx == -1)
                    break;

                hedgeIndx = _LhedgePtrCont[hedgeIndx]._he._DataIndex;
                LtmpFaceHandles.Add(_LhedgePtrCont[hedgeIndx]._f);
                hedgeIndx = _LhedgePtrCont[hedgeIndx]._nhe._DataIndex;
            } while (hedgeIndx != startHedgeIndex);

            return LtmpFaceHandles.AsEnumerable();
        }


        /// <summary>
        /// Iterator.
        /// This is a private method that retrieves all halfedge pointer containers which belong to a specific face handle.
        /// </summary>
        /// <param name="hf">A handle to the center face.</param>
        /// <returns>An Enumerable of haldedge pointer containers.</returns>
        private IEnumerable<HEdgePtrCont> FaceCenterHalfEdges(HandleFace hf)
        {
            return _LhedgePtrCont.FindAll(hedges => hedges._f == hf);
        }


        /// <summary>
        /// Converts a half edge handle to an edge handle.
        /// </summary>
        /// <param name="hh">A halfedge handle to convert.</param>
        /// <returns>HandleEdge. A new Handle to an already existing edge.</returns>
        private HandleEdge HalfEdgeHandleToEdgeHandle(HandleHalfEdge hh)
        {
            return new HandleEdge() { _DataIndex = hh / 2 };
        }


        /// <summary>
        /// Iterator.
        /// Circulate around all the halfedges of a given face handle.
        /// </summary>
        /// <param name="hf">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of halfedge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleHalfEdge> EnFaceHalfEdges(HandleFace hf)
        {
            return FaceCenterHalfEdges(hf).Select(val => _LhedgePtrCont[val._he]._he);
        }


        /// <summary>
        /// Iterator.
        /// Circulate around all the vertice of a given face handle.
        /// </summary>
        /// <param name="hf">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of vertex handles to be used in loops, etc.</returns>
        public IEnumerable<HandleVertex> EnFaceVertices(HandleFace hf)
        {
            return FaceCenterHalfEdges(hf).Select(val => val._v);
        }


        /// <summary>
        /// Iterator.
        /// Circulate around all the edges of a given face handle.
        /// </summary>
        /// <param name="hf">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of edge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleEdge> EnFaceEdges(HandleFace hf)
        {
            return EnFaceHalfEdges(hf).Select(val => HalfEdgeHandleToEdgeHandle(val));
        }


        public List<HandleVertex> IteratorVerticesAroundFaceForTriangles(HandleFace hf)
        {
            List<HandleVertex> LtmpVert = new List<HandleVertex>();
            HandleHalfEdge heh = _LfacePtrCont[hf]._h;
            int indexStart = heh._DataIndex;

            for (int i = 0; i < 3; i++)
            {
                LtmpVert.Add(_LhedgePtrCont[heh]._v);
                heh = _LhedgePtrCont[heh]._nhe;
            }

            return LtmpVert;
        }


        public IEnumerable<HandleVertex> IteratorVerticesAroundFace(HandleFace hf)
        {
            List<HandleVertex> LtmpVert = new List<HandleVertex>();
            HandleHalfEdge heh = _LfacePtrCont[hf]._h;
            int indexStart = heh._DataIndex;

            for (int i = 0; i < 3; i++)
            {
                LtmpVert.Add(_LhedgePtrCont[heh]._v);
                heh = _LhedgePtrCont[heh]._nhe;
            }

            return LtmpVert.AsEnumerable();
        }


        /// <summary>
        /// Iterator.
        /// Circulate around all the faces surrounding a specific face.
        /// </summary>
        /// <param name="hf">A handle to a face used as the 'center' face.</param>
        /// <returns>An Enumerable of face handles to be used in loops, etc.</returns>
        public IEnumerable<HandleFace> EnFaceFaces(HandleFace hf)
        {
            return FaceCenterHalfEdges(hf).Select(val => _LhedgePtrCont[val._he]._f);
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

        public void ResetVerticesToDefault()
        {
            this._LvertexVal.Clear();
            this._LvertexVal = null;

            this._LvertexVal = new List<float3>(this._LvertexValDefault);
        }


        /* Standard transformations on the geometry */
        /// <summary>
        /// This method can scale the object bigger or smaller dependent on the input parameters
        /// </summary>
        /// <param name="scalarX"></param>
        /// <param name="scalarY"></param>
        /// <param name="scalarZ"></param>
        /// <param name="scalarW"></param>
        /// <returns>Boolean - true if the operation was succesful, false if not.</returns>
        public bool Scale(float scalarX, float scalarY, float scalarZ, float scalarW = 1.0f)
        {
            try
            {
                float4 row0 = new float4(scalarX, 0f, 0f, 0f);
                float4 row1 = new float4(0f, scalarY, 0f, 0f);
                float4 row2 = new float4(0f, 0f, scalarZ, 0f);
                float4 row3 = new float4(0f, 0f, 0f, scalarW);
                float4x4 transfMatrix = new float4x4(row0, row1, row2, row3);

                List<float3> tmpVerts = EnAllVertices().Select(vertId => transfMatrix * _LvertexVal[vertId]).ToList();

                _LvertexVal.Clear();
                _LvertexVal = null;
                _LvertexVal = new List<float3>(tmpVerts);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        /// <summary>
        /// This method translates the model to another position.
        /// </summary>
        /// <param name="tX">float factor</param>
        /// <param name="tY">float factor</param>
        /// <param name="tZ">float factor</param>
        /// <returns>bool - true when operation succeeded</returns>
        public bool Translate(float tX, float tY, float tZ)
        {
            try
            {
                float4 row0 = new float4(1f, 0f, 0f, tX);
                float4 row1 = new float4(0f, 1f, 0f, tY);
                float4 row2 = new float4(0f, 0f, 1f, tZ);
                float4 row3 = new float4(0f, 0f, 0f, 1f);
                float4x4 transfMatrix = new float4x4(row0, row1, row2, row3);

                List<float3> tmpVerts = EnAllVertices().Select(vertId => transfMatrix * _LvertexVal[vertId]).ToList();

                _LvertexVal.Clear();
                _LvertexVal = null;
                _LvertexVal = new List<float3>(tmpVerts);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        /// <summary>
        /// Rotates the object on the X-Axis.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public bool RotateX(float alpha)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4 row0 = new float4(1f, 0f, 0f, 0f);
                float4 row1 = new float4(0f, (float)cos, (float)sin, 0f);
                float4 row2 = new float4(0f, (float)-sin, (float)cos, 0f);
                float4 row3 = new float4(0f, 0f, 0f, 1f);
                float4x4 transfMatrix = new float4x4(row0, row1, row2, row3);

                List<float3> tmpVerts = EnAllVertices().Select(vertId => transfMatrix * _LvertexVal[vertId]).ToList();

                _LvertexVal.Clear();
                _LvertexVal = null;
                _LvertexVal = new List<float3>(tmpVerts);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        /// <summary>
        /// Rotates the object on the Y-Axis.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public bool RotateY(float alpha)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4 row0 = new float4((float)cos, 0f, (float)-sin, 0f);
                float4 row1 = new float4(0f, 1f, 0f, 0f);
                float4 row2 = new float4((float)sin, 0f, (float)cos, 0f);
                float4 row3 = new float4(0f, 0f, 0f, 1f);
                float4x4 transfMatrix = new float4x4(row0, row1, row2, row3);

                List<float3> tmpVerts = EnAllVertices().Select(vertId => transfMatrix * _LvertexVal[vertId]).ToList();

                _LvertexVal.Clear();
                _LvertexVal = null;
                _LvertexVal = new List<float3>(tmpVerts);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        /// <summary>
        /// Rotates the object on the Z-Axis.
        /// </summary>
        /// <param name="alpha">a float value representing an angle.</param>
        /// <returns>bool - true if the operation succeeded.</returns>
        public bool RotateZ(float alpha)
        {
            try
            {
                double cos = System.Math.Cos((double)alpha);
                double sin = System.Math.Sin((double)alpha);

                float4 row0 = new float4((float)cos, (float)sin, 0f, 0f);
                float4 row1 = new float4((float)-sin, (float)cos, 0f, 0f);
                float4 row2 = new float4(0f, 0f, 1f, 0f);
                float4 row3 = new float4(0f, 0f, 0f, 1f);
                float4x4 transfMatrix = new float4x4(row0, row1, row2, row3);

                List<float3> tmpVerts = EnAllVertices().Select(vertId => transfMatrix * _LvertexVal[vertId]).ToList();

                _LvertexVal.Clear();
                _LvertexVal = null;
                _LvertexVal = new List<float3>(tmpVerts);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        /// <summary>
        /// Resets the geometry object to default scaling etc.
        /// </summary>
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

    }
}
