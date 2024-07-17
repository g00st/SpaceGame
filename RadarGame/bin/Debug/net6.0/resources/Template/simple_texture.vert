#version 400 core

layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec2 vc;
uniform mat4 u_MVP;
out vec2 textCords;
void main()
{
    textCords = vc;
    gl_Position =  u_MVP* vec4(inPosition, 1.0,1.0);
}