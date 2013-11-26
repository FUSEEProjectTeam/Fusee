using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Fusee.Math;

namespace Fusee.Engine
{

    /// <summary>
    /// Represents a 32-bit color (4 bytes) in the form of RGBA (in byte order: R, G, B, A).
    /// Seen as a 32 bit unsigned integer, each color is in the form AARRGGBB - so the 
    /// most signifcant byte is Alpha and the least significatn byte is the blue channel
    /// </summary>
    [TypeConverter(typeof (ColorConverter))]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ColorUint : IEquatable<ColorUint>, IFormattable
    {
        public static readonly ColorUint Zero = FromBgra(0);
        public static readonly ColorUint Transparent = FromBgra(0x00FFFFFFU);
        public static readonly ColorUint AntiqueWhite	= FromBgra(0xFFFAEBD7U);
        public static readonly ColorUint Aqua	= FromBgra(0xFF00FFFFU);
        public static readonly ColorUint Aquamarine	= FromBgra(0xFF7FFFD4U);
        public static readonly ColorUint Azure	= FromBgra(0xFFF0FFFFU);
        public static readonly ColorUint Beige	= FromBgra(0xFFF5F5DCU);
        public static readonly ColorUint Bisque	= FromBgra(0xFFFFE4C4U);
        public static readonly ColorUint Black	= FromBgra(0xFF000000U);
        public static readonly ColorUint BlanchedAlmond	= FromBgra(0xFFFFEBCDU);
        public static readonly ColorUint Blue	= FromBgra(0xFF0000FFU);
        public static readonly ColorUint BlueViolet	= FromBgra(0xFF8A2BE2U);
        public static readonly ColorUint Brown	= FromBgra(0xFFA52A2AU);
        public static readonly ColorUint BurlyWood	= FromBgra(0xFFDEB887U);
        public static readonly ColorUint CadetBlue	= FromBgra(0xFF5F9EA0U);
        public static readonly ColorUint Chartreuse	= FromBgra(0xFF7FFF00U);
        public static readonly ColorUint Chocolate	= FromBgra(0xFFD2691EU);
        public static readonly ColorUint Coral	= FromBgra(0xFFFF7F50U);
        public static readonly ColorUint CornflowerBlue	= FromBgra(0xFF6495EDU);
        public static readonly ColorUint Cornsilk	= FromBgra(0xFFFFF8DCU);
        public static readonly ColorUint Crimson	= FromBgra(0xFFDC143CU);
        public static readonly ColorUint Cyan	= FromBgra(0xFF00FFFFU);
        public static readonly ColorUint DarkBlue	= FromBgra(0xFF00008BU);
        public static readonly ColorUint DarkCyan	= FromBgra(0xFF008B8BU);
        public static readonly ColorUint DarkGoldenrod	= FromBgra(0xFFB8860BU);
        public static readonly ColorUint DarkGray	= FromBgra(0xFFA9A9A9U);
        public static readonly ColorUint DarkGreen	= FromBgra(0xFF006400U);
        public static readonly ColorUint DarkKhaki	= FromBgra(0xFFBDB76BU);
        public static readonly ColorUint DarkMagenta	= FromBgra(0xFF8B008BU);
        public static readonly ColorUint DarkOliveGreen	= FromBgra(0xFF556B2FU);
        public static readonly ColorUint DarkOrange	= FromBgra(0xFFFF8C00U);
        public static readonly ColorUint DarkOrchid	= FromBgra(0xFF9932CCU);
        public static readonly ColorUint DarkRed	= FromBgra(0xFF8B0000U);
        public static readonly ColorUint DarkSalmon	= FromBgra(0xFFE9967AU);
        public static readonly ColorUint DarkSeaGreen	= FromBgra(0xFF8FBC8FU);
        public static readonly ColorUint DarkSlateBlue	= FromBgra(0xFF483D8BU);
        public static readonly ColorUint DarkSlateGray	= FromBgra(0xFF2F4F4FU);
        public static readonly ColorUint DarkTurquoise	= FromBgra(0xFF00CED1U);
        public static readonly ColorUint DarkViolet	= FromBgra(0xFF9400D3U);
        public static readonly ColorUint DeepPink	= FromBgra(0xFFFF1493U);
        public static readonly ColorUint DeepSkyBlue	= FromBgra(0xFF00BFFFU);
        public static readonly ColorUint DimGray	= FromBgra(0xFF696969U);
        public static readonly ColorUint DodgerBlue	= FromBgra(0xFF1E90FFU);
        public static readonly ColorUint FireBrick	= FromBgra(0xFFB22222U);
        public static readonly ColorUint FloralWhite	= FromBgra(0xFFFFFAF0U);
        public static readonly ColorUint ForestGreen	= FromBgra(0xFF228B22U);
        public static readonly ColorUint Fuchsia	= FromBgra(0xFFFF00FFU);
        public static readonly ColorUint Gainsboro	= FromBgra(0xFFDCDCDCU);
        public static readonly ColorUint GhostWhite	= FromBgra(0xFFF8F8FFU);
        public static readonly ColorUint Gold	= FromBgra(0xFFFFD700U);
        public static readonly ColorUint Goldenrod	= FromBgra(0xFFDAA520U);
        public static readonly ColorUint Gray	= FromBgra(0xFF808080U);
        public static readonly ColorUint Green	= FromBgra(0xFF008000U);
        public static readonly ColorUint GreenYellow	= FromBgra(0xFFADFF2FU);
        public static readonly ColorUint Honeydew	= FromBgra(0xFFF0FFF0U);
        public static readonly ColorUint HotPink	= FromBgra(0xFFFF69B4U);
        public static readonly ColorUint IndianRed	= FromBgra(0xFFCD5C5CU);
        public static readonly ColorUint Indigo	= FromBgra(0xFF4B0082U);
        public static readonly ColorUint Ivory	= FromBgra(0xFFFFFFF0U);
        public static readonly ColorUint Khaki	= FromBgra(0xFFF0E68CU);
        public static readonly ColorUint Lavender	= FromBgra(0xFFE6E6FAU);
        public static readonly ColorUint LavenderBlush	= FromBgra(0xFFFFF0F5U);
        public static readonly ColorUint LawnGreen	= FromBgra(0xFF7CFC00U);
        public static readonly ColorUint LemonChiffon	= FromBgra(0xFFFFFACDU);
        public static readonly ColorUint LightBlue	= FromBgra(0xFFADD8E6U);
        public static readonly ColorUint LightCoral	= FromBgra(0xFFF08080U);
        public static readonly ColorUint LightCyan	= FromBgra(0xFFE0FFFFU);
        public static readonly ColorUint LightGoldenrodYellow	= FromBgra(0xFFFAFAD2U);
        public static readonly ColorUint LightGreen	= FromBgra(0xFF90EE90U);
        public static readonly ColorUint LightGrey	= FromBgra(0xFFD3D3D3U);
        public static readonly ColorUint LightPink	= FromBgra(0xFFFFB6C1U);
        public static readonly ColorUint LightSalmon	= FromBgra(0xFFFFA07AU);
        public static readonly ColorUint LightSeaGreen	= FromBgra(0xFF20B2AAU);
        public static readonly ColorUint LightSkyBlue	= FromBgra(0xFF87CEFAU);
        public static readonly ColorUint LightSlateGray	= FromBgra(0xFF778899U);
        public static readonly ColorUint LightSteelBlue	= FromBgra(0xFFB0C4DEU);
        public static readonly ColorUint LightYellow	= FromBgra(0xFFFFFFE0U);
        public static readonly ColorUint Lime	= FromBgra(0xFF00FF00U);
        public static readonly ColorUint LimeGreen	= FromBgra(0xFF32CD32U);
        public static readonly ColorUint Linen	= FromBgra(0xFFFAF0E6U);
        public static readonly ColorUint Magenta	= FromBgra(0xFFFF00FFU);
        public static readonly ColorUint Maroon	= FromBgra(0xFF800000U);
        public static readonly ColorUint MediumAquamarine	= FromBgra(0xFF66CDAAU);
        public static readonly ColorUint MediumBlue	= FromBgra(0xFF0000CDU);
        public static readonly ColorUint MediumOrchid	= FromBgra(0xFFBA55D3U);
        public static readonly ColorUint MediumPurple	= FromBgra(0xFF9370DBU);
        public static readonly ColorUint MediumSeaGreen	= FromBgra(0xFF3CB371U);
        public static readonly ColorUint MediumSlateBlue	= FromBgra(0xFF7B68EEU);
        public static readonly ColorUint MediumSpringGreen	= FromBgra(0xFF00FA9AU);
        public static readonly ColorUint MediumTurquoise	= FromBgra(0xFF48D1CCU);
        public static readonly ColorUint MediumVioletRed	= FromBgra(0xFFC71585U);
        public static readonly ColorUint MidnightBlue	= FromBgra(0xFF191970U);
        public static readonly ColorUint MintCream	= FromBgra(0xFFF5FFFAU);
        public static readonly ColorUint MistyRose	= FromBgra(0xFFFFE4E1U);
        public static readonly ColorUint Moccasin	= FromBgra(0xFFFFE4B5U);
        public static readonly ColorUint NavajoWhite	= FromBgra(0xFFFFDEADU);
        public static readonly ColorUint Navy	= FromBgra(0xFF000080U);
        public static readonly ColorUint OldLace	= FromBgra(0xFFFDF5E6U);
        public static readonly ColorUint Olive	= FromBgra(0xFF808000U);
        public static readonly ColorUint OliveDrab	= FromBgra(0xFF6B8E23U);
        public static readonly ColorUint Orange	= FromBgra(0xFFFFA500U);
        public static readonly ColorUint OrangeRed	= FromBgra(0xFFFF4500U);
        public static readonly ColorUint Orchid	= FromBgra(0xFFDA70D6U);
        public static readonly ColorUint PaleGoldenrod	= FromBgra(0xFFEEE8AAU);
        public static readonly ColorUint PaleGreen	= FromBgra(0xFF98FB98U);
        public static readonly ColorUint PaleTurquoise	= FromBgra(0xFFAFEEEEU);
        public static readonly ColorUint PaleVioletRed	= FromBgra(0xFFDB7093U);
        public static readonly ColorUint PapayaWhip	= FromBgra(0xFFFFEFD5U);
        public static readonly ColorUint PeachPuff	= FromBgra(0xFFFFDAB9U);
        public static readonly ColorUint Peru	= FromBgra(0xFFCD853FU);
        public static readonly ColorUint Pink	= FromBgra(0xFFFFC0CBU);
        public static readonly ColorUint Plum	= FromBgra(0xFFDDA0DDU);
        public static readonly ColorUint PowderBlue	= FromBgra(0xFFB0E0E6U);
        public static readonly ColorUint Purple	= FromBgra(0xFF800080U);
        public static readonly ColorUint Red	= FromBgra(0xFFFF0000U);
        public static readonly ColorUint RosyBrown	= FromBgra(0xFFBC8F8FU);
        public static readonly ColorUint RoyalBlue	= FromBgra(0xFF4169E1U);
        public static readonly ColorUint SaddleBrown	= FromBgra(0xFF8B4513U);
        public static readonly ColorUint Salmon	= FromBgra(0xFFFA8072U);
        public static readonly ColorUint SandyBrown	= FromBgra(0xFFF4A460U);
        public static readonly ColorUint SeaGreen	= FromBgra(0xFF2E8B57U);
        public static readonly ColorUint Seashell	= FromBgra(0xFFFFF5EEU);
        public static readonly ColorUint Sienna	= FromBgra(0xFFA0522DU);
        public static readonly ColorUint Silver	= FromBgra(0xFFC0C0C0U);
        public static readonly ColorUint SkyBlue	= FromBgra(0xFF87CEEBU);
        public static readonly ColorUint SlateBlue	= FromBgra(0xFF6A5ACDU);
        public static readonly ColorUint SlateGray	= FromBgra(0xFF708090U);
        public static readonly ColorUint Snow	= FromBgra(0xFFFFFAFAU);
        public static readonly ColorUint SpringGreen	= FromBgra(0xFF00FF7FU);
        public static readonly ColorUint SteelBlue	= FromBgra(0xFF4682B4U);
        public static readonly ColorUint Tan	= FromBgra(0xFFD2B48CU);
        public static readonly ColorUint Teal	= FromBgra(0xFF008080U);
        public static readonly ColorUint Thistle	= FromBgra(0xFFD8BFD8U);
        public static readonly ColorUint Tomato	= FromBgra(0xFFFF6347U);
        public static readonly ColorUint Turquoise	= FromBgra(0xFF40E0D0U);
        public static readonly ColorUint Violet	= FromBgra(0xFFEE82EEU);
        public static readonly ColorUint Wheat	= FromBgra(0xFFF5DEB3U);
        public static readonly ColorUint White	= FromBgra(0xFFFFFFFFU);
        public static readonly ColorUint WhiteSmoke	= FromBgra(0xFFF5F5F5U);
        public static readonly ColorUint Yellow	= FromBgra(0xFFFFFF00U);
        public static readonly ColorUint YellowGreen	= FromBgra(0xFF9ACD32U);

        /// <summary>
        /// The red component of the color.
        /// </summary> 
        public byte R;
        /// <summary>
        /// The green component of the color.
        /// </summary>
        public byte G;
        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public byte B;
        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public byte A;

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>
        /// The value of the alpha, red, green, or blue component, depending on the index.
        /// </value>
        /// <param name="index">The index of the component to access. Use 0 for the alpha component, 1 for the red component, 2 for the green component, and 3 for the blue component.</param>
        /// <returns>
        /// The value of the component at the specified index.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public byte this[int index]
        {
            get
            {
            switch (index)
            {
                case 0:
                return this.R;
                case 1:
                return this.G;
                case 2:
                return this.B;
                case 3:
                return this.A;
                default:
                throw new ArgumentOutOfRangeException("index", "Indices for ColorUint run from 0 to 3, inclusive.");
            }
            }
            set
            {
            switch (index)
            {
                case 0:
                this.R = value;
                break;
                case 1:
                this.G = value;
                break;
                case 2:
                this.B = value;
                break;
                case 3:
                this.A = value;
                break;
                default:
                throw new ArgumentOutOfRangeException("index", "Indices for ColorUint run from 0 to 3, inclusive.");
            }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public ColorUint(byte value)
        {
            this.A = this.R = this.G = this.B = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// 
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public ColorUint(float value)
        {
            this.A = this.R = this.G = this.B = ColorUint.ToByte(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorUint(byte red, byte green, byte blue, byte alpha)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
            this.A = alpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorUint(float red, float green, float blue, float alpha)
        {
            this.R = ColorUint.ToByte(red);
            this.G = ColorUint.ToByte(green);
            this.B = ColorUint.ToByte(blue);
            this.A = ColorUint.ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="value">The red, green, blue, and alpha components of the color.</param>
        public ColorUint(float4 value)
        {
            this.R = ColorUint.ToByte(value.x);
            this.G = ColorUint.ToByte(value.y);
            this.B = ColorUint.ToByte(value.z);
            this.A = ColorUint.ToByte(value.w);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="value">The red, green, and blue compoennts of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorUint(float3 value, float alpha)
        {
            this.R = ColorUint.ToByte(value.x);
            this.G = ColorUint.ToByte(value.y);
            this.B = ColorUint.ToByte(value.z);
            this.A = ColorUint.ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="rgba">A packed integer containing all four color components in RGBA order.</param>
        public ColorUint(uint rgba)
        {
            this.A = (byte) (rgba >> 24 & (uint) byte.MaxValue);
            this.B = (byte) (rgba >> 16 & (uint) byte.MaxValue);
            this.G = (byte) (rgba >> 8 & (uint) byte.MaxValue);
            this.R = (byte) (rgba & (uint) byte.MaxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="rgba">A packed integer containing all four color components in RGBA order.</param>
        public ColorUint(int rgba)
        {
            this.A = (byte) (rgba >> 24 & (int) byte.MaxValue);
            this.B = (byte) (rgba >> 16 & (int) byte.MaxValue);
            this.G = (byte) (rgba >> 8 & (int) byte.MaxValue);
            this.R = (byte) (rgba & (int) byte.MaxValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the red, green, and blue, alpha components of the color. This must be an array with four elements.</param><exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception><exception cref="T:System.ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public ColorUint(float[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for ColorUint.");
            this.R = ColorUint.ToByte(values[0]);
            this.G = ColorUint.ToByte(values[1]);
            this.B = ColorUint.ToByte(values[2]);
            this.A = ColorUint.ToByte(values[3]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Fusee.Engine.ColorUint"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the alpha, red, green, and blue components of the color. This must be an array with four elements.</param><exception cref="T:System.ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception><exception cref="T:System.ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public ColorUint(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for ColorUint.");
            this.R = values[0];
            this.G = values[1];
            this.B = values[2];
            this.A = values[3];
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Fusee.Engine.ColorUint"/> to <see cref="T:Fusee.Math.float3"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator float3(ColorUint value)
        {
            return new float3((float) value.R / (float) byte.MaxValue, (float) value.G / (float) byte.MaxValue, (float) value.B / (float) byte.MaxValue);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Fusee.Engine.ColorUint"/> to <see cref="T:Fusee.Math.float4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator float4(ColorUint value)
        {
            return new float4((float)value.R / (float)byte.MaxValue, (float)value.G / (float)byte.MaxValue, (float)value.B / (float)byte.MaxValue, (float)value.A / (float)byte.MaxValue);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Fusee.Math.float3"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ColorUint(float3 value)
        {
            return new ColorUint(value.x, value.y, value.z, 1f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Fusee.Math.float4"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ColorUint(float4 value)
        {
            return new ColorUint(value.x, value.y, value.z, value.w);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:System.Int32"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator uint(ColorUint value)
        {
            return (uint) value.ToRgba();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:System.Int32"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ColorUint(uint value)
        {
            return new ColorUint(value);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:System.Int32"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator int(ColorUint value)
        {
            return value.ToRgba();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:System.Int32"/> to <see cref="T:Fusee.Engine.ColorUint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ColorUint(int value)
        {
            return new ColorUint(value);
        }


        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param><param name="right">The second color to add.</param>
        /// <returns>
        /// The sum of the two colors.
        /// </returns>
        public static ColorUint operator +(ColorUint left, ColorUint right)
        {
            return new ColorUint((float) ((int) left.R + (int) right.R), (float) ((int) left.G + (int) right.G), (float) ((int) left.B + (int) right.B), (float) ((int) left.A + (int) right.A));
        }

        /// <summary>
        /// Assert a color (return it unchanged).
        /// </summary>
        /// <param name="value">The color to assert (unchange).</param>
        /// <returns>
        /// The asserted (unchanged) color.
        /// </returns>
        public static ColorUint operator +(ColorUint value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param><param name="right">The second color to subtract.</param>
        /// <returns>
        /// The difference of the two colors.
        /// </returns>
        public static ColorUint operator -(ColorUint left, ColorUint right)
        {
            return new ColorUint((float) ((int) left.R - (int) right.R), (float) ((int) left.G - (int) right.G), (float) ((int) left.B - (int) right.B), (float) ((int) left.A - (int) right.A));
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param>
        /// <returns>
        /// A negated color.
        /// </returns>
        public static ColorUint operator -(ColorUint value)
        {
            return new ColorUint((float) -value.R, (float) -value.G, (float) -value.B, (float) -value.A);
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="scale">The factor by which to scale the color.</param><param name="value">The color to scale.</param>
        /// <returns>
        /// The scaled color.
        /// </returns>
        public static ColorUint operator *(float scale, ColorUint value)
        {
            return new ColorUint((byte) ((double) value.R * (double) scale), (byte) ((double) value.G * (double) scale), (byte) ((double) value.B * (double) scale), (byte) ((double) value.A * (double) scale));
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The factor by which to scale the color.</param><param name="scale">The color to scale.</param>
        /// <returns>
        /// The scaled color.
        /// </returns>
        public static ColorUint operator *(ColorUint value, float scale)
        {
            return new ColorUint((byte) ((double) value.R * (double) scale), (byte) ((double) value.G * (double) scale), (byte) ((double) value.B * (double) scale), (byte) ((double) value.A * (double) scale));
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param><param name="right">The second color to modulate.</param>
        /// <returns>
        /// The modulated color.
        /// </returns>
        public static ColorUint operator *(ColorUint left, ColorUint right)
        {
            return new ColorUint((byte) ((double) ((int) left.R * (int) right.R) / (double) byte.MaxValue), (byte) ((double) ((int) left.G * (int) right.G) / (double) byte.MaxValue), (byte) ((double) ((int) left.B * (int) right.B) / (double) byte.MaxValue), (byte) ((double) ((int) left.A * (int) right.A) / (double) byte.MaxValue));
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param><param name="right">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ColorUint left, ColorUint right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param><param name="right">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ColorUint left, ColorUint right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>
        /// A packed integer containing all four color components.
        /// </returns>
        public int ToBgra()
        {
            return (int) this.B | (int) this.G << 8 | (int) this.R << 16 | (int) this.A << 24;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>
        /// A packed integer containing all four color components.
        /// </returns>
        public int ToRgba()
        {
            return (int) this.R | (int) this.G << 8 | (int) this.B << 16 | (int) this.A << 24;
        }

        /// <summary>
        /// Converts the color into a three component vector.
        /// </summary>
        /// <returns>
        /// A three component vector containing the red, green, and blue components of the color.
        /// </returns>
        public float3 Tofloat3()
        {
            return new float3((float) this.R / (float) byte.MaxValue, (float) this.G / (float) byte.MaxValue, (float) this.B / (float) byte.MaxValue);
        }

        /// <summary>
        /// Converts the color into a four component vector.
        /// </summary>
        /// <returns>
        /// A four component vector containing all four color components.
        /// </returns>
        public float4 Tofloat4()
        {
            return new float4((float) this.R / (float) byte.MaxValue, (float) this.G / (float) byte.MaxValue, (float) this.B / (float) byte.MaxValue, (float) this.A / (float) byte.MaxValue);
        }

        /// <summary>
        /// Creates an array containing the elements of the color.
        /// </summary>
        /// <returns>
        /// A four-element array containing the components of the color in RGBA order.
        /// </returns>
        public byte[] ToArray()
        {
            return new byte[4]
            {
                R,
                G,
                B,
                A
            };
        }

        /// <summary>
        /// Gets the brightness.
        /// </summary>
        /// <returns>
        /// The Hue-Saturation-Brightness (HSB) saturation for this <see cref="T:Fusee.Engine.ColorUint"/>
        /// </returns>
        public float GetBrightness()
        {
            float fR = (float) R / (float) byte.MaxValue;
            float fG = (float) G / (float) byte.MaxValue;
            float fB = (float) B / (float) byte.MaxValue;
            float AA = fR;
            float BB = fR;
            if ((double)fG > (double)A)
                AA = fG;
            if ((double)fB > (double)A)
                AA = fB;
            if ((double)fG < (double)B)
                BB = fG;
            if ((double)fB < (double)B)
                BB = fB;
            return (float) (((double) AA + (double) BB) / 2.0);
        }

        /// <summary>
        /// Gets the hue.
        /// </summary>
        /// <returns>
        /// The Hue-Saturation-Brightness (HSB) saturation for this <see cref="T:Fusee.Engine.ColorUint"/>
        /// </returns>
        public float GetHue()
        {
            if ((int) this.R == (int) this.G && (int) this.G == (int) this.B)
            return 0.0f;
            float num1 = (float) this.R / (float) byte.MaxValue;
            float num2 = (float) this.G / (float) byte.MaxValue;
            float num3 = (float) this.B / (float) byte.MaxValue;
            float num4 = 0.0f;
            float num5 = num1;
            float num6 = num1;
            if ((double) num2 > (double) num5)
            num5 = num2;
            if ((double) num3 > (double) num5)
            num5 = num3;
            if ((double) num2 < (double) num6)
            num6 = num2;
            if ((double) num3 < (double) num6)
            num6 = num3;
            float num7 = num5 - num6;
            if ((double) num1 == (double) num5)
            num4 = (num2 - num3) / num7;
            else if ((double) num2 == (double) num5)
            num4 = (float) (2.0 + ((double) num3 - (double) num1) / (double) num7);
            else if ((double) num3 == (double) num5)
            num4 = (float) (4.0 + ((double) num1 - (double) num2) / (double) num7);
            float num8 = num4 * 60f;
            if ((double) num8 < 0.0)
            num8 += 360f;
            return num8;
        }

        /// <summary>
        /// Gets the saturation.
        /// </summary>
        /// <returns>
        /// The Hue-Saturation-Brightness (HSB) saturation for this <see cref="T:Fusee.Engine.ColorUint"/>
        /// </returns>
        public float GetSaturation()
        {
            float num1 = (float) this.R / (float) byte.MaxValue;
            float num2 = (float) this.G / (float) byte.MaxValue;
            float num3 = (float) this.B / (float) byte.MaxValue;
            float num4 = 0.0f;
            float num5 = num1;
            float num6 = num1;
            if ((double) num2 > (double) num5)
            num5 = num2;
            if ((double) num3 > (double) num5)
            num5 = num3;
            if ((double) num2 < (double) num6)
            num6 = num2;
            if ((double) num3 < (double) num6)
            num6 = num3;
            if ((double) num5 != (double) num6)
            num4 = ((double) num5 + (double) num6) / 2.0 > 0.5 ? (float) (((double) num5 - (double) num6) / (2.0 - (double) num5 - (double) num6)) : (float) (((double) num5 - (double) num6) / ((double) num5 + (double) num6));
            return num4;
        }

        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param><param name="right">The second color to add.</param><param name="result">When the method completes, completes the sum of the two colors.</param>
        public static void Add(ref ColorUint left, ref ColorUint right, out ColorUint result)
        {
            result.A = (byte) ((uint) left.A + (uint) right.A);
            result.R = (byte) ((uint) left.R + (uint) right.R);
            result.G = (byte) ((uint) left.G + (uint) right.G);
            result.B = (byte) ((uint) left.B + (uint) right.B);
        }

        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param><param name="right">The second color to add.</param>
        /// <returns>
        /// The sum of the two colors.
        /// </returns>
        public static ColorUint Add(ColorUint left, ColorUint right)
        {
            return new ColorUint((float) ((int) left.R + (int) right.R), (float) ((int) left.G + (int) right.G), (float) ((int) left.B + (int) right.B), (float) ((int) left.A + (int) right.A));
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param><param name="right">The second color to subtract.</param><param name="result">WHen the method completes, contains the difference of the two colors.</param>
        public static void Subtract(ref ColorUint left, ref ColorUint right, out ColorUint result)
        {
            result.A = (byte) ((uint) left.A - (uint) right.A);
            result.R = (byte) ((uint) left.R - (uint) right.R);
            result.G = (byte) ((uint) left.G - (uint) right.G);
            result.B = (byte) ((uint) left.B - (uint) right.B);
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param><param name="right">The second color to subtract</param>
        /// <returns>
        /// The difference of the two colors.
        /// </returns>
        public static ColorUint Subtract(ColorUint left, ColorUint right)
        {
            return new ColorUint((float) ((int) left.R - (int) right.R), (float) ((int) left.G - (int) right.G), (float) ((int) left.B - (int) right.B), (float) ((int) left.A - (int) right.A));
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param><param name="right">The second color to modulate.</param><param name="result">When the method completes, contains the modulated color.</param>
        public static void Modulate(ref ColorUint left, ref ColorUint right, out ColorUint result)
        {
            result.A = (byte) ((double) ((int) left.A * (int) right.A) / (double) byte.MaxValue);
            result.R = (byte) ((double) ((int) left.R * (int) right.R) / (double) byte.MaxValue);
            result.G = (byte) ((double) ((int) left.G * (int) right.G) / (double) byte.MaxValue);
            result.B = (byte) ((double) ((int) left.B * (int) right.B) / (double) byte.MaxValue);
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param><param name="right">The second color to modulate.</param>
        /// <returns>
        /// The modulated color.
        /// </returns>
        public static ColorUint Modulate(ColorUint left, ColorUint right)
        {
            return new ColorUint((float) ((int) left.R * (int) right.R), (float) ((int) left.G * (int) right.G), (float) ((int) left.B * (int) right.B), (float) ((int) left.A * (int) right.A));
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The color to scale.</param><param name="scale">The amount by which to scale.</param><param name="result">When the method completes, contains the scaled color.</param>
        public static void Scale(ref ColorUint value, float scale, out ColorUint result)
        {
            result.A = (byte) ((double) value.A * (double) scale);
            result.R = (byte) ((double) value.R * (double) scale);
            result.G = (byte) ((double) value.G * (double) scale);
            result.B = (byte) ((double) value.B * (double) scale);
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The color to scale.</param><param name="scale">The amount by which to scale.</param>
        /// <returns>
        /// The scaled color.
        /// </returns>
        public static ColorUint Scale(ColorUint value, float scale)
        {
            return new ColorUint((byte) ((double) value.R * (double) scale), (byte) ((double) value.G * (double) scale), (byte) ((double) value.B * (double) scale), (byte) ((double) value.A * (double) scale));
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param><param name="result">When the method completes, contains the negated color.</param>
        public static void Negate(ref ColorUint value, out ColorUint result)
        {
            result.A = (byte) ((uint) byte.MaxValue - (uint) value.A);
            result.R = (byte) ((uint) byte.MaxValue - (uint) value.R);
            result.G = (byte) ((uint) byte.MaxValue - (uint) value.G);
            result.B = (byte) ((uint) byte.MaxValue - (uint) value.B);
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param>
        /// <returns>
        /// The negated color.
        /// </returns>
        public static ColorUint Negate(ColorUint value)
        {
            return new ColorUint((float) ((int) byte.MaxValue - (int) value.R), (float) ((int) byte.MaxValue - (int) value.G), (float) ((int) byte.MaxValue - (int) value.B), (float) ((int) byte.MaxValue - (int) value.A));
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param><param name="result">When the method completes, contains the clamped value.</param>
        public static void Clamp(ref ColorUint value, ref ColorUint min, ref ColorUint max, out ColorUint result)
        {
            byte num1 = value.A;
            byte num2 = (int) num1 > (int) max.A ? max.A : num1;
            byte alpha = (int) num2 < (int) min.A ? min.A : num2;
            byte num3 = value.R;
            byte num4 = (int) num3 > (int) max.R ? max.R : num3;
            byte red = (int) num4 < (int) min.R ? min.R : num4;
            byte num5 = value.G;
            byte num6 = (int) num5 > (int) max.G ? max.G : num5;
            byte green = (int) num6 < (int) min.G ? min.G : num6;
            byte num7 = value.B;
            byte num8 = (int) num7 > (int) max.B ? max.B : num7;
            byte blue = (int) num8 < (int) min.B ? min.B : num8;
            result = new ColorUint(red, green, blue, alpha);
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order</param>
        /// <returns>
        /// A color.
        /// </returns>
        public static ColorUint FromBgra(int color)
        {
            return new ColorUint((byte)(color >> 16 & (int)byte.MaxValue), (byte)(color >> 8 & (int)byte.MaxValue), (byte)(color & (int)byte.MaxValue), (byte)(color >> 24 & (int)byte.MaxValue));
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order</param>
        /// <returns>
        /// A color.
        /// </returns>
        public static ColorUint FromBgra(uint color)
        {
            return ColorUint.FromBgra((int) color);
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in RGBA order</param>
        /// <returns>
        /// A color.
        /// </returns>
        public static ColorUint FromRgba(int color)
        {
            return new ColorUint(color);
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in RGBA order</param>
        /// <returns>
        /// A color.
        /// </returns>
        public static ColorUint FromRgba(uint color)
        {
            return new ColorUint(color);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value.</param><param name="max">The maximum value.</param>
        /// <returns>
        /// The clamped value.
        /// </returns>
        public static ColorUint Clamp(ColorUint value, ColorUint min, ColorUint max)
        {
            ColorUint result;
            ColorUint.Clamp(ref value, ref min, ref max, out result);
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two colors.
        /// </summary>
        /// <param name="start">Start color.</param><param name="end">End color.</param><param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param><param name="result">When the method completes, contains the linear interpolation of the two colors.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>
        /// start + (end - start) * amount
        /// </code>
        ///             Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
        /// </remarks>
        public static void Lerp(ref ColorUint start, ref ColorUint end, float amount, out ColorUint result)
        {
            result.A = (byte) ((double) start.A + (double) amount * (double) ((int) end.A - (int) start.A));
            result.R = (byte) ((double) start.R + (double) amount * (double) ((int) end.R - (int) start.R));
            result.G = (byte) ((double) start.G + (double) amount * (double) ((int) end.G - (int) start.G));
            result.B = (byte) ((double) start.B + (double) amount * (double) ((int) end.B - (int) start.B));
        }

        /// <summary>
        /// Performs a linear interpolation between two colors.
        /// 
        /// </summary>
        /// <param name="start">Start color.</param><param name="end">End color.</param><param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>
        /// The linear interpolation of the two colors.
        /// </returns>
        /// 
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// 
        /// <code>
        /// start + (end - start) * amount
        /// </code>
        /// 
        ///             Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
        /// 
        /// </remarks>
        public static ColorUint Lerp(ColorUint start, ColorUint end, float amount)
        {
            return new ColorUint((byte) ((double) start.R + (double) amount * (double) ((int) end.R - (int) start.R)), (byte) ((double) start.G + (double) amount * (double) ((int) end.G - (int) start.G)), (byte) ((double) start.B + (double) amount * (double) ((int) end.B - (int) start.B)), (byte) ((double) start.A + (double) amount * (double) ((int) end.A - (int) start.A)));
        }

        /// <summary>
        /// Performs a cubic interpolation between two colors.
        /// 
        /// </summary>
        /// <param name="start">Start color.</param><param name="end">End color.</param><param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param><param name="result">When the method completes, contains the cubic interpolation of the two colors.</param>
        public static void SmoothStep(ref ColorUint start, ref ColorUint end, float amount, out ColorUint result)
        {
            amount = (double) amount > 1.0 ? 1f : ((double) amount < 0.0 ? 0.0f : amount);
            amount = (float) ((double) amount * (double) amount * (3.0 - 2.0 * (double) amount));
            result.A = (byte) ((double) start.A + (double) ((int) end.A - (int) start.A) * (double) amount);
            result.R = (byte) ((double) start.R + (double) ((int) end.R - (int) start.R) * (double) amount);
            result.G = (byte) ((double) start.G + (double) ((int) end.G - (int) start.G) * (double) amount);
            result.B = (byte) ((double) start.B + (double) ((int) end.B - (int) start.B) * (double) amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two colors.
        /// 
        /// </summary>
        /// <param name="start">Start color.</param><param name="end">End color.</param><param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>
        /// The cubic interpolation of the two colors.
        /// </returns>
        public static ColorUint SmoothStep(ColorUint start, ColorUint end, float amount)
        {
            amount = (double) amount > 1.0 ? 1f : ((double) amount < 0.0 ? 0.0f : amount);
            amount = (float) ((double) amount * (double) amount * (3.0 - 2.0 * (double) amount));
            return new ColorUint((byte) ((double) start.R + (double) ((int) end.R - (int) start.R) * (double) amount), (byte) ((double) start.G + (double) ((int) end.G - (int) start.G) * (double) amount), (byte) ((double) start.B + (double) ((int) end.B - (int) start.B) * (double) amount), (byte) ((double) start.A + (double) ((int) end.A - (int) start.A) * (double) amount));
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colorss.
        /// 
        /// </summary>
        /// <param name="left">The first source color.</param><param name="right">The second source color.</param><param name="result">When the method completes, contains an new color composed of the largest components of the source colorss.</param>
        public static void Max(ref ColorUint left, ref ColorUint right, out ColorUint result)
        {
            result.A = (int) left.A > (int) right.A ? left.A : right.A;
            result.R = (int) left.R > (int) right.R ? left.R : right.R;
            result.G = (int) left.G > (int) right.G ? left.G : right.G;
            result.B = (int) left.B > (int) right.B ? left.B : right.B;
        }

        /// <summary>
        /// Returns a color containing the largest components of the specified colorss.
        /// 
        /// </summary>
        /// <param name="left">The first source color.</param><param name="right">The second source color.</param>
        /// <returns>
        /// A color containing the largest components of the source colors.
        /// </returns>
        public static ColorUint Max(ColorUint left, ColorUint right)
        {
            ColorUint result;
            ColorUint.Max(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colors.
        /// 
        /// </summary>
        /// <param name="left">The first source color.</param><param name="right">The second source color.</param><param name="result">When the method completes, contains an new color composed of the smallest components of the source colors.</param>
        public static void Min(ref ColorUint left, ref ColorUint right, out ColorUint result)
        {
            result.A = (int) left.A < (int) right.A ? left.A : right.A;
            result.R = (int) left.R < (int) right.R ? left.R : right.R;
            result.G = (int) left.G < (int) right.G ? left.G : right.G;
            result.B = (int) left.B < (int) right.B ? left.B : right.B;
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colors.
        /// 
        /// </summary>
        /// <param name="left">The first source color.</param><param name="right">The second source color.</param>
        /// <returns>
        /// A color containing the smallest components of the source colors.
        /// </returns>
        public static ColorUint Min(ColorUint left, ColorUint right)
        {
            ColorUint result;
            ColorUint.Min(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Adjusts the contrast of a color.
        /// 
        /// </summary>
        /// <param name="value">The color whose contrast is to be adjusted.</param><param name="contrast">The amount by which to adjust the contrast.</param><param name="result">When the method completes, contains the adjusted color.</param>
        public static void AdjustContrast(ref ColorUint value, float contrast, out ColorUint result)
        {
            result.A = value.A;
            result.R = ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.R / (double) byte.MaxValue - 0.5)));
            result.G = ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.G / (double) byte.MaxValue - 0.5)));
            result.B = ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.B / (double) byte.MaxValue - 0.5)));
        }

        /// <summary>
        /// Adjusts the contrast of a color.
        /// 
        /// </summary>
        /// <param name="value">The color whose contrast is to be adjusted.</param><param name="contrast">The amount by which to adjust the contrast.</param>
        /// <returns>
        /// The adjusted color.
        /// </returns>
        public static ColorUint AdjustContrast(ColorUint value, float contrast)
        {
            return new ColorUint(ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.R / (double) byte.MaxValue - 0.5))), ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.G / (double) byte.MaxValue - 0.5))), ColorUint.ToByte((float) (0.5 + (double) contrast * ((double) value.B / (double) byte.MaxValue - 0.5))), value.A);
        }

        /// <summary>
        /// Adjusts the saturation of a color.
        /// 
        /// </summary>
        /// <param name="value">The color whose saturation is to be adjusted.</param><param name="saturation">The amount by which to adjust the saturation.</param><param name="result">When the method completes, contains the adjusted color.</param>
        public static void AdjustSaturation(ref ColorUint value, float saturation, out ColorUint result)
        {
            float num = (float) ((double) value.R / (double) byte.MaxValue * 0.212500005960464 + (double) value.G / (double) byte.MaxValue * 0.715399980545044 + (double) value.B / (double) byte.MaxValue * 0.0720999985933304);
            result.A = value.A;
            result.R = ColorUint.ToByte(num + saturation * ((float) value.R / (float) byte.MaxValue - num));
            result.G = ColorUint.ToByte(num + saturation * ((float) value.G / (float) byte.MaxValue - num));
            result.B = ColorUint.ToByte(num + saturation * ((float) value.B / (float) byte.MaxValue - num));
        }

        /// <summary>
        /// Adjusts the saturation of a color.
        /// 
        /// </summary>
        /// <param name="value">The color whose saturation is to be adjusted.</param><param name="saturation">The amount by which to adjust the saturation.</param>
        /// <returns>
        /// The adjusted color.
        /// </returns>
        public static ColorUint AdjustSaturation(ColorUint value, float saturation)
        {
            float num = (float) ((double) value.R / (double) byte.MaxValue * 0.212500005960464 + (double) value.G / (double) byte.MaxValue * 0.715399980545044 + (double) value.B / (double) byte.MaxValue * 0.0720999985933304);
            return new ColorUint(ColorUint.ToByte(num + saturation * ((float) value.R / (float) byte.MaxValue - num)), ColorUint.ToByte(num + saturation * ((float) value.G / (float) byte.MaxValue - num)), ColorUint.ToByte(num + saturation * ((float) value.B / (float) byte.MaxValue - num)), value.A);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </returns>
        public override string ToString()
        {
            return string.Format("A:{0} R:{1} G:{2} B:{3}", (object) this.A, (object) this.R, (object) this.G, (object) this.B);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
            return this.ToString();
            return string.Format("A:{0} R:{1} G:{2} B:{3}", (object) this.A.ToString(format), (object) this.R.ToString(format), (object) this.G.ToString(format), (object) this.B.ToString(format));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "A:{0} R:{1} G:{2} B:{3}", (object) this.A, (object) this.R, (object) this.G, (object) this.B);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents this instance.
        /// 
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            return this.ToString(formatProvider);
            return string.Format(formatProvider, "A:{0} R:{1} G:{2} B:{3}", (object) this.A.ToString(format, formatProvider), (object) this.R.ToString(format, formatProvider), (object) this.G.ToString(format, formatProvider), (object) this.B.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// 
        /// </returns>
        public override int GetHashCode()
        {
            return this.A.GetHashCode() + this.R.GetHashCode() + this.G.GetHashCode() + this.B.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Fusee.Engine.ColorUint"/> is equal to this instance.
        /// 
        /// </summary>
        /// <param name="other">The <see cref="T:Fusee.Engine.ColorUint"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:Fusee.Engine.ColorUint"/> is equal to this instance; otherwise, <c>false</c>.
        /// 
        /// </returns>
        public bool Equals(ColorUint other)
        {
            if ((int) this.R == (int) other.R && (int) this.G == (int) other.G && (int) this.B == (int) other.B)
            return (int) this.A == (int) other.A;
            else
            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to this instance.
        /// 
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// 
        /// </returns>
        public override bool Equals(object value)
        {
            if (value == null || !object.ReferenceEquals((object) value.GetType(), (object) typeof (ColorUint)))
            return false;
            else
            return this.Equals((ColorUint) value);
        }

        private static byte ToByte(float component)
        {
            int num = (int) ((double) component * (double) byte.MaxValue);
            return num < 0 ? (byte) 0 : (num > (int) byte.MaxValue ? byte.MaxValue : (byte) num);
        }

    }
}
