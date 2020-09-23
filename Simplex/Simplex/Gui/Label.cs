using NanoVGDotNet.NanoVG;
using Simplex.Util;

namespace Simplex.Gui
{
    /// <summary>
    /// A Label Control for simple one line text
    /// </summary>
    public class Label : Control
    {
        #region Private Fields

        private TextAlign alignment = TextAlign.Left;
        private int fontsize = 12;
        private string text = "";
        private SimplexColor textColor = new SimplexColor(OpenTK.Color.Black);

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the alignment of the text
        /// </summary>
        public TextAlign Alignment { get => alignment; set => alignment = value; }

        /// <summary>
        /// the fontsize of the text
        /// </summary>
        public int Fontsize { get => fontsize; set => fontsize = value; }

        /// <summary>
        /// the text to be shown
        /// </summary>
        public string Text { get => text; set => text = value; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// draws the control
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="vg"></param>
        protected override void OnDraw(Control parent, NvgContext vg)
        {
            base.OnDraw(parent, vg);
            NanoVg.BeginPath(vg);
            NanoVg.FontBlur(vg, 0.2f);
            NanoVg.FillColor(vg, textColor);
            NanoVg.FontSize(vg, fontsize);
            //NanoVg.TextAlign(vg, (NvgAlign)((int)alignment)|NvgAlign.Middle);
            NanoVg.TextAlign(vg, NvgAlign.Center | NvgAlign.Middle);
            int posX = Left + Width / 2;
            int posY = Top + Height / 2;
            float[] bounds = new float[4];
            NanoVg.TextBounds(vg, 0, 0, text, bounds);
            NanoVg.Text(vg, posX, posY, text);
            NanoVg.ClosePath(vg);
        }

        #endregion Protected Methods
    }
}