 using OpenTK.Mathematics;
using App.Engine;
using App.Engine.Template;
 using OpenTK.Graphics.ES11;
 using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
 using RadarGame.Physics;

 namespace RadarGame.Entities;

public class CompasPanel :IEntitie , IDrawObject
{
    private Vector2 Position;
    private Vector2 Size;
    private TexturedRectangle _compasPanel;
    private TexturedRectangle _compasScreenRender;
    private Texture _comasScreen;
    private View _compasView;
    private VBO _compasVbo;
    private Shader _compaShader = new Shader("resources/Compas/compas.vert",
        "resources/Compas/compas.frag");
    
    //-----------------Compas stuff-----------------
    private float compasRotation = 0;
    private Vector2 compasPosition;
    private IPhysicsObject _parent;
  
    //-----------------overlay -----------------
    private TexturedRectangle _overlay;
    
    public string Name { get; set; }
    
    public CompasPanel (Vector2 position ,Vector2 Size, string name = "CompasPanel")
    {
        Name = name;
        Position = position;
        this.Size = Size;
        
        //other surface render stuff
        _comasScreen = new Texture(500, 500);
        _compasVbo = new VBO(_comasScreen);
        _compasView = new View(_compasVbo);
        _compasScreenRender = new TexturedRectangle( new Vector2(0, 0), new Vector2(500, 500),null, _compaShader);
        compasPosition = new Vector2(0, 0);
        
        //real compas stuff
        _compasPanel = new TexturedRectangle(this.Position,  this.Size, _comasScreen);
        _overlay  = new TexturedRectangle(Position, this.Size, new Texture("resources/Compas/combasback.png"));
        
        
    }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        if (_parent == null)
        {  
           _parent = (IPhysicsObject)  EntityManager.GetObject( "Player"); 
           Console.WriteLine(_parent.Position);
        }else
        {
            compasPosition = _parent.Position;
            compasRotation = _parent.Rotation;
        }
      
        
        
 
    }

    public void onDeleted()
    {
        _compasPanel.Dispose();
        _compasScreenRender.Dispose();
        _comasScreen.Dispose();
        
    }

    public void Draw(List<View> surface)
    {
         _compaShader.Bind();
        _compaShader.setUniform1v( "u_Rotation", (float)Math.Tau- compasRotation);
       _compaShader.setUniformV2f( "u_Position", compasPosition);
       _compasVbo.Bind();
       GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );
       _compasView.Draw( _compasScreenRender);
       surface[2].Draw(_compasPanel);
       surface[2].Draw(_overlay);
       
       
    }
    
}