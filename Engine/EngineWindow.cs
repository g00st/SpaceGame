using App.Engine;
using App.Engine.ImGuisStuff;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using BlendingFactor = OpenTK.Graphics.OpenGL4.BlendingFactor;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using EnableCap = OpenTK.Graphics.OpenGL4.EnableCap;
using FramebufferTarget = OpenTK.Graphics.OpenGL4.FramebufferTarget;
using GL = OpenTK.Graphics.OpenGL4.GL;

namespace Engine;

public class EngineWindow : GameWindow
{
    public static EngineWindow Instance;
    ImGuiController _controller;
    protected View MainView = new View();
    private const int TargetFPS = 60; // Set your target FPS here
    private DateTime _lastFrameTime;
    protected bool _debug = true;


    public static void Quit()
    {
        Instance.Close();
    }
    public EngineWindow(int width, int height, string title) 
        : base(
            new GameWindowSettings() 
            {
                UpdateFrequency = 60.0 
            },
            new NativeWindowSettings() 
            { 
                ClientSize = ( width, height), 
                Title = "hi", 
                Profile = ContextProfile.Core 
            })
    {
       //ErrorChecker.InitializeGLDebugCallback();
        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        this.Resize += e => this.resize();
        GL.Enable(EnableCap.Blend);
    }
    
    
    
    void resize()
    {
        _controller.WindowResized(ClientSize.X, ClientSize.Y);
        MainView.Resize(ClientSize.X, ClientSize.Y);
    }
    
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        _controller.PressChar((char)e.Unicode);
        if ((char)e.Unicode == 'p')
        {
            _debug = !_debug;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        _controller.MouseScroll(e.Offset);
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {   
     
        base.OnRenderFrame(args);  
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);  
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Draw();  
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0) ;
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        if (_debug)
        {
           
            _controller.Update(this, (float)args.Time);
            Debugdraw();
            ImGuiController.CheckGLError("End of frame");
            DrawInfo.DebugDraw();
            _controller.Render();
        }
      
        this.SwapBuffers();

    }
    
    protected virtual void Debugdraw()
    {
       
    }
    protected virtual void Draw()
    {
       
    }
 

    

}
