using NanoVGDotNet.NanoVG;
using System.Collections.Generic;

namespace Simplex.Loaders
{
    /// <summary>
    /// Loader class for loading ttf fonts to be used with nanovg
    /// </summary>
    public class FontLoader
    {
        #region Private Fields

        private Dictionary<int, string> fonts = new Dictionary<int, string>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// loads the defaultfont
        /// </summary>
        public void LoadDefaults()
        {
            NanoVGDotNet.NanoVG.NvgContext vg = ApplicationBase.Instance.MainWindow.Vg;
            //ResourceManager resm = new ResourceManager("default resources", Assembly.GetCallingAssembly());
            //object fontObject =resm.GetObject("OpenSans_Regular");
            int fontId = NanoVg.CreateFont(vg, "sans", "Data/Fonts/OpenSans-Regular.ttf");
        }

        #endregion Public Methods
    }
}