namespace Fusee.Engine.Common
{
    /// <summary>
    ///     Loaded files with
    ///     <see>
    ///         <cref>Audio</cref>
    ///     </see>
    ///     class can be controlled via this <see cref="IAudioStreamImp" /> interface.
    /// </summary>
    public interface IAudioStreamImp
    {
        /// <summary>
        ///     Gets and sets the volume of this <see cref="IAudioStreamImp" /> (0 - 100).
        /// </summary>
        /// <value>
        ///     The volume of this <see cref="IAudioStreamImp" /> (0 - 100).
        /// </value>
        float Volume { set; get; }

        /// <summary>
        ///     Gets and sets a value indicating whether this <see cref="IAudioStreamImp" /> shall be looped.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IAudioStreamImp" /> shall be looped; otherwise, <c>false</c>.
        /// </value>
        bool Loop { set; get; }

        /// <summary>
        ///     Gets and sets the panning of this <see cref="IAudioStreamImp" /> (-100 to +100).
        /// </summary>
        /// <value>
        ///     The panning of this <see cref="IAudioStreamImp" /> (-100 to +100).
        /// </value>
        float Panning { set; get; }

        /// <summary>
        ///     Plays this <see cref="IAudioStreamImp" />.
        /// </summary>
        void Play();

        /// <summary>
        ///     Plays this <see cref="IAudioStreamImp" />.
        /// </summary>
        /// <param name="loop"><c>true</c> if this <see cref="IAudioStreamImp" /> shall be looped; otherwise, <c>false</c>.</param>
        void Play(bool loop);

        /// <summary>
        ///     Pauses this <see cref="IAudioStreamImp" />.
        /// </summary>
        void Pause();

        /// <summary>
        ///     Stops this <see cref="IAudioStreamImp" />.
        /// </summary>
        void Stop();
    }
}