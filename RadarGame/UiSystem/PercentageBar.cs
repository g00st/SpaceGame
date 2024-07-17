using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;

namespace RadarGame.others;

public class PercentageBar
{
    static private TexturedRectangle barFrame = new TexturedRectangle(new Texture("resources/PercentageBar/blood_red_bar_template.png") ,false);
    static private ColoredRectangle bar = new ColoredRectangle( new Vector2(0,0), new Vector2(0,0), new Color4(0,0,0,0.5f), "bar", false);
    public static void  DrawBar(View surface,Vector2 position,  Vector2 size, float percentage, Color4 color, bool centered = false)
    {
        percentage = Math.Clamp(percentage, 0, 1);
        if (centered)
        {
            position = new Vector2(position.X - size.X / 2, position.Y - size.Y / 2);
        }
        barFrame.drawInfo.Position = position;
        barFrame.drawInfo.Size = size;
        bar.drawInfo.Position  = barFrame.drawInfo.Position;
        bar.drawInfo.Size = new Vector2(size.X * percentage, barFrame.drawInfo.Size.Y);
       // bar.drawInfo.Size = new Vector2(bar.drawInfo.Size.X *0.8f, barFrame.drawInfo.Size.Y * 0.8f);
        
         bar.drawInfo.mesh.Shader.Bind();
         bar.drawInfo.mesh.Shader.setUniform4v( "u_Color", color.R, color.G, color.B, color.A);
         surface.Draw(bar);
         surface.Draw(barFrame);
         
    }
}