using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Common
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
        public static Suid GenerateSuid()
        {
            //increment in a thread-safe way... idCounter is static -> the lockObject must be static.
            lock (LockObject)
            {
                _idCounter++;
            }
            return new Suid(_idCounter);
        }

        /// <summary>
        /// An Empty Suid. Can be used to compare whether a Suid is Empty / default.
        /// </summary>
        public static readonly Suid Empty = new Suid(0);

        /// <summary>
        /// Checks if two suids are equal
        /// </summary>      
        public override bool Equals(object obj) => obj is Suid s && s._id == _id;

        /// <summary>
        /// Gets hash code        
        /// </summary>     
        public override int GetHashCode() => _id.GetHashCode();

        /// <summary>
        /// Checks if two suids are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>      
        public static bool operator ==(Suid left, Suid right) => left.Equals(right);

        /// <summary>
        /// Checks if two suids are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>       
        public static bool operator !=(Suid left, Suid right) => !(left == right);
    }
}