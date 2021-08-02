using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Components
{
    /// <summary>
    /// base class for a component that can be attached to a scenenode
    /// </summary>
    public abstract class ComponentBase
    {
        SceneNode _parent;

        /// <summary>
        /// the parent scenenode, by now needs to get set manually
        /// </summary>
        public SceneNode Parent { get => _parent; set => _parent = value; }

        /// <summary>
        /// calls the oncreate function
        /// </summary>
        public ComponentBase()
        {
            OnCreate();
        }

        /// <summary>
        /// initializaton stuff should happen here
        /// gets called by constructor
        /// </summary>
        protected virtual void OnCreate()
        {
          
        }
    }
}
