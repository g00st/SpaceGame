using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine.Template;

public class ColoredRectangle : DrawObject , IDisposable
{

    public DrawInfo drawInfo { get; }

    public void SetColor(Color4 color)
    {
        ((SimpleColorShader)drawInfo.mesh.Shader).setColor(color);
    }

    public ColoredRectangle(Vector2 positon, Vector2 size, Color4 color, string name = "ColoredRectangle", bool centered = false)
    {
        this.drawInfo = new DrawInfo(positon, size, 0, null, name);
        this.drawInfo.Position = positon;
        this.drawInfo.Size = size;
        this.drawInfo.Rotation = 0;

        Bufferlayout bufferlayout = new Bufferlayout();
        bufferlayout.count = 2;
        bufferlayout.normalized = false;
        bufferlayout.offset = 0;
        bufferlayout.type = VertexAttribType.Float;
        bufferlayout.typesize = sizeof(float);

        this.drawInfo.mesh = new Mesh();
        if (centered)
        {
            this.drawInfo.mesh.AddAtribute(bufferlayout, new float[] { -0.5f, -0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f, 0.5f });
        }
        else
        {
        this.drawInfo.mesh.AddAtribute(bufferlayout, new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f });
        }
        this.drawInfo.mesh.AddIndecies(new uint[] { 0, 1, 2, 2, 3, 0 });
        this.drawInfo.mesh.Shader = new SimpleColorShader(color);

    }

    public void Dispose()
    {
        drawInfo.Dispose();
        GC.SuppressFinalize(this);
    }
}
    
   