using RadarGame.Physics;

namespace RadarGame.Entities.Enemys;

public interface IEnemie: IEntitie, IPhysicsObject
{
    
    public EnemyManager EnemyManager { get; set; }
    public bool IsDead();
}