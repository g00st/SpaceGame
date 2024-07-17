using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace App.Engine;

public class Texture : IDisposable
{
    private uint _handle;
    private int _width;

    public int Width => _width;

    public int Height => _height;

    private int _height;

    public uint Handle => _handle;
    public IntPtr PtrHandle => (IntPtr)_handle;
    public Texture(string name)
    {
       ImageResult image =  Loader.LoadTexture(name);
        GL.CreateTextures(TextureTarget.Texture2D, 1,out _handle);
        ErrorChecker.CheckForGLErrors("Texture ");
        GL.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TextureParameter(_handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        _height = image.Height;
        _width = image.Width;
        GL.TextureStorage2D(_handle, 1, SizedInternalFormat.Rgba8,  image.Width, image.Height);
        GL.TextureSubImage2D(_handle, 0, 0, 0, image.Width, image.Height, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        GL.GenerateTextureMipmap(_handle);
        ErrorChecker.CheckForGLErrors("Texture ");

    }

    public Texture(int width, int height)
    {
        _width = width;
        _height = height;
        GL.CreateTextures(TextureTarget.Texture2D, 1,out _handle);
        GL.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TextureParameter(_handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(_handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TextureStorage2D(_handle, 1, SizedInternalFormat.Rgba8,  width, height);
        ErrorChecker.CheckForGLErrors("Texture Bind");
    }

    

        public void Bind(uint slot)
    {
        GL.BindTextureUnit(slot,_handle);
        ErrorChecker.CheckForGLErrors("Texture Bind");
    }

    public void Dispose()
    {
        GL.DeleteTexture(_handle);
        ErrorChecker.CheckForGLErrors("Texture Bind");
    }
}