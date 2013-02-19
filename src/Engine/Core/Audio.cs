using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class Audio
    {
        private readonly IAudioImp _audioImp;

        public Audio(IAudioImp audioImp)
        {
            _audioImp = audioImp;
        }

        public void OpenDevice()
        {
            _audioImp.OpenDevice();
        }

        public void LoadFile(string fileName)
        {
            _audioImp.LoadFile(fileName);
        }

        public void Init()
        {
            _audioImp.Init();
        }

        public void Play()
        {
            _audioImp.Play();
        }

        public void Pause()
        {
            _audioImp.Pause();
        }
    }
}
