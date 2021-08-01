using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Components
{
    public abstract class ComponentBase
    {
        SceneNode _parent;


        public SceneNode Parent { get => _parent; set => _parent = value; }

        public ComponentBase(SceneNode parent)
        {
            _parent = parent;
            OnCreate();
        }

        protected virtual void OnCreate()
        {
          
        }
    }
}
