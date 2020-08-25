-- Vertex
#version 330
in vec3 InPosition;
in vec2 InTexCoord;

smooth out vec2 TexCoord;

uniform mat4 ModelViewProjectionMatrix;

void main()
{
	// transform vertex position
	gl_Position = ModelViewProjectionMatrix * vec4(InPosition,1);
	// pass through texture coordinate
	TexCoord = InTexCoord;
}

-- Fragment
#version 330
smooth in vec2 TexCoord;

out vec4 FragColor;

uniform sampler2D Diffuse;
uniform sampler2D Normal;
uniform sampler2D Position;
uniform sampler2D Emissive;

uniform bool RenderTexCoords = false;

void main()
{
	if (RenderTexCoords) FragColor = vec4(TexCoord, 0, 1);
	else FragColor = textureLod(Diffuse, TexCoord,0);
	//FragColor = vec4(1,0,0,1);
}
