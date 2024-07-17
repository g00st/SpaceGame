using OpenTK.Graphics.OpenGL4;

namespace App.Engine;

public class VBO 
{
    private uint _handle;
    private Texture? _texture;
   
    
    public VBO(Texture texture)
    
    {
        GL.CreateFramebuffers(1, out _handle);
        ErrorChecker.CheckForGLErrors("VBO");
        this.linkTexure(texture);
    }
    
    public int Widht()
    {
        return _texture.Width;
    }
    public int Height()
    {
        return _texture.Height;
    }
    public VBO(uint handle)
    {
        _handle =  handle;
    }
  
    

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _handle);
        if (_texture != null)
        {
            GL.Viewport(0,0, _texture.Width, _texture.Height);
        }
    }
    
    public void Unbind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    public void linkTexure(Texture texture)
    {
        _texture = texture;
        ErrorChecker.CheckForGLErrors("vor VBOLINKTEXTURE");
        Console.Write("handel" + texture.Handle);
        GL.NamedFramebufferTexture(this._handle, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
        ErrorChecker.CheckForGLErrors("VBOLINKTEXTURE");
    }
    public void unLinkTexture()
    {
        _texture = null;
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, 0, 0);
}
   public static VBO VBO_0()
    {
        return new VBO(0);
    }
}