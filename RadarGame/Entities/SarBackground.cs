using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities;

public class SarBackground :IEntitie, IDrawObject
{
    public string Name { get; set; }
    private TexturedRectangle background; 
    Shader shader;
    private float time = 0;
    private View target;
    
    
    
    
    public SarBackground()
    {
        Name = "SarBackground";
        target = DrawSystem.DrawSystem.GetView(1);
        var target2  = DrawSystem.DrawSystem.GetView(0);
        
        var size = (int )(Math.Sqrt( target.vsize.X* target.vsize.X + target.vsize.Y * target.vsize.Y)*0.5f);
        Texture texture = new Texture( target.Width,target.Height);
        shader = new Shader("resources/Template/simple_texture.vert", "resources/starshader.frag");
        background = new TexturedRectangle(target2.vpossition  , new Vector2( 1920, 1080 ),texture, shader, true);
    }
  
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
       
        time += (float)args.Time;
    }

    public void onDeleted()
    {
        shader.Dispose();
        background.Dispose();
    }

    public void Draw(List<View> surface)
    {
       /*
        * uniform vec2 u_resolution;
          uniform vec2 u_position;
          uniform float u_zoom;
          uniform float u_rotation;
          uniform float u_time;
        */
       //update shader
       shader.Bind();
      
      // shader.setUniformV2f( "iResolution", new Vector2(target.Width, target.Height));
       shader.setUniformV2f( "iResolution", DrawSystem.DrawSystem.getViewSize());
        shader.setUniformV2f( "u_position", target.vpossition * 0.1f);
         shader.setUniform1v( "u_zoom", 1f + target.vsize.X* 0.000005f);
            shader.setUniform1v( "u_rotation", 0);
            shader.setUniform1v( "u_time", time); 
        //rotate 180
        //extreme scuff aber funktioniert 
        background.drawInfo.Rotation =  target.rotation + (float) Math.PI;  ;
        
        surface[0].Draw(background);
             
       
    }
}