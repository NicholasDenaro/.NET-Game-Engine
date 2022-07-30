using GameEngine.UI.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static GameEngine.UI.NAudio.SinWaveSound;

namespace GameEngine.UI.NAudio
{
    public class NAudioMMLTrack : ITrack
    {
        private MML mml;
        private IEnumerable<NAudioSound> channels;

        public string Name { get; private set; }

        public static Dictionary<string, NAudioMMLTrack> Tracks { get; private set; } = new Dictionary<string, NAudioMMLTrack>();

        public NAudioMMLTrack(string name, Waves wave, MML mml)
        {
            Name = name;
            Tracks.Add(name, this);
            this.mml = mml;
            this.channels = mml.Channels.Select((channel, i) => new NAudioSound($"{name}_{i}", wave, channel)).ToList();
        }

        public int Length => mml.Channels.Count();

        public IEnumerable<ISound> Channels()
        {
            return channels;
        }

        public ISound GetChannel(int c)
        {
            return channels.Skip(c).First();
        }
    }
}
