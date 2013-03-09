using System.Diagnostics;
using Fusee.Engine;

namespace Examples.SoundTest
{
    class Tests
    {
        private IAudioStream _audio;

        public Tests(IAudioStream audio)
        {
            _audio = audio;
        }

        public bool Test1(int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #1: lower general volume from 100 to 0");
            
            Audio.Instance.SetVolume(100 - (state * 10));       // bug??: check volume 0 

            Debug.WriteLine("---- volume set to: " + Audio.Instance.GetVolume());

            _audio.Play();

            return (state == 10);
        }

        public bool Test2(int state)
        {
            if (state == 0)
            {
                Audio.Instance.SetVolume(100);
                _audio.Volume = 100;                // bug 1: change to relative volume

                _audio.Loop = true;
                _audio.Play();

                Debug.WriteLine("Test #2: looping for 5 seconds");
            }

            if (state == 5)
            {
                _audio.Loop = false;

                return true;
            }

            return false;
        }
    }
}
