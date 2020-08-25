-- Vertex
#version 140
in vec3 InPosition;
in vec4 InColor;
in vec2 InTexCoord;

smooth out vec4 Color;
smooth out vec2 TexCoord;

uniform mat4 ModelViewProjectionMatrix;

void main()
{
	gl_Position = ModelViewProjectionMatrix * vec4(InPosition,1);
	Color = InColor;
	TexCoord = InTexCoord;
}

-- Fragment
#version 140
smooth in vec4 Color;
smooth in vec2 TexCoord;

uniform sampler2D Albedo;
uniform sampler2D NormalTex;

uniform bool HasAlbedo;
uniform bool HasNormalTex;
uniform vec4 FragColor;
uniform Vec4 LightColor;
uniform vec3 LightDir;
out vec4 FragColor;

float near = 0.1; 
float far  = 1000.0; 
  
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}


void main()
{
	float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide by far for demonstration
	if(HasAlbedo)
     FragColor = texture(Albedo, TexCoord);
    else
     FragColor = vec4(1,0,0,1); 
}
