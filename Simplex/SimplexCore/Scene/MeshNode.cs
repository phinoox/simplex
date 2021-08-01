using OpenTK;
using OpenTK.Mathematics;
using Simplex.Core.Components;
using Simplex.Core.Rendering;

namespace Simplex.Core.Scene
{
    public class MeshNode : SceneNode
    {
        #region Private Fields

        private MeshComponent mesh;
        Matrix4 _modelMatrix = Matrix4.Identity;
        public MeshComponent Mesh { get => mesh; set => mesh = value; }
       

        #endregion Private Fields

        #region Public Methods

        public void Render(AlphaMode mode)
        {
            Matrix4 translation = Matrix4.CreateTranslation(GlobalTranslation);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(GlobalRotation);
            Matrix4 scale = Matrix4.CreateScale(GlobalScale);
            Matrix4 ModelMatrix = translation * rotation * scale ;
            mesh.Render(ModelMatrix,mode);
        }

        public void RenderShadowMapSimple(AlphaMode mode)
        {
            Matrix4 translation = Matrix4.CreateTranslation(GlobalTranslation);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(GlobalRotation);
            Matrix4 scale = Matrix4.CreateScale(GlobalScale);
            Matrix4 ModelMatrix = translation * rotation * scale;
            mesh.RenderShadowMapSimple(ModelMatrix, mode);
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