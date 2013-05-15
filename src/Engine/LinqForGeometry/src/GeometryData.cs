// /*
// 	Author: Dominik Steffen
// 	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
// 	Bachlor Thesis Summer Semester 2013
// 	'Computer Science in Media'
// 	Project: LinqForGeometry
// 	Professors:
// 	Mr. Prof. C. Müller
// 	Mr. Prof. W. Walter
// */


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Fusee.Math;
using Fusee.Engine;

using hsfurtwangen.dsteffen.lfg.structs.ptrcontainer;
using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.Importer;

namespace hsfurtwangen.dsteffen.lfg
{

    /// <summary>
    /// This is a container for the geometry of one mesh.
    /// So if a model is imported, it will be represented in the program as an object of this container class.
    /// </summary>
    public class GeometryData
    {
        // Vars
        private Geometry _geometry;

        public List<float3> _LvertexVal;
        private List<float3> _LvertexValDefault;

        public List<float3> _LfaceNormals;
        public List<float3> _LVertexNormals;
        public List<float2> _LuvCoordinates;

        private List<VertexPtrCont> _LvertexPtrCont;
        private List<HEdgePtrCont> _LhedgePtrCont;
        private List<EdgePtrCont> _LedgePtrCont;
        private List<FacePtrCont> _LfacePtrCont;


        /// <summary>
        /// Initializes a new instance of the <see cref="hsfurtwangen.dsteffen.lfg.Geometry"/> class.
        /// </summary>
        public GeometryData(Geometry kc)
        {
            _geometry = kc;

            _LvertexVal = new List<float3>();

            _LvertexPtrCont = new List<VertexPtrCont>();
            _LhedgePtrCont = new List<HEdgePtrCont>();
            _LedgePtrCont = new List<EdgePtrCont>();
            _LfacePtrCont = new List<FacePtrCont>();

            _LfaceNormals = new List<float3>();
            _LVertexNormals = new List<float3>();
            _LuvCoordinates = new List<float2>();
        }


        /// <summary>
        /// Adds a Face in form of a 'FacePtrCont' to the geometry container
        /// </summary>
        /// <param name="face">A GeoFace Struct</param>
        /// <returns></returns>
        public HandleFace AddFace(GeoFace face)
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

            return fHndl;
        }


        /// <summary>
        /// This method adds a face normal vector to a list.
        /// The vector is calculated for the face which handle the method expects.
        /// Normally should not be called by the user. The system is calling it once a face has been inserted to the geometry object.
        /// </summary>
        /// <param name="handleFace">Handle to a face</param>
        public void CalcFaceNormal(HandleFace handleFace)
        {
            IEnumerable<HandleVertex> enVerts = EnFaceVertices(handleFace);
            var Lverts = enVerts.Select(handleVertex => _LvertexVal[handleVertex._DataIndex]).ToList();

            if (Lverts.Count >= 3)
            {

                float3 v0 = Lverts[0];
                float3 v1 = Lverts[1];
                float3 v2 = Lverts[2];

                float3 c1 = float3.Subtract(v0, v1);
                float3 c2 = float3.Subtract(v0, v2);

                float3 n = float3.Cross(c1, c2);

                Debug.WriteLine("CalcNormal: " + float3.Normalize(n));

                _LfaceNormals.Add(
                    float3.Normalize(n)
                    );
            }
        }

        /// <summary>
        /// Only for testing, calculates face normals with the help of the hes.
        /// </summary>
        /// <returns></returns>
        public float3 CalcFaceNormalsToList(HandleFace faceHandle) {
            List<HandleVertex> tmpList = IteratorVerticesAroundFaceForTriangles(faceHandle);

            var v0 = _LvertexVal[tmpList[0]];
            var v1 = _LvertexVal[tmpList[1]];
            var v2 = _LvertexVal[tmpList[2]];

            float3 c1 = float3.Subtract(v0, v1);
            float3 c2 = float3.Subtract(v0, v2);

            float3 n = float3.Cross(c1, c2);

            return float3.Normalize(n);
        }

