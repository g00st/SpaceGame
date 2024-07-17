using App.Engine;
using App.Engine.ImGuisStuff;
using ImGuiNET;
using OpenTK.Mathematics;

namespace App.Engine;

public class DrawInfo :IDisposable
{
    static List<DrawInfo> _drawInfos = new List<DrawInfo>();
    private Vector2 _position;
    private Vector2 _size;
    private float _rotation;
    public Vector2 Position
    {
        get => _position;
        set{
            _position = value;
            Transform = GetTransform();}
    } 

    public Vector2 Size 
    {
        get => _size;
        set{
            _size = value;
            Transform = GetTransform();}
    }
    public float Rotation  
    {
        get => _rotation;
        set{
            _rotation = value;
            Transform = GetTransform();}
    }
    public Mesh mesh ;
    public string Name;
    public Matrix4 Transform {get; private set;}
    
    
    private Matrix4 GetTransform()
    {
        Matrix4 ObjectScalematrix = Matrix4.CreateScale(_size.X,_size.Y, 1.0f);
        Matrix4 ObjectRotaionmatrix = Matrix4.CreateRotationZ(_rotation);
        Matrix4 ObjectTranslationmatrix = Matrix4.CreateTranslation(_position.X,_position.Y,0);

        Matrix4 objectransform = Matrix4.Identity * ObjectScalematrix;
        objectransform *= ObjectRotaionmatrix;
        objectransform *= ObjectTranslationmatrix;
        return  objectransform;
    }
    
    public DrawInfo(Vector2 position, Vector2 size, float rotation, Mesh mesh, string name )
    {
        this.Position = position;
        this.Size = size;
        this.Rotation = rotation;
        this.mesh = mesh;
        _drawInfos.Add(this);
        this.Name = name;
        this.Transform = GetTransform();
    }
    
    
   public static void DebugDraw()
{
    ImGui.Begin("Debug Window");
    ImGui.BeginChild("scrolling", new System.Numerics.Vector2(0, 0), ImGuiChildFlags.Border, ImGuiWindowFlags.HorizontalScrollbar);

    foreach (var drawInfo in _drawInfos)
    {
        if (ImGui.CollapsingHeader(drawInfo.Name))
        {
            
            ImGui.PushID(drawInfo.GetHashCode());
            System.Numerics.Vector2 position = new System.Numerics.Vector2(drawInfo.Position.X, drawInfo.Position.Y);
            if (ImGui.SliderFloat2("Position", ref position, -1000.0f, 1000.0f))
            {
                drawInfo.Position = new Vector2(position.X, position.Y);
            }

            System.Numerics.Vector2 size = new System.Numerics.Vector2(drawInfo.Size.X, drawInfo.Size.Y);
            if (ImGui.SliderFloat2("Size", ref size, 0.1f, 1000.0f))
            {
                drawInfo.Size = new Vector2(size.X, size.Y);
            }

            float rotation = drawInfo.Rotation;
            if (ImGui.SliderFloat("Rotation", ref rotation, -360.0f, 360.0f))
            {
                drawInfo.Rotation = MathHelper.DegreesToRadians(rotation);
            }
            ImGui.PopID(); 
        }
    }

    ImGui.EndChild();
    ImGui.End();
}


   public void Dispose()
   {
       mesh.Dispose();
       _drawInfos.Remove(this);
   //    Console.WriteLine( "DrawInfo disposed");
       GC.SuppressFinalize(this);
   }
}