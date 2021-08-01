using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering.Programs
{
    [VertexShaderSource("WireFrame.Vertex")]
    [FragmentShaderSource("WireFrame.Fragment")]
    public class WireFrameProgram : ProgramBase
    {
        public Uniform<Color4> WireColor { get; set; }

    }
}
