#version 400 core


out vec4 fragColor;
uniform sampler2D tex;
in vec2 textCords;
void main()
{
    fragColor =   texture(tex,textCords);
}