using NanoVGDotNet.NanoVG;
using System;
using System.Resources;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Simplex.Core.Loaders
{
    public class FontLoader
    {
        Dictionary<int, string> fonts = new Dictionary<int, string>();

        public void LoadDefaults()
        {
            NanoVGDotNet.NanoVG.NvgContext vg = ApplicationBase.Instance.MainWindow.Vg;
            //ResourceManager resm = new ResourceManager("default resources", Assembly.GetCallingAssembly());
            //object fontObject =resm.GetObject("OpenSans_Regular");
            int fontId = NanoVg.CreateFont(vg, "sans", "Fonts/OpenSans-Regular.ttf");
        }

    }
}
