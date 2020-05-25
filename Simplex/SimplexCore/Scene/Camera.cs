using OpenTK;

namespace Simplex.Core.Scene
{
    /// <summary>
    /// Different types of Camera
    /// </summary>
    public enum CameraTypes
    {
        /// <summary>
        /// camera shows scene in ortographic mode
        /// </summary>
        Orthographic,

        /// <summary>
        /// camera shows scene in perspective mode
        /// </summary>
        Perspective
    }

    /// <summary>
    /// base class for camera implementation
    /// </summary>
    public class Camera : SceneNode
    {

        #region Private Fields

        /// <summary>
        /// the type of view
        /// </summary>
        private CameraTypes cameraType = CameraTypes.Perspective;

        private float farClip = 1000f;
        private int fov = 65;
        private float nearClip = 0.1f;
        private Matrix4 projectionMatrix = new Matrix4();
        private Vector3 up = Vector3.UnitY;
        private Matrix4 viewMatrix = new Matrix4();

        #endregion Private Fields

        #region Private Methods

        private void calculateProjectionMatrix()
        {
            if (cameraType == CameraTypes.Perspective)
            {
                float ratio = ApplicationBase.Instance.MainWindow.Width / ApplicationBase.Instance.MainWindow.Height;
                Matrix4.CreatePerspectiveFieldOfView(fov, ratio, nearClip, farClip, out projectionMatrix);
            }
            else
            {
                Matrix4.CreateOrthographic(ApplicationBase.Instance.MainWindow.Width, ApplicationBase.Instance.MainWindow.Height, nearClip, farClip, out projectionMatrix);
            }
        }

        private void createViewMatrix()
        {
            viewMatrix = Matrix4.CreateTranslation(-Translation);
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void onRotate()
        {
            base.onRotate();
            createViewMatrix();
        }

        /// <summary>
        /// called when the postion changed
        /// view matrix gets precalculated here
        /// </summary>
        protected override void onTranslate()
        {
            base.onTranslate();
            createViewMatrix();
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// returns the projection matrix based on the default window
        /// </summary>
        /// <returns></returns>
        public Matrix4 getProjectionMatrix()
        {
            calculateProjectionMatrix();
            return projectionMatrix;
        }

        public Matrix4 getViewMatrix()
        {
            createViewMatrix();
            return viewMatrix;
        }

        /// <summary>
        /// lets the camera rotate to look at a specific position
        /// </summary>
        /// <param name="position"></param>
        public void LookAt(Vector3 position) { }

        #endregion Public Methods

    }
}