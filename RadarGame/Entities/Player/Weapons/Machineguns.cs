using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities.Weapons;

public class Machineguns : Weapon , IDrawObject
{
    private bool left = false;
    
    private Vector2 leftMachinegun = new Vector2(100, 50);
    private Vector2 rightMachinegun = new Vector2(100, -50);
    private static TexturedRectangle bullet = new TexturedRectangle(Vector2.Zero, new Vector2(20, 30), new Texture( "resources/Machinegun/red_laser.png" ), "bullet", true);
    private List<Vector4> bullets = new List<Vector4>();
    private Vector2 Position;
    private float Rotation;
    private IPhysicsObject trackingObject = null;

    public Machineguns()
    {
        Damage = 10;
        cooldown = 0.1f;
        energyCost = 0;
        Name = "Machineguns";
        Description = "Machineguns";
        state = Weponstate.ready;
        icon = new TexturedRectangle(Vector2.Zero, new Vector2(50, 50), new Texture("resources/Machinegun/weapon_icon.png"), "Machineguns", true);
    }

    public override void fire()
{
    base.fire();
    if (state == Weponstate.fiering)
    {
       
        
        
        Console.WriteLine("fire Machineguns");
        state = Weponstate.coolingdown;
        Vector2 direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
        direction.Normalize();

        // Determine the offset based on which machine gun is firing
        Vector2 offset = left ? leftMachinegun : rightMachinegun;
        left = !left; // Switch to the other machine gun for the next fire

        // Rotate the offset by the same amount as the weapon's rotation
        float cos = (float)Math.Cos(Rotation);
        float sin = (float)Math.Sin(Rotation);
        offset = new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);

        // Add the offset to the position
        Vector2 position = Position + offset;
        bullets.Add(new Vector4(position.X, position.Y, direction.X, direction.Y));
        Console.WriteLine("Position " + position);
    }
}

    public   override void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
       base.Update(args, keyboardState, mouseState); 
       
        if (trackingObject != null)
        {
            Position = trackingObject.Position;
            Rotation = trackingObject.Rotation  + (float) Math.PI/2;
        }
        else
        {
            trackingObject = (IPhysicsObject)   EntityManager.GetObject("Player");
        } 
            
        for (int i = 0; i < bullets.Count; i++)
        {
            Vector4 bullet = bullets[i];
            bullet = bullet with { Xy = bullet.Xy +  bullet.Zw  *  2000*(float)args.Time };
            bullets[i] = bullet;
            float distance =20;
            
            IColisionObject c =  ColisionSystem.getNearest(   bullet.Xy,  out distance);
            if (distance < 10 && c != trackingObject)
            {
                AnimatedExposion.newExplosion(bullet.Xy, 50);
                if (c is IcanBeHurt)
                {
                   if(  ((IcanBeHurt) c).applyDamage(Damage))
                   {
                       setScore();
                       manager.addEnergy(10);
                   }
                
                }
                bullets.RemoveAt(i);
            }
            else if(Vector2.Distance(Position, bullet.Xy) > 5000)
            {
                bullets.RemoveAt(i);
            }
            
                
        }
        
        
      
    }

    public void Draw(List<View> surface)
    {
        foreach (var bullet in bullets)
        {
          
            Machineguns.bullet.drawInfo.Position = bullet.Xy;
            Machineguns.bullet.drawInfo.Rotation =    -(float)Math.Atan2(bullet.Z, bullet.W) + (float)Math.PI/2   ; 
            surface[1].Draw(Machineguns.bullet);
        }
    }
}