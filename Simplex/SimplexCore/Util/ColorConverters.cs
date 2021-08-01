using System;
using System.ComponentModel;
using System.Globalization;
using OpenTK.Mathematics;

namespace Simplex.Core.Util
{
    /// <summary>
    /// a sadly needed wrapper for easier handling colors in both gui and 3d
    /// </summary>
    [TypeConverter(typeof(Simplex.Core.Util.SimplexColorConverter))]
    public class SimplexColor
    {
        #region Private Fields

        private int a;
        private int b;
        private int g;
        private int r;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public SimplexColor() { }

        /// <summary>
        /// common constructor with parameters, value range 0-255
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">alpha</param>
        public SimplexColor(int r, int g, int b, int a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// coopy constructor for use with opentk color
        /// </summary>
        /// <param name="color"></param>
        public SimplexColor(OpenTK.Mathematics.Color4 color)
        {
            this.r = (int)(color.R * 255);
            this.g = (int)(color.G * 255);
            this.b = (int)(color.B * 255);
            this.a = (int)(color.A * 255);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the alpha value 0-255
        /// </summary>
        public int A { get => a; set => a = value; }

        /// <summary>
        /// the blue value 0-255
        /// </summary>
        public int B { get => b; set => b = value; }

        /// <summary>
        /// the green value 0-255
        /// </summary>
        public int G { get => g; set => g = value; }

        /// <summary>
        /// the red value 0-255
        /// </summary>
        public int R { get => r; set => r = value; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// implicit cast to opentk color
        /// </summary>
        /// <param name="sc"></param>
        public static implicit operator OpenTK.Mathematics.Color4(SimplexColor sc)
        {
            return new OpenTK.Mathematics.Color4(sc.r / 255.0f, sc.g / 255.0f, sc.b / 255.0f, sc.a / 255.0f);
        }

        /// <summary>
        /// implicit cast to simplexcolor
        /// </summary>
        /// <param name="sc"></param>
        public static implicit operator SimplexColor(OpenTK.Mathematics.Color4 sc)
        {
            return new SimplexColor(sc);
        }

        /// <summary>
        /// implicit cast to simplexcolor
        /// </summary>
        /// <param name="sc"></param>
        public static implicit operator SimplexColor(NanoVGDotNet.NanoVG.NvgColor sc)
        {
            return new SimplexColor() { r = (int)sc.R, g = (int)sc.G, b = (int)sc.B, a = (int)sc.A };
        }

        /// <summary>
        /// imlpicit cast to nvgcolor
        /// </summary>
        /// <param name="sc"></param>
        public static implicit operator NanoVGDotNet.NanoVG.NvgColor(SimplexColor sc)
        {
            return new NanoVGDotNet.NanoVG.NvgColor() { R = sc.r, G = sc.g, B = sc.b, A = sc.a };
        }

        #endregion Public Methods
    }

    /// <summary>
    /// color converter class for use in xaml
    /// </summary>
    public class SimplexColorConverter : System.ComponentModel.TypeConverter
    {
        #region Public Methods

        /// <summary>
        /// just converting from string by now
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Name == "String")
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// just converting to string by now
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Name == "String")
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// just converting from string by now
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = (value as string).Split(',');
                if (parts.Length < 4)
                    return base.ConvertFrom(context, culture, value);
                SimplexColor col = new SimplexColor();
                col.R = int.Parse(parts[0]);
                col.G = int.Parse(parts[1]);
                col.B = int.Parse(parts[2]);
                col.A = int.Parse(parts[3]);
                return col;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// just converting to string by now
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationtype");
            if (value.GetType() == typeof(SimplexColor))
            {
                SimplexColor col = value as SimplexColor;
                string result = $"{col.R},{col.G},{col.B},{col.A}";
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Public Methods
    }
}