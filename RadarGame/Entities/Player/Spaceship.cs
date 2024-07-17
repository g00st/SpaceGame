using App.Engine;
using App.Engine.Template;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace RadarGame.Entities;

public class Spaceship : IDisposable
{
    public TexturedRectangle Body { get; set; }
    private TextureAtlasRectangle Exhaust;
    private TextureAtlasRectangle Canon;
    private Random random = new Random();
    private float timer = 0;
    
    //back
    private Vector2 exhaustOffsetBack1;
    private Vector2 exhaustOffsetBack2;
    private Vector2 exhaustOffsetBackf1;
    private Vector2 exhaustOffsetBackf2;
    private Vector2 worldBackExhaustOffset2;
    private Vector2 worldBackExhaustOffset1;
    private  Vector2 worldBackExhaustOffsetf2;
    private  Vector2 worldBackExhaustOffsetf1;
    
    //front
    private Vector2 exhaustOffsetfront;
    private Vector2 wordlexhaustOffsetfront;
    
    //left
    private Vector2 exhaustOffsetLeft;
    private Vector2 wordlexhaustOffsetLeft;
    
    //right
    private Vector2 exhaustOffsetRight;
    private Vector2 wordlexhaustOffsetRight;
    
    //rotationL
    private Vector2 exhaustOffsetRotationL;
    private Vector2 wordlexhaustOffsetRotationL;
    
    //rotationR
    private Vector2 exhaustOffsetRotationR;
    private Vector2 wordlexhaustOffsetRotationR;


    static string filepath = "resources/Sounds/lasergun.wav";





    private int counter = 0;
    
    private bool AccelerationL = false;
    private bool AccelerationR = false;
    private bool AccelerationF = false;
    private bool AccelerationB = false;
    private float Rotationacceleration = 0;
    private bool Fast = false;
    private int atlsindext = 0;
    private int atlsindexc = 3;
    public float canonRotation = 0;
    
    
    private bool shootbool = false;
    private float animationtimer = 0;
    
    
    
    private Vector2 Ehxaustsize = new Vector2(1, 1);
    
    

    public Spaceship()
    {
        var bodyTexture = new Texture("resources/Player/player_spaceship.png") ;
        var exhaustTexture = new Texture("resources/Player/spaceship_exaust2.png");
        var canonTexture = new Texture("resources/Player/spaceship_canon.png");
        Body = new TexturedRectangle(bodyTexture, true);
        Exhaust = new TextureAtlasRectangle(exhaustTexture, Vector2.Zero, new Vector2(1, 4));
        Canon = new TextureAtlasRectangle(canonTexture, Vector2.Zero, new Vector2(1, 4));
        Canon.drawInfo.Size *= 1.5f;
        Body.drawInfo.Size *= 1.5f;
        
        Ehxaustsize = new Vector2(Exhaust.drawInfo.Size.X*2, Exhaust.drawInfo.Size.Y*1.5f);
        //set the offset of the exhaust
        exhaustOffsetBack1 =  new Vector2(-Body.drawInfo.Size.X / 9, -Body.drawInfo.Size.X / 2f  ); 
        exhaustOffsetBack2 =  new Vector2(Body.drawInfo.Size.X / 9, -Body.drawInfo.Size.X / 2f  );
        exhaustOffsetBackf1 =  new Vector2(-Body.drawInfo.Size.X / 9, -Body.drawInfo.Size.X / 2f  );
        exhaustOffsetBackf2 =  new Vector2(Body.drawInfo.Size.X / 9, -Body.drawInfo.Size.X / 2f  );
        
        
        
        
        exhaustOffsetfront =  new Vector2(0, Body.drawInfo.Size.X / 2 );
        
        exhaustOffsetLeft =  new Vector2(-Body.drawInfo.Size.X / 4.5F, 0 );
        exhaustOffsetRight =  new Vector2(Body.drawInfo.Size.X / 4.5f, 0 );
    }

