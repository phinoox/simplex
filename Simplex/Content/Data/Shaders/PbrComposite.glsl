-- VERTEX
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

-- FRAGMENT
#version 330
smooth in vec2 TexCoord;
uniform bool Debug;

out vec4 FragColor;

uniform sampler2D Diffuse;
uniform sampler2D Normal;
uniform sampler2D Position;
uniform sampler2D Emissive;
uniform sampler2D Depth;
uniform vec4 Ambient;

uniform bool RenderTexCoords;
uniform vec2 ViewPortSize;

float znear = 0.1; 
float zfar  = 2000.0; 

float LinearizeDepth(float depth) 
{
	//return depth;
	//return  znear + (zfar-znear)*depth;
	//return znear * zfar / (zfar + depth * (znear - zfar));
    //float z = depth * 2.0 - 1.0; // back to NDC 
    //return (2.0 * znear * zfar) / (zfar + znear - z * (zfar - znear));
	float z_n = 2.0 * depth - 1.0;
    return 2.0 * znear * zfar / (zfar + znear - z_n * (zfar - znear));
}


float CalcShadow(){
	float base = 1;
	float tox = base / 512;
	float toy = base / 512;
	int distance = 3;
	float shadowFinal=0;
	for(int i=-distance;i<=distance;i++){
		shadowFinal += textureLod(Normal, TexCoord + vec2(tox * i       ,toy * i),0).w;
		shadowFinal += textureLod(Normal, TexCoord + vec2(tox * (i * -1),toy * i),0).w;
		shadowFinal += textureLod(Normal, TexCoord + vec2(0             ,toy * i),0).w;
		shadowFinal += textureLod(Normal, TexCoord + vec2(tox * i       ,0      ),0).w;
	}
	float div = 4 * ((distance*2)+1);

	return shadowFinal /div;
}

void main()
{
	if(Debug){
		
		//getting diffuse color
		if(TexCoord.x < 0.5f && TexCoord.y < 0.5f ){
			vec2 halfTexCoord = TexCoord * 2.0f;
			FragColor = textureLod(Diffuse, halfTexCoord,0);
		}//getting normal
		else if(TexCoord.x > 0.5f && TexCoord.y < 0.5f ){
			vec2 halfTexCoord = vec2((TexCoord.x-0.5f)*2.0f,TexCoord.y*2.0f);
			FragColor = vec4(textureLod(Normal, halfTexCoord,0).xyz,1);
		}
		else if(TexCoord.x < 0.5f && TexCoord.y > 0.5f ){
			vec2 halfTexCoord = vec2(TexCoord.x*2.0f,(TexCoord.y-0.5f)*2.0f);
			//FragColor = vec4(textureLod(Position, halfTexCoord,0).xyz,1);
			float depth = textureLod(Emissive, halfTexCoord,0).a;
			FragColor = vec4(textureLod(Emissive, halfTexCoord,0).rrr,1);
		}
		else if(TexCoord.x > 0.5f && TexCoord.y > 0.5f ){
			vec2 halfTexCoord = vec2((TexCoord.x-0.5f)*2.0f,(TexCoord.y-0.5f)*2.0f);
			float depth = textureLod(Depth, halfTexCoord,0).r;
			depth = LinearizeDepth(depth) * 0.06;
			//FragColor = vec4(textureLod(Depth, halfTexCoord,0).rrr,1);
			FragColor = vec4(depth,depth,depth,1);
		}
		else{
			FragColor = vec4(0,0,0,1);
		}
		return;
	}
	float shadow = 1 - CalcShadow();
	vec4 lightColor = vec4(shadow,shadow,shadow,1) + Ambient;
	vec3 diffuse = textureLod(Diffuse, TexCoord,0).rgb;
	FragColor = vec4(diffuse,1) * lightColor;
	//FragColor = vec4(1,0,0,1);
}
