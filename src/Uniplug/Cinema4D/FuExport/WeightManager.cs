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
        private BaseDocument _doc, _polyDoc;
        private List<WeightObject> _weightObjects;
        private Dictionary<long, SceneNodeContainer> _jointObjects;

        public WeightManager(BaseDocument document, BaseDocument polyDoc)
        {
            _doc = document;
            _polyDoc = polyDoc;
            _weightObjects = new List<WeightObject>();
            _jointObjects = new Dictionary<long, SceneNodeContainer>();
        }

        public void CheckOnJoint(SceneNodeContainer snc, BaseObject bo)
        {
            CAJointObject jo = bo as CAJointObject;
            if (jo != null)
            {
                snc.IsBone = true;
                _jointObjects.Add(jo.RefUID(), snc);
            }
        }

        public void AddWeightData(SceneNodeContainer snc, BaseObject bo, PolygonObject polyOb, CAWeightTag weightTag, IEnumerable<int> range)
        {
            if (weightTag == null)
                return;

            List<JointWeightColumn> weightMap = new List<JointWeightColumn>();
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
            }

            _weightObjects.Add(new WeightObject()
            {
                SceneNodeContainer = snc,
                BaseObject = bo,
                WeightTag = weightTag,
                WeightMap = weightMap
            });

        }

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
                wObject.SceneNodeContainer.Components.Add(wComponent);
            }
        }

        private class WeightObject
        {
            public SceneNodeContainer SceneNodeContainer { get; set; }

            public BaseObject BaseObject { get; set; }

            public CAWeightTag WeightTag { get; set; }

            public List<JointWeightColumn> WeightMap { get; set; } 
        }
    }
}
