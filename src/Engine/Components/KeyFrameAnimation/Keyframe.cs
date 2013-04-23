namespace Fusee.KeyFrameAnimation
{
    public class Keyframe
    {
        private float _value;
        private float _time;

        public Keyframe()
        {
            _value = 0;
            _time = 0;
        }

        public Keyframe(float time)
        {
            _value = 0;
            _time = time;
        }

        public Keyframe(float time ,float value)
        {
            _value = value;
            _time = time;
        }

        public float Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public float Value
        {
            get { return _value ; }
            set { _value = value; }
        }
    }
}