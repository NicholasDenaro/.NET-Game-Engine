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

        public NAudioMMLTrack(Waves wave, MML mml)
        {
            this.mml = mml;
            this.channels = mml.Channels.Select(channel => new NAudioSound(wave, channel));
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
