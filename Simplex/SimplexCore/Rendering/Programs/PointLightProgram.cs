using ObjectTK.Shaders.Variables;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Mathematics;

namespace Simplex.Core.Rendering{

  public class PointLightProgram : ProgramBase{
        public Uniform<float> Attentuation { get; set;}
        public Uniform<Color4> LightColor { get; set; }
        public Uniform<float> LightPower {get;set;}
  }

}