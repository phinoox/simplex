using System;
using OpenTK;
using OpenTK.Mathematics;
using Simplex.Core.Util;

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
        
        private float _pitch;
        private float _yaw;
        private float _roll;

        private Vector3 _forward = WorldDefaults.Forward;

        private Matrix4 _viewMatrix = new Matrix4();

        #endregion Private Fields


        public override Vector3 Forward { get =>_forward; }
        public override Vector3 Right { get => Vector3.Cross(_forward,WorldDefaults.Up); }
        public override Vector3 Up { get => WorldDefaults.Up; }
        public float FarClip { get => farClip; set => farClip = value; }
        public int Fov { get => fov; set => fov = value; }
        public float NearClip { get => nearClip; set => nearClip = value; }
        public CameraTypes CameraType { get => cameraType; set => cameraType = value; }


        #region Private Methods

        private void calculateProjectionMatrix()
        {
            if (cameraType == CameraTypes.Perspective)
            {
                float ratio = (float)ApplicationBase.Instance.MainWindow.Size.X / (float)ApplicationBase.Instance.MainWindow.Size.Y;
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), ratio, nearClip, farClip, out projectionMatrix);
            }
            else
            {
                Matrix4.CreateOrthographic(ApplicationBase.Instance.MainWindow.Size.X, ApplicationBase.Instance.MainWindow.Size.Y, nearClip, farClip, out projectionMatrix);
            }
        }

        private void UpdateRotation()
        {
            _forward.X = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(_yaw));
            _forward.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
            _forward.Z = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_yaw));
            _forward = Vector3.Normalize(_forward);
            createViewMatrix();
           // Rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(_pitch), MathHelper.DegreesToRadians(_yaw), MathHelper.DegreesToRadians(_roll));
        }

        private void createViewMatrix()
        {

            Vector3 target = (Translation + _forward);
            _viewMatrix = Matrix4.LookAt(Translation, target, WorldDefaults.Up);
            _rotation = _viewMatrix.ExtractRotation();
            return;

            //Vector3 target = (Translation+Forward );
            _viewMatrix = Matrix4.LookAt(Translation,target,Up);
            return;
            Matrix4 tr = Matrix4.CreateTranslation(-Translation);
            Matrix4 rot = Matrix4.CreateFromQuaternion(Rotation);
            _viewMatrix = rot * tr;
            //viewMatrix = new Matrix4(new Vector4(Right),new Vector4(Up),new Vector4(Forward),new Vector4(-Translation,1));
            //_rotation = viewMatrix.ExtractRotation();
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void onCreate()
        {
            base.onCreate();
            _forward.X = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(_yaw));
            _forward.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
            _forward.Z = (float)Math.Cos(MathHelper.DegreesToRadians(_pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_yaw));
            _forward = Vector3.Normalize(_forward);
            Vector3 target = (Translation + _forward);
            _viewMatrix = Matrix4.LookAt(Translation, target, WorldDefaults.Up);
            this.AvoidRole = true;
        }

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

        public override void Pitch(float angle, bool local = true)
        {
            _pitch += angle;
            if (_pitch > 89.0f)
            {
                _pitch = 89.0f;
            }
            else if (_pitch < -89.0f)
            {
                _pitch = -89.0f;
            }
           
            UpdateRotation();
        }

        public override void Yaw(float angle, bool local = true)
        {
            _yaw += angle;
            UpdateRotation();
        }

        public override void Roll(float angle, bool local = true)
        {
            _roll += angle;
            UpdateRotation();
        }

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
            //createViewMatrix();
            return _viewMatrix;
        }

        /// <summary>
        /// lets the camera rotate to look at a specific position
        /// </summary>
        /// <param name="target"></param>
        public override void LookAt(in Vector3 target) {
            Vector3 dir = (target - Translation).Normalized();
            float pitch = MathF.Asin(dir.Y);
            float yaw = MathF.Atan2(dir.X, dir.Z);
            _pitch = 0;
            _yaw = 0;
            Pitch(pitch);
            Yaw(yaw);
           // viewMatrix = Matrix4.LookAt(Translation,target,up);
           // _rotation = viewMatrix.ExtractRotation();
            //Console.WriteLine($"looking at target {target}");
        }

        #endregion Public Methods

    }
}