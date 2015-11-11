using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{

    public class WeightManager
    {
        private BaseDocument _doc;
        private List<WeightObject> _weightObjects;
        private Dictionary<long, SceneNodeContainer> _jointObjects;

        public WeightManager(BaseDocument document)
        {
            _doc = document;
            _weightObjects = new List<WeightObject>();
            _jointObjects = new Dictionary<long, SceneNodeContainer>();
        }

        public bool CheckOnJoint(BaseObject bo, SceneNodeContainer snc)
        {
            CAJointObject jo = bo as CAJointObject;
            if (jo != null)
            {
                snc.AddComponent(new BoneComponent());
                _jointObjects.Add(jo.RefUID(), snc);
                return true;
            }
            return false;
        }

        public static IEnumerable<int> GetPolyPointIndices(CPolygon polygon, PolygonObject polyOb)
        {
            yield return polygon.a;
            yield return polygon.b;
            yield return polygon.c;
            if (!IsTri(polygon, polyOb))
                yield return polygon.d;
        }

        public static bool IsTri(CPolygon polygon, PolygonObject polyOb)
        {
            return polyOb.GetPointAt(polygon.c) == polyOb.GetPointAt(polygon.d);
        }

        public void AddWeightData(SceneNodeContainer snc, PolygonObject polyOb, CAWeightTag weightTag, IEnumerable<int> range)
        {
            if (weightTag == null || polyOb == null)
                return;

            List<VertexWeightList> weightMap = new List<VertexWeightList>();
            List<float4x4> bindingMatrices = new List<float4x4>();
            foreach (int j in range)
            {
                using (CPolygon poly = polyOb.GetPolygonAt(j))
                {
                    foreach (int iVert in GetPolyPointIndices(poly, polyOb))
                    {
                        var vertexWeights = new List<VertexWeight>();

                        for (int iJoint = 0; iJoint < weightTag.GetJointCount(); iJoint++)
                        {
                            double weight = weightTag.GetWeight(iJoint, iVert);

                            // Leave out zero weights. This will save space for the sparse weight table.
                            if (Math.Abs(weight) > double.Epsilon)
                            {
                                vertexWeights.Add(new VertexWeight {JointIndex = iJoint, Weight = (float) weight});
                            }
                        }

                        vertexWeights.Sort((vw1, vw2) => ((int)(vw1.Weight - vw2.Weight)));
                        weightMap.Add(new VertexWeightList { VertexWeights = vertexWeights });
                    }
                }
            }
            for (int iJoint = 0; iJoint < weightTag.GetJointCount(); iJoint++)
            {
                // Add Binding Matrix
                JointRestState jointRestState = weightTag.GetJointRestState(iJoint);
                float4x4 mat = (float4x4) (jointRestState.m_oMi * weightTag.GetGeomMg());
                bindingMatrices.Add(mat);
            }

            _weightObjects.Add(new WeightObject()
            {
                SceneNodeContainer = snc,
                WeightTag = weightTag,
                WeightMap = weightMap,
                BindingMatrices = bindingMatrices
            });     
        }


        /*  Old version, stores columns of the weight table
        public void AddWeightData(SceneNodeContainer snc, PolygonObject polyOb, CAWeightTag weightTag, IEnumerable<int> range)
        {
            if (weightTag == null || polyOb == null)
                return;

            List<JointWeightColumn> weightMap = new List<JointWeightColumn>();
            List<float4x4> bindingMatrices = new List<float4x4>();
            for (int i = 0; i < weightTag.GetJointCount(); i++)
            {
                JointWeightColumn jointWeightWrapper = new JointWeightColumn()
                {
                    JointWeights = new List<double>()
                };

                foreach (int j in range)
                {
                    using (CPolygon poly = polyOb.GetPolygonAt(j))
                    {
                        jointWeightWrapper.JointWeights.Add(weightTag.GetWeight(i, poly.a));
                        jointWeightWrapper.JointWeights.Add(weightTag.GetWeight(i, poly.b));
                        jointWeightWrapper.JointWeights.Add(weightTag.GetWeight(i, poly.c));

                        double3 c = polyOb.GetPointAt(poly.c);
                        double3 d = polyOb.GetPointAt(poly.d);

                        if (c != d)
                        {
                            // The Polyogon is not a triangle, but a quad. Add the second triangle.
                            jointWeightWrapper.JointWeights.Add(weightTag.GetWeight(i, poly.d));
                        }
                    }
                }
                weightMap.Add(jointWeightWrapper);

                // Add Binding Matrix
                JointRestState jointRestState = weightTag.GetJointRestState(i);
                float4x4 mat = (float4x4)(jointRestState.m_oMi * weightTag.GetGeomMg());
                bindingMatrices.Add(mat);
            }

            _weightObjects.Add(new WeightObject()
            {
                SceneNodeContainer = snc,
                WeightTag = weightTag,
                WeightMap = weightMap,
                BindingMatrices = bindingMatrices
            });
        }
         * */


        public void CreateWeightMap()
        {
            foreach (WeightObject wObject in _weightObjects)
            {
                WeightComponent wComponent = new WeightComponent();
                wComponent.Name = wObject.WeightTag.GetName();
                wComponent.Joints = new List<SceneNodeContainer>();
                for (int i = 0; i < wObject.WeightTag.GetJointCount(); i++)
                {
                    CAJointObject joint = wObject.WeightTag.GetJoint(i, _doc) as CAJointObject;
                    SceneNodeContainer jointSnc;
                    if (_jointObjects.TryGetValue(joint.RefUID(), out jointSnc))
                    {
                        wComponent.Joints.Add(jointSnc);
                    }                   
                }
                wComponent.WeightMap = wObject.WeightMap;
                wComponent.BindingMatrices = wObject.BindingMatrices;
                int inxMesh = wObject.SceneNodeContainer.GetIndexOf<MeshComponent>();
                if (inxMesh == -1)
                    inxMesh = 0;
                wObject.SceneNodeContainer.Components.Insert(inxMesh, wComponent);
            }
        }

        private class WeightObject
        {
            public SceneNodeContainer SceneNodeContainer { get; set; }

            public CAWeightTag WeightTag { get; set; }

            public List<VertexWeightList> WeightMap { get; set; }

            public List<float4x4> BindingMatrices { get; set; } 
        }
    }
}
