using App.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;

namespace RadarGame.Entities.Enemys;

public class EnemyManager: IEntitie
{
    //-----------------Add Enemie types here-----------------
    private List<Type> EnemieTypes = new List<Type>()
    {
        typeof(Mine),
        typeof(Mine),
        typeof(Searcher),
        typeof(Turret),
        typeof(Shooter)        
    };
    private View view;
    private Camera camera;
    private Vector2 InviewSize;
    private Vector2 InviewPosition { get; set; }
    private Vector2 OutviewSize { get; set; }
    private int EnemyCount { get; set; }
    private int MaxEnemyCount { get; set; }
    private float SpawnRate { get; set; }
    private float SpawnTimer { get; set; }
    private float difficultytimer { get; set; } = 0;
    private List<IEnemie> Enemies = new List<IEnemie>();
     Random random = new Random();
    public string Name { get; set; }
    
    
    public EnemyManager()
    {
        view = DrawSystem.DrawSystem.GetView(1);
        Name = "EnemyManager";
        this.view = view;
        InviewSize = view.vsize;
        InviewPosition = view.vpossition;
        OutviewSize = view.vsize * 1.5f;
        MaxEnemyCount = 20;
        SpawnRate = 0.5f;
    }

    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        difficultytimer += (float)args.Time;
        if (difficultytimer > 20)
        {
            difficultytimer = 0;
            MaxEnemyCount += 2;
        }
       
        
        
        if (camera == null)
        {
            camera = (Camera) EntityManager.GetObject("Camera");
        }else
        {
            OutviewSize = camera.getMaxsize() *2;
            InviewSize = camera.getMaxsize();
        }
        InviewPosition = view.vpossition;
        // Check if we need to spawn a new enemy
        SpawnTimer += (float)args.Time;
        if (SpawnTimer > SpawnRate)
        {
            Console.WriteLine("Spawn");
            SpawnTimer = 0;
            SpawnEnemy();
        }
        
        // Check if we need to remove dead enemies
        List<IEnemie> toremove = new List<IEnemie>();
        foreach (var enemy in Enemies)
        {
            if (enemy.IsDead())
            {
                toremove.Add(enemy);
            }
        }
        foreach (var enemy in toremove)
        {
            Enemies.Remove(enemy);
            EntityManager.RemoveObject(enemy);
            EnemyCount--;
        }
        //  move enemies that got to far away back into view   
        moveIntoView();
    }

    private void moveIntoView()
    {
        foreach (var enemy in Enemies)
        {
            if (enemy.Position.X > InviewPosition.X + OutviewSize.X / 2 ||
                enemy.Position.Y > InviewPosition.Y + OutviewSize.Y / 2 ||
                enemy.Position.X < InviewPosition.X - OutviewSize.X / 2 ||
                enemy.Position.Y < InviewPosition.Y - OutviewSize.Y / 2) 
            {
               
                double angle = random.NextDouble() * Math.PI * 2;
                Vector2 position = new Vector2(
                    (float)(OutviewSize.X / 2 * Math.Cos(angle)),
                    (float)(OutviewSize.Y / 2 * Math.Sin(angle))
                );
                var offset = new Vector2((float)(((float)(random.NextDouble())*200 -100) * Math.Cos(angle)), (float)(100 * Math.Sin(angle)));
                position += offset;
                position += InviewPosition;
                ColisionSystem.getNearest(position, out float distance);
                if (distance < 100)
                {
                    continue;
                }

                enemy.Position = position;
            }

        }
    }

    private void SpawnEnemy()
    {
        if (EnemyCount >= MaxEnemyCount)
        {
            return;
        }

        double angle = random.NextDouble() * Math.PI * 2;
        Vector2 position = new Vector2(
            (float)(InviewSize.X / 2 * Math.Cos(angle)),
            (float)(InviewSize.Y / 2 * Math.Sin(angle))
        );
        var offset = new Vector2((float) (100* Math.Cos(angle)),
            (float)(100 * Math.Sin(angle)));
        position += offset;
        position += InviewPosition;
        ColisionSystem.getNearest(position, out float distance);
        if (distance < 200)
        {
            return;
        }

        int index = random.Next(EnemieTypes.Count);
        Type selectedType = EnemieTypes[index];

        IEnemie newEnemy = (IEnemie)Activator.CreateInstance(selectedType, new object[] { position, this });
 
        Enemies.Add(newEnemy);
        EntityManager.AddObject(newEnemy);
        EnemyCount++;
   }

    public void onDeleted()
    {
       
    }
}