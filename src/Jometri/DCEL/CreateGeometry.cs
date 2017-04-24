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
        /// <param name="horizontalResolution">Lines of latitude per hemsphere.</param>
        /// <param name="verticalResolution">Lines of longitude per hemisphere.</param>
        /// <returns></returns>
        public static Geometry CreateSpehreGeometry(float radius, int horizontalResolution, int verticalResolution)
        {
            throw new NotImplementedException();
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

            positions[0] = new float3(-xPos, -yPos, -zPos);
            positions[1] = new float3(xPos, -yPos, -zPos);
            positions[2] = new float3(xPos, -yPos, zPos);
            positions[3] = new float3(-xPos, -yPos, zPos);
            positions[4] = new float3(0, yPos, 0);

            Face baseFace = new Face(6);
            baseFace.OuterHalfEdge = 4;
            //add vertices 
            for (int i = 0; i < 5; i++)
            {
                Vertex current = new Vertex(pyramid.CreateVertHandleId(), positions[i]);
                if (i < 4) current.IncidentHalfEdge = i * 4 + 1;
                if (i == 4) current.IncidentHalfEdge = 3;
                pyramid.DictVertices.Add(current.Handle, current);
            }

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
