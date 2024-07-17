using App.Engine;
using App.Engine.Template;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities.Weapons;

public class GuidedMissile : Weapon
{  
    
   private PlayerObject trackingObject;
   private Vector2 direction;
    public GuidedMissile()
    {
        Damage = 400;
        cooldown = 0.1f;
        energyCost = 400;
        Name = "Missiele";
        Description = "Missile";
        state = Weponstate.ready;
        icon = new TexturedRectangle(Vector2.Zero, new Vector2(50, 50), new Texture("resources/Missile/MissileIcon.png"), "MissilIcon", true);
    }
    
    public override void fire()
    {
        base.fire();
        if (state == Weponstate.fiering)
        {
            Console.WriteLine("fire Machineguns");
            state = Weponstate.coolingdown;
            Vector2 spawnoffset = new Vector2(0, 300);
            // Rotate the offset by the same amount as the weapon's rotation
            float cos = (float)Math.Cos(trackingObject.Rotation);
            float sin = (float)Math.Sin(trackingObject.Rotation);
            spawnoffset = new Vector2(spawnoffset.X * cos - spawnoffset.Y * sin, spawnoffset.X * sin + spawnoffset.Y * cos);
            EntityManager.AddObject(new Missile(trackingObject.Position + spawnoffset, trackingObject.Rotation, trackingObject.PhysicsData.Velocity,manager ));
          //  EntityManager.AddObject(new Missile(trackingObject.Position - spawnoffset, trackingObject.Rotation, trackingObject.PhysicsData.Velocity -spawnoffset*2,manager));
        }
    }
    
    
    public override void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
      
        base.Update(args, keyboardState, mouseState); 
        if (trackingObject == null)
        {
            trackingObject= (PlayerObject) EntityManager.GetObject("Player");
          
        }
        
    }
    
}

public class Missile : IEntitie, IDrawObject, IPhysicsObject, IColisionObject , IcanBeHurt
{
    private bool Alive = true;
    private static List<Missile> missiles = new List<Missile>();
    private Weaponmanager manager;
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {   
        Alive = false;
        
    }

    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    public string Name { get; set; }
    private static int count;
    private TextureAtlasRectangle _missileTexture = new TextureAtlasRectangle(Vector2.Zero, new Vector2(50,50), new Vector2(1, 5),   new Texture("resources/Missile/Missile.png" ), "Missile.png");
    private float animationtimer = 0;

    public Missile(Vector2 position, float rotation, Vector2 vel, Weaponmanager manager)
    {
        
        this.manager = manager;
        _missileTexture.drawInfo.Size = new Vector2(250, 50);
        Static = false;
        Name = "Missile" + count++;
        Position = position;
        Rotation = rotation;
        PhysicsData = new PhysicsDataS
        {
            Velocity = vel,
            Mass = 2f,
            Drag = 0.001f,
            Acceleration = new Vector2(0f, 0f),
            AngularAcceleration = 0f,
            AngularVelocity = 0f
        };
        
        CollisonShape = new List<Vector2>
        {
            new Vector2(-50/2, -250/4),
            new Vector2(50/2, -250/4),
            new Vector2(50/2, 250/4),
            new Vector2(-50/2, 250/4)
        };
        missiles.Add(this);
        
    }


    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        var worldcords = DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position, 1);
        Vector2 targetDirection = worldcords - Position;
        targetDirection.Normalize();

        // Apply a force in the direction of the target
        // Adjust the multiplier as needed
        Vector2 force = new Vector2(0, 2000);

        PhysicsSystem.ApllyForceRotated(this, force);
        this.Rotation = (float)Math.Atan2(targetDirection.Y,targetDirection.X) - (float)Math.PI/2;
        this.PhysicsData = this.PhysicsData with
        {
            AngularVelocity = Math.Clamp(this.PhysicsData.AngularVelocity, -0.5f, 0.5f)
        };


        foreach (var otherMissile in Missile.missiles)
        {
            if (otherMissile != this)
            {
                Vector2 toOtherMissile = otherMissile.Position - this.Position;
                float distance = toOtherMissile.Length;
                if (distance < 200) // Adjust this value as needed
                {
                    // Apply a repulsion force
                    toOtherMissile.Normalize();
                    force -= toOtherMissile * 1000; // Adjust this value as needed
                    PhysicsSystem.ApplyForce(this, force);
                }
            }
        }

        animationtimer += (float)args.Time;

        if (! Alive)
        {
            EntityManager.RemoveObject(this);
            
        
            foreach (var colisionObject in ColisionSystem.getinRadius( Position, 1000, false,true))
            {
         
                if (colisionObject is IcanBeHurt)
                {
                    int damage = 1000 - (int)  Vector2.Distance(Position,   colisionObject.Position);
                    if (((IcanBeHurt)colisionObject).applyDamage(damage))
                    {
                        manager.addEnergy(10);
                    }
                }  
            }
            AnimatedExposion.newExplosion( Position, 500);
            //add mor explosins in a circle
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2((float)Math.Cos(i * Math.PI / 5), (float)Math.Sin(i * Math.PI / 5));
                offset *= 500;
                AnimatedExposion.newExplosion(Position + offset, 1000);
            }
        }
        
    }

    public void onDeleted()
    {
        missiles.Remove(this);
    }

    public void Draw(List<View> surface)
    { 
        _missileTexture.setAtlasIndex(0 ,  1+(int)(animationtimer * 10) % 4);
       _missileTexture.drawInfo.Position = Position;
       _missileTexture.drawInfo.Rotation = Rotation + (float)Math.PI/2;
       surface[1].Draw( _missileTexture);
    }

    public bool applyDamage(int damage)
    {
        Alive = false;
        return true;
    }
}
   
   