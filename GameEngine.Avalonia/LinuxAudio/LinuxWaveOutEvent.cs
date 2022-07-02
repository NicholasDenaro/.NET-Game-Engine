using GameEngine.UI.AvaloniaUI.LinuxAudio.Interop;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI.LinuxAudio
{
    internal class LinuxWaveOutEvent : IWavePlayer, IDisposable, IWavePosition
    {
        private IntPtr ptr;

        private IWaveProvider waveStream;

        private volatile PlaybackState playbackState;

        public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

        public float Volume { get; set; }

        public PlaybackState PlaybackState => playbackState;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public void Dispose()
        {
            if (ptr != IntPtr.Zero)
            {
                Alsa.snd_pcm_close(ptr);
                ptr = IntPtr.Zero;
            }
        }

        public long GetPosition()
        {
            IntPtr t = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(timespec)));
            Alsa.snd_pcm_htimestamp(ptr, IntPtr.Zero, t);
            timespec time = new timespec();
            Marshal.PtrToStructure(t, time);
            long val = time.tv_nsec;
            Marshal.FreeHGlobal(t);
            return val;
        }

        public void Init(IWaveProvider waveProvider)
        {
            this.waveStream = waveProvider;
            int ret = Alsa.snd_pcm_open(ref ptr, "default", snd_pcm_stream_t.SND_PCM_STREAM_PLAYBACK, 0);
            if (ret != 0)
            {
                Console.WriteLine($"{nameof(Alsa.snd_pcm_open)} failed with return code {ret}");
                throw new Exception($"{nameof(Alsa.snd_pcm_open)} failed with return code {ret}");
            }
        }

        public void Pause()
        {
            int ret = Alsa.snd_pcm_pause(ptr, 1);
            if (ret != 0)
            {
                Console.WriteLine($"{nameof(Alsa.snd_pcm_pause)} failed with return code {ret}");
                throw new Exception($"{nameof(Alsa.snd_pcm_pause)} failed with return code {ret}");
            }

            playbackState = PlaybackState.Paused;
        }

        public void Play()
        {
            if (playbackState == PlaybackState.Stopped)
            {
                Task.Run(() =>
                {
                    // hw
                    IntPtr prms = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    int err;
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_malloc)}");
                    if ((err = Alsa.snd_pcm_hw_params_malloc(ref prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_malloc)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_malloc)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_any)}");
                    if ((err = Alsa.snd_pcm_hw_params_any(ptr, prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_any)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_any)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_access)}");
                    if ((err = Alsa.snd_pcm_hw_params_set_access(ptr, prms, snd_pcm_access_t.SND_PCM_ACCESS_RW_NONINTERLEAVED)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_access)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_access)} failed with return code {err}");
                    }
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_format)}");
                    //if ((err = Alsa.snd_pcm_hw_params_set_format(ptr, prms, snd_pcm_format_t.SND_PCM_FORMAT_FLOAT_LE)) < 0) // sounds horrible
                    //if ((err = Alsa.snd_pcm_hw_params_set_format(ptr, prms, snd_pcm_format_t.SND_PCM_FORMAT_S32_LE)) < 0) // does not work
                    if ((err = Alsa.snd_pcm_hw_params_set_format(ptr, prms, snd_pcm_format_t.SND_PCM_FORMAT_S16_LE)) < 0) // sounds kind of close
                    //if ((err = Alsa.snd_pcm_hw_params_set_format(ptr, prms, snd_pcm_format_t.SND_PCM_FORMAT_S8)) < 0) // does not work
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_format)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_format)} failed with return code {err}");
                    }

                    int rate = waveStream.WaveFormat.SampleRate;
                    Console.WriteLine($"Setting rate to {rate}");
                    uint exactRate = (uint)rate;
                    IntPtr exactRateA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)));
                    Marshal.StructureToPtr(exactRate, exactRateA, false);
                    IntPtr dirA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
                    Marshal.StructureToPtr(0, dirA, false);
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_rate_near)}");
                    if ((err = Alsa.snd_pcm_hw_params_set_rate_near(ptr, prms, exactRateA, dirA)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_rate_near)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_rate_near)} failed with return code {err}");
                    }

                    exactRate = (uint)Marshal.ReadInt32(exactRateA);
                    Console.WriteLine($"Rate found: {exactRate}");

                    if (rate != exactRate)
                    {
                        Console.WriteLine($"No matching rate found {rate}!={exactRate}");
                        throw new Exception($"No matching rate found {rate}!={exactRate}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_channels)}");
                    if ((err = Alsa.snd_pcm_hw_params_set_channels(ptr, prms, 1)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_channels)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_channels)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_periods)}");
                    uint periods = 4;
                    //uint periods = 32;
                    if ((err = Alsa.snd_pcm_hw_params_set_periods(ptr, prms, periods, 0)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_periods)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_periods)} failed with return code {err}");
                    }
                    ulong periodSize = 4096;
                    ulong frames = (periodSize * periods) >> 2;
                    Console.WriteLine($"frames: {frames}");
                    IntPtr sizeA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ulong)));
                    Marshal.StructureToPtr(frames, sizeA, false);
                    Console.WriteLine($"request buffer size: {(ulong)Marshal.ReadInt64(sizeA)}");
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)}");
                    if ((err = Alsa.snd_pcm_hw_params_set_buffer_size_near(ptr, prms, sizeA)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)} failed with return code {err}");
                    }

                    ulong bufferSize = (ulong)Marshal.ReadInt64(sizeA);
                    Console.WriteLine($"Buffer size: {bufferSize}");

                    Console.WriteLine("finished setting up params");

                    // update the params;
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params)}");
                    if ((err = Alsa.snd_pcm_hw_params(ptr, prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_hw_params)} failed with return code {err}");
                    }


                    IntPtr val = Marshal.AllocHGlobal(sizeof(uint));
                    Alsa.snd_pcm_hw_params_get_period_time(prms, val, dirA);

                    uint timespanForPeriod = (uint)Marshal.ReadInt32(val);

                    Console.WriteLine($"period time: {timespanForPeriod}");
                    Console.WriteLine($"time for 1 second: {1000000.0 / timespanForPeriod}");

                    //TODO: free prms using Alsa free;

                    //sw
                    prms = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_malloc)}");
                    if ((err = Alsa.snd_pcm_sw_params_malloc(ref prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_malloc)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_sw_params_malloc)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_current)}");
                    if ((err = Alsa.snd_pcm_sw_params_current(ptr, prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_current)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_sw_params_current)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_set_avail_min)}");
                    if ((err = Alsa.snd_pcm_sw_params_set_avail_min(ptr, prms, 1024)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_set_avail_min)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_sw_params_set_avail_min)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_set_start_threshold)}");
                    if ((err = Alsa.snd_pcm_sw_params_set_start_threshold(ptr, prms, 1)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params_set_start_threshold)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_sw_params_set_start_threshold)} failed with return code {err}");
                    }

                    Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params)}");
                    if ((err = Alsa.snd_pcm_sw_params(ptr, prms)) < 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_sw_params)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_sw_params)} failed with return code {err}");
                    }

                    //TODO: free prms using Alsa free

                    IntPtr buffA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * (int)periodSize);


                    Console.WriteLine($"Sample rate: {waveStream.WaveFormat.SampleRate}");
                    Console.WriteLine($"Channels: {waveStream.WaveFormat.Channels}");
                    Console.WriteLine($"Bits per sample: {waveStream.WaveFormat.BitsPerSample}");

                    byte[] readbuffer = new byte[periodSize * sizeof(float)];
                    byte[] buff = new byte[periodSize];
                    float[] fbuff = new float[periodSize];
                    int bytes = 0;
                    int writeAmount = 0;

                    int ret = Alsa.snd_pcm_start(ptr);
                    if (ret != 0)
                    {
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_start)} failed with return code {ret}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_start)} failed with return code {ret}");
                    }

                    Stopwatch sw = Stopwatch.StartNew();
                    int second = 0;
                    int loops = 0;
                    while ((bytes = waveStream.Read(readbuffer, 0, (int)periodSize * sizeof(float))) > 0)
                    {
                        loops++;
                        Buffer.BlockCopy(readbuffer, 0, fbuff, 0, readbuffer.Length);
                        for (int i = 0; i < bytes / sizeof(float); i++)
                        {
                            buff[i] = (byte)((fbuff[i] + 1) * 127);
                        }
                        writeAmount += bytes / sizeof(float);
                        Marshal.Copy(buff, 0, buffA, bytes / sizeof(float));
                        if ((err = Alsa.snd_pcm_writen(ptr, ref buffA, (ulong)bytes / 2 / sizeof(float))) < 0)
                        {
                            Console.WriteLine($"{nameof(Alsa.snd_pcm_writen)} failed with return code {err}");
                            throw new Exception($"{nameof(Alsa.snd_pcm_writen)} failed with return code {err}");
                        }

                        if (sw.ElapsedTicks >= Stopwatch.Frequency)
                        {
                            Console.WriteLine($"{++second:00} | loops {loops:000}  | {writeAmount:00000000} | {writeAmount * 100.0 / rate:000}% {Stopwatch.Frequency * 1.0 / sw.ElapsedTicks}s");
                            sw.Restart();
                        }
                    }

                    Marshal.FreeHGlobal(buffA);

                    Console.WriteLine($"finished writing audio data; {writeAmount} bytes");

                });

                playbackState = PlaybackState.Playing;
            }
            else if (playbackState == PlaybackState.Paused)
            {

                int ret = Alsa.snd_pcm_resume(ptr);
                if (ret != 0)
                {
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_resume)} failed with return code {ret}");
                    throw new Exception($"{nameof(Alsa.snd_pcm_resume)} failed with return code {ret}");
                }

                playbackState = PlaybackState.Playing;
            }
        }

        public void Stop()
        {
            int ret = Alsa.snd_pcm_drop(ptr);
            if (ret != 0)
            {
                Console.WriteLine($"{nameof(Alsa.snd_pcm_drop)} failed with return code {ret}");
                throw new Exception($"{nameof(Alsa.snd_pcm_drop)} failed with return code {ret}");
            }

            playbackState = PlaybackState.Stopped;
        }
    }
}
