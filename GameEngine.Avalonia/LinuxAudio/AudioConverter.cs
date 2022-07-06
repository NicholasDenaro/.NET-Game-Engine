using NAudio.Wave;
using System;
using System.Diagnostics;

namespace GameEngine.UI.AvaloniaUI.LinuxAudio
{
    internal class AudioConverter : WaveProvider32
    {
        private WaveFileReader reader;
        private WaveFormat format;
        private const int outputChannels = 2;
        private const int outputSampleRate = 44100;

        public AudioConverter(WaveFileReader wfr)
        {
            this.reader = wfr;
            this.format = wfr.WaveFormat;
            this.SetWaveFormat(outputSampleRate, outputChannels);
        }

        Stopwatch sw = new Stopwatch();
        long writtenTotal = 0;
        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int written;
            sw.Start();
            if (format.Encoding == WaveFormatEncoding.Pcm)
            {
                written = this.ReadPCMGeneralized2ChannelOut(buffer, offset, sampleCount);
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
                Console.WriteLine($"Time: {(sw.ElapsedTicks + written * Stopwatch.Frequency / outputSampleRate) * 1.0f / Stopwatch.Frequency}s");
                Console.WriteLine($"Written: {writtenTotal}bytes");
            }

            return written;
        }

        private int ReadPCMGeneralized1ChannelOut(float[] buffer, int offset, int sampleCount)
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

        private int ReadPCMGeneralized2ChannelOut(float[] buffer, int offset, int sampleCount)
        {
            int sizeofBits = this.format.BitsPerSample / 8;
            int channels = this.format.Channels;
            int sampleRate = this.format.SampleRate;
            byte[] inputBuffer = new byte[(int)(sampleCount * sizeofBits * channels / 2 * (sampleRate * 1.0f / outputSampleRate))];
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }

            float[] floatBuffer = new float[(int)(sampleCount * channels / 2 * sampleRate * 1.0f / outputSampleRate)];
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
            if (channels == outputChannels)
            {
                for (int i = 0; i < bytes / sizeofBits * outputSampleRate * 1.0f / sampleRate; i++)
                {
                    int j = (int)(i * sampleRate * 1.0f / outputSampleRate);
                    if (i % 2 != j % 2)
                    {
                        j++;
                    }

                    float pc = 1 - (i * sampleRate * 1.0f / outputSampleRate - j);
                    float pd = 1 - pc;

                    float yc = floatBuffer[j];
                    float yd = floatBuffer[Math.Min(j + channels, floatBuffer.Length - channels)];

                    buffer[offset + i] = yc * pc + yd * pd;
                }

                return (int)(bytes / sizeofBits * outputSampleRate * 1.0f / sampleRate);
            }
            else if (channels == 1)
            {
                for (int i = 0; i < bytes / sizeofBits / channels * outputSampleRate * 1.0f / sampleRate; i++)
                {
                    int j = (int)(i * sampleRate * 1.0f / outputSampleRate);
                    float pc = 1 - (i * sampleRate * 1.0f / outputSampleRate - j);
                    float pd = 1 - pc;

                    float yc = floatBuffer[j];
                    float yd = floatBuffer[Math.Min(j + 1, floatBuffer.Length - 1)];

                    for (int c = 0; c < outputChannels; c++)
                    {
                        buffer[offset + i * outputChannels + c] = yc * pc + yd * pd;
                    }
                }

                return (int)(bytes / sizeofBits * outputChannels * outputSampleRate * 1.0f / sampleRate);
            }
            else
            {
                throw new Exception($"Can't handle more than {outputChannels} channels.");
            }
        }
    }
}
