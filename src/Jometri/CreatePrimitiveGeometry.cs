﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// This class provides public methods to create primitive geometry from scratch. The Geometry is stored in DCEL
    /// </summary>
    public class CreatePrimitiveGeometry
    {
        #region Creating Primitives  

        /// <summary>
        /// Creates and returns a Cuboid as DCEL with the given dimensions, centered in the world coordinate system.
        /// </summary>
        /// <param name="dimensionX">Width in X-dimension.</param>
        /// <param name="dimensionY">Height in Y-dimension.</param>
        /// <param name="dimensionZ">Depth in Z-dimension.</param>
        /// <returns></returns>
        public static Geometry CreateCuboidGeometry(float dimensionX, float dimensionY, float dimensionZ)
        {
            //check input
            if (dimensionX <= 0 || dimensionY <= 0 || dimensionZ <= 0) throw new ArgumentException("The dimension values can not be <= 0");

            var xPos = dimensionX / 2.0f;
            var yPos = dimensionY / 2.0f;
            var zPos = dimensionZ / 2.0f;

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
        /// <param name="horizontalResolution">Lines of latitude, smallest value is 3.</param> 
        /// <param name="verticalResolution">Lines of longitude, smallest value is 3.</param>
        /// <returns>A UV-Sphere centered in the world coordinate system as a DCEL.</returns>
        public static Geometry CreateSpehreGeometry(float radius, int horizontalResolution, int verticalResolution)
        {
            //check input
            if (radius <= 0) throw new ArgumentException("Radius can not be <= 0");
            if (horizontalResolution <= 3) horizontalResolution = 3;
            if (verticalResolution <= 2) verticalResolution = 2;

            var sphere = new Geometry();
            var northPole = new Vertex(sphere.CreateVertHandleId(), new float3(0, radius, 0));
            var southPole = new Vertex(sphere.CreateVertHandleId(), new float3(0, -radius, 0));

            var horizontalAngleStep = System.Math.PI * 2 / horizontalResolution; // s
            var verrticalAngleStep = System.Math.PI / verticalResolution; // t

            var currentLatitudeVerticesHandles = new int[horizontalResolution];
            var lastlatitudeVerticesHandles = new int[horizontalResolution]; //stores last vertices to connect them later with the next latitude vertices

            for (var i = 1; i < verticalResolution + 1; i++)
            {
                //create all vertices
                if (i < verticalResolution)
                {
                    for (var j = 0; j < horizontalResolution; j++)
                    {
                        //create all Vertices of current latitude
                        var xPos = (float)(radius * System.Math.Sin(horizontalAngleStep * j) * System.Math.Sin(verrticalAngleStep * i));
                        var yPos = (float)(radius * System.Math.Cos(verrticalAngleStep * (i)));
                        var zPos = (float)(radius * System.Math.Cos(horizontalAngleStep * j) * System.Math.Sin(verrticalAngleStep * i));

                        var circleVertex = new Vertex(sphere.CreateVertHandleId(), new float3(xPos, yPos, zPos));
                        sphere.DictVertices.Add(circleVertex.Handle, circleVertex);
                        currentLatitudeVerticesHandles[j] = circleVertex.Handle;
                    }
                }

                //create faces
                var topHeHandles = new int[horizontalResolution];
                for (var j = 0; j < horizontalResolution; j++)
                {
                    // bottom triangles of sphere
                    if (i == verticalResolution)
                    {
                        var bottomTriangle = new Face(sphere.CreateFaceHandleId());

                        var h1 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h2 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h3 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        topHeHandles[j] = h1.Handle;

                        h1.NextHalfEdge = h2.Handle;
                        h2.NextHalfEdge = h3.Handle;
                        h3.NextHalfEdge = h1.Handle;
                        h1.PrevHalfEdge = h3.Handle;
                        h2.PrevHalfEdge = h1.Handle;
                        h3.PrevHalfEdge = h2.Handle;
                        h1.IncidentFace = bottomTriangle.Handle;
                        h2.IncidentFace = bottomTriangle.Handle;
                        h3.IncidentFace = bottomTriangle.Handle;
                        h1.OriginVertex = currentLatitudeVerticesHandles[j];
                        h2.OriginVertex = j == horizontalResolution - 1 ? currentLatitudeVerticesHandles[0] : currentLatitudeVerticesHandles[j + 1];
                        h3.OriginVertex = southPole.Handle;
                        bottomTriangle.OuterHalfEdge = h1.Handle;

                        southPole.IncidentHalfEdge = h3.Handle;

                        //write changes
                        sphere.DictHalfEdges.Add(h1.Handle, h1);
                        sphere.DictHalfEdges.Add(h2.Handle, h2);
                        sphere.DictHalfEdges.Add(h3.Handle, h3);
                        sphere.DictFaces.Add(bottomTriangle.Handle, bottomTriangle);
                    }

                    // top triangles of sphere
                    else if (i == 1)
                    {
                        var topTriangle = new Face(sphere.CreateFaceHandleId());

                        var h1 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h2 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h3 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        topHeHandles[j] = h1.Handle;

                        h1.NextHalfEdge = h2.Handle;
                        h2.NextHalfEdge = h3.Handle;
                        h3.NextHalfEdge = h1.Handle;
                        h1.PrevHalfEdge = h3.Handle;
                        h2.PrevHalfEdge = h1.Handle;
                        h3.PrevHalfEdge = h2.Handle;
                        h1.IncidentFace = topTriangle.Handle;
                        h2.IncidentFace = topTriangle.Handle;
                        h3.IncidentFace = topTriangle.Handle;
                        h1.OriginVertex = j == horizontalResolution - 1 ? currentLatitudeVerticesHandles[0] : currentLatitudeVerticesHandles[j + 1];
                        h2.OriginVertex = currentLatitudeVerticesHandles[j];
                        h3.OriginVertex = northPole.Handle;
                        topTriangle.OuterHalfEdge = h1.Handle;

                        var currentVertex = sphere.GetVertexByHandle(currentLatitudeVerticesHandles[j]);
                        currentVertex.IncidentHalfEdge = h2.Handle;
                        northPole.IncidentHalfEdge = h3.Handle;

                        //write changes
                        sphere.DictHalfEdges.Add(h1.Handle, h1);
                        sphere.DictHalfEdges.Add(h2.Handle, h2);
                        sphere.DictHalfEdges.Add(h3.Handle, h3);
                        sphere.DictFaces.Add(topTriangle.Handle, topTriangle);
                        sphere.ReplaceVertex(currentVertex);
                    }
                    // middle quads of sphere
                    else
                    {
                        var quad = new Face(sphere.CreateFaceHandleId());

                        var h1 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h2 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h3 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        var h4 = new HalfEdge(sphere.CreateHalfEdgeHandleId());
                        topHeHandles[j] = h1.Handle;

                        h1.NextHalfEdge = h2.Handle;
                        h2.NextHalfEdge = h3.Handle;
                        h3.NextHalfEdge = h4.Handle;
                        h4.NextHalfEdge = h1.Handle;
                        h1.PrevHalfEdge = h4.Handle;
                        h2.PrevHalfEdge = h1.Handle;
                        h3.PrevHalfEdge = h2.Handle;
                        h4.PrevHalfEdge = h3.Handle;
                        h1.IncidentFace = quad.Handle;
                        h2.IncidentFace = quad.Handle;
                        h3.IncidentFace = quad.Handle;
                        h4.IncidentFace = quad.Handle;
                        quad.OuterHalfEdge = h1.Handle;

                        h4.OriginVertex = currentLatitudeVerticesHandles[j];
                        h3.OriginVertex = j == horizontalResolution - 1 ? currentLatitudeVerticesHandles[0] : currentLatitudeVerticesHandles[j + 1];
                        h2.OriginVertex = j == horizontalResolution - 1 ? lastlatitudeVerticesHandles[0] : lastlatitudeVerticesHandles[j + 1];
                        h1.OriginVertex = lastlatitudeVerticesHandles[j];

                        var currentVertex = sphere.GetVertexByHandle(currentLatitudeVerticesHandles[j]);
                        currentVertex.IncidentHalfEdge = h4.Handle;

                        //write changes
                        sphere.DictFaces.Add(quad.Handle, quad);
                        sphere.DictHalfEdges.Add(h1.Handle, h1);
                        sphere.DictHalfEdges.Add(h2.Handle, h2);
                        sphere.DictHalfEdges.Add(h3.Handle, h3);
                        sphere.DictHalfEdges.Add(h4.Handle, h4);
                        sphere.ReplaceVertex(currentVertex);
                    }

                }

                //set twins 
                for (int j = 0; j < horizontalResolution; j++)
                {
                    //set twins of adjacent triangles bottom
                    if (i == 1)
                    {
                        int nextH1Index;
                        nextH1Index = j == 0 ? horizontalResolution - 1 : j - 1;
                        var h1 = sphere.GetHalfEdgeByHandle(topHeHandles[j]);
                        var h2 = sphere.GetHalfEdgeByHandle(h1.NextHalfEdge);
                        var nextH1 = sphere.GetHalfEdgeByHandle(topHeHandles[nextH1Index]);
                        var nextH3 = sphere.GetHalfEdgeByHandle(nextH1.PrevHalfEdge);
                        nextH3.TwinHalfEdge = h2.Handle;
                        h2.TwinHalfEdge = nextH3.Handle;

                        sphere.ReplaceHalfEdge(nextH3);
                        sphere.ReplaceHalfEdge(h2);
                    }
                    else if (i == verticalResolution)
                    {
                        int nextH1Index;
                        nextH1Index = j == 0 ? horizontalResolution - 1 : j - 1;
                        var h1 = sphere.GetHalfEdgeByHandle(topHeHandles[j]);
                        var h3 = sphere.GetHalfEdgeByHandle(h1.PrevHalfEdge);
                        var nextH1 = sphere.GetHalfEdgeByHandle(topHeHandles[nextH1Index]);
                        var nextH2 = sphere.GetHalfEdgeByHandle(nextH1.NextHalfEdge);
                        nextH2.TwinHalfEdge = h3.Handle;
                        h3.TwinHalfEdge = nextH2.Handle;

                        sphere.ReplaceHalfEdge(nextH2);
                        sphere.ReplaceHalfEdge(h3);
                    }
                    else //set twins of adjacent quads 
                    {
                        var h1 = sphere.GetHalfEdgeByHandle(topHeHandles[j]);
                        var h4 = sphere.GetHalfEdgeByHandle(h1.PrevHalfEdge);

                        int nextH1Index;

                        nextH1Index = j == 0 ? horizontalResolution - 1 : j - 1;

                        var nextH1 = sphere.GetHalfEdgeByHandle(topHeHandles[nextH1Index]);
                        var nextH2 = sphere.GetHalfEdgeByHandle(nextH1.NextHalfEdge);

                        nextH2.TwinHalfEdge = h4.Handle;
                        h4.TwinHalfEdge = nextH2.Handle;

                        sphere.ReplaceHalfEdge(nextH2);
                        sphere.ReplaceHalfEdge(h4);
                    }

                    //set twin of face on top
                    if (i > 1)
                    {
                        var h1 = sphere.GetHalfEdgeByHandle(topHeHandles[j]);
                        var lastVertex = sphere.GetVertexByHandle(lastlatitudeVerticesHandles[j]);
                        var topH1 = sphere.GetHalfEdgeByHandle(lastVertex.IncidentHalfEdge);
                        while (true)
                        {
                            if (topH1.TwinHalfEdge == 0) break;
                            topH1 = sphere.GetHalfEdgeByHandle(topH1.NextHalfEdge);
                        }
                        topH1.TwinHalfEdge = h1.Handle;
                        h1.TwinHalfEdge = topH1.Handle;
                        sphere.ReplaceHalfEdge(h1);
                        sphere.ReplaceHalfEdge(topH1);
                    }

                }

                Array.Copy(currentLatitudeVerticesHandles, lastlatitudeVerticesHandles,
                    currentLatitudeVerticesHandles.Length);
            }
            sphere.DictVertices.Add(northPole.Handle, northPole);
            sphere.DictVertices.Add(southPole.Handle, southPole);

            //calculate normals
            var allFaces = sphere.GetAllFaces().ToList();
            foreach (var face in allFaces)
            {
                sphere.SetFaceNormal(sphere.GetFaceVertices(face.Handle).ToList(), face);
            }

            return sphere;
        }

        /// <summary>
        /// Creates and returns a cone with the given dimensions.
        /// </summary>
        /// <param name="baseRadius">The radius of the base circle.</param>
        /// <param name="dimensionY">The height of the cone.</param>
        /// <param name="sliceCount">The horizontal resolution of the base circle. Min value is 3. For a basic cone 15.</param>
        /// <returns></returns>
        public static Geometry CreateConeGeometry(float baseRadius, float dimensionY, int sliceCount)
        {
            //check input
            if (sliceCount < 3) sliceCount = 3;
            if (baseRadius <= 0 || dimensionY <= 0) throw new ArgumentException("You can not input parameters <= 0");

            var cone = new Geometry();
            var northPole = new Vertex(cone.CreateVertHandleId(), new float3(0, dimensionY / 2, 0));
            var southPole = new Vertex(cone.CreateVertHandleId(), new float3(0, -dimensionY / 2, 0));

            var angleStep = System.Math.PI * 2 / sliceCount;
            var yPos = -dimensionY / 2.0f;

            int[] firstHandles = null; //stores the handles of the first slice to connect it later with the last slice

            var lastH3 = new HalfEdge();
            var lastH2 = new HalfEdge();
            var lastVertex = southPole;

            for (var i = 1; i < sliceCount + 1; i++)
            {
                var x = (float)System.Math.Cos(angleStep * i) * baseRadius;
                var z = (float)System.Math.Sin(angleStep * i) * baseRadius;

                var tempVertex = new Vertex(cone.CreateVertHandleId(), new float3(x, yPos, z));

                //south to temp
                var h1 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                var h2 = new HalfEdge(cone.CreateHalfEdgeHandleId()); //twin of h1

                tempVertex.IncidentHalfEdge = h2.Handle;
                h1.OriginVertex = southPole.Handle;
                h2.OriginVertex = tempVertex.Handle;
                h1.TwinHalfEdge = h2.Handle;
                h2.TwinHalfEdge = h1.Handle;

                //temp to north
                var h3 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                var h4 = new HalfEdge(cone.CreateHalfEdgeHandleId());

                northPole.IncidentHalfEdge = h3.Handle;
                southPole.IncidentHalfEdge = h1.Handle;

                h3.OriginVertex = northPole.Handle;
                h4.OriginVertex = tempVertex.Handle;
                h3.TwinHalfEdge = h4.Handle;
                h4.TwinHalfEdge = h3.Handle;

                if (lastVertex == southPole)
                {
                    firstHandles = new[] { tempVertex.Handle, h1.Handle, h4.Handle };
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
                var h5 = new HalfEdge(cone.CreateHalfEdgeHandleId());
                var h6 = new HalfEdge(cone.CreateHalfEdgeHandleId());

                h5.OriginVertex = lastVertex.Handle;
                h6.OriginVertex = tempVertex.Handle;
                h5.TwinHalfEdge = h6.Handle;
                h6.TwinHalfEdge = h5.Handle;

                //create top triangles south-temp-last
                var triangle1 = new Face(cone.CreateFaceHandleId());
                h5.NextHalfEdge = h4.Handle;
                triangle1.OuterHalfEdge = h5.Handle;
                h4.NextHalfEdge = lastH3.Handle;
                lastH3.NextHalfEdge = h5.Handle;
                h5.IncidentFace = triangle1.Handle;
                h4.IncidentFace = triangle1.Handle;
                lastH3.IncidentFace = triangle1.Handle;
                h5.PrevHalfEdge = lastH3.Handle;
                lastH3.PrevHalfEdge = h4.Handle;
                h4.PrevHalfEdge = h5.Handle;

                //north-last-temp
                var triangle2 = new Face(cone.CreateFaceHandleId());
                h6.NextHalfEdge = lastH2.Handle;
                triangle2.OuterHalfEdge = h6.Handle;
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
                cone.DictHalfEdges.Add(h5.Handle, h5);
                cone.DictHalfEdges.Add(h6.Handle, h6);
                cone.ReplaceHalfEdge(lastH2);
                cone.ReplaceHalfEdge(lastH3);
                cone.DictFaces.Add(triangle1.Handle, triangle1);
                cone.DictFaces.Add(triangle2.Handle, triangle2);


                lastH2 = h2;
                lastH3 = h3;
                lastVertex = tempVertex;
            }
            //add south and north pole
            cone.DictVertices.Add(southPole.Handle, southPole);
            cone.DictVertices.Add(northPole.Handle, northPole);

            //create last 2 triangles
            var firstVertex = cone.GetVertexByHandle(firstHandles[0]);
            var firtstH1 = cone.GetHalfEdgeByHandle(firstHandles[1]);
            var firstH4 = cone.GetHalfEdgeByHandle(firstHandles[2]);

            var lastH5 = new HalfEdge(cone.CreateHalfEdgeHandleId());
            var lastH6 = new HalfEdge(cone.CreateHalfEdgeHandleId());

            lastH5.OriginVertex = lastVertex.Handle;
            lastH6.OriginVertex = firstVertex.Handle;
            lastH5.TwinHalfEdge = lastH6.Handle;
            lastH6.TwinHalfEdge = lastH5.Handle;

            //create to triangles south-temp-last, north-last-temp
            var triangleL1 = new Face(cone.CreateFaceHandleId()) { OuterHalfEdge = lastH5.Handle };
            lastH5.NextHalfEdge = firstH4.Handle;
            firstH4.NextHalfEdge = lastH3.Handle;
            lastH3.NextHalfEdge = lastH5.Handle;
            lastH5.IncidentFace = triangleL1.Handle;
            firstH4.IncidentFace = triangleL1.Handle;
            lastH3.IncidentFace = triangleL1.Handle;
            lastH5.PrevHalfEdge = lastH3.Handle;
            lastH3.PrevHalfEdge = firstH4.Handle;
            firstH4.PrevHalfEdge = lastH5.Handle;

            var triangleL2 = new Face(cone.CreateFaceHandleId()) { OuterHalfEdge = lastH6.Handle };
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
            foreach (var face in allFaces)
            {
                cone.SetFaceNormal(cone.GetFaceVertices(face.Handle).ToList(), face);
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
            //check input
            if (dimensionX <= 0 || dimensionY <= 0 || dimensionZ <= 0) throw new ArgumentException("The dimension values can not be <= 0");

            var xPos = dimensionX / 2.0f;
            var yPos = dimensionY / 2.0f;
            var zPos = dimensionZ / 2.0f;

            var pyramid = new Geometry();
            var positions = new float3[5];

            //stores all Vertices positions
            positions[0] = new float3(-xPos, -yPos, -zPos);
            positions[1] = new float3(xPos, -yPos, -zPos);
            positions[2] = new float3(xPos, -yPos, zPos);
            positions[3] = new float3(-xPos, -yPos, zPos);
            positions[4] = new float3(0, yPos, 0);

            var baseFace = new Face(6) { OuterHalfEdge = 4 };
            //create and add vertices 
            for (var i = 0; i < 5; i++)
            {
                Vertex current = new Vertex(pyramid.CreateVertHandleId(), positions[i]);
                if (i < 4) current.IncidentHalfEdge = i * 4 + 1;
                if (i == 4) current.IncidentHalfEdge = 3;
                pyramid.DictVertices.Add(current.Handle, current);
            }

            //create add Edges and Faces
            for (var i = 0; i < 4; i++)
            {
                var h1 = new HalfEdge(i * 4 + 1);
                var h2 = new HalfEdge(i * 4 + 2);
                var h3 = new HalfEdge(i * 4 + 3);
                var h4 = new HalfEdge(i * 4 + 4);

                var sideFace = new Face(i + 2) { OuterHalfEdge = h1.Handle };

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
            foreach (var face in allFaces)
            {
                var faceVertices = pyramid.GetFaceVertices(face.Handle).ToList();
                pyramid.SetFaceNormal(faceVertices, face);
            }

            return pyramid;
        }
        #endregion

        /*private static int[] CopyArray(int[] source)
        {
            var result = new int[source.Length];
            Buffer.BlockCopy(source, 0, result, 0, source.Length * sizeof(int));
            return result;

        }*/
    }
}
