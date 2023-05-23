using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fusee.PointCloud.Common
{
    /// Axis Specification for right handed Z-up coordinate system
    /// *L*eft  - negative X axis
    /// *R*ight - positive X axis
    /// *F*ront - negative Y axis
    /// *B*ack  - positive Y axis 
    /// *D*own  - negative Z axis
    /// *U*p    - positive Z axis
    ///
    /// LRMsk -- 0: left,   1: right - X axis
    /// FBMsk -- 0: front,  1: back  - Y axis
    /// DUMsk -- 0: down,   1: up    - Z axis
    [Flags]
    public enum OctantOrientation : byte
    {
        LeftFrontDown = 0b000,
        RightFrontDown = 0b001,

        LeftBackDown = 0b010,
        RightBackDown = 0b011,

        LeftFrontUp = 0b100,
        RightFrontUp = 0b101,

        LeftBackUp = 0b110,
        RightBackUp = 0b111,



        LeftRightMask = 0b001,
        FrontBackMask = 0b010,
        DownUpMask = 0b100,
        LeftRightFrontBackDownUpMask = DownUpMask | FrontBackMask | LeftRightMask,
    }


    [Flags]
    enum OctantHelper : long
    {
        LevelBits = (long)(0b1111111L << 57),
        OctOrientBits = (long)(~(long)LevelBits),
    }

    //  Current maximum Octree level is 19 
    //  The entire ID denoting the Octant's zero-based level [0 (top)...18 (floor)] and its "path" is stored in a 64 bit word.
    //  The octant's depth is encoded in the most significant first seven bits as a signed 7-bit-integer. Negative
    //  depths denote invalid IDs.
    //  Each level's octant orientation is encoded in three bits (least significant bit: *L*eft(0) / *R*ight (1), 
    //  medium significant bit: *F*ront (0) / *B*ack (1), most significant bit: *D*own (0) / *U*p (1))
    //  Groups of three consecutive bits start at bit 0 (LSB) reaching up to (including) bit 56. Altogether this
    //  allows to encode the octant orientation for 19 levels (= 57 bits / 3).
    // 
    //  |   LevelBits:  7-bit signed int     |    Octant Orientation Bits. Three consecutive bits for each level     |
    //  |                                    |    level 0    |    level 1   |   ...   |   level 17   |   level 18    |
    //  |  SGN  MS                       LS  |  DU | FB | LR | DU | FB | LR |   ...   | DU | FB | LR | DU | FB | LR  |
    //  |  63 | 62 | 61 | 60 | 59 | 58 | 57  |  56 | 55 | 54 | 53 | 52 | 51 |   ...   | 05 | 04 | 03 | 02 | 01 | 00  |
    //  |  MSB                                                                                                  LSB  |
    //  


    public struct OctantId : IEnumerable<(int, OctantOrientation)>, IEquatable<OctantId>
    {
        private long _id = -1;

        public bool Valid => _id >= 0;

        public OctantId(params OctantOrientation[] ooList)
        {
            _id = 0;
            int level = ooList.Length;
            int i = 0;
            for (; ; )
            {
                OctantOrientation oo = ooList[i];
                if ((oo & (~OctantOrientation.LeftRightFrontBackDownUpMask)) != 0)
                    throw new ArgumentException("Invalid octant orientation at level {i}, must be a combination of OctOr flags, i. e. in the range of [0..7]");

                _id |= (long)oo;
                i++;
                if (i >= level)
                    break;
                _id <<= 3;
            }
            _id <<= (19 - level) * 3;
            Level = level;
        }

        public OctantId(long id)
        {
            // Warning: No check performed
            _id = id;
        }

        public OctantId(string potreeName) : this(PotreeNameToOctantOrientations(potreeName)) { }

        public static OctantOrientation PotreeNameToOctantId(char potreeLevelName) => potreeLevelName switch
        {
            '0' => OctantOrientation.LeftFrontDown,
            '1' => OctantOrientation.LeftFrontUp,
            '2' => OctantOrientation.LeftBackDown,
            '3' => OctantOrientation.LeftBackUp,
            '4' => OctantOrientation.RightFrontDown,
            '5' => OctantOrientation.RightFrontUp,
            '6' => OctantOrientation.RightBackDown,
            '7' => OctantOrientation.RightBackUp,
            'r' => OctantOrientation.LeftFrontDown,
            _ => throw new ArgumentException(nameof(potreeLevelName))
        };

        public static OctantOrientation[] PotreeNameToOctantOrientations(string potreeName)
        {
            OctantOrientation[] octOr = new OctantOrientation[potreeName.Length];

            for (int i = 0; i < potreeName.Length; i++)
            {
                octOr[i] = PotreeNameToOctantId(potreeName[i]);
            }

            return octOr;
        }

        private static char OctantOrientationToPotreeName(OctantOrientation octOr) => octOr switch
        {
            OctantOrientation.LeftFrontDown => '0',
            OctantOrientation.LeftFrontUp => '1',
            OctantOrientation.LeftBackDown => '2',
            OctantOrientation.LeftBackUp => '3',
            OctantOrientation.RightFrontDown => '4',
            OctantOrientation.RightFrontUp => '5',
            OctantOrientation.RightBackDown => '6',
            OctantOrientation.RightBackUp => '7',
            _ => throw new ArgumentException(nameof(octOr))
        };

        public static string OctantIdToPotreeName(OctantId octId)
        {
            var levels = octId.Level;

            if (levels < 1)
                return "";
            else if (levels == 1)
                return "r";
            else
            {
                StringBuilder sb = new();
                sb.Append('r');

                for (int i = 1; i < octId.Level; i++)
                {
                    sb.Append(OctantOrientationToPotreeName(octId[i]));
                }
                return sb.ToString();
            }
        }

        public static implicit operator long(OctantId oid) => oid._id;

        /// Zero-based level of the octant ()
        public int Level
        {
            get => (int)(_id >> 57);
            set
            {
                if (!(0 <= value && value <= 18))
                    throw new ArgumentException("Maximum allowed octant level range is [0..18]");

                long orients = _id & ((long)OctantHelper.OctOrientBits);
                long lvlOn57 = ((long)value) << 57;
                _id = (lvlOn57 | orients);
            }
        }

        public OctantOrientation this[int level]
        {
            get
            {
                if (!(0 <= level && level <= Level))
                    throw new ArgumentException($"Current octant ID's maximum level is {Level}");
                return ((OctantOrientation)(_id >> ((18 - level) * 3))) & OctantOrientation.LeftRightFrontBackDownUpMask;
            }
            // Implicitly re-sets the level if exceeded
            set
            {
                if (!(0 <= level && level <= 18))
                    throw new ArgumentException("Maximum allowed octant level range is [0..18]");

                if ((value & (~OctantOrientation.LeftRightFrontBackDownUpMask)) != 0)
                    throw new ArgumentException("Invalid octant orientation, must be a combination of OctOr flags, i. e. in the range of [0..7]");

                if (level > Level)
                    Level = level;

                long octOrMsk = ((long)OctantOrientation.LeftRightFrontBackDownUpMask) << ((18 - level) * 3);

                _id = (((long)value) << ((18 - level) * 3)) | (_id & ~octOrMsk);
            }
        }

        public IEnumerator<(int, OctantOrientation)> GetEnumerator()
        {
            int level = Level;
            int nShift = 18 * 3;
            for (int i = 0; i < level; i++)
            {
                yield return (i, ((OctantOrientation)(_id >> nShift)) & OctantOrientation.LeftRightFrontBackDownUpMask);
                nShift -= 3;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static bool IsLeft(OctantOrientation oo) => (oo & OctantOrientation.LeftRightMask) == 0;
        public static bool IsRight(OctantOrientation oo) => (oo & OctantOrientation.LeftRightMask) != 0;

        public static bool IsFront(OctantOrientation oo) => (oo & OctantOrientation.FrontBackMask) == 0;
        public static bool IsBack(OctantOrientation oo) => (oo & OctantOrientation.FrontBackMask) != 0;

        public static bool IsDown(OctantOrientation oo) => (oo & OctantOrientation.DownUpMask) == 0;
        public static bool IsUp(OctantOrientation oo) => (oo & OctantOrientation.DownUpMask) != 0;

        /// <inheritdoc/>
        public override string ToString()
        {
            return Convert.ToString(this, 2);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals((OctantId)obj);
        }

        /// <inheritdoc/>
        public bool Equals(OctantId other)
        {
            return _id == other._id;
        }
    }
}