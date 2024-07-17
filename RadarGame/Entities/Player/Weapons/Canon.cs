using System.ComponentModel;
using App.Engine;
using App.Engine.Template;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities.Weapons;

public class Canon : Weapon
{
    private IPhysicsObject trackingObject;
    private Spaceship spaceship; 
    private float rotation = 0;
    private Vector2 direction;
    
    
    public Canon()
    {
        Damage = 100;
        cooldown = 0.5f;
        energyCost = 20;
        Name = "Canon";
        Description = "Canon";
        state = Weponstate.ready;
        icon = new TexturedRectangle(Vector2.Zero, new Vector2(50, 50), new Texture("resources/Canon/canonBullet.png"), "Canon", true);
    }

    public override void fire()
    {
        base.fire();
        if (state == Weponstate.fiering)
        {
            Console.WriteLine("fire Canon");
            ((PlayerObject)  trackingObject).spaceship.shoot(); 
            state = Weponstate.coolingdown;
            Console.WriteLine( spaceship.canonRotation);
          
            //rotate offset
            direction.Normalize();
           var  offset =  direction * 100;
            
            
            Bullet b = new Bullet(trackingObject.Position + offset ,  direction , this);
            EntityManager.AddObject(b);
            Vector2 position =trackingObject.Position;
            Console.WriteLine("Position " + position);
             
        }
    }

    public override void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
        base.Update(args, keyboardState, mouseState); 
        if (trackingObject == null)
        {
            trackingObject= (IPhysicsObject) EntityManager.GetObject("Player");
            spaceship = ((PlayerObject) trackingObject).spaceship;
        }else
        {
           var s = DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position, 1);
                  direction = s - trackingObject.Position;
            
        }
       
        
        
    }
    
    
    
}

public class Bullet : IEntitie, IPhysicsObject, IDrawObject, IcanBeHurt, IColisionObject, Ifriendly
{
    private static Random _random = new Random();
    
    private float bullettimer = 0;
    private float animationtimer = 0;
    private static TextureAtlasRectangle _bulletTexture = new TextureAtlasRectangle(Vector2.Zero, new Vector2(50,50), new Vector2(4, 1),   new Texture("resources/Canon/canonBullet.png" ), "canonBullet.png");
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {
        if (! (colidedObject is PlayerObject))
        {
         _state = Bulletstate.exploding;
        }
       
    }

    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    public string Name { get; set; }
    
    
    private float _explosionRadius = 500;
    private int _damage = 100;
    private Bulletstate _state = Bulletstate.fiering;
    public enum Bulletstate
    {
       
        fiering,
        exploding
    }
    private Canon Parent;

    public Bullet(Vector2 position, Vector2 direction,Canon Parent, string name = "Bullet")
    {
        this.Parent = Parent;
        Name = name + _random.Next(0, 10000);
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
            new Vector2(-10, -15),
            new Vector2(10, -15),
            new Vector2(10, 15),
            new Vector2(-10, 15)
        };
    }
    
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
         if (_state == Bulletstate.exploding)
         {
                AnimatedExposion.newExplosion(Position, _explosionRadius);
               var t =ColisionSystem.getinRadius( Position, _explosionRadius/2);
               foreach (var v in t )
               {
                   if( v is IcanBeHurt)
                   {
                       //apply damage distance based
                       int damage = (int) (_damage - Math.Clamp( _damage* (Position - v.Position).Length /_explosionRadius,0,_damage)) ;
                       if( ((IcanBeHurt) v).applyDamage(damage))
                       { 
                          Parent.setScore();
                           Parent.manager.addEnergy(10);
                       }
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
       // throw new NotImplementedException();
    }

    public void Draw(List<View> surface)
    {
        _bulletTexture.drawInfo.Position = Position;
        _bulletTexture.drawInfo.Rotation = Rotation + MathF.PI / 2;
        _bulletTexture.setAtlasIndex( ( int ) (animationtimer*10) %4  ,0);
        surface[1].Draw(_bulletTexture);
    }

    public bool applyDamage(int damage)
    {
        if (bullettimer > 1)
        {
            _state = Bulletstate.exploding;
            return true;
        }
        
        return false;
    }
}
