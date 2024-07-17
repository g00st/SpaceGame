using OpenTK.Mathematics;

namespace RadarGame.Physics;

public interface IColisionObject
{
  public  List<Vector2> CollisonShape { get; set; }
  public void OnColision(IColisionObject colidedObject);
  public bool Static { get; set; }
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  

}