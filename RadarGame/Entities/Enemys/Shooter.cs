using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Engine;
using App.Engine.Template;
using Engine.graphics.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.others;
using RadarGame.Physics;

namespace RadarGame.Entities.Enemys
{
    public class Shooter : IEnemie, IDrawObject, IColisionObject, IcanBeHurt, IPhysicsObject
    {
        // this enemy should try to move towards the player if it is in vision range
        // then it should try to shoot the player
        // first make it, then try to avoid asteroids

        static int id = 0;
        public EnemyManager EnemyManager { get; set; }
        public PlayerObject target;
        public string Name { get; set; }
        public PhysicsDataS PhysicsData { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Center { get; set; }
        public float Rotation { get; set; }
        public List<Vector2> CollisonShape { get; set; }
        public bool Static { get; set; }
        private bool mouseHover = false;
        private bool isDead = false;
        private bool isInRange = false;

        private float fireRate = 2f;
        private int health = 30;
        private int maxHealth = 30;
        private float fireTimer = 0;
        float range = 3000;
        private Vector2 visionRangeMin;
        private Vector2 visionRangeMax;
        private float visionThreshold = 50f;

        private Vector2 size = new Vector2(300, 200);
        private float explosiondistance = 500;

        //fake, stolen from Mine and Turret
        private static Polygon distance = Polygon.Circle(Vector2.Zero, 100, 100, new SimpleColorShader(Color4.Blue), "indicatorTurret", true);
        private static TexturedRectangle texture = new TexturedRectangle(new Vector2(0, 0), new Vector2(100, 100), new Texture("resources/Enemies/Shooter.png"), "Shooter", true);
        public Shooter(Vector2 position, EnemyManager enemyManager)
        {
            Position = position;
            EnemyManager = enemyManager;
            Name = "Shooter" + id++;
            Static = false;
            PlayerObject target = (PlayerObject)EntityManager.GetObject("Player");
            visionRangeMin = new Vector2(0f, 1f);   // CHECK IF REALISTIC
            visionRangeMax = new Vector2(0f, visionThreshold);

            PhysicsData = new PhysicsDataS
            {
                Velocity = Vector2.Zero,
                Mass = 1f,
                Drag = 0.000f,
                Acceleration = Vector2.Zero,
                AngularAcceleration = 0f,
                AngularVelocity = 0.5f
            };
            CollisonShape = new List<Vector2>
            {
            new Vector2(-50, -50),
            new Vector2(50, -50),
            new Vector2(50, 50),
            new Vector2(-50, 50)
            };
        }

        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {

            if (target == null)
            {
                target = (PlayerObject)EntityManager.GetObject("Player");
                return;
            }

            // do stuff
            Vector2 direction = target.Position - Position;
            float distance = direction.Length;
            if (distance < range)
            {
                Rotation = MathF.Atan2(direction.Y, direction.X);
                fireTimer += (float)args.Time;
                if (fireTimer > fireRate)
                {
                    fireTimer = 0;
                    direction.Normalize();

                    EntityManager.AddObject(new Bullet(Position + direction * size.X, direction, 50, 2, 70));
                }
                Movement(target.Position, args);
            }
            // Behaviour();

            // do more stuff
            var mouse = DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position);
            if ((mouse - Position).Length < size.X * 2)
            {
                mouseHover = true;
            }
            else
            {
                mouseHover = false;
            }
        }

        public void Draw(List<View> surface)
        {
            PercentageBar.DrawBar(surface[1], Position + new Vector2(0, 100), new Vector2(200, 50), health / (float)maxHealth, Color4.DarkRed, true);
            if (mouseHover)
            {
                distance.drawInfo.Position = Position;
                distance.drawInfo.Size = new Vector2(range);
                surface[1].Draw(distance);
                TextRenderer.Write(health + "/" + maxHealth, Position + new Vector2(0, 150), new Vector2(50, 50), surface[1], Color4.White, true);
            }
            texture.drawInfo.Position = Position;
            texture.drawInfo.Size = size;
            texture.drawInfo.Rotation = Rotation;
            surface[1].Draw(texture);
        }

        public void OnColision(IColisionObject colidedObject)
        {
            applyDamage(10);
        }

        public bool applyDamage(int damage)
        {
            health -= damage;
            health = Math.Clamp(health, 0, maxHealth);
            if (IsDead())
            {
                AnimatedExposion.newExplosion(Position + new Vector2(20, 10), 200);
                AnimatedExposion.newExplosion(Position + new Vector2(-50, -200), 100);
                AnimatedExposion.newExplosion(Position + new Vector2(60, 100), 300);
                AnimatedExposion.newExplosion(Position + new Vector2(20f, -10f), 100);
                return true;
            }
            return false;
        }

        public bool IsDead()
        {
            if (health <= 0)
            {
                return true;
            }
            return false;
        }
        public bool IsInRange()
        {
            return isInRange;
        }

        public void onDeleted()
        {
            texture.Dispose();
        }

        private void explode()
        {
            if (IsDead()) return;
            isDead = true;
            
         AnimatedExposion.newExplosion(Position, explosiondistance*2);
         foreach (var colisionObject in ColisionSystem.getinRadius( Position, explosiondistance, false,true))
         {
             if (colisionObject is IcanBeHurt)
             {
                 ((IcanBeHurt)colisionObject).applyDamage( (int) (50f * (1 - (Position - colisionObject.Position).Length / explosiondistance)));
             }
         } 
        }

        private void Behaviour()
        {

            return;
        }

        private void Movement(Vector2 input, FrameEventArgs args)
        {

            // WEEE
            Vector2 direction = input - Position;
            direction.Normalize();
            direction = direction * (float)args.Time * 5000;
            PhysicsSystem.ApplyForce(this, direction);
        }

    }

}
