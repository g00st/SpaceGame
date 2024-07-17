using System;
using System.Collections.Generic;
using System.Linq;
/*
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.Audio;

namespace RadarGame.SoundSystem
{
    public static class SinusWave
    {
        private static List<int> Buffers = new List<int>();
        private static List<int> Sources = new List<int>();
        private static ALDevice device;
        private static ALContext context;
        static int sampleFreq = 44100;
        static int freq = 440;
        static int dataCount = sampleFreq / freq;
        static short[] sinData = new short[dataCount];
        // private static int sinDataIndex = 0;
        static bool ToggleSinus = false;
        static int source = 0;
        public static void SetUpSound()
        {
            device = ALC.OpenDevice(null);
            int[] flags = new int[0];
            context = ALC.CreateContext(device, flags);
        }

        public static void Update(FrameEventArgs args, KeyboardState keyboardState)
        {
            // TO DO: change if to switch case
            if(ToggleSinus)
            {
                if (keyboardState.IsKeyReleased(Keys.K))
                {
                    // kill frequenz all
                    StopPlayingSource();
                }
                if (keyboardState.IsKeyReleased(Keys.L))
                {
                    // play changeable frequenz in Loop
                    PlaySinusWaveLoop(sampleFreq, freq);
                }
                if (keyboardState.IsKeyReleased(Keys.I))
                {
                    // play changeable frequenz in no Loop
                    PlaySinusWaveNoLoop(sampleFreq, freq);
                }
                if (keyboardState.IsKeyReleased(Keys.B))
                {
                    sampleFreq = 13655;
                    freq = 494;
                    PlaySinusWaveNoLoop(sampleFreq, freq); // play specific frequenz
                }
                if(keyboardState.IsKeyReleased(Keys.D0))
                {
                    ToggleSinus = false;
                    Console.WriteLine(ToggleSinus);
                }
            } else
            {                
                if(keyboardState.IsKeyReleased(Keys.D0))
                {
                    ToggleSinus = true;
                    Console.WriteLine(ToggleSinus);
                }
            }
            
        }

        public static void DebugDraw()
        {
            ImGuiNET.ImGui.Begin("SinusWave");
            ImGuiNET.ImGui.SliderInt("SampleFrequenz", ref sampleFreq, 4000, 60000);
            ImGuiNET.ImGui.SliderInt("Frequenz", ref freq, 40, 1000);
            ImGuiNET.ImGui.End();
            // ImGuiNET.ImGui.PlotLines("sinData", ref sinData[0], sinData.Length, sinDataIndex, "sinData", 0, 100, new System.Numerics.Vector2(0, 100));

        }
        public static void StopPlayingSource()
        {
            foreach (var source in Sources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
            foreach (var buffer in Buffers)
            {
                AL.DeleteBuffer(buffer);
            }
        }

        public static void CleanUp()
        {
            foreach (var source in Sources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);

            }
            foreach (var buffer in Buffers)
            {
                AL.DeleteBuffer(buffer);
            }

            ALC.DestroyContext(context);
            ALC.CloseDevice(device);

        }
        public static void PlaySinusWaveLoop(int orderedSamplefreq, int orderedFreq)
        {

            OpenALLoader.LoadLibrary();
            // Initialize
            ALC.MakeContextCurrent(context);
            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);
            Console.WriteLine(version);
            Console.WriteLine(vendor);
            Console.WriteLine(renderer);
            // Process
            int buffers = 0;   // no need for int* disgusting buffers
                               //  int otherbuffer = AL.GenBuffer(); int othersource = AL.GenBuffer(); // this is LEGAL
            AL.GenBuffers(1, ref buffers);       // no out ?
            AL.GenSources(1, ref source);
            Buffers.Add(buffers);
            Sources.Add(source);
            // sampleFreq = 44100;   // example freq is sinus curve speed  c
            double dt = 2 * Math.PI / orderedSamplefreq;
            double amp = 0.5;
            // ------------
            // freq = 440;  // standard freqlänge  lambda
            dataCount = orderedSamplefreq / orderedFreq; // f = c / lambda
            sinData = new short[dataCount];

            for (int i = 0; i < sinData.Length; ++i)
            {
                sinData[i] = (short)(amp * short.MaxValue * Math.Sin(i * dt * orderedFreq));
            }

            AL.BufferData(buffers, ALFormat.Mono16, sinData, orderedSamplefreq); // mag []
                                                                                 // AL.BufferData(buffers, ALFormat.Mono16,ref  sinData, sinData.Length, sampleFreq); // ??? short[] nicht zu IntPtr konvertierbar

            AL.Source(source, ALSourcei.Buffer, buffers);
            AL.Source(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);
        }

        // sample 13655 freq 494
        public static void PlaySinusWaveNoLoop(int orderedSampleFreq, int orderedFreq)
        {
            OpenALLoader.LoadLibrary();

            // Initialize
            ALC.MakeContextCurrent(context);
            var version = AL.Get(ALGetString.Version);
            var vendor = AL.Get(ALGetString.Vendor);
            var renderer = AL.Get(ALGetString.Renderer);
            Console.WriteLine(version);
            Console.WriteLine(vendor);
            Console.WriteLine(renderer);
            // Process
            int buffers = 0;   // no need for int* disgusting buffers
                               //  int otherbuffer = AL.GenBuffer(); int othersource = AL.GenBuffer(); // this is LEGAL
            AL.GenBuffers(1, ref buffers);       // no out ?
            AL.GenSources(1, ref source);
            Buffers.Add(buffers);
            Sources.Add(source);
            // sampleFreq = 44100;   // example freq is sinus curve speed  c
            double dt = 2 * Math.PI / orderedSampleFreq;
            double amp = 0.5;
            // ------------
            // freq = 440;  // standard freqlänge  lambda
            dataCount = orderedSampleFreq / orderedFreq; // f = c / lambda
            sinData = new short[dataCount];

            for (int i = 0; i < sinData.Length; ++i)
            {
                sinData[i] = (short)(amp * short.MaxValue * Math.Sin(i * dt * orderedFreq));
            }

            AL.BufferData(buffers, ALFormat.Mono16, sinData, orderedSampleFreq); // mag []
                                                                                 // AL.BufferData(buffers, ALFormat.Mono16,ref  sinData, sinData.Length, sampleFreq); // ??? short[] nicht zu IntPtr konvertierbar

            AL.Source(source, ALSourcei.Buffer, buffers);
            AL.Source(source, ALSourceb.Looping, false);
            AL.SourcePlay(source);
        }


    }
}
*/