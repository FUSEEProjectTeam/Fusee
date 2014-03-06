using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    class FusConverter
    {
        private BaseDocument _polyDoc;

        public SceneContainer FuseefyScene(BaseDocument doc)
        {

            _polyDoc = doc.Polygonize();

            Logger.Debug("Fuseefy Me!");

            BaseContainer machineFeatures = C4dApi.GetMachineFeatures();
            GeData userNameData = machineFeatures.GetDataPointer(C4dApi.MACHINEINFO_USERNAME);
            String userName = userNameData.GetString();

            SceneContainer root = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = userName,
                    Generator = "FUSEE Export Plugin for Cinema4D",
                    Version = 1
                },
                Children = FuseefyOb(_polyDoc.GetFirstObject())
            };
            return root;
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

                float3 normalD;
                if (normalsOb != null)
                {
                    normals.Add(normalsOb[iNorm++]);
                    normals.Add(normalsOb[iNorm++]);
                    normals.Add(normalsOb[iNorm++]);
                    normalD = normalsOb[iNorm++];
                }
                else
                {
                    float3 faceNormal = CalcFaceNormal((float3) a, (float3) b, (float3) c);
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normalD = faceNormal;
                }

                tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 2), (ushort) (nNewVerts + 1)});

                if (c != d)
                {
                    // The Polyogon is not only a triangle, but a quad. Add the second triangle.
                    verts.Add((float3) d);
                    normals.Add(normalD);
                    tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 3), (ushort) (nNewVerts + 2)});
                    nNewVerts += 1;
                }
                nNewVerts += 3;
            }
            return new MeshContainer()
            {
                Normals = normals.ToArray(),
                Vertices = verts.ToArray(),
                Triangles = tris.ToArray(),
            };
        }

        public static float3 CalcFaceNormal(float3 vert0, float3 vert1, float3 vert2)
        {
            float3 v1 = vert0 - vert1;
            float3 v2 = vert0 - vert2;
            return float3.Normalize(float3.Cross(v1, v2));
        }


        private MaterialContainer GetMaterial(BaseObject ob)
        {
            for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
            {
                TextureTag texTag = tag as TextureTag;
                if (texTag == null)
                    continue;
                BaseMaterial material = texTag.GetMaterial();
                // Take the first and leave. Todo: for each found material split the geometry and add a geometry/material combination
                
                MaterialContainer mcRet = new MaterialContainer();
                
                BaseChannel diffuseChannel = material.GetChannel(C4dApi.CHANNEL_COLOR);
                if (diffuseChannel != null)
                {
                    mcRet.HasDiffuse = true;
                    BaseContainer data = diffuseChannel.GetData();
                    mcRet.DiffuseColor = (float3) data.GetVector(C4dApi.BASECHANNEL_COLOR_EX);
                    string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
                    diffuseChannel.InitTexture(new InitRenderStruct(_polyDoc));
                    BaseBitmap bitmap = diffuseChannel.GetBitmap();
                    if (bitmap != null)
                    {
                        BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY);
                        compressionContainer.SetReal(C4dApi.JPGSAVER_QUALITY, 70.0);
                        bitmap.Save(new Filename("C:\\Users\\mch\\Temp\\FuseeWebPlayer\\ABitmap.jpg"), C4dApi.FILTER_JPG, compressionContainer, SAVEBIT.SAVEBIT_0);
                    }
                    diffuseChannel.FreeTexture();
                }

                BaseChannel specularColorChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
                if (specularColorChannel != null)
                {
                    mcRet.HasSpecular = true;
                    BaseContainer data = specularColorChannel.GetData();
                    mcRet.SpecularColor = (float3) data.GetVector(C4dApi.BASECHANNEL_COLOR_EX);
                    string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
                    BaseBitmap bitmap = specularColorChannel.GetBitmap();
                    if (bitmap != null)
                    {
                        BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY);
                        compressionContainer.SetReal(C4dApi.JPGSAVER_QUALITY, 70.0);
                        bitmap.Save(new Filename("C:\\Users\\mch\\Temp\\FuseeWebPlayer\\ABitmapS.jpg"), C4dApi.FILTER_JPG, compressionContainer, SAVEBIT.SAVEBIT_0);
                    }
                }

                BaseChannel specularChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
                if (specularChannel != null)
                {
                    mcRet.HasSpecular = true;
                    BaseContainer data = specularChannel.GetData();
                    for (int i=0, id=0; -1 != (id = data.GetIndexId(i)); i++)
                    {
                        if (data.GetType(id) == C4dApi.DA_REAL)
                        {
                            double d = data.GetReal(id);
                        }
                        else if (data.GetType(id) == C4dApi.DA_VECTOR)
                        {
                            double3 v = data.GetVector(id);
                        }
                    } 
 
                }
                return mcRet;
            }
            return null;
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
                soc.Material = GetMaterial(ob);
                soc.Mesh = GetMesh(ob);
                soc.Children = FuseefyOb(ob.GetDown());
                ret.Add(soc);
                ob = ob.GetNext();
            } while (ob != null);
            return ret;
        }
    }
}
