using NAudio.Wave;
using System;
using System.Diagnostics;

namespace GameEngine.UI.AvaloniaUI.LinuxAudio
{
    internal class AudioConverter : WaveProvider32
    {
        private WaveFileReader reader;
        private WaveFormat format;

        public AudioConverter(WaveFileReader wfr)
        {
            this.reader = wfr;
            this.format = wfr.WaveFormat;
            this.SetWaveFormat(44100, 1);
        }

        Stopwatch sw = new Stopwatch();
        long writtenTotal = 0;
        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int written;
            sw.Start();
            if (format.Encoding == WaveFormatEncoding.Pcm)
            {
                written = this.ReadPCMGeneralized(buffer, offset, sampleCount);
            }
            else
            {
                throw new Exception($"Audio format {format.Encoding} not surpported");
            }

            if (written >= 0)
            {
                writtenTotal += written;
            }
            if (written < sampleCount)
            {
                sw.Stop();
                Console.WriteLine($"Time: {sw.ElapsedTicks * 1.0f / Stopwatch.Frequency}s");
                Console.WriteLine($"Time: {(sw.ElapsedTicks + written * Stopwatch.Frequency / 44100) * 1.0f / Stopwatch.Frequency}s");
                Console.WriteLine($"Written: {writtenTotal}bytes");
            }

            return written;
        }

        private int ReadPCM16C1(float[] buffer, int offset, int sampleCount)
        {
            byte[] inputBuffer = new byte[sampleCount * sizeof(short)];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            short[] shortInputBuffer = new short[bytes / sizeof(short)];
            Buffer.BlockCopy(inputBuffer, 0, shortInputBuffer, 0, bytes);
            for (int i = 0; i < shortInputBuffer.Length; i++)
            {
                buffer[offset + i] = shortInputBuffer[i] * 1.0f / short.MaxValue;
            }
            return bytes / sizeof(short);
        }

