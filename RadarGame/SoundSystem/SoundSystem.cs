/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Audio;
using OpenTK.Mathematics;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace RadarGame.SoundSystem;

public static class SoundSystem
{
    // List<SoundObject> soundObjects = new List<SoundObject>();
    private static Dictionary<WaveOutEvent, int> Sounds = new Dictionary<WaveOutEvent, int>();

    static float globalVolume = 0.5f;
    static bool ToggleSinus = true; // To Toggle between Sinus and Soundsystem Key bindings
    static string samplePath = "resources/Sounds/Laser3.wav";

    // ID history:
    // ID 2: Systemsounds, example bullet shot

    public static void DebugDraw()
    {
        ImGuiNET.ImGui.Begin("SoundSystem");
        // ImGuiNET.ImGui.SliderInt("SampleFrequenz", ref sampleFreq, 4000, 60000);
        ImGuiNET.ImGui.SliderFloat("globalVolume", ref globalVolume, 0.00f, 1.00f);
        ImGuiNET.ImGui.End();
        // ImGuiNET.ImGui.PlotLines("sinData", ref sinData[0], sinData.Length, sinDataIndex, "sinData", 0, 100, new System.Numerics.Vector2(0, 100));

    }
    public static void Update(FrameEventArgs args, KeyboardState keyboardState)
    {
        if(ToggleSinus)
        {
            if (keyboardState.IsKeyReleased(Keys.K))
            {
                // kill all
                StopAllTracks();
            }
            if (keyboardState.IsKeyReleased(Keys.KeyPadAdd))
            {
                // make louder
                globalVolume += 0.1f;
                if (globalVolume > 1.00f) globalVolume = 1.00f;
                ChangeVolumeAll(globalVolume);
            }
            if (keyboardState.IsKeyReleased(Keys.KeyPadSubtract))
            {
                // make quiter
                globalVolume -= 0.1f;
                if(globalVolume < 0.00f) globalVolume = 0;
                ChangeVolumeAll(globalVolume);
            }
            if (keyboardState.IsKeyReleased(Keys.I))
            {
                // try out new library
                NewPlayer();
            }
            if(keyboardState.IsKeyReleased(Keys.D0)) ToggleSinus = false;
        } else
        {
            if(keyboardState.IsKeyReleased(Keys.D0)) ToggleSinus = true;
        }
        
    }
   
    public static void NewPlayer()
    {
        var audioFile = new AudioFileReader(samplePath);
        Console.WriteLine(audioFile);
        Console.WriteLine("In New Player after audioFile");
        var volumeProvider = new VolumeSampleProvider(audioFile);

        // volume between 0.00 and 1.00
        volumeProvider.Volume = globalVolume;
        Console.WriteLine("Before Wave Out");
        var waveOut = new WaveOutEvent();
        waveOut.Init(volumeProvider);
        Console.WriteLine("After Wave Out");
        Sounds.Add(waveOut, 0);
        waveOut.Play();
        Console.WriteLine("After Play");
    }

    // provide relative filepath like "resources/Sounds/Laser3.wav"
    // volume between 0.00f and 1.00f, int id to specify tracks
    public static void PlayThisTrack(String filepath, int id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        var audioFile = new AudioFileReader(filepath);
        Console.WriteLine(audioFile);
        var volumeProvider = new VolumeSampleProvider(audioFile);
        // volume between 0.00 and 1.00
        volumeProvider.Volume = globalVolume;
        var newTrack = new WaveOutEvent();
        newTrack.Init(volumeProvider);
        Sounds.Add(newTrack, id);
        newTrack.Play();
    }

    public static void StopAllTracks()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

        foreach (var track in Sounds)
        {
            track.Key.Stop();
            
        }
        Sounds.Clear();
    }

    public static void StopSpecificTracks(int id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        foreach(var track in Sounds)
        {
            if(track.Value == id)
            {
                track.Key.Stop();
                Sounds.Remove(track.Key);
            }
        }
    }

    public static void PauseSpecificTrack(int id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        
        foreach(var track in Sounds)
        {
            if(track.Value == id) track.Key.Pause();
        }
    }

    public static void UnpauseSpecificTrack(int id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        foreach (var track in Sounds)
        {
            if (track.Value == id) track.Key.Play();
        }
    }

    public static void ChangeVolumeAll(float newVolume)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        if (newVolume < 0.00f || newVolume > 1.00f) return;
        foreach(var track in Sounds)
        {
            track.Key.Pause();
            track.Key.Volume = newVolume;
            track.Key.Play();
        }
    }

    public static void ChangeVolumeSpecific(float newVolume, int id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        if(newVolume < 0.00f || newVolume > 1.00f) return;
        foreach(var track in Sounds)
        {
            if(track.Value == id)
            {
                track.Key.Pause();
                track.Key.Volume = newVolume;
                track.Key.Play();
            }
        }
    }


}
*/