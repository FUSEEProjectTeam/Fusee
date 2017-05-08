using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Jometri.Manipulation;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// This class provides public methods to create geometry from scratch. The Geometry is stored in DCEL
    /// </summary>
    public class CreateGeometry
    {
        #region Creating Primitives  

        /// <summary>
        /// Creates and returns a Cuboid as DCEL with the given dimensions, centred in the wolrds coordinate system.
        /// </summary>
        /// <param name="dimensionX">Width in X-dimension.</param>
        /// <param name="dimensionY">Height in Y-dimenison.</param>
        /// <param name="dimensionZ">Depth in Z-dimension.</param>
        /// <returns></returns>
        public static Geometry CreateCuboidGeometry(float dimensionX, float dimensionY, float dimensionZ)
        {
            float xPos = dimensionX / 2.0f;
            float yPos = dimensionY / 2.0f;
            float zPos = dimensionZ / 2.0f;


            var outlineRectangle = new PolyBoundary //CCW
            {
                Points = new List<float3>
                {
                    new float3(-xPos,yPos,-zPos),
                    new float3(-xPos,-yPos,-zPos),
                    new float3(xPos,-yPos,-zPos),
                    new float3(xPos,yPos,-zPos)
                },
                IsOuter = true
            };

            var outlinesCube = new List<PolyBoundary> { outlineRectangle };
            var cuboid = new Geometry(outlinesCube);
            cuboid.Extrude2DPolygon(dimensionZ, false);

            return cuboid;
        }

        /// <summary>
        /// Creates and returns a UV-Sphere as a DCEL with the specified dimensions centered in the worlds coordinate system.
        /// </summary>
        /// <param name="radius">Radius of the sphere.</param>
        /// <param name="horizontalResolution">Lines of latitude per hemisphere.</param>
        /// <param name="verticalResolution">Lines of longitude per hemisphere, smallest value is 1, has to be an odd number, will always be rounded to the nearest odd number downwards.</param>
        /// <returns></returns>
        public static Geometry CreateSpehreGeometry(float radius, int horizontalResolution, int verticalResolution)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates and returns a cone with the given dimensions
        /// </summary>
        /// <param name="baseRadius"></param>
        /// <param name="dimensionY"></param>
        /// <param name="scliceCount"></param> todo
        /// <returns></returns>
        public static Geometry CreateConeGeometry(float baseRadius, float dimensionY, int sliceCount)
        {
            Geometry cone = new Geometry();
            Vertex northPole = new Vertex(cone.CreateVertHandleId(), new float3(0, dimensionY / 2, 0));
            Vertex southPole = new Vertex(cone.CreateVertHandleId(), new float3(0, -dimensionY / 2, 0));

            double angleStep = System.Math.PI * 2 / sliceCount;
            float yPos = -dimensionY / 2.0f;

            int[] firstHandles=null;

            HalfEdge lastH3 = new HalfEdge();
            HalfEdge lastH2 = new HalfEdge();
            Vertex lastVertex = southPole;

            for (int i = 1; i < sliceCount +1; i++)
            {
                float x = (float)System.Math.Cos(angleStep * i);
                float z = (float)System.Math.Sin(angleStep * i);

                Vertex tempVertex = new Vertex(cone.CreateVertHandleId(), new float3(x, yPos, z));

                //south to temp
                HalfEdge h1 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                HalfEdge h2 = new HalfEdge(cone.CreateHalfEdgeHandleId()); //twin of h1

                tempVertex.IncidentHalfEdge = h2.Handle;
                h1.OriginVertex = southPole.Handle;
                h2.OriginVertex = tempVertex.Handle;
                h1.TwinHalfEdge = h2.Handle;
                h2.TwinHalfEdge = h1.Handle;

                //temp to north
                HalfEdge h3 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                HalfEdge h4 = new HalfEdge(cone.CreateHalfEdgeHandleId());

                northPole.IncidentHalfEdge = h3.Handle;
                southPole.IncidentHalfEdge = h1.Handle;

                h3.OriginVertex = northPole.Handle;
                h4.OriginVertex = tempVertex.Handle;
                h3.TwinHalfEdge = h4.Handle;
                h4.TwinHalfEdge = h3.Handle;

                if (lastVertex == southPole)
                {
                    firstHandles = new[] {tempVertex.Handle, h1.Handle, h4.Handle};
                    lastH3 = h3;
                    lastH2 = h2;
                    cone.DictVertices.Add(tempVertex.Handle, tempVertex);
                    cone.DictHalfEdges.Add(h1.Handle, h1);
                    cone.DictHalfEdges.Add(h2.Handle, h2);
                    cone.DictHalfEdges.Add(h3.Handle, h3);
                    cone.DictHalfEdges.Add(h4.Handle, h4);
                    lastVertex = tempVertex;
                    continue;
                }

                //temp to last
                HalfEdge h5 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                HalfEdge h6 = new HalfEdge(cone.CreateHalfEdgeHandleId());

                h6.OriginVertex = tempVertex.Handle;
                h5.OriginVertex = lastVertex.Handle;
                h5.TwinHalfEdge = h6.Handle;
                h6.TwinHalfEdge = h5.Handle;

                //create to triangles south-temp-last, north-last-temp
                Face triangle1 = new Face(cone.CreateFaceHandleId());
                triangle1.OuterHalfEdge = h5.Handle;
                h5.NextHalfEdge = h4.Handle;
                h4.NextHalfEdge = lastH3.Handle;
                lastH3.NextHalfEdge = h5.Handle;
                h5.IncidentFace = triangle1.Handle;
                h4.IncidentFace = triangle1.Handle;
                lastH3.IncidentFace = triangle1.Handle;
                h5.PrevHalfEdge = lastH3.Handle;
                lastH3.PrevHalfEdge = h4.Handle;
                h4.PrevHalfEdge = h5.Handle;

                Face triangle2 = new Face(cone.CreateFaceHandleId());
                triangle2.OuterHalfEdge = h6.Handle;
                h6.NextHalfEdge = lastH2.Handle;
                lastH2.NextHalfEdge = h1.Handle;
                h1.NextHalfEdge = h6.Handle;
                h6.IncidentFace = triangle2.Handle;
                lastH2.IncidentFace = triangle2.Handle;
                h1.IncidentFace = triangle2.Handle;
                h6.PrevHalfEdge = h1.Handle;
                h1.PrevHalfEdge = lastH2.Handle;
                lastH2.PrevHalfEdge = h6.Handle;

                //write
                cone.DictVertices.Add(tempVertex.Handle, tempVertex);
                cone.DictHalfEdges.Add(h1.Handle, h1);
                cone.DictHalfEdges.Add(h2.Handle, h2);
                cone.DictHalfEdges.Add(h3.Handle, h3);
                cone.DictHalfEdges.Add(h4.Handle, h4);
                cone.DictHalfEdges.Add(h5.Handle,h5);
                cone.DictHalfEdges.Add(h6.Handle,h6);
                cone.ReplaceHalfEdge(lastH2);
                cone.ReplaceHalfEdge(lastH3);
                cone.DictFaces.Add(triangle1.Handle,triangle1);
                cone.DictFaces.Add(triangle2.Handle,triangle2);


                lastH2 = h2;
                lastH3 = h3;
                lastVertex = tempVertex;
            }
            //add south and northpole
            cone.DictVertices.Add(southPole.Handle,southPole);
            cone.DictVertices.Add(northPole.Handle,northPole);

            //create last 2 traingles
            HalfEdge firtstH1 = cone.GetHalfEdgeByHandle(firstHandles[1]);
            HalfEdge firstH4 = cone.GetHalfEdgeByHandle(firstHandles[2]);
            Vertex firstVertex = cone.GetVertexByHandle(firstHandles[0]);

            HalfEdge lastH5 = new HalfEdge(cone.CreateHalfEdgeHandleId());
            HalfEdge lastH6 = new HalfEdge(cone.CreateHalfEdgeHandleId());

            lastH6.OriginVertex = firstVertex.Handle;
            lastH5.OriginVertex = lastVertex.Handle;
            lastH5.TwinHalfEdge = lastH6.Handle;
            lastH6.TwinHalfEdge = lastH5.Handle;

            //create to triangles south-temp-last, north-last-temp
            Face triangleL1 = new Face(cone.CreateFaceHandleId());
            triangleL1.OuterHalfEdge = lastH5.Handle;
            lastH5.NextHalfEdge = firstH4.Handle;
            firstH4.NextHalfEdge = lastH3.Handle;
            lastH3.NextHalfEdge = lastH5.Handle;
            lastH5.IncidentFace = triangleL1.Handle;
            firstH4.IncidentFace = triangleL1.Handle;
            lastH3.IncidentFace = triangleL1.Handle;
            lastH5.PrevHalfEdge = lastH3.Handle;
            lastH3.PrevHalfEdge = firstH4.Handle;
            firstH4.PrevHalfEdge = lastH5.Handle;

            Face triangleL2 = new Face(cone.CreateFaceHandleId());
            triangleL2.OuterHalfEdge = lastH6.Handle;
            lastH6.NextHalfEdge = lastH2.Handle;
            lastH2.NextHalfEdge = firtstH1.Handle;
            firtstH1.NextHalfEdge = lastH6.Handle;
            lastH6.IncidentFace = triangleL2.Handle;
            lastH2.IncidentFace = triangleL2.Handle;
            firtstH1.IncidentFace = triangleL2.Handle;
            lastH6.PrevHalfEdge = firtstH1.Handle;
            firtstH1.PrevHalfEdge = lastH2.Handle;
            lastH2.PrevHalfEdge = lastH6.Handle;

            //write
            cone.ReplaceHalfEdge(firtstH1);
            cone.DictHalfEdges.Add(lastH5.Handle, lastH5);
            cone.DictHalfEdges.Add(lastH6.Handle, lastH6);
            cone.ReplaceHalfEdge(lastH2);
            cone.ReplaceHalfEdge(lastH3);
            cone.ReplaceHalfEdge(firstH4);
            cone.DictFaces.Add(triangleL1.Handle, triangleL1);
            cone.DictFaces.Add(triangleL2.Handle, triangleL2);

            //face normals
            var allFaces = cone.GetAllFaces().ToList();
            foreach (Face face in allFaces)
            {
               cone.SetFaceNormal(cone.GetFaceVertices(face.Handle).ToList(),face);
            }

            return cone;
        }

        /// <summary>
        /// Creates and returns a Pyramid as a DCEL with the specified dimensions centered in the worlds coordinate system.
        /// </summary>
        /// <param name="dimensionX">Width of the Pyramid in X-dimension</param>
        /// <param name="dimensionY">Height of the Pyramid in Y-dimension.</param>
        /// <param name="dimensionZ">Depth of the Pyramid in Z-dimension.</param>
        /// <returns></returns>
        public static Geometry CreatePyramidGeometry(float dimensionX, float dimensionY, float dimensionZ)
        {
            float xPos = dimensionX / 2.0f;
            float yPos = dimensionY / 2.0f;
            float zPos = dimensionZ / 2.0f;

            Geometry pyramid = new Geometry();
            float3[] positions = new float3[5];

            //stores all Vertices positions
            positions[0] = new float3(-xPos, -yPos, -zPos);
            positions[1] = new float3(xPos, -yPos, -zPos);
            positions[2] = new float3(xPos, -yPos, zPos);
            positions[3] = new float3(-xPos, -yPos, zPos);
            positions[4] = new float3(0, yPos, 0);

            Face baseFace = new Face(6);
            baseFace.OuterHalfEdge = 4;
            //create nad add vertices 
            for (int i = 0; i < 5; i++)
            {
                Vertex current = new Vertex(pyramid.CreateVertHandleId(), positions[i]);
                if (i < 4) current.IncidentHalfEdge = i * 4 + 1;
                if (i == 4) current.IncidentHalfEdge = 3;
                pyramid.DictVertices.Add(current.Handle, current);
            }

            //create add Edges and Faces
            for (int i = 0; i < 4; i++)
            {
                HalfEdge h1 = new HalfEdge(i * 4 + 1);
                HalfEdge h2 = new HalfEdge(i * 4 + 2);
                HalfEdge h3 = new HalfEdge(i * 4 + 3);
                HalfEdge h4 = new HalfEdge(i * 4 + 4);

                Face sideFace = new Face(i + 2);

                sideFace.OuterHalfEdge = h1.Handle;

                h1.IncidentFace = sideFace.Handle;
                h2.IncidentFace = sideFace.Handle;
                h3.IncidentFace = sideFace.Handle;
                h4.IncidentFace = baseFace.Handle;

                h1.NextHalfEdge = h2.Handle;
                h2.NextHalfEdge = h3.Handle;
                h3.NextHalfEdge = h1.Handle;

                h1.PrevHalfEdge = h3.Handle;
                h2.PrevHalfEdge = h1.Handle;
                h3.PrevHalfEdge = h2.Handle;

                h3.OriginVertex = pyramid.DictVertices[5].Handle;

                h1.TwinHalfEdge = h4.Handle;
                h4.TwinHalfEdge = h1.Handle;

                if (i < 3)
                {
                    h4.PrevHalfEdge = (i + 1) * 4 + 4;
                    h2.TwinHalfEdge = (i + 1) * 4 + 3;

                    h1.OriginVertex = pyramid.DictVertices[i + 1].Handle;
                    h2.OriginVertex = pyramid.DictVertices[i + 2].Handle;
                    h4.OriginVertex = pyramid.DictVertices[i + 2].Handle;
                }
                else
                {
                    h4.PrevHalfEdge = 4;
                    h2.TwinHalfEdge = 3;

                    h1.OriginVertex = pyramid.DictVertices[i + 1].Handle;
                    h2.OriginVertex = pyramid.DictVertices[1].Handle;
                    h4.OriginVertex = pyramid.DictVertices[1].Handle;
                }
                if (i > 0)
                {
                    h3.TwinHalfEdge = (i - 1) * 4 + 2;
                    h4.NextHalfEdge = (i - 1) * 4 + 4;
                }
                else
                {
                    h3.TwinHalfEdge = 3 * 4 + 2;
                    h4.NextHalfEdge = 3 * 4 + 4;
                }
                //add
                pyramid.DictHalfEdges.Add(h1.Handle, h1);
                pyramid.DictHalfEdges.Add(h2.Handle, h2);
                pyramid.DictHalfEdges.Add(h3.Handle, h3);
                pyramid.DictHalfEdges.Add(h4.Handle, h4);

                pyramid.DictFaces.Add(sideFace.Handle, sideFace);
            }
            pyramid.DictFaces.Add(baseFace.Handle, baseFace);
            pyramid.SetHighestHandles();

            var allFaces = pyramid.GetAllFaces().ToList();
            foreach (Face face in allFaces)
            {
                var faceVertices = pyramid.GetFaceVertices(face.Handle).ToList();
                pyramid.SetFaceNormal(faceVertices, face);
            }

            return pyramid;
        }
        #endregion

    }
}
