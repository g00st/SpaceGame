using OpenTK.Graphics.OpenGL;

namespace App.Engine;

public static class ErrorChecker
{
    private static DebugProc _debugCallback;
    public static void CheckForGLErrors(string context)
    {
        return;
        ErrorCode errorCode;
        while ((errorCode = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error ({context}): {errorCode}"); 
            throw  new Exception($"OpenGL Error ({context}): {errorCode}");
        }
    }
    public static void GLDebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        string errorMessage = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message);
        Console.WriteLine($"OpenGL Debug Message: {source} {type} {id} {severity} - {errorMessage}");
    }
    public static void InitializeGLDebugCallback()
    {
        _debugCallback = GLDebugCallback;
        GL.DebugMessageCallback(_debugCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
    }
}