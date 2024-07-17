using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using App.Engine;
using App.Engine.Template;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RadarGame.Entities
{
    class LinearSlider : IEntitie, IDrawObject
    {
        public Vector2 Begin;
        public Vector2 End;
        public Vector2 Position;
        public float length;
        public TexturedRectangle slider;
        private Polygon _line;
        private Polygon _line2;
        private Polygon _line3;
        private float min;
        private float max;

        // Example Slider:
        // _radarRangeSlider = new LinearSlider(Position + new Vector2(Size.X - (50 +200), 50),
        // Position + new Vector2(Size.X - 50 , 50), new Texture("resources/Radar/slider_circular.png"),
        // new Vector2(Size.X / 25, Size.Y / (25 * 0.5f)), 1000, 10000);
        // _radarRangeSlider.Name = "slider";


        public string Name { get; set; }

        public float getValue()
        {
            //transform position betwine begina and end to value betwine min and max
            float dot = Vector2.Dot(Position - Begin, Vector2.Normalize(End - Begin));
            return MathHelper.Lerp(min, max, dot / length);
        }

        public enum State
        {
            Idle,
            hover,
            dragging
        }
        public State state;

        public LinearSlider(Vector2 begin, Vector2 end, Texture texture, Vector2 size, float min, float max)
        {
            Position = begin;
            Begin = begin;
            End = end;
            length = Vector2.Distance(begin, end);
            _line = Polygon.Line(begin, end, new SimpleColorShader(Color4.Red), "line");
            slider = new TexturedRectangle(Begin, size, texture, "RadarPanel_slider", true);
            this.min = min;
            this.max = max;

        }

        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {
            switch (state)
            {
                case State.Idle:
                    if (checkHover(mouseState))
                    {
                        state = State.hover;
                    }
                    break;
                case State.hover:
                    if (checkHover(mouseState))
                    {
                        if (mouseState.IsButtonDown(MouseButton.Left))
                        {
                            state = State.dragging;
                        }
                    }
                    else
                    {
                        state = State.Idle;
                    }
                    break;
                case State.dragging:
                    if (mouseState.IsButtonDown(MouseButton.Left))
                    {
                        Vector2 Slidervec = End - Begin;
                        Vector2 normSlidervec = Vector2.Normalize(Slidervec);
                        Vector2 transformed = DrawSystem.DrawSystem.GetView().ScreenToViewSpace(new Vector2(mouseState.X, mouseState.Y));
                        Vector2 mousevec = transformed - Begin;
                        float dot = Vector2.Dot(mousevec, normSlidervec);
                        dot = Math.Max(0, Math.Min(dot, length));
                        dot = Math.Clamp(dot, 0, length);
                        Position = Begin + dot * normSlidervec;

                    }
                    else
                    {
                        state = State.Idle;
                    }
                    break;
            }
        }


        private bool checkHover(MouseState mouseState)
        {
            var transformed = DrawSystem.DrawSystem.ScreenToWorldcord(new Vector2(mouseState.X, mouseState.Y), 2);
            // do colision with cirlce defined by slider.drawinfo.position and slider.drawinfo .size.max /2
            if (Vector2.Distance(transformed, slider.drawInfo.Position) < slider.drawInfo.Size.Y * 0.9 * 0.5)
            {
                return true;
            }
            return false;
        }

        public void Draw(List<View> surface)
        {
            slider.drawInfo.Position = Position;
            var old = slider.drawInfo.Size;
            slider.drawInfo.Rotation = (float)Math.Atan2(End.Y - Begin.Y, End.X - Begin.X);
            if (state == State.dragging || state == State.hover)
            {
                slider.drawInfo.Size = new Vector2(slider.drawInfo.Size.X * 1.1f, slider.drawInfo.Size.Y * 1.1f);
            }
            surface[2].Draw(slider);
            surface[2].Draw(_line);
            slider.drawInfo.Size = old;
        }

        public void onDeleted()
        {
            slider.Dispose();
            _line.Dispose();
            _line2.Dispose();
            _line3.Dispose();
        }
    }
}
