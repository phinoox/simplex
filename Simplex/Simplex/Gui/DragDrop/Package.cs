using System;
using Simplex.Gui.Control;

namespace Simplex.Gui.DragDrop
{
    public class Package
    {
        public string Name;
        public object UserData;
        public bool IsDraggable;
        public ControlBase DrawControl;
        public Point HoldOffset;
    }
}
