using App.Engine;
using OpenTK.Mathematics;
using DrawElementsType = OpenTK.Graphics.OpenGL4.DrawElementsType;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;


namespace App.Engine;

public class Mesh : IDisposable
{
    private static VAO currentVAO;
    //--------------------------------------------------------------------------------------------------------
    public static System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
    
    public static int _drawcalls = 0;
    public static float _drawcalltimeavg = 0;
    public static float _drawcalltime = 0;
    public static float _VAOtime = 0;
    public static float _VAOtimeavg = 0;
    public static float _SHADERtime = 0;
    public static float _SHADERtimeavg = 0;
    public static float _TEXTUREtime = 0;
    public static float _TEXTUREtimeavg = 0;
    public static float  _fULLtime = 0;


    //--------------------------------------------------------------------------------------------------------
    public Mesh( VAO vao = null, int verteciesLenght = 0, uint[] Indecies = null, bool noCleanUp = false)
    {   
        _texture = new List<Texture>();
        _Vertecies = new List<float[]>();
        _vao = vao ?? new VAO(); 
        _verteciesLenght = verteciesLenght;
        _Indecies = Indecies;
        this.noCleanUp = noCleanUp;
    }
    
   
    
    protected PrimitiveType _primitiveType = PrimitiveType.Triangles; 
    protected int _verteciesLenght;
    protected Shader _shader;
    protected VAO _vao;
    protected  List<Texture> _texture;
    protected Matrix4 _MVP;
    protected List<float[]> _Vertecies;
    protected uint[] _Indecies;
    protected bool noCleanUp = false;
    
    
    public Shader Shader
    {
        get { return _shader; }
        set { _shader = value; }
    }

    public Texture Texture
    {
        get { return _texture[0]; }
        set { _texture.Add(value); }
    }
    
    public PrimitiveType PrimitiveType
    {
        get { return _primitiveType; }
        set { _primitiveType = value; }
    }
    
    


    public void AddAtribute(Bufferlayout bufferlayout, float[] data)
    {
        if (_verteciesLenght != 0 && _verteciesLenght != data.Length/bufferlayout.count)
        {
            throw new ArgumentException("Atributes must be same lenght " + data.Length/bufferlayout.count + "   " + _verteciesLenght );
        }

        _verteciesLenght = data.Length/bufferlayout.count;
        _vao.LinkAtribute(data,bufferlayout);
       _Vertecies.Add(data);
        
    }
    
    public uint getBuffer(uint index)
    {
        return _vao.GetBuffer(index);
    }
    

    public void AddIndecies(uint[] ind)
    {
        _vao.LinkElements(ind);
        _Indecies = ind;
    }
    
    


    public virtual void Draw( DrawInfo drawInfo, Matrix4 view,Matrix4 Projection)
    {
    
        _stopwatch.Restart();
        if (currentVAO != _vao)
        {
            _vao.Bind();
            currentVAO = _vao;
        }
        _VAOtime += _stopwatch.ElapsedTicks;
        _stopwatch.Restart();
        uint count = 0;
        foreach (var VARIABLE in _texture)
        {
            
            VARIABLE.Bind(count);
            count++;
        }
        _TEXTUREtime += _stopwatch.ElapsedTicks;
        _stopwatch.Restart();
        Matrix4 Model = drawInfo.Transform;
        
        _shader.Bind();
        
        _shader.setUniformM4("u_MVP",  Model * view * Projection);
        _SHADERtime += _stopwatch.ElapsedTicks;
        _stopwatch.Restart();
        GL.DrawElements(_primitiveType, _Indecies.Length, DrawElementsType.UnsignedInt, 0);
        _drawcalltime += _stopwatch.ElapsedTicks;
        
        _drawcalls++;

    }


    public void Dispose()
    {
        if (!noCleanUp)
        {
            _vao.Dispose();
            foreach (var VARIABLE in _texture)
            {
                VARIABLE.Dispose();
            }
        }
        GC.SuppressFinalize(this);
    }
}