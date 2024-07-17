using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

//TODO: Fix horrible code and mvp mess
namespace App.Engine;

public class View
{
    //--------------------------------------------------------------------------------------------------------
    public static System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
    float _drawtime = 0;


    //--------------------------------------------------------------------------------------------------------

    private static VBO currentrendertarget;
    private static int currentwidth;
    private static int currentheight;


    private List<DrawObject> drawObjects;
    private int _width, _height;
    private Vector2 _vpossition;
    private Vector2 _vsize;
    private float _rotation;
    private VBO _rendertarget;
    private Matrix4 _camera;
    private Matrix4 _view;


    public Vector2 vpossition
    {
        get => _vpossition;
        set
        {
            _vpossition = value;
            _camera = calcCameraProjection();
            _view = CalcView();
        }
    }

    public Vector2 vsize
    {
        get => _vsize;
        set
        {
            _vsize = value;
            _camera = calcCameraProjection();
        }
    }

    public float rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _view = CalcView();
        }
    }

    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            _camera = calcCameraProjection();
        }
    }

    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            _camera = calcCameraProjection();
        }
    }


    public void Resize(int width, int height)
    {
        Width = width;
        Height = height;

        GL.Viewport(0, 0, Width, Height);
    }


    public void Draw(DrawObject todraw)
    {
        //check if object is in view


        Vector2 objectPosition = todraw.drawInfo.Position;
        Vector2 objectSize = todraw.drawInfo.Size;

        Vector2 screenTopLeft = _vpossition - _vsize / 2;
        Vector2 screenBottomRight = _vpossition + _vsize / 2;
        if (objectPosition.X + objectSize.X / 2 >= screenTopLeft.X &&
            objectPosition.X - objectSize.X / 2 <= screenBottomRight.X &&
            objectPosition.Y + objectSize.Y / 2 >= screenTopLeft.Y &&
            objectPosition.Y - objectSize.Y / 2 <= screenBottomRight.Y)
        {
            if (currentrendertarget != _rendertarget)
            {
                _rendertarget.Bind();
                currentrendertarget = _rendertarget;
            }

            if (currentwidth != Width || currentheight != Height)
            {
                GL.Viewport(0, 0, Width, Height);
                currentwidth = Width;
                currentheight = Height;
            }

            DrawInfo obj = todraw.drawInfo;
            //Console.Write(objectransform.ToString() + "\n" +" \n");
            obj.mesh.Draw(obj, _view, _camera);
            // _rendertarget.Unbind();
        }
    }


    private Matrix4 calcCameraProjection()
    {
        //Matrix4.CreateOrthographic(1920, 1080, -1.0f, 1.0f)* Matrix4.CreateTranslation(-1,-1,0);

        float left = _vpossition.X - _vsize.X / 2.0f;
        float right = _vpossition.X + _vsize.X / 2.0f;
        float bottom = _vpossition.Y - ((_vsize.X / Width) * Height) / 2.0f;
        float top = _vpossition.Y + ((_vsize.X / Width) * Height) / 2.0f;
        return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
    }

    private Matrix4 CalcView()
    {
        Matrix4 translateToOrigin = Matrix4.CreateTranslation(-_vpossition.X, -_vpossition.Y, 0);
        Matrix4 rotate = Matrix4.CreateRotationZ(_rotation);
        Matrix4 translateBack = Matrix4.CreateTranslation(_vpossition.X, _vpossition.Y, 0);
        return translateToOrigin * rotate * translateBack;
    }

    public Vector2 ScreenToViewSpace(Vector2 screenCoordinate, bool invertY = false)
    {
        float centerX = Width / 2.0f;
        float centerY = Height / 2.0f;

        // Center the screen coordinates around the center of the viewport
        float centeredX = screenCoordinate.X - centerX;
        float centeredY;
        if (invertY)
        {
            centeredY = centerY - screenCoordinate.Y;
        }
        else
        {
            centeredY = screenCoordinate.Y - centerY;
        }

        ; // Y-axis is inverted in screen coordinates

        // Normalize centered screen coordinates
        float normalizedX = (2.0f * centeredX / Width);
        float normalizedY = (2.0f * centeredY / Height);

        // Create the camera projection matrix
        Matrix4 projectionMatrix = calcCameraProjection();

        // Create the camera rotation matrix
        Matrix4 cameraRotationMatrix = Matrix4.CreateRotationZ(_rotation);

        // Calculate the translation to move the camera position to the origin
        Matrix4 translateToOrigin = Matrix4.CreateTranslation(-_vpossition.X, -_vpossition.Y, 0);

        // Calculate the translation to move back to the original position after rotation
        Matrix4 translateBack = Matrix4.CreateTranslation(_vpossition.X, _vpossition.Y, 0);

        // Combine the translations, rotation, and projection matrices
        Matrix4 combinedMatrix = translateToOrigin * cameraRotationMatrix * translateBack * projectionMatrix;

        // Calculate the inverse of the combined matrix
        Matrix4 inverseMatrix = Matrix4.Invert(combinedMatrix);

        // Transform the screen coordinate to view space
        Vector4 viewSpace = new Vector4(normalizedX, normalizedY, 0, 1) * inverseMatrix;

        // Return the transformed coordinates
        return new Vector2(viewSpace.X, viewSpace.Y);
    }

    public View()
    {
        _rendertarget = VBO.VBO_0();
        _vsize = new Vector2(1920, 1080);
        _vpossition = new Vector2(1920 / 2, 1080 / 2);
        drawObjects = new List<DrawObject>();
        _rotation = 0;
        _view = CalcView();
        _camera = calcCameraProjection();
    }

    public void clear()
    {
        _rendertarget.Bind();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }


    public View(VBO rendertarget)
    {
        _rendertarget = rendertarget;
        _vsize = new Vector2(rendertarget.Widht(), rendertarget.Height());
        Width = rendertarget.Widht();
        Height = rendertarget.Height();
        _vpossition = new Vector2(rendertarget.Widht() / 2, rendertarget.Height() / 2);
        _rotation = 0;
        _view = CalcView();
        _camera = calcCameraProjection();
        rendertarget.Bind();
    }
}