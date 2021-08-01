-- VERTEX
#version 330

in vec3 InPosition;
in vec4 InColor;
in vec2 InTexCoord;
in vec3 InNormal;
in vec4 InTangent;

smooth out vec4 Color;
smooth out vec2 TexCoord;

out vec4 Position;
out mat3 TBN;
out mat4 model;
out vec4 FragPos;
out vec3 Normal;
out vec3 Tangent;
out vec3 Bitangent;
uniform mat4 Model;
uniform mat4 Projection;
uniform mat4 View;

void main()
{
    mat4 mvp = Projection * View * Model;
	gl_Position = mvp * vec4(InPosition,1);

}

-- FRAGMENT
#version 330

uniform vec4 WireColor;


layout (location = 0) out vec4 OColor;

void main()
{
     OColor = WireColor;
}