        private int ReadPCM16(float[] buffer, int offset, int sampleCount)
        {
            byte[] inputBuffer = new byte[sampleCount * sizeof(short) * this.format.Channels];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[sampleCount * this.format.Channels];
            short val = 0;
            int maxVal = (int)Math.Pow(2, 15);
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = (short)(val + (inputBuffer[i] << ((i % sizeof(short)) * 8)));
                if (i % sizeof(short) == sizeof(short) - 1)
                {
                    floatBuffer[i / sizeof(short)] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            for (int i = 0; i < floatBuffer.Length / this.format.Channels; i++)
            {
                float y1 = floatBuffer[i * 2];
                float y2 = floatBuffer[i  * 2 + 1];
                //float x1 = -(float)Math.Log2(2 / (y1 + 1) - 1);
                //float x2 = -(float)Math.Log2(2 / (y2 + 1) - 1);
                //float x3 = x1 + x2;
                //float y3 = (float)(2 / (1 + Math.Pow(Math.E, -x3)) - 1);
                //buffer[offset + i] = y3 / 2;
                buffer[offset + i] = (y1 + y2) / 2;
            }

            return bytes / sizeof(short) / this.format.Channels;
        }

        const int sizeofInt24 = 3;
        private int ReadPCM24(float[] buffer, int offset, int sampleCount)
        {
            byte[] inputBuffer = new byte[sampleCount * sizeofInt24 * this.format.Channels];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[sampleCount * this.format.Channels];
            int val = 0;
            int maxVal = (int)Math.Pow(2, 23);
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = val + (inputBuffer[i] << ((i % sizeofInt24 + 1) * 8));
                if (i % sizeofInt24 == sizeofInt24 - 1)
                {
                    val = val >> 8;
                    floatBuffer[i / sizeofInt24] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            for (int i = 0; i < floatBuffer.Length / this.format.Channels; i++)
            {
                float y1 = floatBuffer[i * 2];
                float y2 = floatBuffer[i * 2 + 1];
                buffer[offset + i] = (y1 + y2) / 2;
            }

            return bytes / sizeofInt24 / this.format.Channels;
        }

        private int ReadPCM32(float[] buffer, int offset, int sampleCount)
        {
            byte[] inputBuffer = new byte[sampleCount * sizeof(int) * this.format.Channels];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[sampleCount * this.format.Channels];
            int val = 0;
            int maxVal = int.MaxValue;
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = val + (inputBuffer[i] << ((i % sizeof(int)) * 8));
                if (i % sizeof(int) == sizeof(int) - 1)
                {
                    floatBuffer[i / sizeof(int)] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            for (int i = 0; i < floatBuffer.Length / this.format.Channels; i++)
            {
                float y1 = floatBuffer[i * 2];
                float y2 = floatBuffer[i * 2 + 1];
                buffer[offset + i] = (y1 + y2) / 2;
            }

            return bytes / sizeof(int) / this.format.Channels;
        }

        private int index = 0;
        private int offset48 = 0;
        private float lastVal = 0;
        private int ReadPCM24_48k(float[] buffer, int offset, int sampleCount)
        {
            byte[] inputBuffer = new byte[(int)(sampleCount * sizeofInt24 * this.format.Channels * 48000 * 1.0f / 44100)];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[(int)(sampleCount * this.format.Channels * 48000 * 1.0f / 44100)];
            int val = 0;
            int maxVal = (int)Math.Pow(2, 23);
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = val + (inputBuffer[i] << ((i % sizeofInt24 + 1) * 8));
                if (i % sizeofInt24 == sizeofInt24 - 1)
                {
                    val = val >> 8;
                    floatBuffer[i / sizeofInt24] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            // TODO: Fix the offset issue. Right now we're always assuming i = j = 0, but that assuming
            // likely only holds true on the seconds boundaries. We need to store what the curren offset
            // is and the last value, and use that to influence the interpolation
            // As is though, I'm not sure you can tell the difference between 48k and 44.1k
            for (int i = 0; i < bytes / sizeofInt24 / this.format.Channels * 44100 * 1.0f / 48000; i++)
            {
                int j = (int)(i * 48000 * 1.0f / 44100);
                float pc = 1 - (i * 48000 * 1.0f / 44100 - j);
                float pd = 1 - pc;

                float c1 = floatBuffer[j * this.format.Channels];
                float c2 = floatBuffer[j * this.format.Channels + 1];

                float d1 = floatBuffer[j * this.format.Channels + this.format.Channels];
                float d2 = floatBuffer[j * this.format.Channels + 1 + this.format.Channels];


                float y1 = c1 * pc + d1 * pd;
                float y2 = c2 * pc + d2 * pd;

                buffer[offset + i] = (y1 + y2) / 2;
            }

            return (int)(bytes / sizeofInt24 / this.format.Channels * 44100 * 1.0f / 48000);
        }


        private int ReadPCMGeneralized(float[] buffer, int offset, int sampleCount)
        {
            int sizeofBits = this.format.BitsPerSample / 8;
            int channels = this.format.Channels;
            int sampleRate = this.format.SampleRate;
            byte[] inputBuffer = new byte[(int)(sampleCount * sizeofBits * channels * (sampleRate * 1.0f / 44100))];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[(int)(sampleCount * channels * sampleRate * 1.0f / 44100)];
            int val = 0;
            int maxVal = (int)Math.Pow(2, this.format.BitsPerSample - 1);
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = val + (inputBuffer[i] << ((i % sizeofBits + (sizeof(int) - sizeofBits)) * 8));
                if (i % sizeofBits == sizeofBits - 1)
                {
                    val = val >> ((sizeof(int) - sizeofBits) * 8);
                    floatBuffer[i / sizeofBits] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            // TODO: Fix the offset issue. Right now we're always assuming i = j = 0, but that assuming
            // likely only holds true on the seconds boundaries. We need to store what the curren offset
            // is and the last value, and use that to influence the interpolation
            // As is though, I'm not sure you can tell the difference between 48k and 44.1k
            // TODO: Handle 2 channel output
            for (int i = 0; i < bytes / sizeofBits / channels * 44100 * 1.0f / sampleRate; i++)
            {
                int j = (int)(i * sampleRate * 1.0f / 44100);
                float pc = 1 - (i * sampleRate * 1.0f / 44100 - j);
                float pd = 1 - pc;

                float yc = 0;
                float yd = 0;

                for (int c = 0; c < channels; c++)
                {
                    yc += floatBuffer[j * channels + c] / channels;
                }

                for (int c = 0; c < channels; c++)
                {
                    yd += floatBuffer[Math.Min(j * channels + channels, floatBuffer.Length - 2)] / channels;
                }

                buffer[offset + i] = (yc + yd) / 2;
            }

            return (int)(bytes / sizeofBits / channels * 44100 * 1.0f / sampleRate);
        }
    }
}
