using OpenTK;
using Simplex.Core.Rendering;

namespace Simplex.Core.Scene
{
    public class MeshNode : SceneNode
    {
        #region Private Fields

        private Mesh mesh;

        #endregion Private Fields

        #region Public Methods

        public void Render(Matrix4 VPMatrix)
        {
            Matrix4 ModelMatrix = Matrix4.CreateTranslation(Translation) * Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation);
            Matrix4 mvp = VPMatrix * ModelMatrix;
            mesh.Render(mvp);
        }

        #endregion Public Methods
    }
}