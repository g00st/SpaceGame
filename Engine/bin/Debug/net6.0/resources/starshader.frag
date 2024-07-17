#version 330 core

//stolen from https://www.shadertoy.com/view/tst3WS  


in vec2 textCords;
out vec4 FragColor;

uniform float u_zoom;
uniform vec2 u_position;  // Position of the player or the world
uniform vec2 iResolution; // Screen resolution


vec4 hash42(vec2 p)
{
    vec4 p4 = fract(vec4(p.xyxy) * vec4(.1031, .1030, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx) - 0.5;
}

vec2 hash22(vec2 p)
{
    vec3 p3 = fract(vec3(p.xyx) * vec3(443.897, 441.423, 437.195));
    p3 += dot(p3, p3.yzx + 19.19);
    return -1.0 + 2.0 * fract((p3.xx + p3.yz) * p3.zy);
}

float Gradient2D(in vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(mix(dot(hash22(i + vec2(0.0, 0.0)), f - vec2(0.0, 0.0)),
                   dot(hash22(i + vec2(1.0, 0.0)), f - vec2(1.0, 0.0)), u.x),
               mix(dot(hash22(i + vec2(0.0, 1.0)), f - vec2(0.0, 1.0)),
                   dot(hash22(i + vec2(1.0, 1.0)), f - vec2(1.0, 1.0)), u.x), u.y);
}

const vec3 cold = vec3(255.0, 244.0, 189.0) / 255.0;
const vec3 hot = vec3(181.0, 236.0, 255.0) / 255.0;

vec3 StarFieldLayer(vec2 p, float du, float count, float brightness, float size)
{
    vec2 pi;
    du *= count;
    p *= count;
    pi = floor(p);
    p = fract(p) - 0.5;

    vec4 h = hash42(pi);

    float s = brightness * (0.7 + 0.6 * h.z) * smoothstep(0.8 * du, -0.2 * du, length(p + 0.9 * h.xy) - size * du);

    return s * mix(mix(vec3(1.0), cold, min(1.0, -2.0 * h.w)), hot, max(0.0, 2.0 * h.w));
}

vec3 StarField(vec2 p, float du)
{
    vec3 c;
    c = StarFieldLayer(p, du, 25.0, 0.18, 0.5);
    c += StarFieldLayer(p, du, 15.0, 0.25, 0.5);
    c += StarFieldLayer(p, du, 12.0, 0.50, 0.5);
    c += StarFieldLayer(p, du, 5.0, 1.00, 0.5);
    c += StarFieldLayer(p, du, 3.0, 1.00, 0.9);

    float s = 3.5 * (max(0.2, Gradient2D(2.0 * p * vec2(1.2, 1.9))) - 0.2) / (1.0 - 0.2);
    c += s * StarFieldLayer(p, du, 160.0, 0.10, 0.5);
    c += s * StarFieldLayer(p, du, 80.0, 0.15, 0.5);
    c += s * StarFieldLayer(p, du, 40.0, 0.25, 0.5);
    c += s * StarFieldLayer(p, du, 30.0, 0.50, 0.5);
    c += s * StarFieldLayer(p, du, 20.0, 1.00, 0.5);
    c += s * StarFieldLayer(p, du, 10.0, 1.00, 0.9);

    c *= 1.3;

    float f = 1.0 / sqrt(660.0 * du);

    return f * min(c, 1.0);
}

void main()
{
    float du = 1.0 / iResolution.y;

 
    vec2 uv = textCords * iResolution.xy - 0.5 * iResolution.xy;

   
    uv *= u_zoom;

    // Invert  movements
    uv.x = -uv.x;
    uv.y = -uv.y;

    // Translate to  u_position 
    uv += u_position;

    // Convert to wierd space 
    uv *= du;

    vec3 starColor = StarField(uv, du);
    starColor = max(starColor, vec3(0,0,0.1));
    FragColor = vec4(starColor, 1.0);
}


