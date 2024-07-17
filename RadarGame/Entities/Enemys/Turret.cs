using System.Drawing;
using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.others;
using RadarGame.Physics;

namespace RadarGame.Entities.Enemys;

public class Turret : IEnemie, IDrawObject, IColisionObject, IcanBeHurt
{
    private static int id = 0;
    private bool mouseHover = false;
    private static Polygon distance = Polygon.Circle(Vector2.Zero, 100, 100, new SimpleColorShader( Color4.Blue ),"indicatorTurret", true);
   private static   TexturedRectangle texture = new TexturedRectangle(new Vector2(0,0), new Vector2(100,100), new Texture("resources/Enemies/Turret.png"), "Turret",true);
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {
        if (colidedObject is Bullet)
        {
            return;
            
        }
        applyDamage( 20);
    }
   

    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    public string Name { get; set; }
    private PlayerObject target;
    private float fireRate = 2f;
    private int health = 30;
    private int maxHealth = 30;
    private float fireTimer = 0;
    float range = 3000;
    private Vector2 size = new Vector2(300,200);
    
    public Turret(Vector2 position, EnemyManager entityManager)
    {
        Position = position;
        EnemyManager = entityManager;
        Name = "Turret" + id++;
        PlayerObject target = (PlayerObject)EntityManager.GetObject("Player");
        PhysicsData = new PhysicsDataS
        {
            Velocity = Vector2.Zero,
            Mass = 1f,
            Drag = 0.000f,
            Acceleration = Vector2.Zero,
            AngularAcceleration = 0f,
            AngularVelocity = 0.0f
        };
        
        Static = false;
        var halfsize = size/2;
        CollisonShape = new List<Vector2>
        {
            new Vector2(-halfsize.X, -halfsize.Y),
            new Vector2(halfsize.X, -halfsize.Y),
            new Vector2(halfsize.X, halfsize.Y),
            new Vector2(-halfsize.X, halfsize.Y)
        };
    }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        if (target == null)
        {
            target = (PlayerObject)EntityManager.GetObject("Player");
            return;
        }
        
        Vector2 direction = target.Position - Position;
        float distance = direction.Length;
        if (distance < range)
        {
            Rotation = MathF.Atan2(direction.Y, direction.X);
            fireTimer += (float)args.Time;
            if (fireTimer > fireRate)
            {
                fireTimer = 0;
                direction.Normalize();
                
                EntityManager.AddObject(new Bullet(Position + direction*size.X, direction,50, 10, 100));
            }
        }
        
        var mouse = DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position);
        if ( (mouse - Position).Length < size.X*2)
        {
            mouseHover = true;
        }
        else
        {
            mouseHover = false;
        }
    }

    public void onDeleted()
    {

    }

    public EnemyManager EnemyManager { get; set; }
    public bool IsDead()
    {
         if (health <= 0)
         {
             return true;
         }
            return false;
    }

    public void Draw(List<View> surface)
    {
        PercentageBar.DrawBar(  surface[1], Position + new Vector2(0,100),  new Vector2(200,50), health/(float)maxHealth,  Color4.DarkRed, true );
        if (mouseHover){
            distance.drawInfo.Position = Position;
            distance.drawInfo.Size = new Vector2(range);
            surface[1].Draw(distance);
        TextRenderer.Write(health + "/" + maxHealth,Position + new Vector2(0, 150), new Vector2( 50,50), surface[1], Color4.White, true);
        }
        texture.drawInfo.Position = Position;
        texture.drawInfo.Size = size;
        texture.drawInfo.Rotation = Rotation;
        surface[1].Draw(texture);
    }

    public bool applyDamage(int damage)
    {
        health -= damage;
        health = Math.Clamp( health, 0, maxHealth);
        if (IsDead( ))
        {
            AnimatedExposion.newExplosion(Position + new Vector2( 20,10), 200);
            AnimatedExposion.newExplosion(Position + new Vector2( -50,- 200), 100);
            AnimatedExposion.newExplosion(Position  + new Vector2( 60, 100), 300);
            AnimatedExposion.newExplosion(Position +  new Vector2( 20f, -10f), 100);
            return true;
        }
        return false;
    }
}