#version 400 core

out vec4 fragColor;
uniform sampler2D tex;
uniform vec2 index;       // Index of the subimage (tile) within the atlas
uniform vec2 subimages;   // Number of subimages in the atlas (columns, rows)
uniform bool aplha;       // If the texture has an alpha channel
uniform vec3 color;       // Color to apply to the texture
in vec2 textCords;        // Input texture coordinates within the tile [0, 1]


void main()
{
    // Calculate the size of each subimage in normalized coordinates
    vec2 tileSize = vec2(1.0 / subimages.x, 1.0 / subimages.y);

    // Calculate the offset for the given tile index
    vec2 offset = index * tileSize;

    // Calculate the actual texture coordinates within the atlas
    vec2 actualTexCoords = offset + textCords * tileSize;

    // Sample the texture using the calculated coordinates
    fragColor = texture(tex, actualTexCoords) ;
}
