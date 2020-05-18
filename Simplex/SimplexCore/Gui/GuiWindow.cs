using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Gui
{
    public class GuiWindow : Control
    {
        public void Render()
        {
            foreach(Control child in children)
            {
                if (child.Enabled)
                    child.Draw(this.position);
            }
        }
    }
}
