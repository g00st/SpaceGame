namespace RadarGame.Entities;

public  interface IcanBeHurt
{
    //apply damage return true if the object is destroyed by the damage

    public bool applyDamage(int damage);

}