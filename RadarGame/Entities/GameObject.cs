using App.Engine;
using OpenTK.Mathematics;
using App.Engine.Template;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;


namespace RadarGame.Entities;

public class GameObject : IEntitie, IPhysicsObject , IDrawObject, IColisionObject , IcanBeHurt
{
    //--------------------------------------------------------------------------------
    // THis is a simple game object that can be used to test the Project and Systems
    //--------------------------------------------------------------------------------
    public TexturedRectangle DebugColoredRectangle { get; set; }
    
    public PhysicsDataS PhysicsData { get; set; }
    public List<Vector2> CollisonShape { get; set; }

    static string filepath = "resources/Sounds/kaboommeme.wav";
    public void OnColision(IColisionObject colidedObject)
    {
        if (((IEntitie)colidedObject).Name.Contains("Bullet"))
        {
            //Todo: Add sound effect Kaboom 
         //   Console.WriteLine("Colision with " + colidedObject);
            //SoundSystem.SoundSystem.PlayThisTrack(filepath, 2);
            EntityManager.RemoveObject((IEntitie)colidedObject);
            EntityManager.RemoveObject(this);
        }
        else
        {

            if (colidedObject is IPhysicsObject physicsObject)
            {
                var differencevector = physicsObject.Position - Position;
                PhysicsSystem.ApplyForce(this, -differencevector * 100);
            }
            else
            {
                var differencevector = colidedObject.Position - Position;
                PhysicsSystem.ApplyForce(this, -differencevector * 100);
            }
            
            
          
        }
    }
    private Polygon DebugPolygon = Polygon.Circle( new Vector2(0, 0), 50, 100,new SimpleColorShader(Color4.Ivory), "SDF", true);
    
    public bool applyDamage(int damage)
    {
        Console.WriteLine("Apply Damage");
        EntityManager.RemoveObject(this);
        return true;
    }
    public bool Static { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Center { get; set; }
    public float Rotation { get; set; }
    private int i =0;
    

    public string Name { get; set; }

    public GameObject( Vector2 position, float rotation, string name, int i)
    {
        Position = position;
        Rotation = rotation;
        Static = false;
        Name = name;
        this.i = 1;
        Random random = new Random();
        PhysicsData = PhysicsData with { 
            Velocity = new Vector2((float)random.NextDouble()*100-50 , (float)random.NextDouble()*100-50),
           // Velocity = new Vector2(0 , 0), 
            Mass = 5f, 
            Drag = 0.01f,
            Acceleration = new Vector2(0f, 0f), 
            AngularAcceleration = 0f,
            AngularVelocity = (float)random.NextDouble()*10-5 };  
        DebugColoredRectangle = new TexturedRectangle(
            new OpenTK.Mathematics.Vector2(0f, 0f),
            new OpenTK.Mathematics.Vector2(100f, 100f), 
            new Texture("resources/cirno.png"),
            Name,
            true
            );
        CollisonShape = new List<Vector2>
        {
            new Vector2(-50, -50),
            new Vector2(50, -50),
            new Vector2(50, 50),
            new Vector2(-50, 50)
        };
    }
    public GameObject  (Vector2 position, float rotation, string name, Vector2 vel , float angVel)
    {
        i = 1;
        Static = false;
        Position = position;
        Rotation = rotation;
        Name = name;
        PhysicsData = PhysicsData with { 
            Velocity = vel, 
            Mass = 5f, 
            Drag = 0.01f,
            Acceleration = new Vector2(0f, 0f), 
            AngularAcceleration = 0f,
            AngularVelocity = angVel };  
        DebugColoredRectangle = new TexturedRectangle(
            position,
            new OpenTK.Mathematics.Vector2(50f, 50f), 
            new Texture("resources/cirno.png"),
            Name,
            true
            );
        DebugColoredRectangle.drawInfo.Rotation = rotation;
        CollisonShape = new List<Vector2>
        {
            new Vector2(-25, -25),
            new Vector2(25, -25),
            new Vector2(25, 25),
            new Vector2(-25, 25)
        };
    }
    


    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        DebugColoredRectangle.drawInfo.Position = Position;
        DebugColoredRectangle.drawInfo.Rotation = Rotation;
        DebugPolygon.Position = Position;
        DebugPolygon.Rotation = Rotation;
    }

    public void onDeleted()
    {
       //  Console.WriteLine("Deleted");
        DebugColoredRectangle.Dispose();
    }


    public void Draw(List<View> surface)
    {
        surface[i].Draw(DebugColoredRectangle);
    }
}