using ObjectTK.Shaders;
using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Simplex.Core.Rendering
{
    [VertexShaderSource("PbrComposite.Vertex")]
    [FragmentShaderSource("PbrComposite.Fragment")]
    public class CompositionProgram : Program
    {
        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }

        [VertexAttrib(2, VertexAttribPointerType.Float)]
        public VertexAttrib InTexCoord { get; protected set; }
        public Uniform<bool> Debug { get; set ;}

        public Uniform<Matrix4> ModelViewProjectionMatrix { get; protected set; }
        public TextureUniform Diffuse { get; set; }
        public TextureUniform Normal { get; set; }
        public TextureUniform Position { get; set; }
        public TextureUniform Emissive { get; set; }
        public TextureUniform LightMap { get; set; }
        public TextureUniform ShadowMap { get; set; }

        //public TextureUniform Depth { get; set; }

    }
}