using ObjectTK.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering
{
    public abstract class PostProcess
    {
        private int _width;
        private int _height;
        private Framebuffer _renderBuffer;

        public abstract int SortOrder { get; }

    }
}
