using System;
using System.Collections.Generic;
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
        private Dictionary<CAJointObject, SceneNodeContainer> _jointObjects;

        public WeightManager(BaseDocument document)
        {
            _doc = document;
            _weightObjects = new List<WeightObject>();
            _jointObjects = new Dictionary<CAJointObject, SceneNodeContainer>();
        }

        public void CheckOnJoint(SceneNodeContainer snc, BaseObject bo)
        {
            if (bo.IsInstanceOf(1019362))
            {
                CAJointObject jo = bo as CAJointObject;
                if(jo != null)
                    _jointObjects.Add(jo, snc);
            }
        }

        public void AddWeightData(SceneNodeContainer snc, BaseObject bo, PolygonObject polyOb, CAWeightTag weightTag, IEnumerable<int> range)
        {
            if (weightTag == null)
                return;

            List<JointWeightWrapper> weightMap = new List<JointWeightWrapper>();
            for (int i = 0; i < weightTag.GetJointCount(); i++)
            {
                JointWeightWrapper jointWeightWrapper = new JointWeightWrapper()
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
                        jointWeightWrapper.JointWeights.Add(weightTag.GetWeight(i, poly.d));
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
                for (int i = 0; i < wObject.WeightTag.GetJointCount(); i++)
                {
                    CAJointObject joint = wObject.WeightTag.GetJoint(i, _doc) as CAJointObject;
                    SceneNodeContainer jointSnc = new SceneNodeContainer();
                    if (_jointObjects.TryGetValue(joint, out jointSnc))
                    {
                        wComponent.Joints.Add(jointSnc);                   
                    }                   
                }
                wComponent.Weights = wObject.WeightMap;
                wObject.SceneNodeContainer.Components.Add(wComponent);
            }
        }

        private class WeightObject
        {
            public SceneNodeContainer SceneNodeContainer { get; set; }

            public BaseObject BaseObject { get; set; }

            public CAWeightTag WeightTag { get; set; }

            public List<JointWeightWrapper> WeightMap { get; set; } 
        }
    }
}
