using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK.Buffers;
using Simplex.Core.Rendering;
using Simplex.Core.Rendering.Programs;

namespace Simplex.Core.Components
{
    public class DirectionalLight : LightBase
    {
       
        protected override void OnRender(Matrix4 mvp)
        {
            base.OnRender(mvp);

        }

        protected override void OnCreate()
        {
            base.OnCreate();
          
        }
               
    }
}
