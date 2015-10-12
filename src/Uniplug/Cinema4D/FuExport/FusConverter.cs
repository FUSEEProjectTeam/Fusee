using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    class VertexList : Collection<float3>
    {
        public float3 Min;
        public float3 Max;

        public VertexList() : base()
        {
        }

        public new void Add(float3 v)
        {
            if (Count == 0)
            {
                Min = v;
                Max = v;
            }
            else
            {
                Min.x = (v.x < Min.x) ? v.x : Min.x;
                Min.y = (v.y < Min.y) ? v.y : Min.y;
                Min.z = (v.z < Min.z) ? v.z : Min.z;
                Max.x = (v.x > Max.x) ? v.x : Max.x;
                Max.y = (v.y > Max.y) ? v.y : Max.y;
                Max.z = (v.z > Max.z) ? v.z : Max.z;
            }
            base.Add(v);
        }
    }

    class FusConverter
    {

        private BaseDocument _doc, _polyDoc;
        private List<string> _textureFiles = new List<string>();
        private string _sceneRootDir;

        private Dictionary<long, MaterialComponent> _materialCache;
        private List<AnimationTrackContainer> _tracks = new List<AnimationTrackContainer>();
        private WeightManager _weightManager;
        private bool _animationsPresent;

        public SceneContainer FuseefyScene(BaseDocument doc, string sceneRootDir, out List<string> textureFiles)
        {
            _animationsPresent = false;
            _materialCache = new Dictionary<long, MaterialComponent>();
            _sceneRootDir = sceneRootDir;
            _doc = doc;
            _weightManager = new WeightManager(_doc);

            Logger.Debug("Fuseefy Me!");

            String userName;
            using (BaseContainer machineFeatures = C4dApi.GetMachineFeatures())
            {
                GeData userNameData = machineFeatures.GetDataPointer(C4dApi.MACHINEINFO_USERNAME);
                userName = userNameData.GetString();
            }

            SceneContainer root = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = userName,
                    Generator = "FUSEE Export Plugin for Cinema4D",
                    Version = 1,
                    CreationDate = DateTime.Now.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"))
                },

                Children = FuseefyOb( _doc.GetFirstObject()),
            };

            // CreateWeightMap has to be called after creating the object-tree
            _weightManager.CreateWeightMap();
            textureFiles = _textureFiles;
            return root;
        }


        private static float3 AdjustNormal(float3 normalOrig, float3 normalFace)
        {
            if (float3.Dot(normalOrig, normalFace) < 0)
            {
                return -1.0f*normalOrig;
            }
            else
            {
                return normalOrig;
            }
        }

        private static void AddComponent(SceneNodeContainer snc, SceneComponentContainer scc)
        {
            if (scc == null || snc == null)
                return;

            if (snc.Components == null)
            {
                snc.Components = new List<SceneComponentContainer>();
            }
            snc.Components.Add(scc);
        }

        private static MeshComponent GetMesh(PolygonObject polyOb, float3[] normalsOb, UVWTag uvwTag, IEnumerable<int> range)
        {
            List<float3> normals = new List<float3>();

            ushort nNewVerts = 0;
            VertexList verts = new VertexList();
            List<float2> uvs = new List<float2>();
            List<ushort> tris = new List<ushort>();

            foreach (int i in range)
            {
                int iNorm = i*4;
                double3 a, b, c, d;
                using (CPolygon poly = polyOb.GetPolygonAt(i))
                {
                    a = polyOb.GetPointAt(poly.a);
                    b = polyOb.GetPointAt(poly.b);
                    c = polyOb.GetPointAt(poly.c);
                    d = polyOb.GetPointAt(poly.d);
                }
                
                float2 uvA = new float2(0, 0);
                float2 uvB = new float2(0, 1);
                float2 uvC = new float2(1, 1);
                float2 uvD = new float2(1, 0);

                if (uvwTag != null)
                {
                    using (UVWStruct uvw = uvwTag.GetSlow(i))
                    {
                        uvA = new float2((float) uvw.a.x, 1.0f - (float) uvw.a.y);
                        uvB = new float2((float) uvw.b.x, 1.0f - (float) uvw.b.y);
                        uvC = new float2((float) uvw.c.x, 1.0f - (float) uvw.c.y);
                        uvD = new float2((float) uvw.d.x, 1.0f - (float) uvw.d.y);
                    }
                }

                verts.Add((float3) a);
                verts.Add((float3) b);
                verts.Add((float3) c);

                uvs.Add(uvA);
                uvs.Add(uvB);
                uvs.Add(uvC);

                float3 faceNormal = CalcFaceNormal((float3)a, (float3)b, (float3)c);
                float3 normalD;
                if (normalsOb != null)
                {
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normalD = AdjustNormal(normalsOb[iNorm++], faceNormal);
                }
                else
                {
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normalD = faceNormal;
                }

                tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 2), (ushort) (nNewVerts + 1)});

                if (c != d)
                {
                    // The Polyogon is not a triangle, but a quad. Add the second triangle.
                    verts.Add((float3) d);
                    uvs.Add(uvD);
                    normals.Add(normalD);
                    tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 3), (ushort) (nNewVerts + 2)});
                    nNewVerts += 1;
                }
                nNewVerts += 3;
            }
            Debug.WriteLine(verts.Count);
            return new MeshComponent()
            {
                Normals = normals.ToArray(),
                Vertices = verts.ToArray(),
                Triangles = tris.ToArray(),
                UVs = uvs.ToArray(),
                BoundingBox = new AABBf(verts.Min, verts.Max)
            };
        }

        public static float3 CalcFaceNormal(float3 vert0, float3 vert1, float3 vert2)
        {
            float3 v1 = vert0 - vert1;
            float3 v2 = vert0 - vert2;
            return float3.Normalize(float3.Cross(v1, v2));
        }

        /// <summary>

        /// This method tries to make the best out of C4Ds seldom relationship between objects with
        /// multiple materials which can or can not be restricted to polygon selections and one or more UV sets.
        /// But there are unhandled cases:
        /// Multple UV tags are not supported. Overlapping polygon selections are probably handled differently.
        /// The awkward side effects when changing the tags' order in C4D are not reproduced.
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="soc"></param>


        private void VisitObject(BaseObject ob, SceneNodeContainer snc)
        {
            Collection<TextureTag> textureTags = new Collection<TextureTag>();
            Dictionary<string,  SelectionTag> selectionTags = new Dictionary<string, SelectionTag>();
            UVWTag uvwTag = null;
            CAWeightTag weightTag = null;


            // Iterate over the object's tags
            for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
            {
                // CAWeightTag - Save data to create the weight list later
                CAWeightTag wTag = tag as CAWeightTag;
                if (wTag != null)
                {
                    weightTag = wTag;
                    continue;
                }

                // TextureTag (Material - there might be more than one)
                TextureTag tex = tag as TextureTag;
                if (tex != null)
                {
                    textureTags.Add(tex);
                    continue;
                }
                // UVWTag - the texutre coordinates. We handle only one
                UVWTag uvw = tag as UVWTag;
                if (uvw != null)
                {
                    if (uvwTag == null)
                    {
                        uvwTag = uvw;
                    }
                    else
                    {
                        Logger.Error("Object " + ob.GetName() + " contains more than one texture tags. Cannot handle this. Only the first texture tag will be recognized.");
                    }
                    continue;
                }
                // Selection tag. Only recognize the polygon selections as they might be referenced in a TextureTag
                SelectionTag selection = tag as SelectionTag;
                string selTagName = tag.GetName();
                if (selection != null && selection.GetType() == C4dApi.Tpolygonselection && !string.IsNullOrEmpty(selTagName))    // One Type and three TypeIDs - You C4D programmer guys really suck
                {
                    selectionTags[selTagName] = selection;
                }
                // XPresso Tags - TBD
                XPressoTag xPresso = tag as XPressoTag;
                if (xPresso != null)
                {
                    // Handle XPresso tag
                    continue;
                }
            }

            TextureTag lastUnselectedTag = null;
            Collection<KeyValuePair<SelectionTag, TextureTag>> texSelList = new Collection<KeyValuePair<SelectionTag, TextureTag>>(); // Abused KeyValuePair. Should have been Pair...

            // Now iterate over the textureTags
            foreach (TextureTag texture in textureTags)
            {
                string selRef = "";
                using (BaseContainer texData = texture.GetData())
                {
                    selRef = texData.GetString(C4dApi.TEXTURETAG_RESTRICTION);
                }
                if (string.IsNullOrEmpty(selRef))
                {
                    // This material is not restricted to any polygon selection
                    lastUnselectedTag = texture;
                }
                else
                {
                    SelectionTag sel;
                    if (selectionTags.TryGetValue(selRef, out sel))
                    {
                        texSelList.Add(new KeyValuePair<SelectionTag, TextureTag>(sel, texture));
                    }
                }
            }

            // At this point we have the last texture tag not restricted to a seletion. This will become the Material of this FuseeObjectContainer
            // no matter if this object contains geometry or not
            if (lastUnselectedTag != null)

                AddComponent(snc, GetMaterial(lastUnselectedTag));

            // Further processing only needs to take place if the object contains any geometry at all.
            PolygonObject polyOb = ob as PolygonObject;

            // Check whether the object contains an unpolygonized mesh
            if (polyOb == null)
                polyOb = ob.GetCache(null) as PolygonObject;

            if (polyOb != null)
            {
                float3[] normalOb = polyOb.CreatePhongNormals();

                // Initialize the polygon index set
                int nPolys = polyOb.GetPolygonCount();
                HashSet<int> polyInxs = new HashSet<int>();
                for (int i = 0; i < nPolys; i++)
                    polyInxs.Add(i);

                foreach (KeyValuePair<SelectionTag, TextureTag> texSelItem in texSelList)
                {
                    HashSet<int> polyInxsSubset = new HashSet<int>();
                    BaseSelect bs = texSelItem.Key.GetBaseSelect();
                    int nSegments = bs.GetSegments();
                    for (int iSeg = 0; iSeg < nSegments; iSeg++)
                    {
                        int from = bs.GetRangeA(iSeg);
                        int to = bs.GetRangeB(iSeg);
                        for (int iSel = from; iSel <= to; iSel++)
                        {
                            polyInxs.Remove(iSel);
                            polyInxsSubset.Add(iSel);
                        }
                    }

                    // Now generate Polygons for this subset
                    if (polyInxsSubset.Count > 0)
                    {

                        if (snc.Children == null)
                            snc.Children = new List<SceneNodeContainer>();

                        SceneNodeContainer subSnc = new SceneNodeContainer();

                        AddComponent(subSnc, new TransformComponent()
                        {
                            Translation = new float3(0, 0, 0),
                            Rotation = new float3(0, 0, 0),
                            Scale = new float3(1, 1, 1)

                        });

                        AddComponent(subSnc, GetMaterial(texSelItem.Value));
                        subSnc.Name = snc.Name + "_" + texSelItem.Key.GetName();
                        AddComponent(subSnc, GetMesh(polyOb, normalOb, uvwTag, polyInxsSubset));
                        _weightManager.AddWeightData(subSnc, polyOb, weightTag, polyInxsSubset);

                        snc.Children.Add(subSnc);
                    }
                }

                // The remaining polygons directly go into the original mesh

                AddComponent(snc, GetMesh(polyOb, normalOb, uvwTag, polyInxs));
                _weightManager.AddWeightData(snc, polyOb, weightTag, polyInxs);
            }
            else if (ob.GetType() == C4dApi.Olight)
            {
                using (BaseContainer lightData = ob.GetData())
                // Just for debugging purposes
                for (int i = 0, id = 0; -1 != (id = lightData.GetIndexId(i)); i++)
                {
                    if (lightData.GetType(id) == C4dApi.DA_LONG)
                    {
                        int iii = lightData.GetInt32(id);
                    }
                    if (lightData.GetType(id) == C4dApi.DA_REAL)
                    {
                        double d = lightData.GetFloat(id);
                    }
                    else if (lightData.GetType(id) == C4dApi.DA_VECTOR)
                    {
                        double3 v = lightData.GetVector(id);
                    }
                };
            }
        }


        private MaterialComponent GetMaterial(TextureTag texTag)
        {
            Material material = texTag.GetMaterial() as Material;
            if (material == null)
                return null;

            long materialUid = material.RefUID();

            MaterialComponent mcRet;
            if (_materialCache.TryGetValue(materialUid, out mcRet))
                return mcRet;

            using (BaseContainer materialData = material.GetData())
            {

                mcRet = new MaterialComponent();
                // Just for debugging purposes
                for (int i = 0, id = 0; -1 != (id = materialData.GetIndexId(i)); i++)
                {
                    if (materialData.GetType(id) == C4dApi.DA_LONG)
                    {
                        int iii = materialData.GetInt32(id);
                    }
                    if (materialData.GetType(id) == C4dApi.DA_REAL)
                    {
                        double d = materialData.GetFloat(id);
                    }
                    else if (materialData.GetType(id) == C4dApi.DA_VECTOR)
                    {
                        double3 v = materialData.GetVector(id);
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_COLOR))
                {
                    BaseChannel diffuseChannel = material.GetChannel(C4dApi.CHANNEL_COLOR);
                    if (diffuseChannel != null)
                    {
                        mcRet.Diffuse = new MatChannelContainer();

                        using (BaseContainer data = diffuseChannel.GetData())
                        {
                            mcRet.Diffuse.Color = (float3) (data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Diffuse.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Diffuse.Texture = GetTexture(data, diffuseChannel);
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_SPECULARCOLOR))
                {
                    BaseChannel specularChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
                    if (specularChannel != null)
                    {
                        mcRet.Specular = new SpecularChannelContainer();

                        using (BaseContainer data = specularChannel.GetData())
                        {
                            mcRet.Specular.Color = (float3)(data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Specular.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Specular.Texture = GetTexture(data, specularChannel);
                            mcRet.Specular.Shininess =
                                CalculateShininess((float)materialData.GetFloat(C4dApi.MATERIAL_SPECULAR_WIDTH));
                            mcRet.Specular.Intensity = (float)(1.5 * materialData.GetFloat(C4dApi.MATERIAL_SPECULAR_HEIGHT));
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_LUMINANCE))
                {
                    BaseChannel emissiveChannel = material.GetChannel(C4dApi.CHANNEL_LUMINANCE);
                    if (emissiveChannel != null)
                    {
                        mcRet.Emissive = new MatChannelContainer();

                        using (BaseContainer data = emissiveChannel.GetData())
                        {
                            mcRet.Emissive.Color = (float3)(data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Emissive.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Emissive.Texture = GetTexture(data, emissiveChannel);
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_NORMAL))
                {
                    BaseChannel bumpChannel = material.GetChannel(C4dApi.CHANNEL_NORMAL);
                    if (bumpChannel != null)
                    {
                        mcRet.Bump = new BumpChannelContainer();

                        using (BaseContainer data = bumpChannel.GetData())
                        {
                            mcRet.Bump.Intensity = (float)materialData.GetFloat(C4dApi.MATERIAL_NORMAL_STRENGTH);
                            mcRet.Bump.Texture = GetTexture(data, bumpChannel);
                        }
                    }
                }
            }

            /*
            // Just for debugging purposes
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
            */
            _materialCache[materialUid] = mcRet;
            return mcRet;
        }

        private static float CalculateShininess(float p)
        {
            // The minimum shininess
            float minS = 2.0f;

            // The maximum shininess
            float maxS = 5000;

            return (float)(minS*Math.Pow(maxS/minS, 1.0 - p));
        }

        private string GetTexture(BaseContainer data, BaseChannel matChannel)
        {
            string resultName = null;
            string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
            if (!string.IsNullOrEmpty(texture))
            {
                texture = Path.GetFileNameWithoutExtension(texture);
                texture += ".jpg";
                resultName = texture;

                if (!_textureFiles.Contains(texture))
                {
                    matChannel.InitTexture(new InitRenderStruct(_doc));
                    using (BaseBitmap bitmap = matChannel.GetBitmap())
                    {
                        if (bitmap != null)
                        {
                            using (BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY))
                            {
                                bool bRet = bitmap.SetColorProfile(ColorProfile.GetDefaultSRGB());
                                compressionContainer.SetFloat(C4dApi.JPGSAVER_QUALITY, 70.0);
                                string textureFileAbs = Path.Combine(_sceneRootDir, texture);
                                bitmap.Save(new Filename(textureFileAbs), C4dApi.FILTER_JPG, compressionContainer,
                                    SAVEBIT.SAVEBIT_0);
                                _textureFiles.Add(texture);
                            }
                        }
                        else
                        {
                            resultName = null;
                        }
                    }
                    matChannel.FreeTexture();
                }
            }
            return resultName;
        }

        private List<SceneNodeContainer> FuseefyOb(BaseObject ob)
        {
            bool isAnimRoot = false;
            
            if (ob == null)
                return null;

            List<SceneNodeContainer> ret = new List<SceneNodeContainer>();
            do
            {

                SceneNodeContainer snc = new SceneNodeContainer();


                snc.Name = ob.GetName();
                float3 rotC4d = (float3) ob.GetRelRot();
                AddComponent(snc, new TransformComponent
                {
                    Translation = (float3) ob.GetRelPos(),
                    Rotation = new float3(-rotC4d.y, -rotC4d.x, -rotC4d.z),
                    Scale = (float3) ob.GetRelScale()
                });

                // See if this is a joint - if so, store it for later reference and also pass the information on 
                // to SaveTracks to make it Slerp the rotations (instead of lerp)
                bool isJoint = _weightManager.CheckOnJoint(ob, snc);
                bool hasQuaternionTag = CheckOnQuaternionTag(ob);
                

                // Search for unpolygonized objects holding the animationtracks
                if (SaveTracks(ob, snc, isJoint || hasQuaternionTag))
                {
                    _animationsPresent = true;
                    isAnimRoot = true;
                }

                
                VisitObject(ob, snc);


                // Hope the hierarchy of polygonized objects is the same as the normal one
                var childList = FuseefyOb(ob.GetDown());
                if (childList != null)
                {

                    if (snc.Children == null)
                    {
                        snc.Children = childList;
                    }
                    else
                    {
                        snc.Children.AddRange(childList);
                    }
                }

                if (isAnimRoot)
                {
                    AnimationComponent ac = new AnimationComponent();
                    ac.AnimationTracks = new List<AnimationTrackContainer>(_tracks);
                    snc.AddComponent(ac);
                    _animationsPresent = false;
                    _tracks.Clear();
                }
                ret.Add(snc);
                ob = ob.GetNext();
            } while (ob != null);
            return ret;
        }

        private bool CheckOnQuaternionTag(BaseObject ob)
        {
            // Iterate over the object's tags
            for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
            {
                // In their sheer wisdom, the elders of the C4D SDK decided to not have a class of its own for the quaternion tag.
                // Neither will the world need a constant declaration for the type...
                int tagType = tag.GetType();
                if (tagType == 100001740)
                {
                    BaseContainer data = tag.GetData();
                    int ii = data.GetInt32(1001);  // QUAT_INTER (from tquaterninon.h)
                    switch (ii)
                    {
                        case 1002: // INTER_SLERP, "Linear"
                            break;
                        case 1003: // INTER_SQUAD, "Spline"
                            break;
                        case 1004: // INTER_LOSCH, "Losch"
                            break;
                    }
                    return true;

                    /*
                    for (int i = 0, id = 0; -1 != (id = data.GetIndexId(i)); i++)
                    {
                        if (data.GetType(id) == C4dApi.DA_LONG)
                        {
                            int ii = data.GetInt32(id);
                            switch (ii)
                            {
                                case 1002: // INTER_SLERP,
                                    break;
                                case 1003: // INTER_SQUAD,
                                    break;
                                case 1004: // INTER_LOSCH,
                                    break;
                            }
                        }
                        if (data.GetType(id) == C4dApi.DA_REAL)
                        {
                            double d = data.GetFloat(id);
                        }
                        else if (data.GetType(id) == C4dApi.DA_VECTOR)
                        {
                            double3 v = data.GetVector(id);
                        }
                    }
                    */
                }
            }
            return false;
        }

        private bool SaveTracks(BaseObject ob, SceneNodeContainer snc, bool slerpRotation)
        {

            var builder = new TrackBuilder();
            builder.LerpType = (slerpRotation) ? LerpType.Slerp : LerpType.Lerp;
            CTrack track = ob.GetFirstCTrack();

            // First occurence of animation tracks?
            if (track == null)
                return false;

            while (track != null)
            {
                DescID testID = track.GetDescriptionID();
                DescLevel lv1 = testID.GetAt(0);
                DescLevel lv2 = testID.GetAt(1);

                CCurve curve = track.GetCurve();
                if (curve != null)
                {
                    int keyCount = curve.GetKeyCount();

                    CKey key = null;
                    BaseTime time;
                    for (int i = 0; i < keyCount; i++)
                    {
                        key = curve.GetKey(i);
                        time = key.GetTime();

                        switch (lv1.id)
                        {
                            case 903: // should be replaced with "ID_BASEOBJECT_REL_POSITION"
                                switch (lv2.id)
                                {
                                    case 1000: builder.AddTranslationValue("x", (float)time.Get(), key.GetValue()); break;
                                    case 1001: builder.AddTranslationValue("y", (float)time.Get(), key.GetValue()); break;
                                    case 1002: builder.AddTranslationValue("z", (float)time.Get(), key.GetValue()); break;
                                }
                                break;

                            case 904: // should be replaced with "ID_BASEOBJECT_REL_ROTATION"
                                switch (lv2.id)
                                {
                                    case 1000: builder.AddRotationValue("x", (float)time.Get(), key.GetValue()); break;
                                    case 1001: builder.AddRotationValue("y", (float)time.Get(), key.GetValue()); break;
                                    case 1002: builder.AddRotationValue("z", (float)time.Get(), key.GetValue()); break;
                                }
                                break;

                            case 905: // should be replaced with "ID_BASEOBJECT_REL_SCALE"
                                switch (lv2.id)
                                {
                                    case 1000: builder.AddScaleValue("x", (float)time.Get(), key.GetValue()); break;
                                    case 1001: builder.AddScaleValue("y", (float)time.Get(), key.GetValue()); break;
                                    case 1002: builder.AddScaleValue("z", (float)time.Get(), key.GetValue()); break;
                                }
                                break;
                        }

                    }
                }
                track = track.GetNext();
            }

            builder.BuildTracks(snc, _tracks);
            
            if (_animationsPresent)
                return false;
            return true;
        }
    }
}
