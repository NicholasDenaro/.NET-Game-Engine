using GameEngine.UI.NAudio.LinuxAudio.Interop;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GameEngine.UI.NAudio.LinuxAudio
{
    /* Output buffer:
     * Buffer hax X periods
     * period has Y frames
     * frame has W samples (left + right)
     * sample has 2 bytes (LSB, MSB)
     * This has been extremely useful: https://www.linuxjournal.com/article/6735?page=0,1#N0x19ab2890.0x19ba78d8
    */
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
                    try
                    {
                        (ulong bufferSize, int rate) = SetupDevice();

                        Console.WriteLine($"Sample rate: {waveStream.WaveFormat.SampleRate}");
                        Console.WriteLine($"Channels: {waveStream.WaveFormat.Channels}");
                        Console.WriteLine($"Bits per sample: {waveStream.WaveFormat.BitsPerSample}");

                        ReadAndWriteToBuffer(bufferSize, rate);
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

                int ret = Alsa.snd_pcm_resume(ptr);
                if (ret != 0)
                {
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_resume)} failed with return code {ret}");
                    throw new Exception($"{nameof(Alsa.snd_pcm_resume)} failed with return code {ret}");
                }

                playbackState = PlaybackState.Playing;
            }
        }

        private (ulong bufferSize, int rate) SetupDevice()
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
            //if ((err = Alsa.snd_pcm_hw_params_set_access(ptr, prms, snd_pcm_access_t.SND_PCM_ACCESS_RW_NONINTERLEAVED)) < 0)
            if ((err = Alsa.snd_pcm_hw_params_set_access(ptr, prms, snd_pcm_access_t.SND_PCM_ACCESS_RW_INTERLEAVED)) < 0)
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
            Console.WriteLine($"channels: {(uint)OutputWaveFormat.Channels}");
            if ((err = Alsa.snd_pcm_hw_params_set_channels(ptr, prms, (uint)OutputWaveFormat.Channels)) < 0)
            {
                Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_channels)} failed with return code {err}");
                throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_channels)} failed with return code {err}");
            }

            //uint periods = 1;
            //uint framesInPeriod = 32; // This works
            uint framesInPeriod = 1000;
            Console.WriteLine($"request frames in period near: {framesInPeriod}");
            IntPtr framesInPeriodA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ulong))); ;
            Marshal.StructureToPtr(framesInPeriod, framesInPeriodA, false);
            //uint periods = 32;
            Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_period_size_near)}");
            IntPtr dirUpA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.StructureToPtr(1, dirUpA, false);
            if ((err = Alsa.snd_pcm_hw_params_set_period_size_near(ptr, prms, framesInPeriodA, dirUpA)) < 0)
            {
                Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_period_size_near)} failed with return code {err}");
                throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_period_size_near)} failed with return code {err}");
            }

            framesInPeriod = (uint)Marshal.ReadInt64(framesInPeriodA);
            Console.WriteLine($"set frames in period: {framesInPeriod}");

            //ulong periodSize = 4096;
            //ulong frames = (periodSize * periods) / sizeof(short) / (ulong)this.OutputWaveFormat.Channels;
            //Console.WriteLine($"periodSize: {periodSize}");
            //Console.WriteLine($"frames: {frames}");
            ulong size = framesInPeriod * sizeof(short) * (ulong)this.OutputWaveFormat.Channels;
            Console.WriteLine($"buffer size: {size}");
            //IntPtr sizeA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ulong)));
            //Marshal.StructureToPtr(size, sizeA, false);
            //Console.WriteLine($"request buffer size: {(ulong)Marshal.ReadInt64(sizeA)}");
            //Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)}");
            //if ((err = Alsa.snd_pcm_hw_params_set_buffer_size_near(ptr, prms, sizeA)) < 0)
            //{
            //    Console.WriteLine($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)} failed with return code {err}");
            //    throw new Exception($"{nameof(Alsa.snd_pcm_hw_params_set_buffer_size_near)} failed with return code {err}");
            //}

            //ulong bufferSize = (ulong)Marshal.ReadInt64(sizeA);
            //Console.WriteLine($"Buffer size: {bufferSize}");

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

            Alsa.snd_pcm_hw_params_free(prms);

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

            Alsa.snd_pcm_sw_params_free(prms);

            return (size, rate);
        }

        private bool first = true;
        private void ReadAndWriteToBuffer(ulong bufferSize, int rate)
        {
            IntPtr buffA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * (int)bufferSize);

            Console.WriteLine($"stream block align: {waveStream.WaveFormat.BlockAlign}");
            // read half the amount of floats, because we need to convert to shorts which are 2 bytes
            int samples = (int)bufferSize / sizeof(short);
            byte[] outBuffer = new byte[samples * sizeof(short)];
            float[] inputFloatBuffer = new float[samples];
            int inputBufferSize = samples * sizeof(float);
            byte[] inputBuffer = new byte[inputBufferSize];
            int bytes = 0;
            int writeAmount = 0;

            Stopwatch sw = Stopwatch.StartNew();
            int second = 0;
            int loops = 0;
            int err = 0;

            short[] vals = new short[1];
            ulong channels = 2;

            Console.WriteLine($"inputbuffer size: {inputBufferSize}");
            Console.WriteLine($"outBuffer size: {outBuffer.Length}");

            while ((bytes = waveStream.Read(inputBuffer, 0, inputBufferSize)) > 0)
            {
                //Console.WriteLine($"input buffer read: {bytes}");
                ulong floatsRead = (ulong)bytes / sizeof(float);
                //Console.WriteLine($"floatsRead: {floatsRead}");
                ulong outputBytes = floatsRead * sizeof(short);
                //Console.WriteLine($"output buffer size: {outputBytes}");
                loops++;
                Buffer.BlockCopy(inputBuffer, 0, inputFloatBuffer, 0, inputBuffer.Length);
                //Console.WriteLine($"successfully copied input bytes to inputFloatBuffer");
                for (int i = 0; i < (int)floatsRead; i++)
                {
                    // convert float to short and then write
                    vals[0] = (short)(inputFloatBuffer[i] * short.MaxValue);
                    // write the vals to the output buffer;
                    Buffer.BlockCopy(vals, 0, outBuffer, i * sizeof(short), vals.Length * sizeof(short));
                }
                //Console.WriteLine($"successfully copied float bytes to outBuffer");

                writeAmount += bytes / sizeof(float);
                Marshal.Copy(outBuffer, 0, buffA, (int)outputBytes);
                //Console.WriteLine($"Marshaled outputBytes to buffA");
                //if ((err = Alsa.snd_pcm_writen(ptr, ref buffA, outputBytes / magicNumberForOutputFramesOrSomething)) < 0)
                //if ((err = Alsa.snd_pcm_writei(ptr, buffA, outputBytes / magicNumberForOutputFramesOrSomething)) < 0)
                ulong outputFrames = outputBytes / sizeof(short) / channels;
                if ((err = Alsa.snd_pcm_writei(ptr, buffA, outputFrames)) < 0)
                {
                    Console.WriteLine($"{nameof(Alsa.snd_pcm_writei)} failed with return code {err}");
                    Console.WriteLine($"\tBytes read from input: {bytes}");
                    Console.WriteLine($"\tTried to write output bytes: {outputBytes}");
                    Console.WriteLine($"\tTried to write output frames: {outputFrames}");
                    Console.WriteLine($"\toutBuffer size: {outBuffer.Length}");
                    if ((err = Alsa.snd_pcm_recover(ptr, err, 0)) < 0)
                    {
                        Console.WriteLine($"Failed to recover from error");
                        Console.WriteLine($"{nameof(Alsa.snd_pcm_recover)} failed with return code {err}");
                        throw new Exception($"{nameof(Alsa.snd_pcm_writei)} failed with return code {err}");
                    }
                }

                if (first)
                {
                    Console.WriteLine("First loop");
                    Console.WriteLine($"\tBytes read from input: {bytes}");
                    Console.WriteLine($"\tTried to write output bytes: {outputBytes}");
                    Console.WriteLine($"\tTried to write output frames: {outputFrames}");
                    Console.WriteLine($"\toutBuffer size: {outBuffer.Length}");
                    first = false;
                }

                //if (sw.ElapsedTicks >= Stopwatch.Frequency)
                //{
                //    Console.WriteLine($"{++second:00} | loops {loops:000}  | {writeAmount:00000000} | {writeAmount * 100.0 / rate:000}% {Stopwatch.Frequency * 1.0 / sw.ElapsedTicks}s");
                //    sw.Restart();
                //}
            }

            Marshal.FreeHGlobal(buffA);

            Console.WriteLine($"finished writing audio data; {writeAmount} bytes");
        }


        private void ReadAndWriteToBuffer2Channel(ulong periodSize, int rate)
        {
            IntPtr buffA = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * (int)periodSize);

            // read half the amount of floats, because we need to convert to shorts which are 2 bytes
            int inputBufferSize = (int)periodSize * sizeof(float) / sizeof(short);
            byte[] inputBuffer = new byte[inputBufferSize];
            byte[] outBuffer = new byte[periodSize];
            float[] inputFloatBuffer = new float[inputBufferSize / sizeof(float)];
            int bytes = 0;
            int writeAmount = 0;

            Stopwatch sw = Stopwatch.StartNew();
            int second = 0;
            int loops = 0;
            int err = 0;

            short[] vals = new short[1];

            ulong magicNumberForOutputFramesOrSomething = 2;

            while ((bytes = waveStream.Read(inputBuffer, 0, inputBufferSize)) > 0)
            {
                ulong floatsRead = (ulong)bytes / sizeof(float);
                ulong outputBytes = floatsRead * sizeof(short);
                loops++;
                Buffer.BlockCopy(inputBuffer, 0, inputFloatBuffer, 0, inputBuffer.Length);
                for (int i = 0; i < (int)floatsRead; i++)
                {
                    // convert float to short and then write
                    vals[0] = (short)(inputFloatBuffer[i] * short.MaxValue);
                    // write the vals to the output buffer;
                    Buffer.BlockCopy(vals, 0, outBuffer, i * sizeof(short), vals.Length * sizeof(short));
                }
                writeAmount += bytes / sizeof(float);
                Marshal.Copy(outBuffer, 0, buffA, (int)outputBytes);
                //if ((err = Alsa.snd_pcm_writen(ptr, ref buffA, outputBytes / magicNumberForOutputFramesOrSomething)) < 0)
                if ((err = Alsa.snd_pcm_writei(ptr, buffA, outputBytes / magicNumberForOutputFramesOrSomething)) < 0)
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
