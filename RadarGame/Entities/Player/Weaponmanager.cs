using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Entities.Weapons;
using RadarGame.others;

namespace RadarGame.Entities;

public class Weaponmanager: IEntitie ,IDrawObject
{
    private List<Weapon> weapons = new List<Weapon>();
    private float energy = 0;
    private float maxEnergy = 1000;
    private float energyRegen = 1;
    private Weapon currentWeapon = null;
    private ColoredRectangle cooldownBar = new ColoredRectangle( Vector2.Zero , new Vector2(10,10),  new Color4(0,0,0.2f,0.4f), "cooldownBar" , true);
    private TexturedRectangle Iconframe = new TexturedRectangle(  new Texture("resources/Player/IconFrame.png") , true);
    private TexturedRectangle IconframeSelected = new TexturedRectangle(  new Texture("resources/Player/IconselectedFrame.png") , true);
    private TexturedRectangle IconframeBackground = new TexturedRectangle(  new Texture("resources/Player/IconBackground.png") , true);
     Vector2 iconSize = new Vector2(80,80);
    Vector2 iconPosition = new Vector2(1920/2 - 80*1.5f,100);

    private bool toggle = false;

    private float energyRegenRate = 2;
    private float energyRegenTimer = 0;
    public string Name { get; set; }
    
    
    public Weaponmanager()
    {
        
        energy = maxEnergy;
        Name = "Weaponmanager";
        weapons.Add(new Machineguns());
        weapons.Add(new Canon());
        weapons.Add(new GuidedMissile());
        currentWeapon = weapons[1];
        weapons[2].cooldown = 1f;

        foreach (var w in weapons)
        {
            w.manager = this;
        }
            
             EntityManager.AddObject(weapons[0]);
                EntityManager.AddObject(weapons[1]);
                EntityManager.AddObject(weapons[2]);
             iconPosition = new Vector2(1920/2 - 80* weapons.Count/2.0f,100);
        
    }

    public void addEnergy( int amount)
    {
        energy += amount;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }
  
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        energyRegenTimer += (float)args.Time;
        if(toggle)
        {
            if (weapons[0].state == Weapon.Weponstate.ready && energy >= weapons[0].energyCost)
            {
                weapons[0].fire();
                energy -= weapons[0].energyCost;
            }
        }
        if (energyRegenTimer >= energyRegenRate)
        {
            energy += energyRegen;
            energy = Math.Clamp(energy, 0, maxEnergy);
            
            energyRegenTimer = 0;
        }
        
        for (int i = 1; i < weapons.Count; i++)
        {
            if (keyboardState.IsKeyDown(Keys.D1 + i-1))
            {
                currentWeapon = weapons[i];
            }
        }
        if (keyboardState.IsKeyReleased(Keys.Space))
        {
            toggle = !toggle;
        }
       
        if (mouseState.IsButtonDown( MouseButton.Left))
        {
            if (currentWeapon.state == Weapon.Weponstate.ready && energy >= currentWeapon.energyCost)
            {
                currentWeapon.fire();
                energy -= currentWeapon.energyCost;
            }
           
        }
        
        
        
    }

    public void onDeleted()
    {
        //throw new NotImplementedException();
    }
    
    private void drawIcons(View surface)
    {
        int i = 0;
        float space = 1.5F;
        foreach (var wepon in weapons)
        {
            
            cooldownBar.drawInfo.Position = iconPosition + new Vector2(i * iconSize.X * space, 0);
            Iconframe.drawInfo.Position = iconPosition + new Vector2(i * iconSize.X * space, 0);
            Iconframe.drawInfo.Size = iconSize;
            IconframeSelected.drawInfo.Position = iconPosition + new Vector2(i * iconSize.X* space , 0);
            IconframeSelected.drawInfo.Size = iconSize * 1.1f;
            IconframeBackground.drawInfo.Position = iconPosition + new Vector2(i * iconSize.X* space, 0);
            IconframeBackground.drawInfo.Size = iconSize;
            
            surface.Draw(IconframeBackground);
           

           if (wepon.icon != null)
           {
                wepon.icon.drawInfo.Position = iconPosition + new Vector2(i * iconSize.X * space, 0);
                wepon.icon.drawInfo.Size = iconSize;
                surface.Draw(wepon.icon);
           }
           if (wepon.state == Weapon.Weponstate.coolingdown)
           {
                                      
               cooldownBar.drawInfo.Size = iconSize * new Vector2(wepon.getColdownPercent());
               surface.Draw(cooldownBar);
                                      
           }
            if (wepon == currentWeapon)
            {
                surface.Draw(IconframeSelected);
            }
            else
            {
                surface.Draw(Iconframe);
            }

            
            
            i++;
            
        }
        
    }


    public void Draw(List<View> surface)
    {
       PercentageBar.DrawBar( surface[2], new Vector2( surface[2].vsize.X/2,surface[2].vsize.Y- 60 )    , new Vector2( 1000,10)  ,  energy /maxEnergy, new Color4(0,0,1f,1f), true); 
       drawIcons( surface[2]);
    }
}