using App.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.graphics.Template
{
    public class TextureAtlasRectangle : DrawObject, IDisposable
    {
        private static VAO _sharedVAOcenter = null;
        private static VAO _sharedVAO = null;
        private static Shader _sharedShader = new Shader("resources/Template/simple_texture.vert",
            "resources/Template/texture_atlastexture.frag");
        
        private Vector2 _textureSize;
        private Vector2 _subTextures;

        public TextureAtlasRectangle(Vector2 position, Vector2 size, Vector2 subTextures , Texture? texture, string name = "AtlasedTexturedRectangle", Shader? shader= null)
        {
            this.drawInfo = new DrawInfo(position, size, 0, null, name);

            _textureSize = new Vector2(texture.Width, texture.Height);
            _subTextures = subTextures;
           
          


                if (_sharedVAOcenter == null)
                {
                    Bufferlayout bufferlayout = new Bufferlayout();
                    bufferlayout.count = 2;
                    bufferlayout.normalized = false;
                    bufferlayout.offset = 0;
                    bufferlayout.type = VertexAttribType.Float;
                    bufferlayout.typesize = sizeof(float);
                    _sharedVAOcenter = new VAO();
                    _sharedVAOcenter.LinkAtribute(new float[] { -0.5f, -0.5f, 0.5f, -0.5f, 0.5f, 0.5f, -0.5f, 0.5f },
                        bufferlayout);
                    _sharedVAOcenter.LinkAtribute(new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f },
                        bufferlayout);
                    _sharedVAOcenter.LinkElements(new uint[] { 0, 1, 2, 2, 3, 0 });
                }

                this.drawInfo.mesh = new Mesh(_sharedVAOcenter, 4, new uint[] { 0, 1, 2, 2, 3, 0 }, noCleanUp: true);

                if (shader != null)
                {
                    this.drawInfo.mesh.Shader = shader;
                }
                else
                {
                    this.drawInfo.mesh.Shader = _sharedShader;
                }
            
            

            if (texture != null) this.drawInfo.mesh.Texture = texture;
            

        }
        
        public TextureAtlasRectangle(Texture texture, Vector2 position, Vector2 subTextures  ,Shader? shader =null ) : this(position, new Vector2(texture.Width /subTextures.X,texture.Height / subTextures.Y ), subTextures ,  texture, "atlasrec",shader )
        {
        
        }
        
        public void setAtlasIndex(int indexX, int indexY)
        {
            this.drawInfo.mesh.Shader.Bind();
            this.drawInfo.mesh.Shader.setUniformV2f("index", new Vector2(indexX, indexY));
            this.drawInfo.mesh.Shader.setUniformV2f ("subimages", _subTextures);
        }
        
        
        

        public DrawInfo drawInfo { get; }

        public void Dispose()
        {
            drawInfo.Dispose();
        }
    }
}
