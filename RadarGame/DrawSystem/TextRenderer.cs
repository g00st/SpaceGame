using App.Engine;
using Engine.graphics.Template;
using OpenTK.Mathematics;

namespace RadarGame.Entities;

public class TextRenderer
{
    
    
    static TextureAtlasRectangle _texturedRectangle = new TextureAtlasRectangle(
        new OpenTK.Mathematics.Vector2(0f, 0f),
        new OpenTK.Mathematics.Vector2(500f, 500f),
        new OpenTK.Mathematics.Vector2(8, 12),
        new Texture("resources/Textrenderer/nullptr_hq4x.png"),
        "TextChars",
         new Shader("resources/Template/simple_texture.vert",
            "resources/Textrenderer/Text.frag")
        
    );
    
    public static void Write(string text, OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 size,  View surface,Color4 color , bool centered = false)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int ch = (int)text[i];
            ch -= 32;
            
            _texturedRectangle.setAtlasIndex(ch % 8, 11-ch / 8);
            _texturedRectangle.drawInfo.mesh.Shader.setUniform4v("color", color.R,color.G,color.B,color.A);
            if (centered)
            {
                _texturedRectangle.drawInfo.Position = position + new OpenTK.Mathematics.Vector2(i * size.X,  0)- new OpenTK.Mathematics.Vector2(text.Length * size.X / 2 -size.X / 2, 0);
            }
            else
            {
                _texturedRectangle.drawInfo.Position = position + new OpenTK.Mathematics.Vector2(i * size.X, 0);
            }
            _texturedRectangle.drawInfo.Size = size;
            _texturedRectangle.drawInfo.Rotation = 0;
            surface.Draw(_texturedRectangle);
        }
        
    }
    
}