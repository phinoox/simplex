using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// main class for rendering a 3D scene...I guess
    /// </summary>
    public class Renderer
    {
        #region Public Methods

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="cam">the camera to be used</param>
        /// <param name="delta">the time since the last frame in milliseconds</param>
        public void Render(Camera cam, float delta, List<MeshNode> meshes)
        {
            GL.ClearColor(Color.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            Matrix4 view = cam.getViewMatrix();
            Matrix4 projection = cam.getProjectionMatrix();
            //vp.M44=1;
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(view,projection);
            }
        }

        #endregion Public Methods
    }
}