using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace App.Engine;

public static class Loader
{
    public static string  LoadVertexShader(string name)
    {
        return File.ReadAllText (name);
    }
    public static string  LoadFragmentShader(string name)
    {
        return File.ReadAllText (name);
    }
    
    public static string  LoadComputeShader(string name)
    {
        return File.ReadAllText (name);
    }


    public static ImageResult LoadTexture(string name)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);


        ImageResult image = ImageResult.FromStream(File.OpenRead(name), ColorComponents.RedGreenBlueAlpha);
        return image;
    }

   
    
}






