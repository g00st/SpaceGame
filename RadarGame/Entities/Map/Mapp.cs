using System.Drawing;
using App.Engine;
using App.Engine.Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RadarGame.Physics;


namespace RadarGame.Entities;

public class Mapp : IEntitie, IDrawObject
{
    private Vector2 MapSize; //size of map
    private Vector2 MapPosition; //center of map
    private List<MapPolygon > MapPolygons; //list of polygons that make up the map
    private ColoredRectangle debugRect = new ColoredRectangle(new Vector2(0, 0), Vector2.One, Color4.Aqua, "t", true); //debug rectangle
    private List<Vector4> EnemiePositions = new List<Vector4>();//precomputed positions for enemies V4(x,y, ocupied , radius)
    private List<Vector4> Ocuppied = new List<Vector4>(); //list of ocuppied areas

    public string Name { get; set; }
    private int Count = 0;
    
    
    
public Mapp(Vector2 mapSize, Vector2 mapPosition)
    {
        MapSize = mapSize;
        MapPosition = mapPosition;
        Name = "Mapp";
        MapPolygons = new List<MapPolygon>();
        CreateBorder();
        
        List<Vector4> borders = new List<Vector4>(this.Ocuppied);
        
        //add some random polygons to the map
        Random random = new Random(); 
        RectanglePack(100, 5000, 10, 200, 1000);
        
        
        
        foreach (var rect  in Ocuppied)
        {
            var x= MapPolygon.CreateRandomPolygon( new Vector2(rect.X + (rect.Z - rect.X) / 2, rect.Y + (rect.W - rect.Y) / 2), new Vector2(rect.Z - rect.X, rect.W - rect.Y), 100, 10, "RandomPolygon" + Ocuppied.IndexOf(rect), random);
            MapPolygons.Add(x);
        }
       // PrecomputeEnemyPositions(10000);
        
        foreach( var polygon in MapPolygons)
        {
            EntityManager.AddObject(polygon);
        }
        
    }


    private void RectanglePack(float initialSize, float maxSize, float sizeIncrement, int initialCount, float spacing)
    {
        Random random = new Random();

    
        for (int i = 0; i < initialCount; i++)
        {
            float x = MapPosition.X - MapSize.X / 2 + spacing + (float)random.NextDouble() * (MapSize.X - initialSize - 2 * spacing);
            float y = MapPosition.Y - MapSize.Y / 2 + spacing + (float)random.NextDouble() * (MapSize.Y - initialSize - 2 * spacing);
            Vector4 newRect = new Vector4(x, y, x + initialSize, y + initialSize);

            if (!CheckCollision(newRect, new Vector4()) && IsWithinMapBounds(newRect) && !IsInStartingArea(newRect))
            {
                Ocuppied.Add(newRect);
            }
        }


        float size = initialSize;

      
        while (size <= maxSize)
        {
            bool sizeIncreased = false;

          
            for (int i = 0; i < Ocuppied.Count; i++)
            {
                Vector4 rect = Ocuppied[i];

            
                Vector4 newRect = new Vector4(rect.X, rect.Y, rect.Z + sizeIncrement, rect.W + sizeIncrement);

              
                if (CheckCollision(newRect, rect) || !IsWithinMapBounds(newRect) || IsInStartingArea(newRect))
                {
                    continue;
                }
                
                Ocuppied[i] = newRect;
                sizeIncreased = true;
            }

        
            if (!sizeIncreased)
            {
                break;
            }

        
            size += sizeIncrement;
        }
    }
    
    
private void PrecomputeEnemyPositions(int count)
{
    EnemiePositions = new List<Vector4>();
    Random random = new Random();
    for (int i = 0; i < count; i++)
    {
        float x = (float)random.NextDouble() * MapSize.X - MapSize.X / 2;
        float y = (float)random.NextDouble() * MapSize.Y - MapSize.Y / 2;
        float radius = 100;
        Vector4 rect = new Vector4(x, y, radius, 0);

        if (CheckCollision(rect.Xy,100) || !IsWithinMapBounds(rect) || EnemiePositions.Contains(rect))
        {
            i--;
            continue;
        }

        EnemiePositions.Add(rect);
    }
    
}


private bool IsWithinMapBounds(Vector4 rect)
{
    return rect.X >= MapPosition.X - MapSize.X / 2 && rect.Y >= MapPosition.Y - MapSize.Y / 2 && rect.Z <= MapPosition.X + MapSize.X / 2 && rect.W <= MapPosition.Y + MapSize.Y / 2;
}

private bool IsInStartingArea(Vector4 rect)
{
    float startingAreaSize = 2000;
    float halfSize = startingAreaSize / 2;
    
    if (rect.Z > -halfSize && rect.X < halfSize && rect.W > -halfSize && rect.Y < halfSize)
    {
        return true;
    }
    return false;
}




    

