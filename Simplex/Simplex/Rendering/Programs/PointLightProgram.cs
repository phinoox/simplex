using ObjectTK.Shaders.Variables;
using OpenTK.Graphics.OpenGL;
using System.Numerics;

namespace Simplex.Rendering{

  public class PointLightProgram : ProgramBase{
       [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }
        public Uniform<float> Attentuation { get; set;}
        public Uniform<Vector4> LightColor { get; set; }
        public Uniform<float> LightPower {get;set;}
  }

}