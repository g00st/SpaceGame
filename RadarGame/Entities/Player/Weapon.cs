using App.Engine;
using App.Engine.Template;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.Entities;

public class Weapon : IEntitie 
{
    public Score score { get; set; }
    public int Damage { get; set; }
    public float  cooldown { get; set; }
    protected float  currentCooldown { get; set; }
    public int energyCost { get; set; }
    public string Name { get; set; }

    public Weaponmanager manager;
        public void setScore()
        {
            if (score == null)
            {
                score = (Score)EntityManager.GetObject("Score");
            }else
            {
                score.AddPoint();
            }
           
        }

        
        
    public virtual void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        if (state == Weponstate.coolingdown)
        {
          //  Console.WriteLine("coolingdown");
            currentCooldown -= (float)args.Time;
            if (currentCooldown <= 0)
            {
                state = Weponstate.ready;
            }
        }
    }

    public void onDeleted()
    {
       
    }

    public TexturedRectangle icon { get; set; }
    public string Description { get; set; }
    public Weponstate state { get; set; }
    
    public enum Weponstate
    {
     ready,
     fiering,
     coolingdown
    }
    
    public virtual void fire()
    {
       // Console.WriteLine("fireb");
        if (state == Weponstate.ready)
        {
            currentCooldown = cooldown;
            state = Weponstate.fiering;
        }
    }
    public float getColdownPercent()
    {
        return currentCooldown / cooldown;
    }
    
   
    
}