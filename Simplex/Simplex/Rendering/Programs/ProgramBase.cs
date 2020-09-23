using System.Numerics;
using ObjectTK.Shaders;
using ObjectTK.Shaders.Variables;
using OpenTK.Graphics.OpenGL;

namespace Simplex.Rendering{
    public class ProgramBase : Program{
         [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }

         [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InTexCoord { get; protected set; }

        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InNormal { get; protected set; }

        [VertexAttrib(4, VertexAttribPointerType.Float)]
        public VertexAttrib InTangent { get; protected set; }

        [VertexAttrib(4, VertexAttribPointerType.UnsignedByte, true)]
        public VertexAttrib InColor { get; protected set; }

       
        public Uniform<Matrix4x4> Projection { get; protected set; }
        public Uniform<Matrix4x4> View { get; protected set; }
        public Uniform<Matrix4x4> Model { get; protected set; }


    }
}