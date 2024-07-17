
using ImGuiNET;
using OpenTK.Mathematics;

namespace RadarGame.Physics;

public  static class PhysicsSystem
{
    private static readonly List<IPhysicsObject> _physicsObjects = new List<IPhysicsObject>();
    public static void Update( double deltaTime)
    {
        foreach (var physicsObject in _physicsObjects)
        {
            
            var newVel = physicsObject.PhysicsData.Velocity  +physicsObject.PhysicsData.Acceleration * (float)deltaTime;
            newVel = new Vector2( Math.Clamp(newVel.X, -1000f, 1000f), Math.Clamp(newVel.Y, -1000f, 1000f)  );
            var dragforce = 0.5f * newVel.Length * newVel.Length * Math.Clamp(  physicsObject.PhysicsData.Drag , 0f, 10f);
            Vector2 vectordragforce;
            if (newVel.LengthSquared > 0)
            {
                vectordragforce = newVel.Normalized() * dragforce * (float)deltaTime;
            }
            else
            {
                vectordragforce = Vector2.Zero;
            }
            newVel -= vectordragforce / Math.Clamp( physicsObject.PhysicsData.Mass, 0.1f, 1000f);
            
            
            

            var newAngVel = physicsObject.PhysicsData.AngularVelocity  +physicsObject.PhysicsData.AngularAcceleration* (float)deltaTime;
            var angDragforce = 0.5f * newAngVel* newAngVel * Math.Clamp(  physicsObject.PhysicsData.Drag , 0f, 10f) * (float)deltaTime;
            if (newAngVel > 0)
            {
                newAngVel -= angDragforce / Math.Clamp( physicsObject.PhysicsData.Mass, 0.1f, 1000f);
            }
            else
            {
                newAngVel += angDragforce / Math.Clamp( physicsObject.PhysicsData.Mass, 0.1f, 1000f);
            }
           // newAngVel -=  angDragforce / Math.Clamp( physicsObject.PhysicsData.Mass, 0.1f, 1000f);
          
           
            
            physicsObject.PhysicsData = physicsObject.PhysicsData with {Velocity = newVel, AngularVelocity = newAngVel, Acceleration = Vector2.Zero, AngularAcceleration = 0f};
            physicsObject.Position += physicsObject.PhysicsData.Velocity * (float)deltaTime;
            
            physicsObject.Rotation += newAngVel * (float)deltaTime;
            if (!float.IsFinite(physicsObject.Rotation))
            {
                throw new  System.Exception("Rotation is not finite");
            }
            if(physicsObject.Rotation > 2 * MathF.PI)
            {
                physicsObject.Rotation -= 2 * MathF.PI;
            }
               if(physicsObject.Rotation < 0)
                {
                    physicsObject.Rotation += 2 * MathF.PI;
                }
               if (!float.IsFinite(physicsObject.Rotation))
               {
                   throw new  System.Exception("Rotation is not finite");
               }
           
        }
      
    }
    
    public static void ApplyForce(IPhysicsObject physicsObject, Vector2 force)
    {
        physicsObject.PhysicsData = physicsObject.PhysicsData with {Acceleration = physicsObject.PhysicsData.Acceleration + force / physicsObject.PhysicsData.Mass};
    }
    
    public static void ApllyForceRotated(IPhysicsObject physicsObject, Vector2 force)
    {
        var rotation = physicsObject.Rotation;
        Vector2 rotatedForce = new Vector2(
            force.X * (float)Math.Cos(rotation) - force.Y * (float)Math.Sin(rotation),
            force.X * (float)Math.Sin(rotation) + force.Y * (float)Math.Cos(rotation)
        );
        ApplyForce(physicsObject, rotatedForce);
    }

    public static void ApplyAngularForce(IPhysicsObject physicsObject, float torque)
    {
        physicsObject.PhysicsData = physicsObject.PhysicsData with {AngularAcceleration = physicsObject.PhysicsData.AngularAcceleration + torque / physicsObject.PhysicsData.Mass};
    }
    
    
    public static void AddObject(IPhysicsObject physicsObject)
    {
        _physicsObjects.Add(physicsObject);
    }
    public static void RemoveObject(IPhysicsObject physicsObject)
    {
        _physicsObjects.Remove(physicsObject);
    }
    
    public static void ClearObjects()
    {
        _physicsObjects.Clear();
    }

    public static void DebugDraw()
    {
        ImGui.Begin("Physics Debug Window");
        ImGui.BeginChild("scrolling", new System.Numerics.Vector2(0, 0), ImGuiChildFlags.Border,
            ImGuiWindowFlags.HorizontalScrollbar);

        foreach (var physicsObject in _physicsObjects)
        {
            if (ImGui.CollapsingHeader(physicsObject.Name))
            {
                ImGui.PushID(physicsObject.GetHashCode());
                System.Numerics.Vector2 position =
                    new System.Numerics.Vector2(physicsObject.Position.X, physicsObject.Position.Y);
                ImGui.Text("Position X: " + physicsObject.Position.X + " Y: " + physicsObject.Position.Y);
                ImGui.Text("Velocity X: " + physicsObject.PhysicsData.Velocity.X + " Y: " +
                           physicsObject.PhysicsData.Velocity.Y);
                ImGui.Text("Acceleration X: " + physicsObject.PhysicsData.Acceleration.X + " Y: " +
                           physicsObject.PhysicsData.Acceleration.Y);
                ImGui.Text("Angular Velocity: " + physicsObject.PhysicsData.AngularVelocity);
                ImGui.Text("Angular Acceleration: " + physicsObject.PhysicsData.AngularAcceleration);
                ImGui.Text("Rotation: " + physicsObject.Rotation);
                ImGui.Text("Mass: " + physicsObject.PhysicsData.Mass);
                ImGui.Text("Drag: " + physicsObject.PhysicsData.Drag);
                ImGui.Text("Center X: " + physicsObject.Center.X + " Y: " + physicsObject.Center.Y);
                //add cloapsable submenu for editing menu for editing
                ImGui.Indent(10f);
                if (ImGui.CollapsingHeader("Edit"))
                {
                   
                    ImGui.InputFloat2("Position", ref position);
                    float drag = physicsObject.PhysicsData.Drag;
                    float mass = physicsObject.PhysicsData.Mass;
                     System.Numerics.Vector2 velocity = new System.Numerics.Vector2( physicsObject.PhysicsData.Velocity.X, physicsObject.PhysicsData.Velocity.Y);

                    ImGui.InputFloat("Drag", ref drag);
                    ImGui.InputFloat("Mass", ref mass);
                    ImGui.InputFloat2("Velocety", ref velocity);
                    physicsObject.PhysicsData = physicsObject.PhysicsData with {Drag = drag, Mass = mass, Velocity = new Vector2(velocity.X, velocity.Y)};
                    physicsObject.Position = new Vector2(position.X, position.Y);
                   
                } 
                ImGui.Unindent(10f);
                
                
                ImGui.PopID();
             
              
            }  
            
        }
        ImGui.EndChild();
        ImGui.End();
        
        
    }

}