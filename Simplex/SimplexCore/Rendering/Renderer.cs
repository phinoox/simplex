using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Simplex.Core.Rendering
{
    public class Renderer
    {

        public void Render(Camera cam,float delta)
        {
            GL.ClearColor(.9f, .9f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        }

    }
}
