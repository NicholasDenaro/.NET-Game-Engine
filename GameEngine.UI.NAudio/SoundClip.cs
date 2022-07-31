using GameEngine.UI.Audio;
using GameEngine.UI.NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI.NAudio
{
    public class SoundClip : ISound
    {
        MemoryStream stream;
        public static Dictionary<string, SoundClip> Sounds { get; private set; } = new Dictionary<string, SoundClip>();

        public SoundClip(Assembly assembly, string name)
        {
            var resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{name}");
            stream = new MemoryStream();

            WaveFileReader wfr = new WaveFileReader(resource);
            Console.WriteLine($"wfr.Length: {wfr.Length}");
            AudioConverter ac = new AudioConverter(wfr);
            int read = 0;
            byte[] buf = new byte[1000 * ac.WaveFormat.Channels * (ac.WaveFormat.BitsPerSample / 8)];
            while ((read = ac.Read(buf, 0, buf.Length)) > 0)
            {
                Console.WriteLine($"{read}/{buf.Length}");
                stream.Write(buf, 0, read);
                TotalSamples += read / ac.WaveFormat.Channels / (ac.WaveFormat.BitsPerSample / 8);
            }

            stream.Position = 0;

            Name = name;
            Sounds.Add(Name, this);
        }

        public string Name { get; private set; }

        public int TotalSamples { get; private set; }

        public Stream GetStream()
        {
            stream.Position = 0;
            return stream;
        }
    }
}
