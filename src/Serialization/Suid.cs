namespace Fusee.Serialization
{
    /// <summary>
    /// Session unique Id.
    /// </summary>
    public struct Suid
    {
        private static readonly object LockObject = new object();
        private static ulong _idCounter = 0;
        private readonly ulong _id;
        private Suid(ulong id)
        {
            _id = id;
        }

        /// <summary>
        /// Generate a new session unique Id.
        /// </summary>
        /// <returns></returns>
        public static Suid GenerateSuid()
        {
            //increment threadsafe... idCounter is static -> the lockObject must be static.
            lock (LockObject)
            {
                _idCounter = _idCounter + 1;
            }
            return new Suid(_idCounter);
        }

        /// <summary>
        /// An Empty Suid. Can be used to compare whether a Suid is Empty / default.
        /// </summary>
        public static readonly Suid Empty = new Suid(0);

    }
}