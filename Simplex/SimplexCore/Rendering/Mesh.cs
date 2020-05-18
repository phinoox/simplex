using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK;
using ObjectTK.Buffers;

namespace Simplex.Core.Rendering
{
    public class Mesh
    {
        ObjectTK.Buffers.VertexArray vbo;
        List<PbrMaterial> materials = new List<PbrMaterial>();

        public VertexArray Vbo { get => vbo; set => vbo = value; }
        public List<PbrMaterial> Materials { get => materials; set => materials = value; }
    }
}
