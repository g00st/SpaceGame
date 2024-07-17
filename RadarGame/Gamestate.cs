using App.Engine;
using OpenTK.Mathematics;
using RadarGame.Entities;
using RadarGame.Entities.Enemys;
using RadarGame.Physics;
using System.Collections.Generic;
using System;

namespace RadarGame
{
    public class Gamestate
    {
        private static State _currState;
        private static List<IEntitie> currentEntities = new List<IEntitie>();
        public static State CurrState
        {
            get => _currState;
            set => onStateChange(value);
        }
        public enum State
        {
            MainMenu,
            Game,
            Pause,
            GameOver
        }

        public static void onStateChange(State neState)
        {
            if (neState == _currState)
            {
                return;
            }
            Console.WriteLine("State Changed to: " + neState);
            switch (neState)
            {
                case State.MainMenu:
                    foreach (var entity in currentEntities)
                    {
                        entity.onDeleted();
                    }
                    EntityManager.ClearObjects();
                    EntityManager.AddObject(new StartScreen());
                    break;
                case State.Game:
                    if (CurrState == State.MainMenu)
                    {
                        EntityManager.ClearObjects();
                        // EntityManager.AddObject(new cursor());
                        EntityManager.AddObject(new PlayerObject(Vector2.Zero, 0f, "Player"));
                        EntityManager.AddObject(new CompasPanel(DrawSystem.DrawSystem.getViewSize(2) - new Vector2(200, 200), new Vector2(150, 150), "CompasPanel"));
                        EntityManager.AddObject(new Pauser());
                        EntityManager.AddObject(new Score(0, new Vector2(100, 100), new Vector2(30, 30)));
                        EntityManager.AddObject(new Mapp(new Vector2(50000, 50000), Vector2.Zero));
                        EntityManager.AddObject(new EnemyManager());
                    }
                    else if (CurrState == State.Pause)
                    {
                        foreach (var g in currentEntities)
                        {
                            Console.WriteLine("Adding Object: " + g.Name);
                            EntityManager.AddObject(g);
                        }
                        EntityManager.DeleteObject(EntityManager.GetObject("PauseScreen"));
                    }
                    break;
                case State.Pause:
                    currentEntities = EntityManager.GetObjects();
                    EntityManager.RemoveAllObjects();
                    EntityManager.AddObject(new PauseScreen());
                    break;
                case State.GameOver:
                    EntityManager.ClearObjects();
                    EntityManager.AddObject(new GameoverScreen());
                    break;
            }
            _currState = neState;
        }
    }
}
