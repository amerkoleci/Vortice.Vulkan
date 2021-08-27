#version 450

layout(location = 0) in vec4 in_var_POSITION;
layout(location = 1) in vec4 in_var_COLOR;
layout(location = 0) out vec4 out_var_COLOR;

void main()
{
    gl_Position = in_var_POSITION;
    out_var_ThisIsAnError = in_var_COLOR;
}
