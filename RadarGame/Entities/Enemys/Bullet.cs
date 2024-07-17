using App.Engine;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Entities.Weapons;
using RadarGame.Physics;

namespace RadarGame.Entities.Enemys;

public class Bullet : IEntitie, IPhysicsObject, IDrawObject, IcanBeHurt, IColisionObject
{
    private static int id = 0;
    private  bool exploding = false;
    private float bullettimer = 0;
    private float animationtimer = 0;
    private static TextureAtlasRectangle _bulletTexture = new TextureAtlasRectangle(Vector2.Zero, new Vector2(50,50), new Vector2(4, 1),   new Texture("resources/Canon/canonBullet.png" ), "canonBullet.png");
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {
        if (! (colidedObject is PlayerObject))
        {
         _state = Weapons.Bullet.Bulletstate.exploding;
        }
       
    }

    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    public string Name { get; set; }
    
    
    private float _explosionRadius = 500;
    private int _damage = 100;
    private Weapons.Bullet.Bulletstate _state = Weapons.Bullet.Bulletstate.fiering;
    public enum Bulletstate
    {
        fiering,
        exploding
    }
    private int size;

    public Bullet(Vector2 position, Vector2 direction, int size,int damage, int explosionradous, string name = "BulletEnemei" )
    {
        _explosionRadius =  explosionradous;
        this.size = size;
        _damage = damage;
        Name = name + id++;
        Position = position;
        Rotation =   MathF.Atan2(direction.Y, direction.X);
        Vector2 acceleration = direction;
        acceleration.Normalize();
        acceleration *= 1000;
        PhysicsData = new PhysicsDataS
        {
            Velocity = acceleration,
            Mass = 1f,
            Drag = 0.000f,
            Acceleration = acceleration,
            AngularAcceleration = 0f,
            AngularVelocity = 0f
        };
        
        Static = false;
        
        CollisonShape = new List<Vector2>
        {
            new Vector2(-size/2, -size/2),
            new Vector2(size/2, -size/2),
            new Vector2(size/2, size/2),
            new Vector2(-size/2, size/2)
        };
        
    }
    
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
         if (_state == Weapons.Bullet.Bulletstate.exploding)
         {
                AnimatedExposion.newExplosion(Position, _explosionRadius);
               var t =ColisionSystem.getinRadius( Position, _explosionRadius/2);
               foreach (var v in t )
               {
                   if( v is IcanBeHurt)
                   {
                       //apply damage distance based
                      // int damage = (int) (_damage - Math.Clamp( _damage* (Position - v.Position).Length /_explosionRadius,0,_damage)) ;
                      
                       ((IcanBeHurt)v).applyDamage(_damage);

                   }
               }
                EntityManager.RemoveObject(this);
         }else
         {
                bullettimer += (float)args.Time;
                if (bullettimer > 10)
                {
                    EntityManager.RemoveObject(this);
                }
         }
         animationtimer += (float)args.Time;
        
       
    }

    public void onDeleted()
    {

    }

    public void Draw(List<View> surface)
    {
        _bulletTexture.drawInfo.Size = new Vector2(size,size);
        _bulletTexture.drawInfo.Position = Position;
        _bulletTexture.drawInfo.Rotation = Rotation + MathF.PI / 2;
        _bulletTexture.setAtlasIndex( ( int ) (animationtimer*10) %4  ,0);
        surface[1].Draw(_bulletTexture);
    }

    public bool applyDamage(int damage)
    {
        if (bullettimer > 0.05)
        {
            _state = Weapons.Bullet.Bulletstate.exploding;
            return true;
        }
        
        return false;
    }
}
