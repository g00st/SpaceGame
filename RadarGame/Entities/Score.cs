using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using App.Engine;
using App.Engine.Template;

namespace RadarGame.Entities
{
    public class Score : IDrawObject, IEntitie
    {
        private int scorePoints;
        Vector2 Position1;
        Vector2 Position2;

        public string Name { get ; set ; }

        public Score(int currentscore, Vector2 Position1_, Vector2 Position2_)
        {
            scorePoints = currentscore;
            Name = "Score";
            Position1 = Position1_;
            Position2 = Position2_;
        }

        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {
            
        }

        public void Draw(List<View> surface)
        {
            TextRenderer.Write("Score: " + scorePoints, Position1, Position2, surface[2], Color4.White);
        }

        public void AddPoint()
        {
            scorePoints++;
            Console.WriteLine(scorePoints);
        }

        public void ScoreReset()
        {
            scorePoints = 0;
        }


        public void onDeleted()
        {
            UiSystem.ScoreSystem.addHighscore(scorePoints);
        }
    }
}
