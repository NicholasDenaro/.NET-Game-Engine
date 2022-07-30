using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameEngine.UI.Audio.ISoundPlayer;

namespace GameEngine.UI.NAudio
{
    public class SourceWaveOutEvent : IWavePlayer, IDisposable, IWavePosition
    {
        private IWaveProvider waveStream;

        private volatile PlaybackState playbackState;

        private long pos;

        public WaveEvent EventEmitter;

        public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

        public float Volume { get; set; }

        public PlaybackState PlaybackState => playbackState;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public BufferedStream Stream { get; private set; } = new BufferedStream(new MemoryStream(new byte[44100 * 80], true));

        public void Dispose()
        {
        }

        public long GetPosition()
        {
            return pos;
        }

        public void Init(IWaveProvider waveProvider)
        {
            this.waveStream = waveProvider;
            pos = 0;
        }

        public void Pause()
        {
            playbackState = PlaybackState.Paused;
        }

        public void Play()
        {
            //Console.WriteLine("playing sourcewaveoutevent");
            if (playbackState == PlaybackState.Stopped)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        //Console.WriteLine($"Sample rate: {waveStream.WaveFormat.SampleRate}");
                        //Console.WriteLine($"Channels: {waveStream.WaveFormat.Channels}");
                        //Console.WriteLine($"Bits per sample: {waveStream.WaveFormat.BitsPerSample}");

                        await ReadAndWriteToBuffer();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });

                playbackState = PlaybackState.Playing;
            }
            else if (playbackState == PlaybackState.Paused)
            {
                playbackState = PlaybackState.Playing;
            }
        }
        private async Task ReadAndWriteToBuffer()
        {
            //int samples = this.OutputWaveFormat.SampleRate;
            int samples = 128;
            int frames = samples * this.OutputWaveFormat.Channels;
            float[] inputFloatBuffer = new float[frames];
            int inputBufferSize = frames * sizeof(float);
            byte[] inputBuffer = new byte[inputBufferSize];
            int bytes = 0;
            int writeAmount = 0;

            Stopwatch sw = Stopwatch.StartNew();
            int second = 0;
            int loops = 0;
            int err = 0;

            short[] vals = new short[1];
            ulong channels = 2;

            //Console.WriteLine($"inputbuffer size: {inputBufferSize}");

            while ((bytes = waveStream.Read(inputBuffer, 0, inputBufferSize)) > 0)
            {
                sw.Stop();
                //Console.WriteLine($"read time: {sw.Elapsed}");
                sw.Start();
                //Console.WriteLine($"input buffer read: {bytes}");
                ulong floatsRead = (ulong)bytes / sizeof(float);
                //Console.WriteLine($"floats read {floatsRead}");
                //Console.WriteLine($"floatsRead: {floatsRead}");
                loops++;
                Buffer.BlockCopy(inputBuffer, 0, inputFloatBuffer, 0, inputBuffer.Length);
                //Console.WriteLine($"successfully copied float bytes to outBuffer");

                pos += (long)floatsRead / this.OutputWaveFormat.Channels;
                //await EventEmitter(inputFloatBuffer, this.OutputWaveFormat.Channels);
                Stream.Position = pos * sizeof(float) * this.OutputWaveFormat.Channels;
                Stream.Write(inputBuffer, 0, bytes);
                Stream.Position -= bytes;
                //await Task.Delay(TimeSpan.FromMilliseconds(1));
                //Console.WriteLine($"wait: {(int)floatsRead * 1.0 / this.OutputWaveFormat.Channels / this.OutputWaveFormat.SampleRate}s");
                sw.Restart();
                //await Task.Delay(TimeSpan.FromSeconds((int)floatsRead * 1.0 / this.OutputWaveFormat.Channels / this.OutputWaveFormat.SampleRate));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                sw.Stop();
                //Console.WriteLine($"delay time: {sw.Elapsed}");
                sw.Restart();
            }

            //Console.WriteLine($"finished writing audio data; {writeAmount} bytes");
        }

        public void Stop()
        {
            playbackState = PlaybackState.Stopped;
        }
    }
}

