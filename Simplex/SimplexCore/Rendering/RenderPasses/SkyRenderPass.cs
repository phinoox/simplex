using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering.RenderPasses
{
    public class SkyRenderPass : RenderPass
    {
        public override int SortOrder => 10;

        protected override void OnRender(Scene3D scene)
        {
           
        }

        protected override void OnResize(int width, int height)
        {
           
        }
    }
}
