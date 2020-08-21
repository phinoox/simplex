-- Vertex
#version 330




in vec3 InPosition;
in vec4 InColor;
in vec2 InTexCoord;
in vec3 InNormal;

smooth out vec4 Color;
smooth out vec2 TexCoord;
smooth out vec3 Normal;
smooth out vec4 Position;

uniform mat4 ModelViewProjectionMatrix;

void main()
{
	gl_Position = ModelViewProjectionMatrix * vec4(InPosition,1);
	Color = InColor;
	TexCoord = InTexCoord;
	Normal = normalize((ModelViewProjectionMatrix * vec4(InNormal,0)).xyz);
	Position = gl_Position;
}

-- Fragment
#version 330
smooth in vec4 Color;
smooth in vec2 TexCoord;
smooth in vec3 Normal;
smooth in vec4 Position;

uniform sampler2D Albedo;
uniform sampler2D NormalTex;
uniform sampler2D MetalTex;
uniform sampler2D RoughnessTex;
uniform sampler2D EmissiveMap;

uniform float Metalicness;
uniform float Roughness;
uniform float EmissiveFactor;



uniform uint Flags;
uniform float NormalFactor;
uniform vec4 FragColor;
uniform vec4 LightColor;
uniform vec4 Ambient;
uniform vec3 LightDir;
uniform uint Time;

layout (location = 0) out vec4 OColor;
layout (location = 1) out vec4 ONormal;
layout (location = 2) out vec4 OPosition;
layout (location = 3) out vec4 OEmissive;

const uint FLAG_ALBEDO = 1u << 0;
const uint FLAG_NORMAL = 1u << 1;
const uint FLAG_METALL = 1u << 2;
const uint FLAG_ROUGHNESS = 1u << 3;
const uint FLAG_EMISSIVE = 1u << 4;

float near = 0.1; 
float far  = 1000.0; 
  
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

mat4 rotationX( in float angle ) {
	return mat4(	1.0,		0,			0,			0,
			 		0, 	cos(angle),	-sin(angle),		0,
					0, 	sin(angle),	 cos(angle),		0,
					0, 			0,			  0, 		1);
}

mat4 rotationY( in float angle ) {
	return mat4(	cos(angle),		0,		sin(angle),	0,
			 				0,		1.0,			 0,	0,
					-sin(angle),	0,		cos(angle),	0,
							0, 		0,				0,	1);
}

mat4 rotationZ( in float angle ) {
	return mat4(	cos(angle),		-sin(angle),	0,	0,
			 		sin(angle),		cos(angle),		0,	0,
							0,				0,		1,	0,
							0,				0,		0,	1);
}


void main()
{
    
    vec3 normalTex = normalize(texture(NormalTex,TexCoord).rgb *2.0-1);
    //vec3 lightDir = normalize((rotationY(int(Time * 0.01f) % 360) * vec4(LightDir,0)).xyz);
    vec3 norm = normalize(Normal + normalTex);
    vec4 color;
    float diff = max(dot(norm, LightDir), 0.0) *3.5f;
    vec4 directional = vec4(LightColor.rgb * diff,1);
	float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide by far for demonstration
	if((Flags & FLAG_ALBEDO)== FLAG_ALBEDO)
     color = texture(Albedo, TexCoord);
    else
     color = vec4(1,0,0,1);
     vec4 emissive = vec4(0,0,0,0);
     if((Flags & FLAG_EMISSIVE)==FLAG_EMISSIVE){
       emissive = texture(EmissiveMap,TexCoord);
     }
     color = color * (directional+Ambient);
     OColor = color;
     ONormal = vec4(norm,1);
     OPosition = Position;
     OEmissive = emissive;
}
