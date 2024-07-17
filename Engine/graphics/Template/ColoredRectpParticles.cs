using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine.Template;

public class ColoredRectpParticles : DrawObject , IDisposable
{
    
    public DrawInfo drawInfo { get; }
    private Bufferlayout posslayout;
    private int count;
    private ComputeShader computeShader;
    private float maxDistance;
    private float mas;
    private Vector2 gravity;
    private Vector2 wind;
    private Vector2 maxVelocity;
    private Vector4 color;
    

    public int  Count {get{return count;}}

    public void SetColor(Color4 color)
    {
        this.color = new Vector4(color.R, color.G, color.B, color.A);
        drawInfo.mesh.Shader.Bind();
        drawInfo.mesh.Shader.setUniform4v("u_Color", color.R, color.G, color.B, color.A) ;
    }

    public ColoredRectpParticles(Vector2 positon, Vector2 size, Color4 color,int count, string name = "ColoredRectPariclesystem" )
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
        this.count = count;
        this.drawInfo.mesh = new InstancedMesh(count);
        this.drawInfo.mesh.AddAtribute(bufferlayout, new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f });
        this.drawInfo.mesh.AddIndecies(new uint[] { 0, 1, 2, 2, 3, 0 });
        
        posslayout = new Bufferlayout();
        posslayout.count = 2;
        posslayout.normalized = false;
        posslayout.offset = 0;
        posslayout.type = VertexAttribType.Float;
        posslayout.typesize = sizeof(float);
        
        var f = new float[count*2];
        for (int i = 0; i < count; i++)
        {
            f[i*2] = i*100; 
            f[i*2+1] = 0.0f;
        }
        
        ((InstancedMesh) this.drawInfo.mesh).AddInstancedAtribute(posslayout,f,"poss");
        this.drawInfo.mesh.Shader = new Shader( "resources/Template/instanced_poss.vert","resources/Template/InstancedFrag.frag");
        SetColor(color);
        
        computeShader = new ComputeShader("resources/Template/ParticleUpdate.comp");
        computeShader.AddBuffer((int)((InstancedMesh) this.drawInfo.mesh).getBuffer(1),0);
        int vels = 0;
        GL.CreateBuffers( 1, out vels);
        float[] velos = new float[count*2];
        for (int i = 0; i < count; i++)
        {
            velos[i*2] = 0.0f;
            velos[i*2+1] = 0.0f;
        }
        this.gravity = new Vector2(0,0);
        this.wind = new Vector2(0,0);
        this.maxVelocity = new Vector2(10,10);
        this.maxDistance = 100;
        this.mas = 0.1f;
        
        GL.NamedBufferData(vels, count* sizeof(float)*2,velos ,BufferUsageHint.DynamicDraw );
        computeShader.AddBuffer(vels,1);
        computeShader.setUniformV2f( "gravity", new Vector2(0,0));
        computeShader.setUniformV2f( "wind", new Vector2(0,0));
        computeShader.setUniformV2f( "maxVelocity", new Vector2(10,10));
        computeShader.setUniform1v( "maxDistance", 100);
        computeShader.setUniform1v( "mas", 0.1f);
        computeShader.setUniform1v( "time",  System.DateTime.Now.Millisecond);
    }
    public void Update()
    {
        //Console.WriteLine("update");
       
        computeShader.Bind();
        computeShader.setUniform1v( "time",  System.DateTime.Now.Millisecond);
        computeShader.Dispatch(count,1,1);
        computeShader.wait();
       
          
           
       

    }
    public void setwWind(Vector2 wind)
    {
        this.wind = wind = this.drawInfo.Position - wind;
        computeShader.setUniformV2f("wind", wind);
    }
    public void setPositions(float[] positions)
    {
        ((InstancedMesh) this.drawInfo.mesh).ChangeInstancedAtributeData(posslayout,positions,"poss");
    }   
    
    public void DrawImguiDebug()
    {
        ImGui.Begin("Particle System");
        ImGui.Text("Particle Count: " + count);
        //add sliders for all atributes
        ImGui.PushID( this.GetHashCode());
        System.Numerics.Vector2 gravity = new System.Numerics.Vector2( this.gravity.X, this.gravity.Y);
        ImGui.SliderFloat2("Gravity", ref gravity, -1.0f, 1.0f);
        if (gravity.X != this.gravity.X || gravity.Y != this.gravity.Y)
        {
            this.gravity = new Vector2(gravity.X, gravity.Y);
            computeShader.setUniformV2f("gravity", new Vector2(gravity.X, gravity.Y));
        }
        System.Numerics.Vector2 wind = new System.Numerics.Vector2(this.wind .X, this.wind.Y);
        ImGui.SliderFloat2("Wind", ref wind, -1.0f, 1.0f);
        if (wind.X != this.wind.X || wind.Y != this.wind.Y)
        {
            this.wind = new Vector2(wind.X, wind.Y);
            computeShader.setUniformV2f("wind", new Vector2(wind.X, wind.Y));
        } 
        float maxVelocity = this.maxVelocity.X;
        ImGui.SliderFloat("Max Velocity", ref maxVelocity, 0.0f, 10.0f);
        if ( maxVelocity != this.maxVelocity.X)
        {
            this.maxVelocity = new Vector2(maxVelocity, maxVelocity);
            computeShader.setUniformV2f("maxVelocity", new Vector2(maxVelocity, maxVelocity));
        }
        float maxDistance = this.maxDistance;
        ImGui.SliderFloat("Max Distance", ref maxDistance, 0.0f, 1000.0f);
        if (maxDistance != this.maxDistance)
        {
            this.maxDistance = maxDistance;
            computeShader.setUniform1v("maxDistance", maxDistance);
        }
        System.Numerics.Vector4 color =  new System.Numerics.Vector4(this.color.X, this.color.Y, this.color.Z, this.color.W);
        ImGui.ColorPicker4("Color", ref color);
        SetColor(new Color4(color.X, color.Y, color.Z, color.W));
        ImGui.PopID();
        ImGui.End();   
    }

    public void Dispose()
    {
        drawInfo.Dispose();
        GC.SuppressFinalize(this);
    }
}