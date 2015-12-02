using System.Diagnostics;
using Fusee.Engine;

namespace Examples.SoundTest
{
    internal class Tests
    {
        public bool Test1(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #1: play, pause, stop and global stop");

            if (state == 2)
            {
                Debug.WriteLine("----> play for five seconds");
                audio.Play();
            }

            if (state == 7)
            {
                Debug.WriteLine("----> pause music for two seconds");
                audio.Pause();
            }

            if (state == 9)
            {
                Debug.WriteLine("----> play music for five seconds");
                audio.Play();
            }

            if (state == 14)
            {
                Debug.WriteLine("----> stop music for two seconds");
                audio.Stop();
            }

            if (state == 16)
            {
                Debug.WriteLine("----> play music for five seconds");
                audio.Play();
            }

            if (state == 21)
            {
                Debug.WriteLine("----> global stop for two seconds");
                Audio.Instance.Stop();
            }

            if (state == 23)
            {
                Debug.WriteLine("----> play music for five seconds");
                audio.Play();
            }

            if (state != 28)
                return false;

            audio.Stop();
            return true;
        }

        public bool Test2(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #2: lower global volume from 100 to 0");

            if (state >= 2)
            {
                Audio.Instance.SetVolume(100 - ((state - 2)*20));
                Debug.WriteLine("----> global volume set to: " + Audio.Instance.GetVolume());

                audio.Play();
            }

            return state == 7;
        }

        public bool Test3(IAudioStream audio, int state)
        {
            if (state == 0)
            {
                Audio.Instance.SetVolume(100);
                audio.Volume = 0;

                Debug.WriteLine("Test #3: raise individual volume from 0 to 100");
            }

            if (state >= 2)
            {
                audio.Volume = ((state - 2)*20);
                Debug.WriteLine("----> individual volume set to: " + audio.Volume);

                audio.Play();
            }

            return (state == 7);
        }

        public bool Test4(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #4: looping for 2 seconds with loop as attribute");

            if (state == 2)
            {
                audio.Loop = true;
                audio.Play();
            }

            if (state != 4)
                return false;

            audio.Loop = false;
            return true;
        }

        public bool Test5(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #5: looping for 2 seconds with loop as parameter");

            if (state == 2)
                audio.Play(true);

            if (state != 4)
                return false;

            audio.Loop = false;
            return true;
        }

        public bool Test6(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #6: global panning from left to right");

            if (state == 2)
                audio.Play();

            if (state >= 2 && state < 19)
            {
                Audio.Instance.SetPanning(-100 + ((state - 2)*12.5f));
                Debug.WriteLine("----> global panning set to: " + audio.Panning);
            }

            if (state != 19)
                return false;

            audio.Stop();
            Audio.Instance.SetPanning(0);

            return true;
        }

        public bool Test7(IAudioStream audio, int state)
        {
            if (state == 0)
                Debug.WriteLine("Test #7: individual panning from right to left");

            if (state == 2)
                audio.Play();

            if (state >= 2 && state < 19)
            {
                audio.Panning = 100 - ((state - 2)*12.5f);
                Debug.WriteLine("----> individual panning set to: " + audio.Panning);
            }

            if (state != 19)
                return false;

            audio.Stop();
            return true;
        }
    }
}