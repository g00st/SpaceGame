using App.Engine;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.Entities;

public class AnimatedExposion : IEntitie, IDrawObject
{
    private static AnimatedExposion? _instance;
    private static float _explosionTime = 1;
    private int frames = 8*3;
    private  static List<Vector4> Explosions = new List<Vector4>();
    private static TextureAtlasRectangle _explosionTexture = new TextureAtlasRectangle(
        Vector2.Zero, 
        Vector2.Zero, 
        new Vector2(8, 3),
        new Texture("resources/Explosion/explosion.png"), "explosion");
    
    public static void  newExplosion(Vector2 position, float radius)
    {
      if (_instance == null)
      {
          _instance = new AnimatedExposion();
            EntityManager.AddObject(_instance);
      }
      Explosions.Add(  new Vector4( position.X, position.Y, radius, 0));
    }

   
    public string Name { get; set; } = "AnimatedExposion";

    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
       
        for (int i = 0; i < Explosions.Count; i++)
        {
            Vector4 e = Explosions[i];
         //   Console.WriteLine(e.W);
            e.W += (float)args.Time;
            if (e.W > _explosionTime)
            {
                Explosions.RemoveAt(i);
                i--;
            }
            else
            {
                Explosions[i] = e;
            }
        }
    }

    public void onDeleted()
    {
        Console.WriteLine("Deleted ------------------------------------------------------------");
         _instance = null;
    }

    public void Draw(List<View> surface)
    {
        foreach (var e in Explosions)
        {
            int currentFrame = (int)(e.W / _explosionTime * frames);
           _explosionTexture.setAtlasIndex(  currentFrame %8, 8-currentFrame/8);
            _explosionTexture.drawInfo.Position = new Vector2(e.X, e.Y);
            _explosionTexture.drawInfo.Size = new Vector2(e.Z, e.Z);
            surface[1].Draw(_explosionTexture);
        }
    }
}