using App.Engine;
using App.Engine.Template;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.others;
using RadarGame.Physics;


namespace RadarGame.Entities;

public class PlayerObject : IEntitie, IPhysicsObject, IDrawObject , IColisionObject , IcanBeHurt
{
    private int _health = 100;
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {
        if (colidedObject is Ifriendly)
        {
            return;
        }
        var differencevector = colidedObject.Position - Position;
        
        PhysicsSystem.ApplyForce(this, -differencevector * 100);

       this.applyDamage( (int) (this.PhysicsData.Velocity.Length * 0.01f));
       if (colidedObject is IcanBeHurt)
       {
           ((IcanBeHurt)colidedObject).applyDamage( (int) (this.PhysicsData.Velocity.Length * 0.01f));
       }
       
      //  Console.WriteLine("Colision with " + ((IEntitie)colidedObject).Name);
    }
    private float deathTimer = 2f;
    private bool alive = true;
    
    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    public string Name { get; set; }
    
    public TexturedRectangle Spaceship { get; set; }
    private TextureAtlasRectangle Exhaust;
   
    private Vector2 lastPosition;
    private float lastRotation;
    private Random random = new Random();
    private float timer = 0;
    public Spaceship spaceship;
    private Camera camera;
    
    public PlayerObject(Vector2 position, float rotation, string name = "Player")
    {
        Static = false;
        Name = name;
        Position = position;
        Rotation = rotation; 
        PhysicsData = new PhysicsDataS
        {
            Velocity = new Vector2(0, 0),
            Mass = 5f,
            Drag = 0.02f,
            Acceleration = new Vector2(0f, 0f),
            AngularAcceleration = 0f,
            AngularVelocity = 0f
        };
        CollisonShape = new List<Vector2>
        {
            new Vector2(-65, -150),
            new Vector2(65, -150),
            new Vector2(65, 150),
            new Vector2(-65, 150)
        };
        spaceship = new Spaceship();
         camera = new Camera(this);
        EntityManager.AddObject( new Weaponmanager() );
        EntityManager.AddObject( camera);
    }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        this.PhysicsData = this.PhysicsData with { AngularVelocity = Math.Clamp(this.PhysicsData.AngularVelocity, -1f, 1f) };
       
        spaceship.Update(Position, Rotation, args);
        var t=  DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position);
        spaceship.setCanonRotation( (float)Math.Atan2(t.Y - Position.Y, t.X - Position.X));
        
        if (timer > 0.1f)
        {
            Exhaust.setAtlasIndex(0, random.Next(0, 4));
            timer = 0;
        } 
        
        
        Vector2 force = new Vector2(0, 0);
        float   torque = 0;
        
        
        if (keyboardState.IsKeyDown(Keys.W))
        {
            force += new Vector2(0f, 500f);
        }
        if (keyboardState.IsKeyDown(Keys.Q))
        {
           force += new Vector2(-500f, 0f);
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            force += new Vector2(0f, -500f);
        }
        if (keyboardState.IsKeyDown(Keys.E))
        {
          force += new Vector2(500f, 0f);
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            torque -= 10f;
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            torque += 10f;
        }
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            force += new Vector2(0, 1000);
        }
       
        
        spaceship.Accelerate(force);
        spaceship.Rotate(torque*0.5F);
        PhysicsSystem.ApplyAngularForce( this, torque);
        Vector2 rotatedForce = new Vector2(
            force.X * (float)Math.Cos(Rotation) - force.Y * (float)Math.Sin(Rotation),
            force.X * (float)Math.Sin(Rotation) + force.Y * (float)Math.Cos(Rotation)
        );
        PhysicsSystem.ApplyForce(this, rotatedForce);

        if (alive && _health <= 0)
        {
            AnimatedExposion.newExplosion(Position, 500);
            alive = false;
        }
        if (!alive)
        {
            AnimatedExposion.newExplosion(Position + new Vector2( (float)random.NextDouble()*500-250,(float)random.NextDouble()*500-200)  ,200);
            deathTimer -= (float)args.Time;
            if (deathTimer < 0)
            {
                Gamestate.CurrState = Gamestate.State.GameOver;
            }
        }



    }

    public void onDeleted()
    {
        spaceship.Dispose();
    }

    public void Draw(List <View> surface)
    {
     PercentageBar.DrawBar( surface[2], new Vector2( surface[2].vsize.X/2,surface[2].vsize.Y- 75 )    , new Vector2( 1000,10)  ,  _health /100f, new Color4(1,0,0f,1f), true);   
     spaceship.Draw(surface[1]);
    }

    public bool applyDamage(int damage)
    {
        camera.shake(damage );
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            return  true;
        }
        _health = Math.Clamp(_health, 0, 100);

        return false;
    }
}