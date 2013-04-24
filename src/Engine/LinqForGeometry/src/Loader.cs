/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachlor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.globalinf;
using Fusee.Math;

namespace hsfurtwangen.dsteffen.lfg
{
    class Loader
    {
        static Geometry _lfgSys;

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            _lfgSys = new Geometry();
            stopWatch.Start();
            //_lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/plane_square_1.obj");
            _lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_square_1.obj");
            //_lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_triangle_1.obj");
            //_lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/hellknight.obj");
            stopWatch.Stop();
            TimeSpan timeSpan = stopWatch.Elapsed;
            string timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
            Console.WriteLine("\n\n     Time needed to complete the whole process: " + timeDone + "\n\n");

            DoSomeIteratorsOnVertices();
            DoSomeIteratorsOnFaces();

        }

        // http://stackoverflow.com/questions/8582344/does-c-sharp-have-isnullorempty-for-list-ienumerable for testing only
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        /// <summary>
        /// Perform some iterators on vertex based stuff ...
        /// </summary>
        private static void DoSomeIteratorsOnVertices()
        {
            // Iterate in a star over one given vertex and print out to which other vertices it is connected.
            // Because its fun do it for every vertex in the geometry
            for (int i = 0; i < _lfgSys._LverticeHndl.Count; i++)
            {
                if (i < _lfgSys._LverticeHndl.Count)
                {
                    IEnumerable<HandleVertex> enhandlevert = _lfgSys.StarIterateVertex(new HandleVertex() { _DataIndex = i });
                    IEnumerable<HandleHalfEdge> enhandlehedgeInc = _lfgSys.StarVertexIncomingHalfEdge(new HandleVertex() { _DataIndex = i });
                    IEnumerable<HandleHalfEdge> enhandlehedgeOut = _lfgSys.StarVertexOutgoingHalfEdge(new HandleVertex() { _DataIndex = i });
                    IEnumerable<HandleFace> enhandleAdjacentFaces = _lfgSys.VertexAdjacentFaces(new HandleVertex() { _DataIndex = i });

                    if (!IsNullOrEmpty(enhandlevert))
                    {
                        Console.Write("$$$ Vertex " + i + " is connected to verts: ");
                        foreach (HandleVertex verthandle in enhandlevert)
                        {
                            Console.Write(verthandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandlehedgeInc))
                    {
                        Console.Write("$$$ Vertex " + i + " is connected to INCOMING halfedge: ");
                        foreach (HandleHalfEdge hedgehandle in enhandlehedgeInc)
                        {
                            Console.Write(hedgehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandlehedgeOut))
                    {
                        Console.Write("$$$ Vertex " + i + " is connected to OUTGOING halfedge: ");
                        foreach (HandleHalfEdge hedgehandle in enhandlehedgeOut)
                        {
                            Console.Write(hedgehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandleAdjacentFaces))
                    {
                        Console.Write("$$$ Vertex " + i + " is connected to ADJACENT face: ");
                        foreach (HandleFace facehandle in enhandleAdjacentFaces)
                        {
                            Console.Write(facehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }

                }
            }
        }

        /// <summary>
        /// Perform some iterators on face based stuff ... 
        /// </summary>
        private static void DoSomeIteratorsOnFaces()
        {
            for (int i = 0; i < _lfgSys._LfaceHndl.Count; i++)
            {
                if (i < _lfgSys._LfaceHndl.Count)
                {

                    IEnumerable<HandleVertex> enhandlevert = _lfgSys.FaceSurroundingVertices(new HandleFace() {_DataIndex = i});
                    IEnumerable<HandleHalfEdge> enhandlehedges = _lfgSys.FaceSurroundingHalfEdges(new HandleFace() {_DataIndex = i});
                    IEnumerable<HandleEdge> enhandleedges = _lfgSys.FaceSurroundingEdges(new HandleFace() { _DataIndex = i });
                    IEnumerable<HandleFace> enhandlefaces = _lfgSys.FaceSurroundingFaces(new HandleFace() { _DataIndex = i });


                    if (!IsNullOrEmpty(enhandlevert))
                    {
                        Console.Write("$$$ Face " + i + " is surrounded by VERTICES: ");
                        foreach (HandleVertex verthandle in enhandlevert)
                        {
                            Console.Write(verthandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandlehedges))
                    {
                        Console.Write("$$$ Face " + i + " is surrounded by HALFEDGES: ");
                        foreach (HandleHalfEdge hedgehandle in enhandlehedges)
                        {
                            Console.Write(hedgehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandleedges))
                    {
                        Console.Write("$$$ Face " + i + " is surrounded by EDGES: ");
                        foreach (HandleEdge edgehandle in enhandleedges)
                        {
                            Console.Write(edgehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }


                    if (!IsNullOrEmpty(enhandlefaces))
                    {
                        Console.Write("$$$ Face " + i + " is surrounded by FACES: ");
                        foreach (HandleFace facehandle in enhandlefaces)
                        {
                            Console.Write(facehandle._DataIndex + " ");
                        }
                        Console.Write("\n");
                    }

                }
            }
        }
    }
}
