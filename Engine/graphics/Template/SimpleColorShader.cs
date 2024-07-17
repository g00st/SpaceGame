using OpenTK.Mathematics;

namespace App.Engine.Template;

public class SimpleColorShader : Shader
{
    private Color4 _color;
    public SimpleColorShader( Color4 color) : base("resources/Template/simple_MVP.vert",
        "resources/Template/single_color.frag")
    {
        _color = color;
        this.Bind();
        this.setUniform4v("u_Color", _color.R, _color.G, _color.B, _color.A);
    }
    
    public void setColor(Color4 color)
    {
        _color = color;
        this.Bind();
        this.setUniform4v("u_Color", _color.R, _color.G, _color.B, _color.A);
    }
}