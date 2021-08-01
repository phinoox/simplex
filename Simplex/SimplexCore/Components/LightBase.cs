using OpenTK.Mathematics;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Components
{
    public abstract class LightBase : ComponentBase,IDisposable
    {

        public LightBase(SceneNode parent) : base(parent) { }

        public virtual void Dispose()
        {
           
        }

        public void Render(Matrix4 mvp)
        {
            OnRender(mvp);
        }

        protected virtual void OnRender(Matrix4 mvp)
        {

        }
    }
}
