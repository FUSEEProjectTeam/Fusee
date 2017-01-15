using System.Collections.Generic;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace FuExport
{
    class TrackBuilder
    {

        public Dictionary<float, float3> TranslationKeys { get; set; }

        public Dictionary<float, float3> RotationKeys { get; set; }

        public Dictionary<float, float3> ScaleKeys { get; set; }

        public LerpType LerpType { get; set; }


        private List<AnimationKeyContainerBase> BuildFloat3Keys(Dictionary<float, float3> dictionary)
        {
            List<AnimationKeyContainerBase> ret = new List<AnimationKeyContainerBase>();

            foreach (var pair in dictionary)
            {
                ret.Add(new AnimationKeyContainerFloat3()
                {
                    Time = pair.Key,
                    Value = pair.Value
                });
            }

            return ret;
        }

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
                    tmp.y = -(float)value;
                if (id == "y")
                    tmp.x = -(float)value;
                if (id == "z")
                    tmp.z = -(float)value;

                RotationKeys[time] = tmp;
            }

            else
            {
                if (id == "x")
                    tmp = new float3(0, -(float)value, 0);
                if (id == "y")
                    tmp = new float3(-(float)value, 0, 0);
                if (id == "z")
                    tmp = new float3( 0, 0, -(float)value);

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

        public void BuildTracks(SceneNodeContainer snc, List<AnimationTrackContainer> list)
        {
            TransformComponent transform = snc.GetTransform();
            
            if (transform == null)
                return;

            if (TranslationKeys != null) 
                list.Add(new AnimationTrackContainer()
                {
                    SceneComponent = transform,
                    Property = "Translation",
                    KeyType = KeyType.Float3,
                    KeyFrames = BuildFloat3Keys(TranslationKeys)
                });

            if(RotationKeys != null)
                list.Add(new AnimationTrackContainer()
                {
                    SceneComponent = transform,
                    Property = "Rotation",
                    KeyType = KeyType.Float3,
                    LerpType = this.LerpType,
                    KeyFrames = BuildFloat3Keys(RotationKeys)
                });

            if(ScaleKeys != null)
                list.Add(new AnimationTrackContainer()
                {
                    SceneComponent = transform,
                    Property = "Scale",
                    KeyType = KeyType.Float3,
                    KeyFrames = BuildFloat3Keys(ScaleKeys)
                });
        }



    }
}
