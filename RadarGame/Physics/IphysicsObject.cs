using OpenTK.Mathematics;

namespace RadarGame.Physics;

public interface IPhysicsObject
{
    PhysicsDataS PhysicsData { get; set; }
    Vector2 Position { get; set; }
    Vector2 Center { get; set; }
    float Rotation { get; set; }
    string Name { get; set; }
}

public struct PhysicsDataS
{
    public float Mass { get; set; }
    public float Drag { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Acceleration { get; set; }
    public float AngularVelocity { get; set; }
    public float AngularAcceleration { get; set; }
}