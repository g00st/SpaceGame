#version 400 core

out vec4 fragColor;
uniform sampler2D tex;
uniform vec2 index;       
uniform vec2 subimages; 
uniform vec4 color;       
in vec2 textCords;       


void main()
{
  
    vec2 tileSize = vec2(1.0 / subimages.x, 1.0 / subimages.y);
    vec2 offset = index * tileSize;
    vec2 actualTexCoords = offset + textCords * tileSize;
    fragColor = vec4 (color.rgb,  texture(tex, actualTexCoords).a );
}
