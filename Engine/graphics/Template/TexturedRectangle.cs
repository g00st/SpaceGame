using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine.Template;

public class TexturedRectangle : DrawObject , IDisposable
{
    private static VAO _sharedVAOcenter =null;
    private static VAO _sharedVAO = null;
    private static Shader _sharedShader = new Shader("resources/Template/simple_texture.vert",
        "resources/Template/simple_texture.frag");
    
    public DrawInfo drawInfo { get; }

    public TexturedRectangle(Vector2 positon, Vector2 size, Texture? texture, string name = "TexturedRectangle", bool originCenter = false)
    {
        this.drawInfo = new DrawInfo(new Vector2(), new Vector2(), 0, null, name);
        this.drawInfo.Position = positon;
        this.drawInfo.Size = size;
        this.drawInfo.Rotation = 0;
       
        if(originCenter)
        {
            if (_sharedVAOcenter == null)
            {
                Bufferlayout bufferlayout = new Bufferlayout();
                bufferlayout.count = 2;
                bufferlayout.normalized = false;
                bufferlayout.offset = 0;
                bufferlayout.type = VertexAttribType.Float;
                bufferlayout.typesize = sizeof(float);
                _sharedVAOcenter = new VAO();
                _sharedVAOcenter.LinkAtribute( new float[] { -0.5f, -0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f, 0.5f }, bufferlayout);
                _sharedVAOcenter.LinkAtribute( new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f }, bufferlayout);
                _sharedVAOcenter.LinkElements(new uint[] { 0, 1, 2, 2, 3, 0 });
            }
            this.drawInfo.mesh = new Mesh(_sharedVAOcenter, 4, new uint[] { 0, 1, 2, 2, 3, 0 },  noCleanUp: true);
            
        }
        else
        {
            if (_sharedVAO == null)
            {
                Bufferlayout bufferlayout = new Bufferlayout();
                bufferlayout.count = 2;
                bufferlayout.normalized = false;
                bufferlayout.offset = 0;
                bufferlayout.type = VertexAttribType.Float;
                bufferlayout.typesize = sizeof(float);
                _sharedVAO = new VAO();
                _sharedVAO.LinkAtribute(new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f },
                    bufferlayout);
                _sharedVAO.LinkAtribute(new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f },
                    bufferlayout);
                _sharedVAO.LinkElements(new uint[] { 0, 1, 2, 2, 3, 0 });
            }

            this.drawInfo.mesh = new Mesh(_sharedVAO , 4, new uint[] { 0, 1, 2, 2, 3, 0 }, noCleanUp: true);
        }
         this.drawInfo.mesh.Shader = _sharedShader;
        if (texture != null) this.drawInfo.mesh.Texture = texture;
    }

    public TexturedRectangle(Texture texture, bool centerd =false) : this(new Vector2(0, 0), new Vector2(texture.Width, texture.Height), texture ,"TexturedRectangele", centerd )
    {
        
    }
    
    public TexturedRectangle(Vector2 positon, Vector2 size, Texture? texture, Shader shader, bool centered = false) : this(positon, size, texture, "TexturedRectangle", centered)
    {
        this.drawInfo.mesh.Shader = shader;
    }
    

    public void Dispose()
    {
        drawInfo.Dispose();
    }
}