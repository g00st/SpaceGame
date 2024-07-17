using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.Entities
{
    public class Background : IEntitie
    {
        public string Name { get; set; }
        private TexturedRectangle _background;
        private Shader _shader;
        private float _time;
        private View _target;

        public Background()
        {
            Name = "Background";
            _target = DrawSystem.DrawSystem.GetView(0); // Use layer 0
            var target2 = DrawSystem.DrawSystem.GetView(1);

            var size = (int)(Math.Sqrt(_target.vsize.X * _target.vsize.X + _target.vsize.Y * _target.vsize.Y) * 0.5f);
            Texture texture = new Texture(_target.Width, _target.Height);
            _shader = new Shader("resources/Background/star_bg.vert", "resources/Background/star_bg.frag");
            _background = new TexturedRectangle(target2.vpossition, new Vector2(1920, 1080), texture, _shader, true);
        }

        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {
            _time += (float)args.Time;
        }

        public void onDeleted()
        {
            _shader.Dispose();
            _background.Dispose();
        }

        public void Draw(View surface)
        {
            _shader.Bind();
            _shader.setUniformV2f("iResolution", new Vector2(surface.Width, surface.Height));
            _shader.setUniform1v("iTime", _time);
            _background.drawInfo.Position = new Vector2(surface.Width / 2, surface.Height / 2);
            _background.drawInfo.Size = new Vector2(surface.Width, surface.Height); // Ensure the size matches the surface
            surface.Draw(_background);
            _shader.Unbind();
        }
    }
}
