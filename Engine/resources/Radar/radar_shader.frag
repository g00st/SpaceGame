#version 420 core

 layout(binding = 0) uniform sampler2D lastFrame; 
out vec4 fragColor;
in vec2 VC; // Vertex coordinates in screen space
uniform vec2 u_TextureSize; // Size of the texture we are rendering to
uniform float u_RadaScrenrange; // Range of the radar screen in meters
uniform float u_RadarRange; // Range of the radar in meters
uniform float u_Distance; // Distance from antenne of radar point intersection
uniform float u_AntennaRotation; // Rotation of the radar antenne in rads
const float PI = 3.14159265359;
float angleBetween(float angle, vec2 vec) {
    float dotProduct = cos(angle) * vec.x + sin(angle) * vec.y;
    return acos(dotProduct);
}
void main()
{
  
    
    vec2 ofccenter = VC - vec2(0.5, 0.5);
    
    vec4 newColor = vec4(0.0, 0.0, 0.0, 1.0);
    // Calculate the distance from the center of the texture
    float distancef = length(ofccenter);
    distancef *=  u_RadaScrenrange;
    //Clucualte distance in meters
    
   
    //newColor= vec4(0,0, 0, 1.0);
    float pointdistance = abs(u_Distance  - distancef);
    if(   u_Distance  > distancef  ){
        
        //newColor = vec4(1.0, 0.0, 0.0, 1.0);
    }
    //LINE IN RARDAR
    float roationVC = atan(ofccenter.x , ofccenter.y);
    if (roationVC < 0.0) roationVC += 2.0 * PI;
    float adjustedAntennaRotation = mod(u_AntennaRotation, 2.0 * PI);

    float f = length (ofccenter);
   
    float mult = 0.009;
    if(distancef < u_RadarRange ){
    if (abs(adjustedAntennaRotation - roationVC) < mult / f ||
        abs(adjustedAntennaRotation - roationVC + 2.0 * PI) <mult/ f ||
        abs(adjustedAntennaRotation - roationVC - 2.0 * PI) <mult / f ) {
        newColor.g = 0.02 +1 * 1/ pointdistance *4 ;
        
        
    }}
   
    if (mod (distancef,  1000)  < 50) {
        newColor.b = mod (distancef,  1000)/100;
    }
   
    if (length(ofccenter) > 0.5) { newColor.a = 0.0; }

    
    newColor.g = max(newColor.g + texture(lastFrame, VC).g* 0.985, 0.13);
    newColor.b += newColor.g/2;
    
  
    
    
    
    fragColor = newColor;
}