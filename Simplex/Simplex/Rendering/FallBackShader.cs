using OpenTK.Graphics.OpenGL;

namespace Simplex.Rendering
{

    public class FallBackShader
    {
        public const string Vertex = @"
        -- Vertex
#version 140
in vec3 InPosition;
in vec4 InColor;

smooth out vec4 Color;

uniform mat4 ModelViewProjectionMatrix;

void main()
{
	gl_Position = ModelViewProjectionMatrix * vec4(InPosition,1);
	Color = InColor;
}

";
        public const string Fragment = @"
        -- Fragment
#version 140
smooth in vec4 Color;

out vec4 FragColor;

void main()
{
	FragColor = Color;
}";

        public static string GetFallbackShader(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.VertexShader:
                    return Vertex;
                case ShaderType.FragmentShader:
                    return Fragment;
                default:
                    return "";
            }
        }

    }
}
