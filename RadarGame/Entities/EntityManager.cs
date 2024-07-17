using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;


namespace RadarGame.Entities;

public static class EntityManager
{
    public static List<IEntitie> GameObjects { get; set; } = new List<IEntitie>();
    
    private static List<IEntitie> _toDelete = new List<IEntitie>();
    private static List<IEntitie>_toRemove = new List<IEntitie>();
    private static List<IEntitie> _toAdd = new List<IEntitie>();
    public static List<String> Names { get; set; } = new List<string>();


    public static List<IEntitie> GetObjects()
    {
        return new List<IEntitie>(GameObjects);
    }
    public static void RemoveAllObjects()
    {
        foreach (var gameObject in GameObjects)
        {
            EntityManager.RemoveObject(gameObject);
        }
    }

    public static void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.Update(args, keyboardState, mouseState);
        }
        foreach (var gameObject in _toAdd)
        {
            GameObjects.Add(gameObject);
            if (gameObject is IPhysicsObject physicsObject)
            {
                Physics.PhysicsSystem.AddObject(physicsObject);
            }

            if (gameObject is IDrawObject drawObject)
            {
                DrawSystem.DrawSystem.AddObject(drawObject);
            }
            if (gameObject is IColisionObject colisionObject)
            {
                Physics.ColisionSystem.AddObject(colisionObject);
            }
          
        }
        
        foreach (var gameObject in _toDelete)
        {
            if (gameObject is IPhysicsObject physicsObject)
            {
                Physics.PhysicsSystem.RemoveObject(physicsObject);
            }

            if (gameObject is IDrawObject drawObject)
            {
                DrawSystem.DrawSystem.RemoveObject(drawObject);
            }
            if (gameObject is IColisionObject colisionObject)
            {
                Physics.ColisionSystem.RemoveObject(colisionObject);
            }
          
            gameObject.onDeleted();
            GameObjects.Remove(gameObject);
        }
        foreach (var gameObject in _toRemove)
        {
            if (gameObject is IPhysicsObject physicsObject)
            {
                Physics.PhysicsSystem.RemoveObject(physicsObject);
            }

            if (gameObject is IDrawObject drawObject)
            {
                DrawSystem.DrawSystem.RemoveObject(drawObject);
            }
            if (gameObject is IColisionObject colisionObject)
            {
                Physics.ColisionSystem.RemoveObject(colisionObject);
            }
            GameObjects.Remove(gameObject);
        }
        
        _toDelete.Clear();
        _toAdd.Clear();
        _toDelete.Clear();
        _toRemove.Clear();
        
    }
    
    public static void AddObject(IEntitie gameObject)
    {
        if (Names.Contains(gameObject.Name))
        {
            throw new Exception("Name already exists");
        }
        Names.Add(gameObject.Name);
        _toAdd.Add(gameObject);
    }
    public static void DeleteObject(IEntitie gameObject)
    {
        Names.Remove(gameObject.Name);
        if (!_toDelete.Contains(gameObject))
            _toDelete.Add(gameObject);
    }
    public static void RemoveObject(IEntitie gameObject)
    {
        Names.Remove(gameObject.Name);
        if (!_toRemove.Contains(gameObject))
            _toRemove.Add(gameObject);
    }
    
   
    

    public static IEntitie GetObject(string name)
    {
        return GameObjects.Find(x => x.Name == name);
    }
    public static List<T> GetObjectsbyType<T>( T type)
    {
        return GameObjects.FindAll(x => x is T).Cast<T>().ToList();
    }
   


    public static void ClearObjects()
    {
        foreach (var gameObject in GameObjects)
        {
           EntityManager.DeleteObject( gameObject);
        }
       
    }
    
    public static void DebugDraw()
    {
        ImGuiNET.ImGui.Begin("EntityManagerDebug");
        ImGuiNET.ImGui.Text("GameObjects: " + GameObjects.Count);
        ImGuiNET.ImGui.Text("Names: " + Names.Count);
        ImGuiNET.ImGui.Text("toDelete: " + _toDelete.Count);
        ImGuiNET.ImGui.Text("toRemove: " + _toRemove.Count);
        ImGuiNET.ImGui.Text("toAdd: " + _toAdd.Count);
        
        //mak loop for all objects
        foreach (var gameObject in GameObjects)
        {
            ImGuiNET.ImGui.Text(gameObject.Name);
        }
        ImGuiNET.ImGui.End();
      
    }
}