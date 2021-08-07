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
uniform vec3 EyeDir;
uniform bool RenderTexCoords;
uniform vec2 ViewPortSize;

float znear = 0.1; 
float zfar  = 2000.0; 

float LinearizeDepth(float depth) 
{
	float z_n = 2.0 * depth - 1.0;
    return 2.0 * znear * zfar / (zfar + znear - z_n * (zfar - znear));
}

vec3 AnglesFromVectors(vec3 forward)
{
	const vec3 up = vec3(0,1,0);
	const float pi = 3.141598f;
    // Yaw is the bearing of the forward vector's shadow in the xy plane.
    float yaw = atan(forward.z, forward.x);

    // Pitch is the altitude of the forward vector off the xy plane, toward the down direction.
    float pitch = -asin(forward.y);

    // Find the vector in the xy plane 90 degrees to the right of our bearing.
    float planeRightX = sin(yaw);
    float planeRightZ = -cos(yaw);
	
    // Roll is the rightward lean of our up vector, computed here using a dot product.
    float roll = asin(up.x*planeRightX + up.z*planeRightZ);
    // If we're twisted upside-down, return a roll in the range +-(pi/2, pi)
    if(up.y < 0)
        roll = sign(roll) * pi - roll;
	vec3 angles;
    // Convert radians to degrees.
    angles.x   =   yaw * 180 / pi;
    angles.y = pitch * 180 / pi;
    angles.z  =  roll * 180 / pi;

	return angles;
}

vec4 CalcSkyColor(vec2 texCoord,vec3 eyeDir){
	vec4 horizonColor = vec4(0,0,1,1);
	vec4 zenitColor = vec4(0.5f,0.75f,1,1);
	vec3 angles = AnglesFromVectors(eyeDir);
	float skyCoord = texCoord.y + eyeDir.y;
	if(skyCoord > 1)
	{
		float diff = skyCoord -1;
		skyCoord = skyCoord -diff;
	}
	if(skyCoord < 0)
	  return horizonColor;
	return mix(horizonColor,zenitColor,skyCoord);
}


float CalcShadow(sampler2D tex){
	float base = 1;
	float tox = base / 512;
	float toy = base / 512;
	int distance = 1;
	float shadowFinal=0;
	for(int i=-distance;i<=distance;i++){
		shadowFinal += textureLod(tex, TexCoord + vec2(tox * i       ,toy * i),0).w;
		shadowFinal += textureLod(tex, TexCoord + vec2(tox * (i * -1),toy * i),0).w;
		shadowFinal += textureLod(tex, TexCoord + vec2(0             ,toy * i),0).w;
		shadowFinal += textureLod(tex, TexCoord + vec2(tox * i       ,0      ),0).w;
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
	
	float depth = textureLod(Depth, TexCoord,0).r;
	//depth = LinearizeDepth(depth);
	if(depth > 0.9999f)
	{
	  FragColor = CalcSkyColor(TexCoord,EyeDir);
	  return;	
	}
	float shadow = 1 - CalcShadow(Emissive);
	vec4 lightColor = vec4(shadow,shadow,shadow,1) + Ambient;
	vec3 diffuse = textureLod(Diffuse, TexCoord,0).rgb;
	FragColor = vec4(diffuse,1) * lightColor;
	//FragColor = vec4(1,0,0,1);
}
