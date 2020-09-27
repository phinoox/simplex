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
        }

        #endregion Public Methods
    }
}