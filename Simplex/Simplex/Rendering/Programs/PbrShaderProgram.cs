using ObjectTK.Shaders;
using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using Simplex.Util;

namespace Simplex.Rendering
{

    /// <summary>
    /// Basic ShaderProgram for pbr based rendering 
    /// </summary>
    [VertexShaderSource("PbrBase.Vertex")]
    [FragmentShaderSource("PbrBase.Fragment")]
    public class PbrShaderProgram : ProgramBase
    {
        public Uniform<Vector3> LightDir { get; set; }
        public Uniform<OpenTK.Color> LightColor { get; set; }

        public Uniform<Vector4> Ambient { get; set; }
        public Uniform<Vector4> FragColor { get; set; }
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