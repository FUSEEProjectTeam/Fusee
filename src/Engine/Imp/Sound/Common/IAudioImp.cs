namespace Fusee.Engine.Imp.Sound.Common
{
    /// <summary>
    /// Interface for the Audio implementation. The implementation of this interface is responsible for management of all <see cref="IAudioStreamImp" /> instances.
    /// This is used as a container for all the <see cref="IAudioStreamImp" /> and handles Sound functions globally.
    /// </summary>
    public interface IAudioImp
    {

        /// <summary>
        /// Implementation Tasks: Opens the device. All <see cref="IAudioStreamImp" /> derivate instances have to be wiped and the globalvolume of a Listener(if 3D Sound is intended) can be set to maximum.
        /// </summary>
        void OpenDevice();

        /// <summary>
        /// Implementation Tasks: Closes the device. All instances of <see cref="IAudioStreamImp" /> derivate have to be disposed in order to free up memory.
        /// </summary>
        void CloseDevice();

        /// <summary>
        /// Implementation Tasks: Loads the specified file from an external source (examples: hard drive, network, etc) into memory for usage inside of the application.
        /// </summary>
        /// <param name="fileName">Name of the file that should include the file type ending and a absolute or relative path, e.g. C:\sound.ogg .</param>
        /// <param name="streaming">if set to <c>true</c> [streaming].</param>
        /// <returns>A derivate of <see cref="IAudioStreamImp" /> should be returned upon succesful filepath resolution.</returns>
        IAudioStreamImp LoadFile(string fileName, bool streaming);

        /// <summary>
        /// Implementation Tasks: Stops all <see cref="IAudioStreamImp" /> derivates playbacks that this instance is responsible for.
        /// </summary>
        void Stop();

        /// <summary>
        /// Implemenation Tasks: Sets the global volume of the application. In 3D Sound usually a Listener is used to accomplish this task. 
        /// In 2D Sound a simple iteration through all <see cref="IAudioStreamImp" /> derivates that this instance is responsible for is sufficient.
        /// </summary>
        /// <param name="val">The val.</param>
        void SetVolume(float val);

        /// <summary>
        /// Implementation Tasks: Gets the global volume of the application. In 3D Sound a Listener is usually used to accomplish this task.
        /// In 2D Sound an internal master volume property can be implemented that handles all <see cref="IAudioStreamImp" /> derivates volumes that this instance is responsible for.
        /// </summary>
        /// <returns>A number in float precision.</returns>
        float GetVolume();

        /// <summary>
        /// Implemenation Tasks: Sets the panning for all <see cref="IAudioStreamImp" /> derivates that this instance is responsible for.
        /// </summary>
        /// <param name="val">The panning value.</param>
        void SetPanning(float val);
    }
}