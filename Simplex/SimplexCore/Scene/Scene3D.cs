using Simplex.Core.Rendering;
using System.Collections.Generic;

namespace Simplex.Core.Scene
{
    /// <summary>
    /// main class for a 3d scene
    /// </summary>
    public class Scene3D
    {
        #region Private Fields

        private Camera currentCamera = new Camera();
        private SceneNode rootNode = new SceneNode();
        

        public SceneNode RootNode { get => rootNode; set => rootNode = value; }
        public Camera CurrentCamera { get => currentCamera; set => currentCamera = value; }

        #endregion Private Fields

        #region Public Methods

        
        #endregion Public Methods
    }
}