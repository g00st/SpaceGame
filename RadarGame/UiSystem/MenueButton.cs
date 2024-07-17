
using App.Engine;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.UiSystem;

public class MenueButton : IDisposable
{
    private TextureAtlasRectangle button;
    private OpenTK.Mathematics.Vector2 position;
    private OpenTK.Mathematics.Vector2 size;
    private bool isHover = false;
    
    public MenueButton(Vector2 position, Vector2 size, string buttonTexture)
    {
        this.button = new TextureAtlasRectangle( position,size, new Vector2( 1,2) , new Texture(buttonTexture), buttonTexture);
        this.position = position;
        this.size = size;
    }
    
    public bool Update(MouseState mouseState)
    {
        var mousePosition = DrawSystem.DrawSystem.ScreenToWorldcord(new Vector2(mouseState.X, mouseState.Y), 2);
        if (mousePosition.X > position.X- size.X/2 && mousePosition.X < position.X + size.X/2 && mousePosition.Y > position.Y- size.Y/2 && mousePosition.Y < position.Y + size.Y/2)
        {
            Console.WriteLine("Hover");
            isHover = true;
            if (mouseState.IsButtonReleased(MouseButton.Left))
            {
                return true;
            }

            return false;
        }
        isHover = false;
        return false;
    }
    
    
    public void Draw(View surface)
    {
        if (isHover)
        {
            Console.WriteLine("Hover");
            button.setAtlasIndex( 1,1);
        }
        else
        {
            button.setAtlasIndex( 1,2);
        }
        button.drawInfo.Position = position;
        button.drawInfo.Size = size;
        surface.Draw(button);
    }

    public void Dispose()
    {
        button.Dispose();
    }
}