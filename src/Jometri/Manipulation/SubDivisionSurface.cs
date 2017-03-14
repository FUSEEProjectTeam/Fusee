using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    public static class SubDivisionSurface
    {

        /// <summary>
        /// Performs a SubDivisonSurface with a given Geometry with Chalres Loops Algorithm
        /// </summary>
        /// <param name="_geometry"></param>
        /// <returns></returns>
        public static Geometry loopSubDivision(Geometry _geometry)
        {

            Geometry subdiviosnedGeometry = new Geometry();
            int vertexHanlde = 0;
            int halfEdgeHandle = 0;


            for (int i = 1; i < _geometry.DictFaces.Count; i++)
            {
                Face currentFace = _geometry.DictFaces[i];
                HalfEdge currentHalfEdge = _geometry.DictHalfEdges[currentFace.InnerHalfEdges[0]];
                HalfEdge nextHalfEdge = _geometry.DictHalfEdges[currentHalfEdge.NextHalfEdge];

                while (currentHalfEdge != nextHalfEdge)
                {
                    Debug.WriteLine("CurrentHEH:"+currentHalfEdge.Handle+ " nextHEH:"+nextHalfEdge.Handle);

                    Vertex currentOriginVertex = _geometry.DictVertices[currentHalfEdge.OriginVertex];
                    Vertex nextVertex = _geometry.DictVertices[nextHalfEdge.OriginVertex];

                    float3 newVertexPos = getMiddleOfVertices(currentOriginVertex, nextVertex);

                    

                    subdiviosnedGeometry.DictVertices.Add(vertexHanlde, currentOriginVertex);
                    vertexHanlde++;

                    //add new Vertex which is in Middle
                    subdiviosnedGeometry.DictVertices.Add(vertexHanlde,new Vertex(subdiviosnedGeometry.HighestVertHandle, newVertexPos));
                    vertexHanlde++;

                    //Add Half Edges
                    HalfEdge newHalfEdge = new HalfEdge(halfEdgeHandle,vertexHanlde-2,halfEdgeHandle+1,halfEdgeHandle+2);
                    subdiviosnedGeometry.DictHalfEdges.Add(halfEdgeHandle,newHalfEdge);                
                    halfEdgeHandle++;

                    HalfEdge twinHalfEdge = new HalfEdge(halfEdgeHandle, vertexHanlde - 1);
                    subdiviosnedGeometry.DictHalfEdges.Add(halfEdgeHandle,twinHalfEdge);
                    halfEdgeHandle++;

                    nextHalfEdge = _geometry.DictHalfEdges[nextHalfEdge.NextHalfEdge];
                    
                    //subdiviosnedGeometry.SetHighestHandles();                 
                }
            }

            return subdiviosnedGeometry;
        }

        //public static Geometry loopSDTest(Geometry _geometry)
        //{
        //    int vertexHandler = _geometry.DictHalfEdges[1].OriginVertex;
        //    float3 currentVertPos = _geometry.DictVertices[vertexHandler].VertData.Pos;


        //}

        private static float3 getMiddleOfVertices(Vertex a, Vertex b)
        {
            
            float newX = (a.VertData.Pos.x + b.VertData.Pos.x / 2.0f);
            float newY = (a.VertData.Pos.y + b.VertData.Pos.y / 2.0f);
            float newZ = (a.VertData.Pos.z + b.VertData.Pos.z / 2.0f);

            return new float3(newX,newY,newZ);
        }
    }
}
