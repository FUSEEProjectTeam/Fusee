using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    [SceneSaverPlugin(1000048,
       Name = "FUSEE 3D (*.fus)",
       Suffix = "fus")
    ]

    // The main class for your plugin
    public class FuExporter : SceneSaverData
    {

        /* Your Executeable code for the plugin
         * This is your main function
         * Never change return type - this is declarated in c++ code
         */

        public override bool Init(GeListNode node)
        {
            return true;
        }


        public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
        {
            BaseDocument polyDoc = doc.Polygonize();

            Logger.Debug("Fuseefy Me!");

            SceneContainer root = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = "",
                    Generator = "FUSEE Export Plugin for Cinema4D",
                    Version = 1
                },
                Children = FuseefyOb(polyDoc.GetFirstObject())
            };

            var ser = new Serializer();
            using (var file = File.Create(name.GetString()))
            {
                ser.Serialize(file, root);
            }

            return FILEERROR.FILEERROR_NONE;
        }


        private static MeshContainer GetMesh(BaseObject ob)
        {
            PolygonObject polyOb = ob as PolygonObject;
            if (polyOb == null)
                return null;

            float3[] normalsOb = polyOb.CreatePhongNormals();
            List<float3> normals = new List<float3>();

            int nPolys = polyOb.GetPolygonCount();
            ushort nNewVerts = 0;
            List<float3> verts = new List<float3>();
            List<ushort> tris = new List<ushort>();

            int iNorm = 0;
            for (int i = 0; i < nPolys; i++)
            {
                CPolygon poly = polyOb.GetPolygonAt(i);
                double3 a = polyOb.GetPointAt(poly.a);
                double3 b = polyOb.GetPointAt(poly.b);
                double3 c = polyOb.GetPointAt(poly.c);
                double3 d = polyOb.GetPointAt(poly.d);

                verts.Add((float3) a);
                verts.Add((float3) b);
                verts.Add((float3) c);

                normals.Add(normalsOb[iNorm++]);
                normals.Add(normalsOb[iNorm++]);
                normals.Add(normalsOb[iNorm++]);

                tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 2), (ushort) (nNewVerts + 1)});

                if (c != d)
                {
                    // The Polyogon is not only a triangle, but a quad. Add the second triangle.
                    verts.Add((float3) d);
                    normals.Add(normalsOb[iNorm]);
                    tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 3), (ushort) (nNewVerts + 2)});
                    nNewVerts += 1;
                }
                iNorm++; // Consume the 4th normal anyway
                nNewVerts += 3;
            }
            return new MeshContainer()
            {
                Normals = normals.ToArray(),
                Vertices = verts.ToArray(),
                Triangles = tris.ToArray(),
            };
        }


        private float3 GetColor(BaseObject ob)
        {
            for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
            {
                TextureTag texTag = tag as TextureTag;
                if (texTag == null)
                    continue;
                BaseMaterial material = texTag.GetMaterial();
                return (float3)material.GetAverageColor(C4dApi.CHANNEL_COLOR);
                // BaseChannel channel = material.GetChannel()
            }
            return new float3(0, 0, 0);
        }


        private List<SceneObjectContainer> FuseefyOb(BaseObject ob)
        {
            if (ob == null)
                return null;

            List<SceneObjectContainer> ret = new List<SceneObjectContainer>();
            do
            {
                SceneObjectContainer soc = new SceneObjectContainer();

                soc.Name = ob.GetName();

                double4x4 mtxD = ob.GetMl();
                soc.Transform = (float4x4) mtxD;
                soc.Color = GetColor(ob);
                soc.Mesh = GetMesh(ob);
                soc.Children = FuseefyOb(ob.GetDown());
                ret.Add(soc);
                ob = ob.GetNext();
            } while (ob != null);
            return ret;
        }
    }
}
