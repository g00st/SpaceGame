using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.Entities;

public class Pauser: IEntitie

{
    public string Name { get; set; } = "Pauser";
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Gamestate.CurrState = Gamestate.State.Pause;
        }
        if (keyboardState.IsKeyDown(Keys.G))
        {
            Console.WriteLine("Game Over");
            Gamestate.CurrState = Gamestate.State.GameOver;
        }
    }

    public void onDeleted()
    {
       
    }
}