    private void CreateBorder()
    {
       float borderSize = 10000;
       // add 4 polygons to the map that make up the border
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(0.5f, 0.5f));
            points.Add(new Vector2(-0.5f, 0.5f));
            points.Add(new Vector2(-0.5f, -0.5f));
            points.Add(new Vector2(0.5f, -0.5f));
            
            MapPolygon top = new MapPolygon(points, new Vector2(0, MapSize.Y / 2 + borderSize /2), new Vector2(0, 0), new Vector2(MapSize.X, borderSize), "Top");
            MapPolygon bottom = new MapPolygon(points, new Vector2(0, -MapSize.Y / 2- borderSize /2), new Vector2(0, 0), new Vector2(MapSize.X, borderSize), "Bottom");
            MapPolygon left = new MapPolygon(points, new Vector2(-MapSize.X / 2- borderSize /2, 0), new Vector2(0, 0), new Vector2(borderSize, MapSize.Y), "Left");
            MapPolygon right = new MapPolygon(points, new Vector2(MapSize.X / 2+ borderSize /2, 0), new Vector2(0, 0), new Vector2(borderSize, MapSize.Y), "Right");
            
            //adthe for rectangles as occupied
            borderSize /= 2;
            
            Vector4 topRect = new Vector4(-MapSize.X / 2, MapSize.Y / 2, MapSize.X / 2, MapSize.Y / 2 + borderSize);
            Vector4 bottomRect = new Vector4(-MapSize.X / 2, -MapSize.Y / 2 - borderSize, MapSize.X / 2, -MapSize.Y / 2);
            Vector4 leftRect = new Vector4(-MapSize.X / 2 - borderSize, -MapSize.Y / 2, -MapSize.X / 2, MapSize.Y / 2);
            Vector4 rightRect = new Vector4(MapSize.X / 2, -MapSize.Y / 2, MapSize.X / 2 + borderSize, MapSize.Y / 2);
            
            
            
            MapPolygons.Add(top);
            MapPolygons.Add(bottom);
            MapPolygons.Add(left);
            MapPolygons.Add(right);
    }
    
    public bool CheckCollision(Vector4 newRect, Vector4 ignoreRect)
    {
        foreach (var occupiedRect in Ocuppied)
        {
            if (occupiedRect == ignoreRect)
            {
                continue;
            }
            
            if (IsColliding(newRect, occupiedRect))
            {
                return true; 
            }
        }
        return false;
    }

    public bool CheckCollision(Vector2 point, float radius)
    {
        foreach (var occupiedRect in Ocuppied)
        {
            if (IsColliding(point, radius, occupiedRect))
            {
                return true;
            }


        }

        return false;
    }

    private bool IsColliding(Vector4 rect1, Vector4 rect2)
        {
            return (rect1.X < rect2.Z && rect1.Z > rect2.X &&
                    rect1.Y < rect2.W && rect1.W > rect2.Y);
        }
    
        private bool IsColliding(Vector2 point,float radius, Vector4 rect)
        {
            float closestX = Math.Max(rect.X, Math.Min(point.X, rect.Z));
            float closestY = Math.Max(rect.Y, Math.Min(point.Y, rect.W));
            float distanceX = point.X - closestX;
            float distanceY = point.Y - closestY;
            return (distanceX * distanceX + distanceY * distanceY) < (radius * radius);
        }
    
        public void Update(FrameEventArgs args, KeyboardState keyboardState, MouseState mouseState)
        {
        }

        public void onDeleted()
        {
        
        }

        public void Draw(List<View> surface)
        {
            foreach (var enemiePosition in EnemiePositions)
            {
                debugRect.drawInfo.Position = enemiePosition.Xy;
                debugRect.drawInfo.Size = new Vector2(200);
                surface[1].Draw(debugRect);
            
            }
        }
}