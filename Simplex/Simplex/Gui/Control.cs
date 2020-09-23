using NanoVGDotNet.NanoVG;
using Simplex.Util;
using System.Collections.Generic;
using System.ComponentModel;

namespace Simplex.Gui
{
    /// <summary>
    /// base class for a gui element
    /// </summary>
    public class Control
    {
        #region Private Fields

        private SimplexColor backgroundColor = new SimplexColor(OpenTK.Color.Orange);
        private SimplexColor borderColor = new SimplexColor(OpenTK.Color.Black);
        private float borderRadius = 0;
        private float borderThickness = 0;
        private OpenTK.Rectangle bounds = new OpenTK.Rectangle() { X = 0, Y = 0, Width = 50, Height = 20 };
        private bool enabled = true;
        private bool focused = false;
        private bool hovered = false;
        private string name;
        private bool scissorContents = true;
        private SimplexColor textColor = new SimplexColor(OpenTK.Color.Black);
        private bool visible = true;

        #endregion Private Fields

        #region Protected Fields

        /// <summary>
        /// the childelements of the control
        /// when compositing in an inherited class, nested controls should not be put here
        /// </summary>
        protected List<Control> children = new List<Control>();

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public Control()
        {
            this.borderThickness = 1;
            borderRadius = 1;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the backgroundcolor
        /// </summary>
        public SimplexColor BackgroundColor { get => backgroundColor; set => backgroundColor = value; }

        /// <summary>
        /// the borderradius
        /// </summary>
        public float BorderRadius { get => borderRadius; set => borderRadius = value; }

        /// <summary>
        /// the boundaries relative to the parent control
        /// </summary>
        public OpenTK.Rectangle Bounds { get => bounds; set => bounds = value; }

        /// <summary>
        /// returns if the control can be dragged
        /// </summary>
        public virtual bool CanDrag
        {
            get => false;
        }

        /// <summary>
        /// returns if the control can be focused
        /// </summary>
        public virtual bool CanFocus { get => false; }
        /// <summary>
        /// the child controls
        /// </summary>
        public List<Control> Children { get => children; private set => children = value; }

        /// <summary>
        /// gets or sets if the control is enabled
        /// a disabled control will not receive any events
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled { get => enabled; set => enabled = value; }

        /// <summary>
        /// gets or sets if the control is focused
        /// </summary>
        public bool Focused { get => focused; set => focused = value; }

        /// <summary>
        /// the height of the control
        /// </summary>
        public int Height
        {
            get => bounds.Height;
            set { bounds.Height = value; OnResized(); }
        }

        /// <summary>
        /// gets or sets if the control is hovered
        /// </summary>
        public bool Hovered { get => hovered; set => hovered = value; }

        /// <summary>
        /// the x coordinate relative to the parent control
        /// </summary>
        public int Left
        {
            get => bounds.Left;
            set { bounds.X = value; OnPositionChanged(); }
        }

        /// <summary>
        /// the name for easier finding
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// gets or sets if this control scissors its content to its own bounds
        /// </summary>
        public bool ScissorContents { get => scissorContents; set => scissorContents = value; }

        /// <summary>
        /// the y coordinate relative to the parent control
        /// </summary>
        public int Top
        {
            get => bounds.Y;
            set { bounds.Y = value; OnPositionChanged(); }
        }

        /// <summary>
        /// the width of the control
        /// </summary>
        public int Width
        {
            get => bounds.Width;
            set { bounds.Width = value; OnResized(); }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// gets called when the control gets dragged
        /// </summary>
        /// <param name="x">the absolute mouse x coord</param>
        /// <param name="y">the absolute mouse y coord</param>
        /// <param name="deltaX">the incremental amount of movement in X</param>
        /// <param name="deltaY">the incremental amount of movement in Y</param>
        protected virtual void OnDrag(int x, int y, int deltaX, int deltaY) { }

        /// <summary>
        /// gets called when the dragging of the control ended
        /// </summary>
        protected virtual void OnDragEnd() { }

        /// <summary>
        /// gets called when the control starts to be dragged
        /// </summary>
        protected virtual void OnDragStart() { }

        /// <summary>
        /// gets called when the control gets drawn
        /// to prevent the background from being drawn or implement your own way,dont call base.OnDraw
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="vg"></param>
        protected virtual void OnDraw(Control parent, NvgContext vg)
        {
            NanoVg.BeginPath(vg);
            NanoVg.RoundedRect(vg, Left, Top, Width, Height, borderRadius);
            NanoVg.FillColor(vg, backgroundColor);
            NanoVg.Fill(vg);

            if (borderThickness > 0)
            {
                NanoVg.RoundedRect(vg, Left, Top, Width, Height, borderRadius);
                if (!hovered)
                {
                    NanoVg.StrokeColor(vg, borderColor);
                    NanoVg.StrokeWidth(vg, borderThickness);
                }
                else
                {
                    NanoVg.StrokeColor(vg, new SimplexColor(255, 0, 255, 255));
                    NanoVg.StrokeWidth(vg, borderThickness + 3);
                }

                NanoVg.Stroke(vg);
            }
            NanoVg.ClosePath(vg);
        }

        /// <summary>
        /// gets called when another control gets dropped on this
        /// only gets called when candrop returns true
        /// </summary>
        /// <param name="ctrl">the control that gets dropped</param>
        protected virtual void OnDrop(Control ctrl) { }

        /// <summary>
        /// gets called when the control gets focus
        /// </summary>
        protected virtual void OnFocusGained() { }

        /// <summary>
        /// gets called when the control lost focus
        /// </summary>
        protected virtual void OnFocusLost() { }

        /// <summary>
        /// gets called when the control gets hovered
        /// </summary>
        protected virtual void OnHovered() { }

        /// <summary>
        /// Click event , gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if the click is handled  by the control</returns>
        protected virtual bool OnMouseClick()
        {
            return false;
        }

        /// <summary>
        /// Click event , gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if the click is handled  by the control</returns>
        protected virtual bool OnMouseDown()
        {
            return false;
        }

        /// <summary>
        /// gets called when the mouse entered the controls bounds
        /// </summary>
        protected virtual void OnMouseEntered()
        {
        }

        /// <summary>
        /// gets called when the mouse left the controls bounds
        /// </summary>
        protected virtual void OnMouseLeft() { }

        /// <summary>
        /// gets called when the mouse gets moved
        /// </summary>
        /// <param name="x">the x coord relative to the parent</param>
        /// <param name="y">the y coord relative to the parent</param>
        protected virtual void OnMouseMove(int x, int y) { }

        /// <summary>
        /// Click event , gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if the click is handled  by the control</returns>
        protected virtual bool OnMouseUp()
        {
            return false;
        }

        /// <summary>
        /// gets called when the position has changed
        /// </summary>
        protected virtual void OnPositionChanged() { }

        /// <summary>
        /// gets called when the size has changed
        /// </summary>
        protected virtual void OnResized() { }

        /// <summary>
        /// gets called when the draggeing visual for the Control should be rendered
        /// ToDo : add nanovg congtext and drag position parameters
        /// </summary>
        protected virtual void RenderDragControl() { }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// returns if the given control can be dropped on this control
        /// gets asked mainly for visual representation
        /// </summary>
        /// <param name="ctrl">the control to be dropped</param>
        /// <returns>true if the control can handle the drop</returns>
        public virtual bool CanDrop(Control ctrl) { return false; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool ContainsPoint(int x, int y)
        {
            return bounds.Contains(x, y);
        }

        /// <summary>
        /// Tells the control it currently gets dragged
        /// </summary>
        /// <param name="x">X coord in absolute Window position</param>
        /// <param name="y">Y coord in absolute Window position</param>
        /// <param name="deltaX">the incremental amount of movement in X</param>
        /// <param name="deltaY">the incremental amount of movement in Y</param>
        public void Drag(int x, int y, int deltaX, int deltaY)
        {
            OnDrag(x, y, deltaX, deltaY);
        }

        /// <summary>
        /// tells the control the dragging ended
        /// </summary>
        public void DragEnd()
        {
            OnDragEnd();
        }

        /// <summary>
        /// tells the control the dragging started
        /// </summary>
        public void DragStart()
        {
            OnDragStart();
        }

        /// <summary>
        /// draws the control
        /// </summary>
        /// <param name="parent">the parent control</param>
        /// <param name="vg">the nanovg context to draw to</param>
        public void Draw(Control parent, NvgContext vg)
        {
            OnDraw(parent, vg);
            NanoVg.Translate(vg, Left, Top);
            foreach (Control child in children)
            {
                if (child.visible)
                    child.Draw(this, vg);
            }
            NanoVg.Translate(vg, -Left, -Top);
            NanoVg.ResetScissor(vg);
        }
        /// <summary>
        /// tells the control another control gets dropped on it
        /// </summary>
        /// <param name="ctrl"></param>
        public void Drop(Control ctrl)
        {
            OnDrop(ctrl);
        }

        /// <summary>
        /// tries to find a child control at the given position
        /// </summary>
        /// <param name="x">the x coord relative to the parent</param>
        /// <param name="y">the y coord relative to the parent</param>
        /// <returns>the control at the position,null if none found</returns>
        public Control FindChildAtPosition(int x, int y)
        {
            int relativeX = x - bounds.X;
            int relativeY = y - bounds.Y;

            foreach (Control child in children)
            {
                if (child.ContainsPoint(relativeX, relativeY))
                {
                    return child;
                }
                Control found = child.FindChildAtPosition(relativeX, relativeY);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// tries to find a child control recursive by it's name
        /// </summary>
        /// <param name="name">the name of the child control</param>
        /// <param name="recursive">if the search should be recursive </param>
        /// <returns>the first control with a matching name,null if none found</returns>
        public Control FindChildByName(string name, bool recursive = true)
        {
            foreach (Control child in children)
            {
                if (child.name == name)
                    return child;
            }

            if (!recursive)
                return null;

            foreach (Control child in children)
            {
                Control found = child.FindChildByName(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// tells the control it gets focused
        /// </summary>
        public void Focus()
        {
            this.focused = true;
        }

        /// <summary>
        /// tell the control it's hovered
        /// </summary>
        public void Hover()
        {
            this.OnHovered();
        }

        /// <summary>
        /// Click event , gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if the click is handled  by the control</returns>
        public bool MouseClick()
        {
            return OnMouseClick();
        }

        /// <summary>
        /// MouseDown call,gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if this control handles the event</returns>
        public bool MouseDown()
        {
            return OnMouseDown();
        }

        /// <summary>
        /// tell the control the mouse entered
        /// </summary>
        public void MouseEntered()
        {
            this.hovered = true;
            OnMouseEntered();
        }

        /// <summary>
        /// tell the control the mouse left its bounds
        /// </summary>
        public void MouseLeft()
        {
            this.hovered = false;
            OnMouseLeft();
        }

        /// <summary>
        /// gets called when the mouse gets moved from the parent
        /// </summary>
        /// <param name="x">the x coord relative to the parent</param>
        /// <param name="y">the y coord relative to the parent</param>
        public void MouseMove(int x, int y)
        {
            OnMouseMove(x, y);
        }
        /// <summary>
        /// MouseUp call,gets called from the guirenderer usually
        /// </summary>
        /// <returns>true if the event is handled</returns>
        public bool MouseUp()
        {
            return OnMouseUp();
        }
        /// <summary>
        /// tells the control it lost focused
        /// </summary>
        public void UnFocus()
        {
            this.focused = false;
            OnFocusLost();
        }

        #endregion Public Methods
    }
}