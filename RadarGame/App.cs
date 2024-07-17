using App.Engine;
using App.Engine.Template;
using Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Entities;
using RadarGame.DrawSystem;
using RadarGame.Physics;

//using RadarGame.SoundSystem;

namespace RadarGame;

public class App : EngineWindow
{
    
    /// <summary>
    /// Debug stufff
    /// </summary>
   private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
   private double fps = 0;
   private float[] fpsList = new float[100];
   private int fpsListindex = 0;
   private double _EntityTime = 0;
   private float[] _EntityTimeList = new float[100];
   private int _EntityTimeListIndex = 0;
    private double _PhysicsTime = 0;
    private float[] _PhysicsTimeList = new float[100];
    private int _PhysicsTimeListIndex = 0;
    
    private double RadarTime = 0;
    private  float[] RadarTimeList = new float[100];
    private int RadarTimeListIndex = 0;
    
    private double _DrawTime = 0;
    private float[] _DrawTimeList = new float[100];
    private int _DrawTimeListIndex = 0;
    private double _DebugDrawTime = 0;
    private double _ColisionTime = 0;
    private float[] _ColisionTimeList = new float[100];
    private int _ColisionTimeListIndex = 0;

    private int _AudioVolumeListIndex = 0;
    private float[] AudioVolumeList = new float[100];

 


    public App() : base(1440, 900, "Radargame"){
        Gamestate.CurrState = Gamestate.State.MainMenu;

        // SoundSystem.SoundSystem.TrySinusIsUnsafe();  // FUNZT :D
        //SoundSystem.SinusWave.SetUpSound(); // once per start
        // SoundSystem.SoundSystem.PlayFileDotWave(path); // probe wav is fehlerhaft? not sure yet
        WindowState = WindowState.Maximized;
        DrawSystem.DrawSystem.Init( MainView,3);
        //EntityManager.AddObject(new cursor());
        //EntityManager.AddObject(new PlayerObject( MainView.vpossition, 0f, "Player"));
        //EntityManager.AddObject( new cursor( "cursor4"));
        //EntityManager.AddObject(new CompasPanel( MainView.vsize - new Vector2(200, 200), new Vector2(150, 150), "CompasPanel"));
       // EntityManager.AddObject(new Mapp( new Vector2(1000), new Vector2(0,0)));
      
        
       /* for (int i = 0; i < 100; i++)
        {
            GameObject gameObject = new GameObject( MainView.vpossition, 0f, "test"+i, 0);
            EntityManager.AddObject(gameObject);
        }
        */
        EntityManager.AddObject(new StartScreen());

        // EntityManager.AddObject( new cursor( "cursore"));


    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {

       

        fpsList[fpsListindex] = 1/ (float)args.Time;
        fpsListindex = (fpsListindex + 1) % fpsList.Length;
        fps = 1/ args.Time;
        double time = args.Time;
        base.OnUpdateFrame(args);
        _stopwatch.Restart();
        EntityManager.Update(args, KeyboardState, MouseState);
        _EntityTime = _stopwatch.Elapsed.TotalMilliseconds;
        _EntityTimeList[_EntityTimeListIndex] = (float)_EntityTime;
        _EntityTimeListIndex = (_EntityTimeListIndex + 1) % _EntityTimeList.Length;
        _stopwatch.Restart();
        PhysicsSystem.Update(time);
        _PhysicsTime = _stopwatch.Elapsed.TotalMilliseconds;
        _PhysicsTimeList[_PhysicsTimeListIndex] = (float)_PhysicsTime;
        _PhysicsTimeListIndex = (_PhysicsTimeListIndex + 1) % _PhysicsTimeList.Length;
        _stopwatch.Restart();
        ColisionSystem.Update();
        _ColisionTime = _stopwatch.Elapsed.TotalMilliseconds;
        _ColisionTimeList[_ColisionTimeListIndex] = (float)_ColisionTime ;
        _ColisionTimeListIndex = (_ColisionTimeListIndex + 1) % _ColisionTimeList.Length;
        _stopwatch.Restart();
        //SoundSystem.SoundSystem.Update(args, KeyboardState);


    }

    protected override void Draw()
    {
        _stopwatch.Restart();
        DrawSystem.DrawSystem.Draw(MainView);
        _DrawTime = _stopwatch.Elapsed.TotalMilliseconds;
        _DrawTimeList[_DrawTimeListIndex] = (float) _DrawTime;
        _DrawTimeListIndex = (_DrawTimeListIndex + 1) % _DrawTimeList.Length;
      
        
    }
    
    protected override void Debugdraw()
    {
        
        
        ImGuiNET.ImGui.Begin("Debug");
        
        //offset slider 
        
        System.Numerics.Vector2 of = new System.Numerics.Vector2(MapPolygon.offset.X, MapPolygon.offset.Y);
            ImGuiNET.ImGui.DragFloat2("Offset"  , ref of, 0.1f);
         MapPolygon.offset = new Vector2(of.X, of.Y);   
            
        
        ImGuiNET.ImGui.Text("FPS: " + fps);
        ImGuiNET.ImGui.Text("Entity Update Time: " + _EntityTime);
        ImGuiNET.ImGui.Text("Physics Update Time: " + _PhysicsTime);
        ImGuiNET.ImGui.Text("Colision Update Time: " + _ColisionTime);
        
        //draw fps lineplot
        ImGuiNET.ImGui.PlotLines("FPS", ref fpsList[0], fpsList.Length, fpsListindex, "FPS", 0, 100,  new System.Numerics.Vector2(0, 100));
        ImGuiNET.ImGui.PlotLines("Entity Update Time", ref _EntityTimeList[0], _EntityTimeList.Length, _EntityTimeListIndex, "Entity Update Time", 0, 100,  new System.Numerics.Vector2(0, 100));
        ImGuiNET.ImGui.PlotLines("Physics Update Time", ref _PhysicsTimeList[0], _PhysicsTimeList.Length, _PhysicsTimeListIndex, "Physics Update Time", 0, 100,  new System.Numerics.Vector2(0, 100));
        ImGuiNET.ImGui.PlotLines("Colision Update Time", ref _ColisionTimeList[0], _ColisionTimeList.Length, _ColisionTimeListIndex, "Colision Update Time", 0, 100,  new System.Numerics.Vector2(0, 100));
        ImGuiNET.ImGui.PlotLines("Draw Time", ref _DrawTimeList[0], _DrawTimeList.Length, _DrawTimeListIndex, "Draw Time", 0, 100,  new System.Numerics.Vector2(0, 100));
        // Wenn Lautst�rke auslesbar hier verzeichnen bitte
        ImGuiNET.ImGui.PlotLines("LautSt�rke", ref AudioVolumeList[0], AudioVolumeList.Length, _AudioVolumeListIndex, "LautSt�rke", 0, 100, new System.Numerics.Vector2(0, 100));
        ImGuiNET.ImGui.End();
        Physics.PhysicsSystem.DebugDraw();
       // SoundSystem.SoundSystem.DebugDraw();
        EntityManager.DebugDraw();
        DrawSystem.DrawSystem.DebugDraw();
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        //SoundSystem.SinusWave.CleanUp();
    }
    
    
    
}