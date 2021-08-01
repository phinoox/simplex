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

        private Camera _currentCamera = new Camera();
        private SceneNode _rootNode = new SceneNode();
        private LightNode _directionalLight;

        public SceneNode RootNode { get => _rootNode; set => _rootNode = value; }
        public Camera CurrentCamera { get => _currentCamera; set => _currentCamera = value; }
        public LightNode DirectionalLight { get => _directionalLight; set => _directionalLight = value; }

        #endregion Private Fields

        public void Init()
        {
            _directionalLight = new LightNode();
            _directionalLight.LightType = LightTypes.DIRECTIONAL;
        }


        #region Public Methods

        
        #endregion Public Methods
    }
}