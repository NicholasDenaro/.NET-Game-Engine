using NAudio.Wave;
using System;
using System.Diagnostics;

namespace GameEngine.UI.NAudio
{
    internal class AudioConverter : WaveProvider32
    {
        private WaveFileReader reader;
        private WaveFormat format;
        private const int outputChannels = 2;
        private const int outputSampleRate = 44100;

        public AudioConverter(WaveFileReader wfr)
        {
            Console.WriteLine("Using AudioConverter");
            reader = wfr;
            format = wfr.WaveFormat;
            SetWaveFormat(outputSampleRate, outputChannels);
        }

        Stopwatch sw = new Stopwatch();
        //long writtenTotal = 0;
        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int written;
            //if (!sw.IsRunning)
            //{
            //    sw.Start();
            //}

            if (format.Encoding == WaveFormatEncoding.Pcm)
            {
                written = ReadPCMGeneralized2ChannelOut(buffer, offset, sampleCount);
            }
            else
            {
                throw new Exception($"Audio format {format.Encoding} not surpported");
            }

            //if (written >= 0)
            //{
            //    writtenTotal += written;
            //}
            //if (written < sampleCount)
            //{
            //    sw.Stop();
            //    Console.WriteLine($"Time: {sw.ElapsedTicks * 1.0f / Stopwatch.Frequency}s");
            //    Console.WriteLine($"Time: {(sw.ElapsedTicks + written * Stopwatch.Frequency / outputSampleRate) * 1.0f / Stopwatch.Frequency}s");
            //    Console.WriteLine($"Written: {writtenTotal}bytes");
            //}

            return written;
        }

        private int ReadPCMGeneralized2ChannelOut(float[] buffer, int offset, int sampleCount)
        {
            //Console.WriteLine($"AudioConverter:\n\tsamples:{sampleCount}");
            int sizeofBits = format.BitsPerSample / 8;
            int channels = format.Channels;
            int sampleRate = format.SampleRate;
            //Console.WriteLine($"\tsizeofBits:{sizeofBits}");
            //Console.WriteLine($"\tchannels:{channels}");
            //Console.WriteLine($"\tsampleRate:{sampleRate}");
            int bufferSize = (int)(sampleCount * sizeofBits * channels / 2 * (sampleRate * 1.0f / outputSampleRate));
            //Console.WriteLine($"\tbufferSize:{bufferSize}");
            int bufferSizeAligned = bufferSize / format.BlockAlign * format.BlockAlign;
            if (bufferSizeAligned < bufferSize)
            {
                bufferSizeAligned += format.BlockAlign;
            }
            //Console.WriteLine($"\tbufferSizeAligned:{bufferSizeAligned}");
            byte[] inputBuffer = new byte[bufferSizeAligned];
            //byte[] inputBuffer = new byte[bufferSize];
            //Console.WriteLine($"\tinputBuffer.Length:{inputBuffer.Length}");
            int bytes = reader.Read(inputBuffer, 0, inputBuffer.Length);
            if (bytes <= 0)
            {
                return bytes;
            }
            //Console.WriteLine($"\tbytes:{bytes}");

            //int floatBufferSize = (int)(sampleCount * channels / 2 * sampleRate * 1.0f / outputSampleRate);
            int floatBufferSize = inputBuffer.Length * sizeofBits;
            //Console.WriteLine($"float buffer size: {floatBufferSize}");

            float[] floatBuffer = new float[floatBufferSize];
            int val = 0;
            int maxVal = (int)Math.Pow(2, format.BitsPerSample - 1);
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                val = val + (inputBuffer[i] << (i % sizeofBits + (sizeof(int) - sizeofBits)) * 8);
                if (i % sizeofBits == sizeofBits - 1)
                {
                    val = val >> (sizeof(int) - sizeofBits) * 8;
                    floatBuffer[i / sizeofBits] = val * 1.0f / maxVal;
                    val = 0;
                }
            }

            //Console.WriteLine("\tfinished writing to float buffer");

            // TODO: Fix the offset issue. Right now we're always assuming i = j = 0, but that assuming
            // likely only holds true on the seconds boundaries. We need to store what the curren offset
            // is and the last value, and use that to influence the interpolation
            // As is though, I'm not sure you can tell the difference between 48k and 44.1k
            if (channels == outputChannels)
            {
                int samplesOut = (int)(bytes / sizeofBits * outputSampleRate * 1.0f / sampleRate);
                //Console.WriteLine($"\tWriting {samplesOut} samples");
                for (int i = 0; i < samplesOut; i++)
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
                //Console.WriteLine($"\twrite :{samplesOut} bytes");

                return samplesOut;
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