    public void Update(Vector2 position, float rotation, FrameEventArgs args)
    {
        timer += (float)args.Time;
        Body.drawInfo.Position = position;
        Body.drawInfo.Rotation = rotation + (float)Math.PI / 2;
        Exhaust.drawInfo.Rotation = rotation + (float)Math.PI / 2;
        Canon.drawInfo.Position = position;
        //vector of the direction the ship is facing
        Vector2 direction = new Vector2((float)Math.Cos(rotation + (float)Math.PI / 2), (float)Math.Sin(rotation + (float)Math.PI / 2));
        direction.Normalize();
        
        //back
         worldBackExhaustOffset1 = new Vector2(
            exhaustOffsetBack1.X * (float)Math.Cos(rotation) - exhaustOffsetBack1.Y * (float)Math.Sin(rotation),
            exhaustOffsetBack1.X * (float)Math.Sin(rotation) + exhaustOffsetBack1.Y * (float)Math.Cos(rotation)
        );
         worldBackExhaustOffset2 = new Vector2(
            exhaustOffsetBack2.X * (float)Math.Cos(rotation) - exhaustOffsetBack2.Y * (float)Math.Sin(rotation),
            exhaustOffsetBack2.X * (float)Math.Sin(rotation) + exhaustOffsetBack2.Y * (float)Math.Cos(rotation)
        );
         
        worldBackExhaustOffsetf2 = new Vector2(
            exhaustOffsetBackf2.X * (float)Math.Cos(rotation) - exhaustOffsetBackf2.Y * (float)Math.Sin(rotation),
            exhaustOffsetBackf2.X * (float)Math.Sin(rotation) + exhaustOffsetBackf2.Y * (float)Math.Cos(rotation)
        );
        worldBackExhaustOffsetf1 = new Vector2(
            exhaustOffsetBackf1.X * (float)Math.Cos(rotation) - exhaustOffsetBackf1.Y * (float)Math.Sin(rotation),
            exhaustOffsetBackf1.X * (float)Math.Sin(rotation) + exhaustOffsetBackf1.Y * (float)Math.Cos(rotation)
        );
        
        worldBackExhaustOffset2 = position +  worldBackExhaustOffset2;
        worldBackExhaustOffset1 = position + worldBackExhaustOffset1;
        worldBackExhaustOffsetf2 = position + worldBackExhaustOffsetf2;
        worldBackExhaustOffsetf1 = position + worldBackExhaustOffsetf1;
         
        //front
                wordlexhaustOffsetfront = new Vector2(
                    exhaustOffsetfront.X * (float)Math.Cos(rotation) - exhaustOffsetfront.Y * (float)Math.Sin(rotation),
                    exhaustOffsetfront.X * (float)Math.Sin(rotation) + exhaustOffsetfront.Y * (float)Math.Cos(rotation)
                );
        wordlexhaustOffsetfront = position + wordlexhaustOffsetfront;
            
        //left
        wordlexhaustOffsetLeft = new Vector2(
            exhaustOffsetLeft.X * (float)Math.Cos(rotation) - exhaustOffsetLeft.Y * (float)Math.Sin(rotation),
            exhaustOffsetLeft.X * (float)Math.Sin(rotation) + exhaustOffsetLeft.Y * (float)Math.Cos(rotation)
        );
        wordlexhaustOffsetLeft = position + wordlexhaustOffsetLeft;
        
        //right
        wordlexhaustOffsetRight = new Vector2(
            exhaustOffsetRight.X * (float)Math.Cos(rotation) - exhaustOffsetRight.Y * (float)Math.Sin(rotation),
            exhaustOffsetRight.X * (float)Math.Sin(rotation) + exhaustOffsetRight.Y * (float)Math.Cos(rotation)
        );
        wordlexhaustOffsetRight = position + wordlexhaustOffsetRight;
        
        
     
       //booster animation
        if (timer > 0.1f)
        {
            counter++;
            counter = counter % 4;
            atlsindext = counter;
            timer = 0;
        }
        
        //canon animation
        if (shootbool)
        {
            animationtimer += (float)args.Time;
            if (animationtimer > 0.1f)
            {
                atlsindexc--;
                if (atlsindexc == 0)
                {
                    atlsindexc = 3;
                    shootbool = false;
                }
                animationtimer = 0;
            }
        }
    }
    
    public void Accelerate( Vector2 force)
    {
        AccelerationR = force.X > 0;
        AccelerationL = force.X < 0;
        AccelerationF = force.Y > 0;
        Fast = force.Y > 300;
        
        AccelerationB = force.Y < 0;
      
    }
    public void Rotate(float torque)
    {
        Rotationacceleration = -torque;
        if (torque != 0) 
        {
             AccelerationR = true;
                    AccelerationL = true;
        }
       
       
    }
    
    public void setCanonRotation(float rotation)
    {
        canonRotation = rotation;
        Canon.drawInfo.Rotation = rotation;
    }
    public void shoot()
    {
        if (shootbool) return;
        shootbool = true;
        atlsindexc = 4;
       //SoundSystem.SoundSystem.PlayThisTrack(filepath, 2);
    }

    public void Draw(View surface)
    {
      
        Exhaust.setAtlasIndex(0, atlsindext);  
        if (AccelerationF)
        {
            Exhaust.drawInfo.Size = Ehxaustsize;
            Exhaust.drawInfo.Rotation = Body.drawInfo.Rotation+ 0.2f*Rotationacceleration;
            if (Fast)
            {
               
                Exhaust.drawInfo.Position = worldBackExhaustOffsetf1; 
                surface.Draw(Exhaust);
                Exhaust.drawInfo.Position = worldBackExhaustOffsetf2; 
                surface.Draw(Exhaust);
            }else
            {
                Exhaust.drawInfo.Size = Ehxaustsize* 0.7f;
                Exhaust.drawInfo.Position = worldBackExhaustOffset1; 
                surface.Draw(Exhaust);
                Exhaust.drawInfo.Position = worldBackExhaustOffset2; 
                surface.Draw(Exhaust);
            }
        }
        if (AccelerationB)
        {
            Exhaust.drawInfo.Position = wordlexhaustOffsetfront; 
            Exhaust.drawInfo.Rotation = Body.drawInfo.Rotation + (float)Math.PI;
            Exhaust.drawInfo.Size = Ehxaustsize* 0.5f;
            surface.Draw(Exhaust);
            
        }
        
        if (AccelerationL)
        {
            Exhaust.drawInfo.Position = wordlexhaustOffsetRight; 
            Exhaust.drawInfo.Rotation = Body.drawInfo.Rotation + (float)Math.PI / 2 + 0.2f*Rotationacceleration; ;
            Exhaust.drawInfo.Size = Ehxaustsize* 0.5f;
            surface.Draw(Exhaust);
        }
        if (AccelerationR)
        {
            Exhaust.drawInfo.Position = wordlexhaustOffsetLeft; 
            Exhaust.drawInfo.Rotation = Body.drawInfo.Rotation - (float)Math.PI / 2 + 0.2f*Rotationacceleration;
            Exhaust.drawInfo.Size = Ehxaustsize* 0.5f;
            surface.Draw(Exhaust);
        }

       
        surface.Draw(Body);
        Canon.setAtlasIndex(0, atlsindexc);
        surface.Draw(Canon);
        
    }

    public void Dispose()
    {
        Body.Dispose();
        Exhaust.Dispose();
        Canon.Dispose();
    }
}