using NanoVGDotNet.NanoVG;
using System;

namespace Simplex.Gui
{
    /// <summary>
    /// Control Element for drawing an imange
    /// </summary>
    public class ImageControl : Control
    {
        #region Private Fields

        private int image;
        private string imagePath;

        #endregion Private Fields

        #region Protected Methods

        /// <summary>
        /// draws the image
        /// </summary>
        /// <param name="parent">the parent control</param>
        /// <param name="vg">the nanovg context</param>
        protected override void OnDraw(Control parent, NvgContext vg)
        {
            base.OnDraw(parent, vg);
            if (String.IsNullOrEmpty(imagePath))
                return;
            if (image == 0)
                image = NanoVg.CreateImage(ref vg, imagePath, 0);
            NvgPaint imgPaint = NanoVg.ImagePattern(vg, Left, Top, Width, Height, 0, image, 1);
            NanoVg.BeginPath(vg);
            NanoVg.RoundedRect(vg, Left, Top, Width, Height, BorderRadius);
            NanoVg.FillPaint(vg, imgPaint);
            NanoVg.Fill(vg);
        }

        #endregion Protected Methods
    }
}