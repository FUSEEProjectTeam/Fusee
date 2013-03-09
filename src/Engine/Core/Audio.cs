// Fusee uses the OpenAL library. See http://www.openal.org for details.
// OpenAL is used under the terms of the LGPL, version x.

namespace Fusee.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public class Audio
    {
        private static Audio _instance;

        private IAudioImp _audioImp;

        internal IAudioImp AudioImp
        {
            set
            {
                _audioImp = value;
                _audioImp.OpenDevice();
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        internal void CloseDevice()
        {
            _audioImp.CloseDevice();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="streaming"></param>
        /// <returns></returns>
        public IAudioStream LoadFile(string fileName, bool streaming = false)
        {
            return _audioImp.LoadFile(fileName, streaming);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            _audioImp.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            _audioImp.Pause();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _audioImp.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Play(IAudioStream stream)
        {
            _audioImp.Play(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Pause(IAudioStream stream)
        {
            _audioImp.Pause(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Stop(IAudioStream stream)
        {
            _audioImp.Stop(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetVolume(float val)
        {
            _audioImp.SetVolume(val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetVolume()
        {
            return _audioImp.GetVolume();
        }

        /// <summary>
        /// Provides the Instance of the Audio Class.
        /// </summary>
        public static Audio Instance
        {
            get { return _instance ?? (_instance = new Audio()); }
        }
    }
}