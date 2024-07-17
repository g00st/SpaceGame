#version 420  core

layout(location = 0) in vec2 inPosition;
uniform mat4 u_MVP;
out vec2 VC;
void main()
{
    VC = inPosition;
    gl_Position =  u_MVP* vec4(inPosition,1.0, 1.0);
}