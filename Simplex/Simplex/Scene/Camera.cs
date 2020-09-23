using System;
using System.Numerics;

namespace Simplex.Scene
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
        private Matrix4x4 projectionMatrix = new Matrix4x4();
        private Vector3 up = Vector3.UnitY;
        private Matrix4x4 viewMatrix = new Matrix4x4();

        #endregion Private Fields

        #region Private Methods

        private void calculateProjectionMatrix()
        {
            if (cameraType == CameraTypes.Perspective)
            {
                float ratio = (float)ApplicationBase.Instance.MainWindow.Width / (float)ApplicationBase.Instance.MainWindow.Height;
               projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(OpenTK.MathHelper.DegreesToRadians(fov), ratio, nearClip, farClip);
            }
            else
            {
              projectionMatrix =  Matrix4x4.CreateOrthographic(ApplicationBase.Instance.MainWindow.Width, ApplicationBase.Instance.MainWindow.Height, nearClip, farClip);
            }
        }

        private void createViewMatrix()
        {
            Vector3 target = (Translation+Forward );
            viewMatrix = Matrix4x4.CreateLookAt(Translation,target,up);
           // viewMatrix = new Matrix4(new Vector4(Right),new Vector4(Up),new Vector4(Forward),new Vector4(-Translation,1));
            //_rotation = viewMatrix.ExtractRotation();
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
        public Matrix4x4 getProjectionMatrix()
        {
            calculateProjectionMatrix();
            return projectionMatrix;
        }

        public Matrix4x4 getViewMatrix()
        {
            //createViewMatrix();
            return viewMatrix;
        }

        /// <summary>
        /// lets the camera rotate to look at a specific position
        /// </summary>
        /// <param name="target"></param>
        public override void LookAt(in Vector3 target) {
           base.LookAt(target);
           // viewMatrix = Matrix4.LookAt(Translation,target,up);
           // _rotation = viewMatrix.ExtractRotation();
            //Console.WriteLine($"looking at target {target}");
        }

        #endregion Public Methods

    }
}