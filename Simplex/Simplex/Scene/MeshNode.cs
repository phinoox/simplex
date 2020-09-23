using System.Numerics;
using Simplex.Rendering;

namespace Simplex.Scene
{
    public class MeshNode : SceneNode
    {
        #region Private Fields

        private Mesh mesh;
        Matrix4x4 _modelMatrix = Matrix4x4.Identity;
        public Mesh Mesh { get => mesh; set => mesh = value; }
       

        #endregion Private Fields

        #region Public Methods

        public void Render(Camera cam)
        {
            Matrix4x4 translation = Matrix4x4.CreateTranslation(Translation);
            Matrix4x4 rotation = Matrix4x4.CreateFromQuaternion(Rotation);
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale);
            Matrix4x4 ModelMatrix = translation * rotation * scale ;
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