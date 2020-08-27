using System;

namespace Simplex.Core.Rendering{

    public class MaterialBase : IDisposable{

        protected ProgramBase _shaderProgram;
        public ProgramBase ShaderProgram {get =>_shaderProgram ;set => _shaderProgram = value;}

        public void Dispose()
        {
            if (ShaderProgram != null)
                ShaderProgram.Dispose();
        }

        public virtual void Init(){

        }

        public virtual void Use(){
        }
    }
}