#version 400 core

uniform vec4 u_Color;
in vec2 dist;
out vec4 fragColor;
in vec2 vertexPosition;
void main()
{

    vec2 center = vec2(0.5, 0.5);
    float brighness = distance(center, vertexPosition);
    vec4 tf  = u_Color - brighness*2;
    tf = clamp(tf, 0.0, 1.0);
    
    float normalizedDist = length(dist) * 0.001;
    float falloff = max(0.0, 1.0 - sqrt(normalizedDist)-brighness*2);
    fragColor = vec4(tf.xyz, falloff);
}