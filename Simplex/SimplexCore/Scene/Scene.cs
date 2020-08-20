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
        private Renderer sceneRenderer = new Renderer();

        public SceneNode RootNode { get => rootNode; set => rootNode = value; }
        public Camera CurrentCamera { get => currentCamera; set => currentCamera = value; }

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="delta">milliseconds</param>
        public void Render(float delta)
        {
            List<MeshNode> nodes = rootNode.FindChildByType<MeshNode>();
            sceneRenderer.Render(currentCamera, delta, nodes);
        }

        #endregion Public Methods
    }
}