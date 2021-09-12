#version 450

layout(location = 0) in vec4 in_var_POSITION;
layout(location = 1) in vec4 in_var_COLOR;
layout(location = 0) out vec4 out_var_COLOR;

#include "Includes/A.glsl"

void main()
{
    gl_Position = in_var_POSITION;
    out_var_COLOR = INCLUDED_MACRO(in_var_COLOR);
}
