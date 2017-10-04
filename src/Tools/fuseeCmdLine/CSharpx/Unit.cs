//Use project level define(s) when referencing with Paket.
//#define CSX_UNIT_INTERNAL // Uncomment this to set visibility to internal.

using System;

namespace CSharpx
{
#if !CSX_UNIT_INTERNAL
    public
#endif
    struct Unit : IEquatable<Unit>
    {
        private static readonly Unit @default = new Unit();

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public static Unit Default { get { return @default; } }
    }
}
