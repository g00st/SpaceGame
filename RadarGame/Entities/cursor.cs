using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities;

public class cursor: IEntitie , IDrawObject
{
    public string Name { get; set; }
    private ColoredRectangle _cursor;
    private Polygon distance;
    private Vector2 start;
    private Vector2 end;
    private SimpleColorShader _shader;
    
    public cursor( string name = "cursor"
    )
    {
        _shader = new SimpleColorShader(Color4.Red);
        distance = Polygon.Circle(Vector2.Zero, 100, 100, _shader, "DebugPolygon", true);
        Name = name;
        _cursor = new ColoredRectangle(new OpenTK.Mathematics.Vector2(0, 0), new OpenTK.Mathematics.Vector2(10, 10), Color4.Aqua, "cursor", true);
    }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    { 
        var x = mouseState.Position;
        x = DrawSystem.DrawSystem.ScreenToWorldcord(x);
       _cursor.drawInfo.Position = x;
       distance.drawInfo.Position = x;
       float xx = 0;
       
           ColisionSystem.getNearest(x, out xx);
           distance.drawInfo.Size = new OpenTK.Mathematics.Vector2(xx);
           start = x;
           end = x + new Vector2(1000, 0);
        /*
           if (ColisionSystem.castRay(start, end) == null)
                                             {
                                                 _shader.setColor(Color4.Blue);
                                             }
                                             else
                                             {
                                                 _shader.setColor(Color4.Green);
                                             }
       */
    }

    public void onDeleted()
    {
        _cursor.Dispose();
    }

  

    public void Draw(List<View> surface)
    { 
       // ColisionSystem.draw(surface[1]);
        surface[1].Draw(distance);
        surface[1].Draw(_cursor);
        _cursor.drawInfo.Position = start;
        surface[1].Draw(_cursor);
        _cursor.drawInfo.Position = end;
        surface[1].Draw(_cursor);
        
       
    }
}
