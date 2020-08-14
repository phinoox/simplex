using ObjectTK.Buffers;
using OpenTK;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{

    public class MeshData
    {

        private string attributeName;

        public string AttributeName { get => attributeName; set => attributeName = value; }


    }

    public class MeshPrimitive{

        private PbrMaterial material;
        private List<MeshData> meshDatas = new List<MeshData>();
        public List<MeshData> MeshDatas { get => meshDatas; }
        public PbrMaterial Material { get => material; set => material = value; }
    }
    
    /// <summary>
    /// base class for mesh information
    /// </summary>
    public class Mesh
    {
        #region Private Fields

        
        private ObjectTK.Buffers.VertexArray vbo;

        private List<MeshPrimitive> primitives = new List<MeshPrimitive>();
        

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the vertexbuffer object for the vertices
        /// </summary>
        public VertexArray Vbo { get => vbo; set => vbo = value; }
        public List<MeshPrimitive> Primitives { get => primitives; set => primitives = value; }


        #endregion Public Properties

        #region Public Methods

        public void Render(Matrix4 mvp)
        {
        }

        #endregion Public Methods
    }
}