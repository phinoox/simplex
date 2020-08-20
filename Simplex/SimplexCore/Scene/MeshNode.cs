using OpenTK;
using Simplex.Core.Rendering;

namespace Simplex.Core.Scene
{
    public class MeshNode : SceneNode
    {
        #region Private Fields

        private Mesh mesh;
        Matrix4 _modelMatrix = Matrix4.Identity;
        public Mesh Mesh { get => mesh; set => mesh = value; }

        #endregion Private Fields

        #region Public Methods

        public void Render(in Matrix4 view,in Matrix4 projection)
        {
            Matrix4 ModelMatrix = Matrix4.CreateTranslation(Translation) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateScale(Scale) ;
            Matrix4 mvp = ModelMatrix * view * projection ;
            mesh.Render(mvp);
        }

        #endregion Public Methods
    }
}