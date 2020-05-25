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
            GL.ClearColor(.9f, .9f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(cam.getProjectionMatrix() * cam.getViewMatrix());
            }
        }

        #endregion Public Methods
    }
}