using App.Engine;
using App.Engine.Template;
using Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.UiSystem;

namespace RadarGame.Entities;

public class PauseScreen : IEntitie , IDrawObject
{
    private MenueButton StartButton;
    private MenueButton ResumeButton;
    private MenueButton QuitButton;
    private TexturedRectangle Background;


    public PauseScreen( )
    {
        StartButton = new MenueButton( 
            DrawSystem.DrawSystem.getViewSize(2)/2 ,
            new Vector2( 500,100),
            "resources/Buttons/Main.png"
        );
        
        ResumeButton = new MenueButton( 
            DrawSystem.DrawSystem.getViewSize(2)/2  + new Vector2(0, -150),
            new Vector2( 500,100),
            "resources/Buttons/Resume.png"
        );
        QuitButton = new MenueButton(
            DrawSystem.DrawSystem.getViewSize(2) / 2 + new Vector2(0, -150*2),
            new Vector2(500, 100),
            "resources/Buttons/Quit.png"
        );
        Name = "PauseScreen"; 
        Background = new TexturedRectangle( new Vector2(0,0), DrawSystem.DrawSystem.getViewSize(2), new Texture("resources/Background/SpaceshipOverlayClosed.png"));
        
        
    }
    
    
    public void Draw(List<View> surface)
    {
        surface[2].Draw(Background);
        StartButton.Draw( surface[2]);
        ResumeButton.Draw(surface[2]);
        QuitButton.Draw(surface[2]);
    } 

    public string Name { get; set; }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
        if ( StartButton.Update(mouseState))
        {
            Console.WriteLine("StartButton Pressed");
            Gamestate.CurrState = Gamestate.State.MainMenu;
           
        }
        if ( ResumeButton.Update( mouseState))
        {
            Console.WriteLine("ResumeButton Pressed");
            Gamestate.CurrState = Gamestate.State.Game;
           
        }

        if (QuitButton.Update( mouseState))
        {
            Console.WriteLine("QuitButton Pressed");
            EngineWindow.Quit();
        }
    }

    public void onDeleted()
    {
        QuitButton.Dispose();
        StartButton.Dispose();
        ResumeButton.Dispose();
        Background.Dispose();
        
    }
}
