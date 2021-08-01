using ObjectTK.Shaders;
using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace Simplex.Core.Rendering
{
    [VertexShaderSource("PbrComposite.Vertex")]
    [FragmentShaderSource("PbrComposite.Fragment")]
    public class CompositionProgram : ProgramBase
    {
       
        [VertexAttrib(2, VertexAttribPointerType.Float)]
        public VertexAttrib InTexCoord { get; protected set; }
        public Uniform<bool> Debug { get; set ;}
        [VertexAttrib(2, VertexAttribPointerType.Float)]
        public Uniform<Vector2> ViewPortSize { get; protected set; }
        public Uniform<Color4> Ambient { get; set; }

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