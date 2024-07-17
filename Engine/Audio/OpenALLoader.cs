using System.Runtime.InteropServices;

namespace Engine.Audio;

public class OpenALLoader
{
    private static string openALWindows = "OpenAL32.dll"; 
    private static string openALMacOS = "libopenal.dylib"; 
    private static string openALLinux = "libopenal.so";
   
    
    
    
    public static void LoadLibrary()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoadLibraryWindows("resources/OpenALlibs/"+openALWindows);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            throw new PlatformNotSupportedException("Platform not supported.");
            // LoadLibraryMacOS(libraryPath);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException("Platform not supported.");
         
           // LoadLibraryLinux(libraryPath);
        }
        else
        {
            throw new PlatformNotSupportedException("Platform not supported.");
        }
    }
    private static void LoadLibraryWindows(string libraryPath)
    {
        IntPtr handle = LoadLibrary(libraryPath);
        if (handle == IntPtr.Zero)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Exception($"Failed to load OpenAL library on Windows. Error code: {errorCode}");
        }
    }

    [DllImport("kernel32", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string fileName);


}