using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities.Weapons;
/*
public class AutoLaser : Weapon
{
    private float range = 500;
    private IPhysicsObject trackingObject = null;

    public AutoLaser()
    {
        Damage = 5;
        cooldown = 0.1f;
        energyCost = 5;
        Name = "AutoLaser";
        Description = "AutoLaser";
        state = Weponstate.ready;
        icon = new TexturedRectangle(Vector2.Zero, new Vector2(50, 50),
            new Texture("resources/AutoLaser/weapon_icon.png"), "AutoLaser", true);
    }

    public override void fire()
    {
        base.fire();
        if (state == Weponstate.fiering)
        {
            state = Weponstate.coolingdown;
            Vector2 direction;
            var Position = trackingObject.Position;
            var X = ColisionSystem.getinRadius(Position, 500);

            X.Remove((IColisionObject)trackingObject);
            if (X.Count > 0)
            {
                X.Sort((a, b) =>
                    Vector2.Distance(Position, a.Position).CompareTo(Vector2.Distance(Position, b.Position)));
                Vector2 v = X[0].Position - Position;
            }
          
        }
    }
    public   override void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        base.Update(args, keyboardState, mouseState); 
       
        if (trackingObject == null)
        {
            trackingObject = (IPhysicsObject)   EntityManager.GetObject("Player");
        } 
}
    
   */