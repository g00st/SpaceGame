#version 400 core

layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec2 instancePosition; // Using vec3 for world coordinates


uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;
out vec2 dist;
out vec2 vertexPosition;

void main()
{
    vertexPosition = inPosition;
    float scale = length(instancePosition) * 0.01; 
    dist = instancePosition;
    vec4 worldPosition = u_Model * vec4((inPosition-0.5) * scale, 0.0, 1.0); // Scale the particle size
  //  vec4 worldPosition = u_Model * vec4(inPosition-0.5, 0.0, 1.0)  ;
    worldPosition.xy += instancePosition;
    vec4 cameraPosition = u_View * worldPosition;
    gl_Position = u_Projection * cameraPosition;
    //gl_Position = vec4((inPosition*0.1) +(gl_InstanceID *0.1), 0.0, 1.0) ;
}


