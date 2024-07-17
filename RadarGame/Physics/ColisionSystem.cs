using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using RadarGame.Entities;

namespace RadarGame.Physics;

public static class ColisionSystem
{
    private static List<ColisionData> _colisionData = new List<ColisionData>();
    private static List<ColisionData> _staticObjects = new List<ColisionData>();
   
    public static List<System.Numerics.Vector3> Debugpoints { get; set; } = new List<System.Numerics.Vector3>();
    private static Polygon deugcircle = Polygon.Circle(Vector2.Zero, 100, 100, new SimpleColorShader(Color4.Yellow), "DebugPolygon", true);
    private struct ColisionData
    {
        public IColisionObject O { get; set; }
        //public Polygon Shape { get; set; }
        public float Distance { get; set; }
         
    }

    public static void AddObject(IColisionObject colisionObject)
    {
        ColisionData newColisionData = new ColisionData();
        newColisionData.O = colisionObject;
        newColisionData.Distance = colisionObject.CollisonShape.Max(x => x.Length);
        //newColisionData.Shape = new Polygon(colisionObject.CollisonShape, new SimpleColorShader(Color4.Red), colisionObject.Position,Vector2.One, 0, "DebugPolygon", true);
        if (colisionObject.Static)
        {
            _staticObjects.Add(newColisionData);
        }else
        {
            _colisionData.Add(newColisionData);
        }
        
    }
    public static void RemoveObject(IColisionObject colisionObject)
    {
         if (colisionObject.Static) _staticObjects.RemoveAll(x => x.O == colisionObject);
         else _colisionData.RemoveAll(x => x.O == colisionObject);
    }
    
    public static void Update()
    {
       
            
            
        for (int i = 0; i < _colisionData.Count; i++)
        {
            
            foreach (var staticObject in _staticObjects)
            {
                if (_colisionData[i].Distance + staticObject.Distance < Vector2.Distance(_colisionData[i].O.Position, staticObject.O.Position))
                {
                    continue;
                }
                if (SAT(_colisionData[i].O, staticObject.O))
                {
                    _colisionData[i].O.OnColision(staticObject.O);
                    staticObject.O.OnColision(_colisionData[i].O);
                }
            }
            
            
            for (int j = i+1; j < _colisionData.Count; j++)
            { 
                //if the objects are the same, skip
                if (_colisionData[i].O == _colisionData[j].O)
                {
                    continue;
                }
                //if the distance between the objects is greater than the distance between the objects farthest points, skip
                
                if (_colisionData[i].Distance + _colisionData[j].Distance < Vector2.Distance(_colisionData[i].O.Position, _colisionData[j].O.Position))
                {
                    continue;
                }
                
                if (SAT(_colisionData[i].O, _colisionData[j].O))
                {
                    _colisionData[i].O.OnColision(_colisionData[j].O);
                    _colisionData[j].O.OnColision(_colisionData[i].O);
                }
            }
        }
    }

