using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Util
{
    public class SimplexColor
    {
        int r;
        int g;
        int b;
        int a;

        public SimplexColor() { }
        public SimplexColor(int r,int g,int b,int a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public SimplexColor(OpenTK.Color color)
        {
            this.r = color.R;
            this.g = color.G;
            this.b = color.B;
            this.a = color.A;
        }

        public static implicit operator OpenTK.Color(SimplexColor sc)
        {
            return new OpenTK.Color(sc.r, sc.g, sc.b, sc.a);
        }

        

        public static implicit operator NanoVGDotNet.NanoVG.NvgColor(SimplexColor sc)
        {
            return new NanoVGDotNet.NanoVG.NvgColor() {R=sc.r, G=sc.g, B=sc.b, A=sc.a };
        }

        public static implicit operator SimplexColor (OpenTK.Color sc)
        {
            return new SimplexColor() { r=sc.R, g=sc.G, b=sc.B, a=sc.A };
        }

        public static implicit operator SimplexColor(NanoVGDotNet.NanoVG.NvgColor sc)
        {
            return new SimplexColor() { r = (int)sc.R, g = (int)sc.G, b = (int)sc.B, a = (int)sc.A };
        }


    }
}
