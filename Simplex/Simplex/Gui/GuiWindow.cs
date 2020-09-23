using NanoVGDotNet.NanoVG;

namespace Simplex.Gui
{
    /// <summary>
    /// A GuiWindow to hold other controls
    /// Main Feature is it can be dragged and closed
    /// </summary>
    public class GuiWindow : Control
    {
        #region Private Fields

        private bool showDecorations = true;
        private Label titleLabel = new Label();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public GuiWindow() : base()
        {
            titleLabel.Alignment = TextAlign.Center;
            titleLabel.Fontsize = 18;
            titleLabel.Height = 25;
            titleLabel.BackgroundColor = new Util.SimplexColor(0, 0, 255, 255);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// abusing draggin for moving the window
        /// </summary>
        public override bool CanDrag => true;

        /// <summary>
        /// determines if the decorations get rendered,meaning the title and the close button
        /// </summary>
        public bool ShowDecorations { get => showDecorations; set => showDecorations = value; }

        /// <summary>
        /// the title that gets displayed at the top
        /// </summary>
        public string Title
        {
            get => titleLabel.Text;
            set => titleLabel.Text = value;
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        protected override void OnDrag(int x, int y, int deltaX, int deltaY)
        {
            this.Left -= deltaX;
            this.Top -= deltaY;
            if (Left < 0)
                Left = 0;
            if (Top < titleLabel.Height)
                Top = titleLabel.Height;
            base.OnDrag(x, y, deltaX, deltaY);
        }

        /// <summary>
        ///
        /// </summary>
        protected override void OnDragStart()
        {
            base.OnDragStart();
        }

        /// <summary>
        /// draws the guiwindow
        /// </summary>
        /// <param name="parent">for guiwindows this should be null</param>
        /// <param name="vg">the nanovg context</param>
        protected override void OnDraw(Control parent, NvgContext vg)
        {
            base.OnDraw(parent, vg);
        }

        /// <summary>
        /// moving the decorations with the guiwindow
        /// </summary>
        protected override void OnPositionChanged()
        {
            base.OnPositionChanged();
            this.titleLabel.Left = this.Left;
            this.titleLabel.Top = this.Top - titleLabel.Height;
        }

        /// <summary>
        /// resizing the window and the decorations
        /// </summary>
        protected override void OnResized()
        {
            base.OnResized();
            this.titleLabel.Width = this.Width;
        }

        #endregion Protected Methods

        #region Public Methods

        public override bool ContainsPoint(int x, int y)
        {
            if (titleLabel.ContainsPoint(x, y))
                return true;
            return base.ContainsPoint(x, y);
        }
        /// <summary>
        /// renders the guiwindow
        /// </summary>
        /// <param name="vg">the nanovg context</param>
        public void Render(NvgContext vg)
        {
            if (ScissorContents)
                NanoVg.Scissor(vg, Left, Top, Width, Height);

            this.Draw(null, vg);
            if (ScissorContents)
                NanoVg.ResetScissor(vg);
            titleLabel.Draw(this, vg);
        }

        #endregion Public Methods
    }
}