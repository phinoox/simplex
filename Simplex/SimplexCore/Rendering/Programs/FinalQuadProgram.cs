using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering.Programs
{
    [VertexShaderSource("ScreenQuad_v.Vertex")]
    [FragmentShaderSource("SimpleTexture_f.Fragment")]
    class FinalQuadProgram : ProgramBase
    {
        public TextureUniform Diffuse { get; set; }
    }
}
