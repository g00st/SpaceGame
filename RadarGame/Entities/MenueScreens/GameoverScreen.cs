using App.Engine;
using App.Engine.Template;
using Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.UiSystem;

namespace RadarGame.Entities;




public class GameoverScreen : IEntitie , IDrawObject{

    int[] highscore = new int[5];
    public GameoverScreen()
    { 
        Background = new TexturedRectangle( new Vector2(0,0), DrawSystem.DrawSystem.getViewSize(2), new Texture("resources/Background/SpaceshipOverlayClosed.png"));
        HighscoreScreen = new TexturedRectangle( new Vector2(0,0), DrawSystem.DrawSystem.getViewSize(2)+ new Vector2(-200,0), new Texture("resources/Background/SpaceshipOverlayHigh.png"));
        Gameover = new TexturedRectangle( DrawSystem.DrawSystem.getViewSize(2) / 2 + new Vector2(0, 200),  new Vector2( 800,400), new Texture("resources/Background/GameOver.png"), "hi",  true);
        StartButton = new MenueButton(
            DrawSystem.DrawSystem.getViewSize(2) / 2 + new Vector2(0, 0),
            new Vector2( 500,100),
            "resources/Buttons/Main.png");
        QuitButton = new MenueButton(
            DrawSystem.DrawSystem.getViewSize(2) / 2 + new Vector2(0, -150),
            new Vector2(500, 100),
            "resources/Buttons/Quit.png");
        Name = "GameoverScreen"; 
       
        highscore = UiSystem.ScoreSystem.getHighscore();
    }
    private MenueButton StartButton;
    private MenueButton QuitButton; 
    private TexturedRectangle Background;
    private TexturedRectangle HighscoreScreen;
    private TexturedRectangle Gameover;


    public string Name { get; set; }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
        if (StartButton.Update(mouseState) )
        {
            Console.WriteLine("StartButton Pressed");
            Gamestate.CurrState = Gamestate.State.MainMenu;
           
        }
        if (QuitButton.Update(mouseState))
        {
            Console.WriteLine("QuitButton Pressed");
            EngineWindow.Quit();
        }
    }

    public void onDeleted()
    {
        StartButton.Dispose();
        QuitButton.Dispose();
        Background.Dispose();
    }

    public void Draw(List<View> surface)
    {
        surface[2].Draw(Background);
        surface[2].Draw(HighscoreScreen);
        StartButton.Draw(surface[2]);
        QuitButton.Draw(surface[2]);
        surface[2].Draw(Gameover);
        Vector2 padding = new Vector2(0, 10);
        for (int i = 0; i < highscore.Length; i++)
        {        
            TextRenderer.Write("Highscore " + (i+1) + " :" + highscore[i], new Vector2( 400, surface[2].vsize.Y- 550) - padding, new Vector2(30,30), surface[2], Color4.Aqua, true);
            padding += new Vector2(0,80);
        }
    }
}