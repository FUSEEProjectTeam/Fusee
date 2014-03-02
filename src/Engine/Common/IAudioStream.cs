namespace Fusee.Engine
{
    /// <summary>
    ///     Loaded files with
    ///     <see>
    ///         <cref>Audio</cref>
    ///     </see>
    ///     class can be controlled via this <see cref="IAudioStream" /> interface.
    /// </summary>
    public interface IAudioStream
    {
        /// <summary>
        ///     Gets or sets the volume of this <see cref="IAudioStream" /> (0 - 100).
        /// </summary>
        /// <value>
        ///     The volume of this <see cref="IAudioStream" /> (0 - 100).
        /// </value>
        float Volume { set; get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IAudioStream" /> shall be looped.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IAudioStream" /> shall be looped; otherwise, <c>false</c>.
        /// </value>
        bool Loop { set; get; }

        /// <summary>
        ///     Gets or sets the panning of this <see cref="IAudioStream" /> (-100 to +100).
        /// </summary>
        /// <value>
        ///     The panning of this <see cref="IAudioStream" /> (-100 to +100).
        /// </value>
        float Panning { set; get; }

        /// <summary>
        ///     Plays this <see cref="IAudioStream" />.
        /// </summary>
        void Play();

        /// <summary>
        ///     Plays this <see cref="IAudioStream" />.
        /// </summary>
        /// <param name="loop"><c>true</c> if this <see cref="IAudioStream" /> shall be looped; otherwise, <c>false</c>.</param>
        void Play(bool loop);

        /// <summary>
        ///     Pauses this <see cref="IAudioStream" />.
        /// </summary>
        void Pause();

        /// <summary>
        ///     Stops this <see cref="IAudioStream" />.
        /// </summary>
        void Stop();
    }
}