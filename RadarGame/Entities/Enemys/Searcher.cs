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
    public class Searcher : IEnemie, IDrawObject, IColisionObject, IcanBeHurt
    {
        // this enemy should try to move towards the player if it is in vision range
        // first make it, then try to avoid asteroids

        static int id = 0;
        public EnemyManager EnemyManager { get ; set ; }
        public PlayerObject target;
        public string Name { get ; set ; }
        public PhysicsDataS PhysicsData { get ; set ; }
        public Vector2 Position { get ; set ; }
        public Vector2 Center { get ; set ; }
        public float Rotation { get ; set ; }
        public List<Vector2> CollisonShape { get ; set ; }
        public bool Static { get ; set ; }
        private bool isDead = false;
        private bool ligth = false;
        private bool isInRange = false;
        private bool mouseHover = false;
        private bool turbo = false;
        private Vector2 visionRangeMin;
        private Vector2 visionRangeMax;
        private float visionThreshold = 2000;  // maybe change
        private int direction = 0;
        private float Explosiontimer = 0;
        private float Explosiontime = 1f;
        private float blinkSpeed = 0.01f;

        private float explosiondistance = 500;
        //fake, stolen from Mine
        private static TextureAtlasRectangle texture = new TextureAtlasRectangle(new Vector2(0, 0), new Vector2(100, 100), new Vector2(1, 2), new Texture("resources/Enemies/Searcher.png"), "Searcher");

        public Searcher(Vector2 position, EnemyManager enemyManager)
        {
            Position = position;
            EnemyManager = enemyManager;
            Name = "Searcher" + id++;
            Static = false;
            visionRangeMin = new Vector2(0f, 1f);   // CHECK IF REALISTIC
            visionRangeMax = new Vector2(0f, visionThreshold);
            PlayerObject target = (PlayerObject)EntityManager.GetObject("Player");

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

            var distance = (target.Position - Position).Length;

            if (distance < explosiondistance)
            {
                Explosiontimer += (float)args.Time;
                if (Explosiontimer > Explosiontime)
                {
                    explode();
                }
                if (Explosiontimer % blinkSpeed < blinkSpeed / 2)
                {
                    ligth = true;
                }
                else
                {
                    ligth = false;
                }
            }
            else
            {
                Explosiontimer = 0;
                blinkSpeed = 0.1f;
            }

            // do stuff
            Behaviour(args);

            // do more stuff
            var mouse = DrawSystem.DrawSystem.ScreenToWorldcord(mouseState.Position);
            if ((mouse - Position).Length < 200)
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
            PercentageBar.DrawBar(surface[1], Position + new Vector2(0, 100), new Vector2(200, 50), 1, Color4.DarkRed, true);
            if (mouseHover)
            {

                TextRenderer.Write("1/1", Position + new Vector2(0, 150), new Vector2(50, 50), surface[1], Color4.White, true);
            }
            texture.setAtlasIndex(1, ligth ? 1 : 2);
            texture.drawInfo.Position = Position;
            texture.drawInfo.Rotation = Rotation - ((float)Math.PI) * 0.5f;
            surface[1].Draw(texture);
        }

        public void OnColision(IColisionObject colidedObject)
        {
            explode();
        }

        public bool applyDamage(int damage)
        {
            if (IsDead()) return false;
            explode();
            return true;
        }

        public bool IsDead()
        {
            return isDead;
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

        private void Behaviour(FrameEventArgs args)
        {
            Vector2 direction = target.Position - Position;
            float distance = direction.Length;
            Vector2 randomDirection = new Vector2(0, 0);
            if (distance < visionThreshold)
            {
                Rotation = MathF.Atan2(direction.Y, direction.X);
                IColisionObject? ifStuff = ColisionSystem.castRay(visionRangeMin, visionRangeMax, this);
                if (ifStuff != null)
                {
                    // asteroid is in raycast, avoid

                    randomDirection = Position + new Vector2(50, -50); // noch nich fest, aber das is die richtung
                    Movement(randomDirection, args);
                    turbo = true;

                } else
                {
                    // asteroid is not in raycast, move
                    Movement(target.Position, args);
                    turbo = false;
                }
                // Movement(target.Position, args);
            }
            return;
        }

        private void Movement(Vector2 inputMovement, FrameEventArgs args)
        {
            // WEEEE
            Vector2 direction = inputMovement - Position;
            direction.Normalize();
            if (turbo)
            {
                direction = direction * (float)args.Time * 15000;
            } else
            {
                direction = direction * (float)args.Time * 5000;
            }
            
            PhysicsSystem.ApplyForce(this, direction);
        }
    }
}
