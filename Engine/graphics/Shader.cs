using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine;

struct UniformData
{
    public ActiveUniformType Type;
     public int Location;
}

public class Shader : IDisposable
{
    private static Shader currentShader = null;
    private int vertexHandle;
    private int fragmentHandle;
    private int _Handle;
    private Dictionary<string, UniformData> uniformLocations;
    
    public Shader(string vertex, string fragment)
    {
        uniformLocations = new Dictionary<string, UniformData>();
        string vertexData = Loader.LoadVertexShader(vertex);
        string fragementData = Loader.LoadFragmentShader(fragment);
        
         vertexHandle= GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexHandle, vertexData);
        GL.CompileShader(vertexHandle);
        
        fragmentHandle=  GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentHandle, fragementData);
        GL.CompileShader(fragmentHandle);
        

        int res =0;
        GL.GetShader(vertexHandle,ShaderParameter.CompileStatus, out  res);
        if (1 == res) { Console.WriteLine("vertex shader compiled: "+ vertex); }
        else
        {
            Console.WriteLine( " shader compilation error: "+ vertex + "--------------------------------------\n"+  GL.GetShaderInfoLog(vertexHandle) + "\n--------------------------------------");
            throw new  Exception("shader compilation error");
        }
        GL.GetShader(fragmentHandle,ShaderParameter.CompileStatus, out  res);
        if (1 == res) { Console.WriteLine("frag shader compiled: " +fragment); }
        else
        {
            Console.WriteLine(" shader compilation error: "+fragment  + "--------------------------------------\n"+ GL.GetShaderInfoLog(fragmentHandle)+ "\n--------------------------------------");
            throw new  Exception("shader compilation error");
        }

        _Handle = GL.CreateProgram();
        GL.AttachShader(_Handle,vertexHandle);
        GL.AttachShader(_Handle,fragmentHandle);
        GL.LinkProgram(_Handle);
        GL.ValidateProgram(_Handle);
        GenerateUniforms();
    }
    

    private void GenerateUniforms()
    {
        // Get the number of active uniforms in the shader program
        GL.GetProgram(_Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

        // Query and store uniform information
        for (int i = 0; i < uniformCount; i++)
        {
            string uniformName = GL.GetActiveUniform(_Handle, i, out _, out ActiveUniformType uniformType);
           // Console.WriteLine(uniformName +"  "+ uniformType);
            
            int location = GL.GetUniformLocation(_Handle, uniformName);
            UniformData t;
            t.Type = uniformType;
            t.Location = location;
            uniformLocations.Add(uniformName, t);

            // You can store the uniform type if needed
            // For example: uniformTypes.Add(uniformName, uniformType);
        }
    }

    public void Bind()
    {
       
        if (currentShader != this) GL.UseProgram(_Handle);
        currentShader = this;
        ErrorChecker.CheckForGLErrors("Shader Bind");
    } 
    public void Unbind (){GL.UseProgram(0);}

    
   
    public void setUniform1i(string name,int v1)
    {
        GL.Uniform1( GL.GetUniformLocation(_Handle, name),v1);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }
    public void setUniform1v(string name,float v1)
    {
        GL.Uniform1( GL.GetUniformLocation(_Handle, name),v1);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }

    public void setUniformV2f(string name, Vector2 v2)
    {
        GL.Uniform2( GL.GetUniformLocation(_Handle, name),v2);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }   
    
    public void setUniformM4(string name,Matrix4 v1)
    {
      
        GL.UniformMatrix4( GL.GetUniformLocation(_Handle, name),false,ref v1);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }   
    
    public void setUniform4v(string name,float v1,float v2,float v3,float v4)
    {
        GL.Uniform4( GL.GetUniformLocation(_Handle, name),v1,v2,v3,v4);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }
    
    public void setuniform2vArray(string name, Vector2[] v2)
    {
        float[] data = new float[v2.Length * 2];
        for (int i = 0; i < v2.Length; i++)
        {
            data[i * 2] = v2[i].X;
            data[i * 2 + 1] = v2[i].Y;
        }
        GL.Uniform2(GL.GetUniformLocation(_Handle, name), v2.Length, data);
        ErrorChecker.CheckForGLErrors("Shader Bind");
    }

    public void Dispose()
    {
        GL.DeleteProgram(_Handle);
        GL.DeleteShader(vertexHandle);
        GL.DeleteShader(fragmentHandle);
        GC.SuppressFinalize(this);
    }
}