using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    class TrackBuilder
    {

        public Dictionary<float, float3> TranslationKeys { get; set; }

        public Dictionary<float, float3> RotationKeys { get; set; }

        public Dictionary<float, float3> ScaleKeys { get; set; }

        private List<AnimationKeyContainerBase> BuildTranslationKeys()
        {
            List<AnimationKeyContainerBase> ret = new List<AnimationKeyContainerBase>();

            foreach (var pair in TranslationKeys)
            {
                ret.Add(new AnimationKeyContainerFloat3()
                {
                    Time = pair.Key,
                    Value = pair.Value
                });
            }

            return ret;
        }

        private List<AnimationKeyContainerBase> BuildRotationKeys()
        {
            List<AnimationKeyContainerBase> ret = new List<AnimationKeyContainerBase>();

            foreach (var pair in RotationKeys)
            {
                ret.Add(new AnimationKeyContainerFloat3()
                {
                    Time = pair.Key,
                    Value = pair.Value
                });
            }

            return ret;
        }

        private List<AnimationKeyContainerBase> BuildScaleKeys()
        {
            List<AnimationKeyContainerBase> ret = new List<AnimationKeyContainerBase>();

            foreach (var pair in ScaleKeys)
            {
                ret.Add(new AnimationKeyContainerFloat3()
                {
                    Time = pair.Key,
                    Value = pair.Value
                });
            }

            return ret;
        }

        public void AddTranslationValue(string id, float time, double value)
        {
            if (TranslationKeys == null)
                TranslationKeys = new Dictionary<float, float3>();

            float3 tmp = new float3();
            if (TranslationKeys.TryGetValue(time, out tmp))
            {
                if (id == "x")
                    tmp.x = (float) value;
                if (id == "y")
                    tmp.y = (float) value;
                if (id == "z")
                    tmp.z = (float) value;

                TranslationKeys[time] = tmp;
            }

            else
            {
                if (id == "x")
                    tmp = new float3((float)value, 0, 0);
                if (id == "y")
                    tmp = new float3(0, (float) value, 0);
                if (id == "z")
                    tmp = new float3(0, 0, (float) value);

                TranslationKeys.Add(time, tmp);

            }


        }

        public void AddRotationValue(string id, float time, double value)
        {
            if (RotationKeys == null)
                RotationKeys = new Dictionary<float, float3>();

            float3 tmp = new float3();
            if (RotationKeys.TryGetValue(time, out tmp))
            {
                if (id == "x")
                    tmp.x = (float)value;
                if (id == "y")
                    tmp.y = (float)value;
                if (id == "z")
                    tmp.z = (float)value;

                RotationKeys[time] = tmp;
            }

            else
            {
                if (id == "x")
                    tmp = new float3((float)value, 0, 0);
                if (id == "y")
                    tmp = new float3(0, (float)value, 0);
                if (id == "z")
                    tmp = new float3(0, 0, (float)value);

                RotationKeys.Add(time, tmp);

            }

        }

        public void AddScaleValue(string id, float time, double value)
        {
            if (ScaleKeys == null)
                ScaleKeys = new Dictionary<float, float3>();

            float3 tmp = new float3();
            if (ScaleKeys.TryGetValue(time, out tmp))
            {
                if (id == "x")
                    tmp.x = (float)value;
                if (id == "y")
                    tmp.y = (float)value;
                if (id == "z")
                    tmp.z = (float)value;

                ScaleKeys[time] = tmp;
            }

            else
            {
                if (id == "x")
                    tmp = new float3((float)value, 0, 0);
                if (id == "y")
                    tmp = new float3(0, (float)value, 0);
                if (id == "z")
                    tmp = new float3(0, 0, (float)value);

                ScaleKeys.Add(time, tmp);
            }

        }

        public void BuildTracks(SceneObjectContainer soc, List<AnimationTrackContainer> list)
        {
            if (TranslationKeys != null) 
                list.Add(new AnimationTrackContainer()
                {
                    SceneObject = soc,
                    Property = "transform.GlobalPosition",
                    KeyType = typeof(float3),
                    KeyFrames = BuildTranslationKeys()

                });

            if(RotationKeys != null)
                list.Add(new AnimationTrackContainer()
                {
                    SceneObject = soc,
                    Property = "Transform.Rotation",
                    KeyType = typeof(float3),
                    KeyFrames = BuildRotationKeys()

                });

            if(ScaleKeys != null)
                list.Add(new AnimationTrackContainer()
                {
                    SceneObject = soc,
                    Property = "Transform.Scale",
                    KeyType = typeof(float3),
                    KeyFrames = BuildScaleKeys()

                });
        }



    }
}
