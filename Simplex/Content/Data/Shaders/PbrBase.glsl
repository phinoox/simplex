-- Vertex
#version 330

in vec3 InPosition;
in vec4 InColor;
in vec2 InTexCoord;
in vec3 InNormal;
in vec4 InTangent;

smooth out vec4 Color;
smooth out vec2 TexCoord;

smooth out vec4 Position;
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
	FragPos = Model * vec4(InPosition,1);
	model = Model;
	Color = InColor;
	TexCoord = InTexCoord;
	
    mat3 normalMatrix = transpose(inverse(mat3(model)));
	Tangent = normalize(normalMatrix * InTangent.xyz);
	Normal = normalize(normalMatrix * InNormal);
    Bitangent = cross(Normal,Tangent)*InTangent.w;
    
	//TBN = mat3(tangent,bitangent,Normal);
	Position = gl_Position;
}

-- Fragment
#version 330
smooth in vec4 Color;
smooth in vec2 TexCoord;
in vec3 Normal;
smooth in vec4 Position;
in vec4 FragPos;

//in mat3 TBN;
in mat4 model;
in vec3 Tangent;
in vec3 Bitangent;

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

void main()
{
    
	mat3 TBN = mat3(normalize(Tangent),normalize( Bitangent),normalize(Normal));
    vec3 normalTex = normalize(texture(NormalTex,TexCoord).rgb *2.0-1);
    normalTex = normalize(TBN * normalTex);
    //vec3 lightDir = normalize((rotationY(int(Time * 0.01f) % 360) * vec4(LightDir,0)).xyz);
    vec3 norm = normalTex;//normalize(Normal + normalTex) * NormalFactor;
    //norm = vec3(norm.x,norm.y,norm.z * -1);
    vec4 color;
    vec3 lightDir = normalize((model * vec4(LightDir,1)).xyz);
    float diff = max(dot(norm, -LightDir), 0.0);
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
     OPosition = FragPos;
     OEmissive = emissive;
}
