using ObjectTK.Buffers;
using OpenTK;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// base class for mesh information
    /// </summary>
    public class Mesh
    {
        #region Private Fields

        private List<PbrMaterial> materials = new List<PbrMaterial>();
        private ObjectTK.Buffers.VertexArray vbo;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the materials to be used,
        /// didn't think this through yet
        /// </summary>
        public List<PbrMaterial> Materials { get => materials; set => materials = value; }

        /// <summary>
        /// the vertexbuffer object for the vertices
        /// </summary>
        public VertexArray Vbo { get => vbo; set => vbo = value; }

        #endregion Public Properties

        #region Public Methods

        public void Render(Matrix4 mvp)
        {
        }

        #endregion Public Methods
    }
}