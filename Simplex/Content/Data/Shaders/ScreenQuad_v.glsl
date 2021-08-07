-- VERTEX
#version 330
in vec3 InPosition;
in vec2 InTexCoord;

smooth out vec2 TexCoord;

void main()
{
	// transform vertex position..it's just a screen quad, so no need
	gl_Position =  vec4(InPosition,1);
	// pass through texture coordinate
	TexCoord = InTexCoord;
}