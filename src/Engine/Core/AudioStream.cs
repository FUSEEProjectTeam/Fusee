using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    class AudioStream
    {
        /// <summary>
        /// For internal purposes. Do not use in application code.
        /// </summary>
        public IAudioStreamImp _asImp;

        /// <summary>
        ///     Gets or sets the volume of this <see cref="IAudioStreamImp" /> (0 - 100).
        /// </summary>
        /// <value>
        ///     The volume of this <see cref="IAudioStreamImp" /> (0 - 100).
        /// </value>
        public float Volume { set { _asImp.Volume = value; } get { return _asImp.Volume; } }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IAudioStreamImp" /> shall be looped.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IAudioStreamImp" /> shall be looped; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { set { _asImp.Loop = value; } get { return _asImp.Loop; } }

        /// <summary>
        ///     Gets or sets the panning of this <see cref="IAudioStreamImp" /> (-100 to +100).
        /// </summary>
        /// <value>
        ///     The panning of this <see cref="IAudioStreamImp" /> (-100 to +100).
        /// </value>
        public float Panning { set { _asImp.Panning = value; } get { return _asImp.Panning; } }

        /// <summary>
        ///     Plays this <see cref="IAudioStreamImp" />.
        /// </summary>
        public void Play() { _asImp.Play();}

        /// <summary>
        ///     Plays this <see cref="IAudioStreamImp" />.
        /// </summary>
        /// <param name="loop"><c>true</c> if this <see cref="IAudioStreamImp" /> shall be looped; otherwise, <c>false</c>.</param>
        void Play(bool loop) { _asImp.Play(loop); }

        /// <summary>
        ///     Pauses this <see cref="IAudioStreamImp" />.
        /// </summary>
        public void Pause() { _asImp.Pause(); }

        /// <summary>
        ///     Stops this <see cref="IAudioStreamImp" />.
        /// </summary>
        public void Stop() { _asImp.Stop(); }
    }
}
