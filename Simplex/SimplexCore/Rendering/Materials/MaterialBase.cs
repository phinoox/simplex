using System;
using OpenTK.Graphics.OpenGL4;

namespace Simplex.Core.Rendering{

    public enum AlphaMode
    {
        OPAQUE,
        MASKED,
        BLEND
    }



    public class MaterialBase : IDisposable{

        protected ProgramBase _shaderProgram;
        public ProgramBase ShaderProgram {get =>_shaderProgram ;set => _shaderProgram = value;}
        private AlphaMode _blending = AlphaMode.OPAQUE;
        private bool _doubleSided = false;

        public AlphaMode Blending { get => _blending; set => _blending = value; }
        public bool DoubleSided { get => _doubleSided; set => _doubleSided = value; }

        public void Dispose()
        {
            if (ShaderProgram != null)
                ShaderProgram.Dispose();
        }

        public virtual void Init(){

        }

        public virtual void Use(){
            if (DoubleSided)
                GL.Disable(EnableCap.CullFace);
            else
                GL.Enable(EnableCap.CullFace);
        }
    }
}