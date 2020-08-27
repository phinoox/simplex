using ObjectTK.Shaders.Variables;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Simplex.Core.Rendering{

  public class PointLightProgram : ProgramBase{
       [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }
        public Uniform<float> Attentuation { get; set;}
        public Uniform<Color> LightColor { get; set; }
        public Uniform<float> LightPower {get;set;}
  }

}