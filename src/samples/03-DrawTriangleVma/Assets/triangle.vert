#version 450

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec4 inColor;

layout (location = 0) out vec4 color;

out gl_PerVertex 
{
    vec4 gl_Position;   
};

void main() 
{
	gl_Position = vec4(inPosition.xyz, 1.0);
	color = inColor;
}
