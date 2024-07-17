using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine;

public class ComputeShader
{
    private int  _ProgramHandle;
    private int _ShaderHandle;
    private Dictionary<int,int> BufferHandles = new Dictionary<int, int>() ;
    private Dictionary<int, int> ImageHandles = new Dictionary<int, int>();

    public ComputeShader(string path)
    {
        _ProgramHandle = GL.CreateProgram();
        string source = Loader.LoadComputeShader(path);
        _ShaderHandle = GL.CreateShader(ShaderType.ComputeShader);
        GL.ShaderSource(_ShaderHandle, source);
        GL.CompileShader(_ShaderHandle);
        int res =0;
        GL.GetShader(_ShaderHandle,ShaderParameter.CompileStatus, out  res);
        if (1 == res) { Console.WriteLine("Compute shader compiled: "); }
        else
        {
            Console.WriteLine( " compute shader compilation error: "+  "--------------------------------------\n"+  GL.GetShaderInfoLog(_ShaderHandle) + "\n--------------------------------------");
            throw new  Exception("shader compilation error");
        }
        
        GL.AttachShader(_ProgramHandle,_ShaderHandle);
        GL.LinkProgram(_ProgramHandle);
        GL.ValidateProgram(_ProgramHandle);
        
        
    }

    public void Dispatch(int x, int y, int z)
    {
        
        GL.UseProgram(_ProgramHandle);
        GL.DispatchCompute(x, y, z);
        
    }
    public void AddBuffer(int buffer, int location)
    {
        BufferHandles.Add(location,buffer);
    }
    public void AddImage(int texture, int location, TextureAccess access)
    {
        ImageHandles.Add(location,texture);
    }
    
    public void BindImage(int slot, int texture, TextureAccess access)
    {
        GL.BindImageTexture(slot, texture, 0, false, 0, access, SizedInternalFormat.Rgba8);
    }
    public void BindBuffer(int slot, int buffer)
    {
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, slot, buffer);
        ErrorChecker.CheckForGLErrors("BindBuffer");
    }

    public void Bind()
    {
        foreach (var buffer in BufferHandles)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, buffer.Key, buffer.Value);            
        }

        foreach (var image in ImageHandles)
        {
            GL.BindImageTexture(image.Key, image.Value, 0, false, 0, TextureAccess.ReadWrite,
                SizedInternalFormat.Rgba8);
        }
        
    }
    public void  wait()
    {
        
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
    }
    public void setUniform1i(string name,int v1)
    {
        GL.UseProgram(_ProgramHandle);
        GL.Uniform1( GL.GetUniformLocation(_ProgramHandle, name),v1);
    }
    public void setUniform1v(string name,float v1)
    {
        GL.UseProgram(_ProgramHandle);
        GL.Uniform1( GL.GetUniformLocation(_ProgramHandle, name),v1);
    }

    public void setUniformV2f(string name, Vector2 v2)
    {
        GL.UseProgram(_ProgramHandle);
        GL.Uniform2( GL.GetUniformLocation(_ProgramHandle, name),v2);
    }   
    
    public void setUniformM4(string name,Matrix4 v1)
    {
        GL.UseProgram(_ProgramHandle);
        GL.UniformMatrix4( GL.GetUniformLocation(_ProgramHandle, name),false,ref v1);
    }   
    
    public void setUniform4v(string name,float v1,float v2,float v3,float v4)
    {
        GL.UseProgram(_ProgramHandle);
        GL.Uniform4( GL.GetUniformLocation(_ProgramHandle, name),v1,v2,v3,v4);
        
    }   
}