using NanoVGDotNet.NanoVG;
using OpenTK;
using Simplex.Core.Util;
using System;
using System.Collections.Generic;

namespace Simplex.Core.Gui
{
    /// <summary>
    /// Main class nested in the ApplicationWIndow that handles gui renderering
    /// also handles basic gui logic like hovering,focus etc
    /// </summary>
    public class GuiRenderer
    {
        #region Private Fields

        private static GuiRenderer defaultRenderer;
        private Control currentHovered = null;
        private Control draggedControl = null;
        private Queue<Func<bool>> drawCommands = new Queue<Func<bool>>();
        private Control focusedControl = null;
        private Vector2 lastMousePos;
        private Vector2 mouseDelta = Vector2.Zero;
        private Control mouseDownControl = null;
        private bool mouseInWindow = false;
        private List<GuiWindow> windows = new List<GuiWindow>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public GuiRenderer()
        {
            if (defaultRenderer == null)
                defaultRenderer = this;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// returns the default guirenderer instance - so the first created
        /// </summary>
        public static GuiRenderer DefaultRenderer { get => defaultRenderer; set => defaultRenderer = value; }

        /// <summary>
        /// the list of guiwindows
        /// </summary>
        public List<GuiWindow> Windows { get => windows; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// draws some text the next render call
        /// </summary>
        /// <param name="text">the text to display</param>
        /// <param name="x">the absolte x position of the text</param>
        /// <param name="y">the absolute y position of the text</param>
        public void DrawText(string text, float x, float y)
        {
            drawCommands.Enqueue(() =>
           {
               NvgContext vg = ApplicationBase.Instance.MainWindow.Vg;
               NanoVg.FillColor(vg, new SimplexColor(0, 0, 0, 1));
               NanoVg.Text(vg, x, y, text);
               return true;
           }
                );
        }

        /// <summary>
        /// tells the guirenderer about a mousedown event
        /// </summary>
        /// <param name="x">the absolute x position of the mouse</param>
        /// <param name="y">the absolute y position of the mouse</param>
        /// <returns>true if a control handled the event</returns>
        public bool MouseDown(int x, int y)
        {
            if (!mouseInWindow)
                return false;
            if (currentHovered != null)
            {
                mouseDownControl = currentHovered;
                return currentHovered.MouseDown();
            }
            return false;
        }

        /// <summary>
        /// gets called when the mouse entered the window
        /// </summary>
        public void MouseEntered()
        {
            mouseInWindow = true;
        }

        /// <summary>
        /// gets called when the mouse left the window
        /// </summary>
        public void MouseLeave()
        {
            mouseInWindow = false;
            currentHovered = null;
        }

        /// <summary>
        /// tells the guirenderer about a mousemove event
        /// </summary>
        /// <param name="x">the absolute x position of the mouse</param>
        /// <param name="y">the absolute y position of the mouse</param>
        /// <returns>true if a control handled the event</returns>
        public void MouseMove(int x, int y)
        {
            if (!mouseInWindow)
                return;
            Vector2 mousePos = new Vector2(x, y);
            if (lastMousePos != null)
            {
                mouseDelta = lastMousePos - mousePos;
            }

            lastMousePos = mousePos;
            if (draggedControl != null)
            {
                draggedControl.Drag(x, y, (int)mouseDelta.X, (int)mouseDelta.Y);
            }
            else if (mouseDownControl != null && mouseDownControl.CanDrag)
            {
                draggedControl = mouseDownControl;
                draggedControl.DragStart();
            }

            bool found = false;
            foreach (GuiWindow gwin in windows)
            {
                if (gwin.ContainsPoint(x, y))
                {
                    found = true;
                    Control tmp = gwin;
                    gwin.MouseMove(x, y);
                    Control underMouse = gwin.FindChildAtPosition(x, y);
                    if (underMouse != null)
                    {
                        tmp = underMouse;
                    }

                    if (currentHovered == null)
                    {
                        currentHovered = tmp;
                        tmp.MouseEntered();
                    }
                    else if (currentHovered != tmp)
                    {
                        currentHovered.MouseLeft();
                        currentHovered = tmp;
                        tmp.MouseEntered();
                    }
                }
            }
            if (!found && currentHovered != null)
            {
                currentHovered.MouseLeft();
                currentHovered = null;
            }
        }

        /// <summary>
        /// tells the guirenderer about a mouseup event
        /// </summary>
        /// <param name="x">the absolute x position of the mouse</param>
        /// <param name="y">the absolute y position of the mouse</param>
        /// <returns>true if a control handled the event</returns>
        public bool MouseUp(int x, int y)
        {
            if (draggedControl != null)
            {
                draggedControl.DragEnd();
                draggedControl = null;
            }
            if (!mouseInWindow)
                return false;
            if (currentHovered != null)
            {
                if (draggedControl != null)
                {
                    if (currentHovered.CanDrop(draggedControl))
                        currentHovered.Drop(draggedControl);
                    draggedControl.DragEnd();
                    draggedControl = null;
                }
                else if (mouseDownControl != null)
                {
                    if (mouseDownControl == currentHovered)
                    {
                        mouseDownControl.MouseClick();
                        if (mouseDownControl.CanFocus)
                        {
                            if (focusedControl != null)
                                focusedControl.UnFocus();
                            focusedControl = mouseDownControl;
                            focusedControl.Focus();
                        }
                    }
                    mouseDownControl = null;
                }

                return currentHovered.MouseUp();
            }
            else if (mouseDownControl != null)
            {
                mouseDownControl = null;
                if (draggedControl != null)
                {
                    draggedControl.DragEnd();
                    draggedControl = null;
                }
            }

            return false;
        }

        /// <summary>
        /// renders the gui
        /// </summary>
        public void Render()
        {
            NvgContext vg = ApplicationBase.Instance.MainWindow.Vg;
            foreach (GuiWindow win in windows)
            {
                win.Render(vg);
            }
            while (drawCommands.Count > 0)
            {
                Func<bool> drawFunc = drawCommands.Dequeue();
                drawFunc();
            }
        }

        #endregion Public Methods
    }
}