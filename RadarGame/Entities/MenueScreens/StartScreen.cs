using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System;
using Engine;
using RadarGame.UiSystem;

namespace RadarGame.Entities
{
    public class StartScreen : IDrawObject, IEntitie
    {
        private MenueButton StartButton;
        private MenueButton QuitButton;
        private TexturedRectangle Background;
        private TexturedRectangle Overlay;
        private Shader _shader;
        private float _time;
        private int counter = 0;
        private View _target;

        public StartScreen()
        {
            StartButton = new MenueButton(
                DrawSystem.DrawSystem.getViewSize(2) / 2,
                new Vector2(500, 100),
                "resources/Buttons/Start.png"
            );
            QuitButton = new MenueButton(
                DrawSystem.DrawSystem.getViewSize(2) / 2 + new Vector2(0, -150),
                new Vector2(500, 100),
                "resources/Buttons/Quit.png"
            );
            Name = "Startscreen";
            _target = DrawSystem.DrawSystem.GetView(2);
            _shader = new Shader("resources/Background/star_bg.vert", "resources/Background/star_bg.frag");
            Background = new TexturedRectangle(Vector2.Zero, new Vector2(_target.vsize.X, _target.vsize.Y), null, _shader, true);
            Overlay = new TexturedRectangle( Vector2.Zero, new Vector2(_target.vsize.X, _target.vsize.Y), new Texture("resources/Background/SpaceshipOverlay.png"),"Overlay", false);
        }

        public void Draw(List<View> surface)
        { 
            Background.drawInfo.Position = new Vector2(surface[2].vsize.X/2, surface[2].vsize.Y/2);
            Background.drawInfo.Size = new Vector2(surface[2].vsize.X, surface[2].vsize.Y); // Ensure the size matches the surface
            
            _shader.Bind();
            _shader.setUniformV2f("iResolution", new Vector2(surface[2].vsize.X, surface[2].vsize.Y));
            _shader.setUniform1v("iTime", _time); 
            
            
            surface[2].Draw(Background);
            surface[2].Draw(Overlay);
            _shader.Unbind();

            StartButton.Draw(surface[2]);
            QuitButton.Draw(surface[2]);
            //TextRenderer.Write("00000 abcdefgABCDEFG Hello 123" + counter, new Vector2(100, 100), new Vector2(30, 30), surface[2], Color4.White);
        }

        public string Name { get; set; }
        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {
            counter++;
            _time += (float)args.Time;
            if (QuitButton.Update( mouseState))
            {
                Console.WriteLine("QuitButton Pressed");
                EngineWindow.Quit();
            }
            if ( StartButton.Update( mouseState))
            {
                Console.WriteLine("StartButton Pressed");
                Gamestate.CurrState = Gamestate.State.Game;
            }
        }

        public void onDeleted()
        {
           StartButton.Dispose();
            QuitButton.Dispose();
            Background.Dispose();
            _shader.Dispose();
        }
    }
}
