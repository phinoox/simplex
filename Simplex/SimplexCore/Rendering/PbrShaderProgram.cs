using ObjectTK.Shaders;
using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Simplex.Core.Rendering
{

    /// <summary>
    /// Basic ShaderProgram for pbr based rendering 
    /// </summary>
    [VertexShaderSource("PbrBase.Vertex")]
    [FragmentShaderSource("PbrBase.Fragment")]
    public class PbrShaderProgram : Program
    {
        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }

        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InTexCoord { get; protected set; }
        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InNormal { get; protected set; }

        [VertexAttrib(4, VertexAttribPointerType.UnsignedByte, true)]
        public VertexAttrib InColor { get; protected set; }

        public Uniform<Matrix4> ModelViewProjectionMatrix { get; protected set; }

        public Uniform<Vector3> LightDir { get; set; }
        public Uniform<Color> LightColor { get; set; }

        public Uniform<Color> Ambient { get; set; }
        public Uniform<Color> FragColor { get; set; }
        public TextureUniform Albedo { get; set; }

        public TextureUniform NormalTex { get; set; }
        public TextureUniform MetalTex { get; set; }
        public Uniform<float> Metalicness { get; set; }
        public TextureUniform RoughnessTex { get; set; }
        public Uniform<float> Roughness { get; set; }
        public TextureUniform EmissiveMap { get; set; }
        public Uniform<float> EmissiveFactor { get; set; }
        public Uniform<uint> Flags { get; set; }

        public Uniform<float> NormalFactor { get; set; }


        public Uniform<uint> Time { get; set; }
        public PbrShaderProgram() : base()
        {

        }

    }

}