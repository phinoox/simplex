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

        public void Render(Camera cam)
        {
            Matrix4 translation = Matrix4.CreateTranslation(Translation);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(Rotation);
            Matrix4 scale = Matrix4.CreateScale(Scale);
            Matrix4 ModelMatrix = translation * rotation * scale ;
            mesh.Render(ModelMatrix);
        }

        protected override void onDestroy(){
            if(mesh!=null)
                mesh.Dispose();
        }

        public override void CalculateBounds(){
            this.Bounds = this.mesh.Bounds;
            base.CalculateBounds();
        }

        #endregion Public Methods
    }
}