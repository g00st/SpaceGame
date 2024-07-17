#version 400 core
out vec4 fragColor;
in vec2 textCords;
uniform float random;
uniform sampler2D tex;
uniform vec2[10] points;
uniform vec2 offset;
const int N = 10;
//https://iquilezles.org/articles/distfunctions2d/ dist function for polygon
float sdPolygon( in vec2[N] v, in vec2 p )
{
  float d = dot(p-v[0],p-v[0]);
  float s = 1.0;
  for( int i=0, j=N-1; i<N; j=i, i++ )
  {
    vec2 e = v[j] - v[i];
    vec2 w =    p - v[i];
    vec2 b = w - e*clamp( dot(w,e)/dot(e,e), 0.0, 1.0 );
    d = min( d, dot(b,b) );
    bvec3 c = bvec3(p.y>=v[i].y,p.y<v[j].y,e.x*w.y>e.y*w.x);
    if( all(c) || all(not(c)) ) s*=-1.0;
  }
  return s*sqrt(d);
}

void main()
{
  //  fragColor =   vec4(textCords, 0.0, 1.0);
  //distance of center of the screen
  float distance = length(textCords - vec2(0.5, 0.5));
  vec3 color = texture(tex, textCords*0.2 + random*0.5).rgb;

  distance =sdPolygon(points,( textCords + vec2(-0.5) )*0.900);
  if (distance > 0.0)
  {
    distance = 0.001;
  }
  distance = abs (distance);
   
  fragColor =  vec4((color*distance)*3 , 1.0) ;
}