        /// <summary>
        /// This method calculates a vertex normal. Starting Point is a handle to a vertex.
        /// It will iterate over all faces adjacent to the vertex the handle points to.
        /// </summary>
        /// <param name="handleVertex">A handle to a vertex</param>
        /// <returns>float3 value which is the normal vektor for the vertex</returns>
        public float3 CalcVertexNormal(HandleVertex handleVertex)
        {
            List<float3> adjacentfaceNormals = new List<float3>();
            IEnumerable<HandleFace> adjacentfaces = EnVertexAdjacentFaces(handleVertex);
            int faceNormalsCount = _LfaceNormals.Count;

            foreach (HandleFace handleFace in adjacentfaces)
            {
                try
                {
                    if (handleFace._DataIndex != -1)
                    {
                        if (faceNormalsCount > handleFace._DataIndex)
                        {
                            adjacentfaceNormals.Add(
                                _LfaceNormals[handleFace]
                                );
                        }
                        else
                        {
                            throw new Exception("Runtime Error: Face handle is not in the face list.");
                        }
                    }
                    else
                    {
                        throw new Exception("Runtime Error: Face handle is invalid.");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            //Debug.WriteLine(" \n --- New Vertex Normal Calc --- ");
            /*
            foreach (var faceNormal in adjacentfaceNormals)
            {
                Debug.WriteLine("%%% Adj. Face Normal: " + faceNormal);
            }
            */
            float3 sumNormals = new float3();

            //Debug.WriteLine(" \n ---Zwischenergebnisse--- ");
            foreach (float3 faceNormal in adjacentfaceNormals)
            {
                sumNormals += faceNormal;
                //Debug.WriteLine(" now -> " + sumNormals);
            }
            //Debug.WriteLine("Ergebnis: " + sumNormals);
            //Debug.WriteLine(" --- End Vert Normal Calc--- \n");

            //Debug.WriteLine("\n Normal with loop: " + sumNormals);
            sumNormals /= adjacentfaceNormals.Count;
            //Debug.WriteLine("Normal with loop divided: " + sumNormals);
            float3 normalized = float3.Normalize(sumNormals);
            //Debug.WriteLine("Normal with loop normalized : " + normalized + "\n");
            return normalized;
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
                }
                else
                {
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
                }
            }
        }


        /// <summary>
        /// Adds a vertex to the geometry container.
        /// </summary>
        /// <param name="val">Generic data type value.</param>
        public HandleVertex AddVertex(float3 val)
        {
            int index = DoesVertexExist(val);

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

                return new HandleVertex() { _DataIndex = _LvertexPtrCont.Count - 1 };
            }
            else
            {
                HandleVertex vHndl = new HandleVertex();
                vHndl._DataIndex = index;
                return vHndl;
            }
        }


        /// <summary>
        /// Returns the data corresponding to a vertex handle
        /// </summary>
        /// <param name="hv">HandleVertex with ID the data is wanted to be retrieved</param>
        /// <returns></returns>
        public float3 GetVertexData(HandleVertex hv)
        {
            return _LvertexVal[hv._DataIndex];
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
        /// This method adds a edge to the container. The edge is 'drawn' between two vertices
        /// It first checks if a connection is already present. If so it returns a handle to this connection
        /// If not it will establish a connection between the two input vertices.
        /// </summary>
        /// <param name="hv1">Vertex From</param>
        /// <param name="hv2">Vertex To</param>
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
        /// <returns></returns>
        public bool GetOrAddConnection(HandleVertex hv1, HandleVertex hv2, out HandleEdge he)
        {
            int index = -1;
            if (_LedgePtrCont.Count != 0 && _LhedgePtrCont.Count != 0)
            {
                index = _LedgePtrCont.FindIndex(
                    edgePtrCont => _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv1._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv2._DataIndex
                        ||
                    _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv2._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv1._DataIndex
                    );
            }
            if (index >= 0)
            {
                if (globalinf.LFGMessages._DEBUGOUTPUT)
                {
                    Console.WriteLine("     Existing edge found - Not creating a new one.");
                }
                he = new HandleEdge() { _DataIndex = index };
                return true;
            }
            else
            {
                if (globalinf.LFGMessages._DEBUGOUTPUT)
                {
                    Console.WriteLine("     Edge not found - creating new one.");
                }
                he._DataIndex = CreateConnection(hv1, hv2)._DataIndex;
                return false;
            }
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
        public IEnumerable<HandleHalfEdge> EnStarVertexIncomingHalfEdge(HandleVertex hv)
        {
            return VertexCenterHalfEdges(hv).Select(val => _LhedgePtrCont[_LhedgePtrCont[val._he]._he]._he);
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
        /// <returns>An Enumerable of HalfEdge handles to be used in loops, etc.</returns>
        public IEnumerable<HandleFace> EnVertexAdjacentFaces(HandleVertex hv)
        {
            return VertexCenterHalfEdges(hv).Select(val => val._f);
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

            while (true)
            {
                LtmpVert.Add(_LhedgePtrCont[heh]._v);
                heh = _LhedgePtrCont[heh]._nhe;

                if (LtmpVert.Count >= 3) { break; }
            }

            return LtmpVert;
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

    }
}