using App.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame;
using RadarGame.Entities;
using RadarGame.Physics;

public class Camera : IEntitie, IDrawObject
{
    IEntitie target;
    private float zoom = 1;
    private float _rotation = 0;
    private Vector2 _position = new Vector2(0, 0);
    private Vector2 _baseSize = new Vector2(1920, 1080)* 0.5f;
    private Vector2 _size = new Vector2(1920, 1080) ;
    private float _maxZoom = 30;
    private float _minZoom = 10f;
    private Vector2 _shake = Vector2.Zero;
    private float _shakeActive = 0;
    private float _shakeIntensity = 0;
    Random random = new Random();
    
  
    // Smoothing factors for position and rotation
    private float positionSmoothing = 0.05f;
    private float rotationSmoothing = 0.01f;

    public Camera(IEntitie target)
    {
        Name = "Camera";
        this.target = target;
        EntityManager.AddObject( new SarBackground());
    }
    
    public Vector2 getMaxsize()
    {
        return _baseSize + new Vector2(200) * _maxZoom;
    }

    public string Name { get; set; }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {

        if (_shakeActive  > 0)
        {
          
            _shake = new Vector2((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * _shakeIntensity;

          
            _shakeActive -= (float)args.Time;
        }
        else
        {
            _shake = Vector2.Zero;
        }
       
      

    
        _position = Vector2.Lerp(_position, ((IPhysicsObject)target).Position, positionSmoothing);
        _position += _shake;
        zoom += zoom * 0.1f * mouseState.ScrollDelta.Y;
        zoom = Math.Clamp(zoom, _minZoom, _maxZoom);

        _size = _baseSize + new Vector2(200) * zoom;
    }
    public void shake( float intensity)
    {
        _shakeActive =1;
        intensity = Math.Clamp(intensity, 0, 100);
        _shakeIntensity = intensity;
    }
    

    
   
    public void onDeleted()
    {

    }

    public void Draw(List<View> surface)
    {
       // surface[1].rotation = -_rotation;
        surface[1].vpossition = _position;
        surface[1].vsize = _size;
    }
}