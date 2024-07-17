using System.Drawing;
using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;


namespace RadarGame.Entities;

public class MapPolygon : IEntitie , IDrawObject , IColisionObject
{
    private Polygon _polygon;
    public static Vector2 offset = new Vector2(0,0);
    private ColoredRectangle _debugColoredRectangle;
    private float _rotation;
    //private static Shader _shader = new SimpleColorShader(Color4.Gray);
    private static Texture _texture = new Texture("resources/Map/moon.png");
    private static Shader _shader = new Shader("resources/Template/simple_texture.vert", "resources/Map/Map.frag");
    private static Random _random = new Random();
    private float rand;
    public List<Vector2> CollisonShape { get; set; }
    public void OnColision(IColisionObject colidedObject)
    {
        return;
    }

    public bool Static { get; set; }
    public Vector2 Position { get; set; }

    float IColisionObject.Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    public Vector2 Rotation { get; set; }
    public Vector2 Size { get; set; }
    public List<Vector2> Points { get; set; }
    public List<Vector2> ShaderPoints { get; set; }
    public string Name { get; set; }
    public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
    {
       return;
    }

    public void onDeleted()
    {
        //_polygon.Dispose();
    }

    public MapPolygon(List<Vector2> points  , Vector2 position, Vector2 rotation, Vector2 Size ,string name)
    {
        Points = points;
        Position = position;
        Rotation = rotation;
        Name = name;
        _polygon = new Polygon(points, _shader, Position, Size , 0,name +"poly", lines:false);
        _polygon.drawInfo.mesh.Texture = _texture;
        rand = (float)_random.NextDouble();
        
     /*   _debugColoredRectangle = new ColoredRectangle(
            Position,
            new Vector2(10, 10),
            Color4.Aqua,
            Name +"Center",
            true
        );
        */
        //scale to size
        
        //shaderpoints is Points + 1,1 
        ShaderPoints =  new List<Vector2>(points.ConvertAll( x => x + new Vector2(1,1)));
        
        
        
        CollisonShape = new List<Vector2>(points);
        for (int i = 0; i < CollisonShape.Count; i++)
        {
            CollisonShape[i] *= Size;
        }
        Static = true;
    }
    
    public static MapPolygon CreateRandomPolygon(Vector2 center, Vector2 maxwidth, float minradius, int pointCount, string name , Random random)
    {
        List<Vector2> points = GenerateRandomConvexPolygon(new Vector2(1,1), pointCount, random);
        
        
        
        
        //find center
        Vector2 sum = new Vector2(0,0);
        foreach (var point in points)
        {
            sum += point;
        }
        var center2 = sum / points.Count;
        
        //move to center
        for (int i = 0; i < points.Count; i++)
        {
            points[i] -= center2;
        }
        //fix minwidth if any point is outside of center + /minwidth

        foreach (int i in Enumerable.Range(0, points.Count))
        {
            var point = points[i];
            if (point.Length < 1/minradius)
            {
                point.Normalize();
                point *= 1/minradius;
            }
        }
        
        return new MapPolygon(points, center, new Vector2(0,0),maxwidth ,name);
        return null;
    }

   
    
     public static List<Vector2> GenerateRandomConvexPolygon(Vector2 bounds, int n, Random RAND)
    {
        // Generate two lists of random X and Y coordinates
        List<double> xPool = new List<double>(n);
        List<double> yPool = new List<double>(n);

        for (int i = 0; i < n; i++)
        {
            xPool.Add(RAND.NextDouble() * bounds.X);
            yPool.Add(RAND.NextDouble() * bounds.Y);
        }

        // Sort them
        xPool.Sort();
        yPool.Sort();

        // Isolate the extreme points
        double minX = xPool[0];
        double maxX = xPool[n - 1];
        double minY = yPool[0];
        double maxY = yPool[n - 1];

        // Divide the interior points into two chains & Extract the vector components
        List<double> xVec = new List<double>(n);
        List<double> yVec = new List<double>(n);

        double lastTop = minX, lastBot = minX;

        double x;
        for (int i = 1; i < n - 1; i++)
        {
            x = xPool[i];

            if (RAND.NextDouble() < 0.5)
            {
                xVec.Add(x - lastTop);
                lastTop = x;
            }
            else
            {
                xVec.Add(lastBot - x);
                lastBot = x;
            }
        }

        xVec.Add(maxX - lastTop);
        xVec.Add(lastBot - maxX);

        double lastLeft = minY, lastRight = minY;

        double y;
        for (int i = 1; i < n - 1; i++)
        {
            y = yPool[i];

            if (RAND.NextDouble() < 0.5)
            {
                yVec.Add(y - lastLeft);
                lastLeft = y;
            }
            else
            {
                yVec.Add(lastRight - y);
                lastRight = y;
            }
        }

        yVec.Add(maxY - lastLeft);
        yVec.Add(lastRight - maxY);

        // Randomly pair up the X- and Y-components
        Shuffle(yVec, RAND);

        // Combine the paired up components into vectors
        List<Vector2> vec = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            vec.Add(new Vector2((float)xVec[i], (float)yVec[i]));
        }

        // Sort the vectors by angle
        vec.Sort((v1, v2) => Math.Atan2(v1.Y, v1.X).CompareTo(Math.Atan2(v2.Y, v2.X)));

        // Lay them end-to-end
        
        x = 0;
        y = 0;
        double minPolygonX = 0;
        double minPolygonY = 0;
        List<Vector2> points = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            points.Add(new Vector2((float)x, (float)y));

            x += vec[i].X;
            y += vec[i].Y;

            minPolygonX = Math.Min(minPolygonX, x);
            minPolygonY = Math.Min(minPolygonY, y);
        }

        // Move the polygon to the original min and max coordinates
        double xShift = minX - minPolygonX;
        double yShift = minY - minPolygonY;

        for (int i = 0; i < n; i++)
        {
            Vector2 p = points[i];
            points[i] = new Vector2(p.X + (float)xShift, p.Y + (float)yShift);
        }

        return points;
    }

    // Fisher-Yates shuffle algorithm for shuffling the Y components
    private static void Shuffle<T>(IList<T> list, Random RAND)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RAND.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public void Draw(List<View> surface)
    {
        _shader.Bind();
        _shader.setUniform1v("random", rand);
        _shader.setUniformV2f("offset", offset);
        _shader.setuniform2vArray("points", Points.ToArray());
        surface[1].Draw(_polygon);
     //   surface[1].Draw(_debugColoredRectangle);
    }
}