    private static bool SAT( IColisionObject A,  IColisionObject B)
    {
       // var VerticesA = A.CollisonShape.Select(x => x + A.Position).ToList();
        //var VerticesB = B.CollisonShape.Select(x => x + B.Position).ToList();
        var VerticesA = A.CollisonShape.Select(x => RotatePoint(x, A.Rotation) + A.Position).ToList();
        var VerticesB = B.CollisonShape.Select(x => RotatePoint(x, B.Rotation) + B.Position).ToList();
        
        for (int i = 0; i < VerticesA.Count; i++)
        {
            var edge = VerticesA[(i + 1) % VerticesA.Count] - VerticesA[i];
            var normal = new Vector2(-edge.Y, edge.X).Normalized();
            var minA = float.MaxValue;
            var maxA = float.MinValue;
            var minB = float.MaxValue;
            var maxB = float.MinValue;
            foreach (var vertex in VerticesA)
            {
                var projection = Vector2.Dot(vertex, normal);
                minA = Math.Min(minA, projection);
                maxA = Math.Max(maxA, projection);
            }
            foreach (var vertex in VerticesB)
            {
                var projection = Vector2.Dot(vertex, normal);
                minB = Math.Min(minB, projection);
                maxB = Math.Max(maxB, projection);
            }
            if (maxA < minB || maxB < minA)
            {
                return false;
            }
        }
        for (int i = 0; i < VerticesB.Count; i++)
        {
            var edge = VerticesB[(i + 1) % VerticesB.Count] - VerticesB[i];
            var normal = new Vector2(-edge.Y, edge.X).Normalized();
            var minA = float.MaxValue;
            var maxA = float.MinValue;
            var minB = float.MaxValue;
            var maxB = float.MinValue;
            foreach (var vertex in VerticesA)
            {
                var projection = Vector2.Dot(vertex, normal);
                minA = Math.Min(minA, projection);
                maxA = Math.Max(maxA, projection);
            }
            foreach (var vertex in VerticesB)
            {
                var projection = Vector2.Dot(vertex, normal);
                minB = Math.Min(minB, projection);
                maxB = Math.Max(maxB, projection);
            }
            if (maxA < minB || maxB < minA)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private static Vector2 RotatePoint(Vector2 point, float angle)
    {
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        // Rotate point
        Vector2 rotatedPoint = new Vector2(
            point.X * cos - point.Y * sin,
            point.X * sin + point.Y * cos
        );

        return rotatedPoint;
    }
    private static float fastSDF(Vector2 point, ColisionData tocheck)
    {
         return    (point-   tocheck.O.Position).Length -tocheck.Distance;  
    }
    
    private static float exacktSDF(Vector2 point, ColisionData tocheck)
    {
        // Rotate the collision shape points and add the object's position
        var Vertices = tocheck.O.CollisonShape.Select(x => RotatePoint(x, tocheck.O.Rotation) + tocheck.O.Position).ToList();
        
            float d = Vector2.DistanceSquared(point, Vertices[0]);
            float s = 1.0f;
            for (int i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i, i++)
            {
                Vector2 e = Vertices[j] - Vertices[i];
                Vector2 w = point - Vertices[i];
                Vector2 b = w - e * Math.Clamp(Vector2.Dot(w, e) / Vector2.Dot(e, e), 0.0f, 1.0f);
                d = Math.Min(d, b.LengthSquared);
                bool[] c = new bool[3] { point.Y >= Vertices[i].Y, point.Y < Vertices[j].Y, e.X * w.Y > e.Y * w.X };
                if (c.All(x => x) || c.All(x => !x)) s *= -1.0f;
            }
            return s * MathF.Sqrt(d);
        
    }
    
    public static  List<IColisionObject> getinRadius(Vector2 point, float radius, bool onStatic = false, bool onDynamic = true)
    {
        List<IColisionObject> result = new List<IColisionObject>();
        if (onDynamic)
        {
            foreach (var ob in _colisionData)
            {
                if (Vector2.Distance(point, ob.O.Position) < ob.Distance + radius)
                {
                    if (fastSDF(point, ob) < radius)
                    {
                        if (exacktSDF(point, ob) < radius)
                        {
                            result.Add(ob.O);
                        }
                    }
                }
            }
        }

        if (onStatic)
        {
            foreach (var ob in _staticObjects)
            {
                if (Vector2.Distance(point, ob.O.Position) < ob.Distance + radius)
                {
                    if (fastSDF(point, ob) < radius)
                    {
                        if (exacktSDF(point, ob) < radius)
                        {
                            result.Add(ob.O);
                        }
                    }
                }
            }
        }

        return result;
    }


    public static IColisionObject getNearest(Vector2 point, out float dist)
    {
        IColisionObject nearest = null;
        float min = float.MaxValue;
        foreach (var ob in _colisionData)
        {
            var distance = fastSDF(point, ob);
            if (distance < min)
            {
                 distance  = exacktSDF( point, ob);
                 if (distance < min)
                 {
                     min = distance;
                     nearest = ob.O;
                 }
            }
        }
        foreach (var ob in _staticObjects)
        {
            var distance = fastSDF(point, ob);
            if (distance < min)
            {
                distance  = exacktSDF( point, ob);
                if (distance < min)
                {
                    min = distance;
                    nearest = ob.O;
                }
            }
        }
        
        dist = min;
        return nearest; 
    }
    
    public static IColisionObject? castRay(Vector2 start, Vector2 end, IColisionObject ignoreObject)
    {
        IColisionObject? nearest = null;
        float min = float.MaxValue;
        Vector2 direction = (end - start).Normalized();
        float rayLength = (end - start).Length;

        foreach (var ob in _colisionData)
        {
            // Early check: if the ray doesn't intersect with the bounding circle of the object, skip this object
            float distanceToObj = (ob.O.Position - start).Length;
            if (distanceToObj > ob.Distance + rayLength)
            {
                continue;
            }

            if (LineSAT(start, end, ob.O))
            {
                var distance = fastSDF(start, ob);
                if (distance < min)
                {
                    distance = exacktSDF(start, ob);
                    if (distance < min && nearest != ignoreObject) // CHECK IF BROKEN
                    {
                        min = distance;
                        nearest = ob.O;
                    }
                }
            }
        }
        foreach (var ob in _staticObjects)
        {
            // Early check: if the ray doesn't intersect with the bounding circle of the object, skip this object
            float distanceToObj = (ob.O.Position - start).Length;
            if (distanceToObj > ob.Distance + rayLength)
            {
                continue;
            }

            if (LineSAT(start, end, ob.O))
            {
                var distance = fastSDF(start, ob);
                if (distance < min)
                {
                    distance = exacktSDF(start, ob);
                    if (distance < min && nearest != ignoreObject)   // CHECK IF BROKEN
                    {
                        min = distance;
                        nearest = ob.O;
                    }
                }
            }
        }
        
        
        
        return nearest;
    }
   

    public static void draw(View v)
    {
      /*  foreach (var c in _colisionData)
        {
            c.Shape.drawInfo.Position = c.O.Position;
            c.Shape.drawInfo.Rotation = c.O.Rotation;
            v.Draw(c.Shape);
            return  Raymarch( start, end - start, Vector2.Distance( start, end));
            
        }*/

        foreach (var debug in Debugpoints)
        {
            deugcircle.drawInfo.Position = new Vector2(debug.X, debug.Y);
            deugcircle.drawInfo.Size = new Vector2(debug.Z);
            v.Draw(deugcircle);
        }
        Debugpoints.Clear();
        
    }
    
    private static bool LineSAT(Vector2 start, Vector2 end, IColisionObject B)
{
    // Define the vertices of the line segment
    var VerticesA = new List<Vector2> { start, end };
    
    // Get the vertices of the collision object B
    var VerticesB = B.CollisonShape.Select(x => RotatePoint(x, B.Rotation) + B.Position).ToList();

    // Check for separation on the axes of the line segment
    for (int i = 0; i < VerticesA.Count; i++)
    {
        var edge = VerticesA[(i + 1) % VerticesA.Count] - VerticesA[i];
        var normal = new Vector2(-edge.Y, edge.X).Normalized();
        var minA = float.MaxValue;
        var maxA = float.MinValue;
        var minB = float.MaxValue;
        var maxB = float.MinValue;
        foreach (var vertex in VerticesA)
        {
            var projection = Vector2.Dot(vertex, normal);
            minA = Math.Min(minA, projection);
            maxA = Math.Max(maxA, projection);
        }
        foreach (var vertex in VerticesB)
        {
            var projection = Vector2.Dot(vertex, normal);
            minB = Math.Min(minB, projection);
            maxB = Math.Max(maxB, projection);
        }
        if (maxA < minB || maxB < minA)
        {
            return false;
        }
    }

    // Check for separation on the axes of the collision object B
    for (int i = 0; i < VerticesB.Count; i++)
    {
        var edge = VerticesB[(i + 1) % VerticesB.Count] - VerticesB[i];
        var normal = new Vector2(-edge.Y, edge.X).Normalized();
        var minA = float.MaxValue;
        var maxA = float.MinValue;
        var minB = float.MaxValue;
        var maxB = float.MinValue;
        foreach (var vertex in VerticesA)
        {
            var projection = Vector2.Dot(vertex, normal);
            minA = Math.Min(minA, projection);
            maxA = Math.Max(maxA, projection);
        }
        foreach (var vertex in VerticesB)
        {
            var projection = Vector2.Dot(vertex, normal);
            minB = Math.Min(minB, projection);
            maxB = Math.Max(maxB, projection);
        }
        if (maxA < minB || maxB < minA)
        {
            return false;
        }
    }

    return true;
}

    
    





}