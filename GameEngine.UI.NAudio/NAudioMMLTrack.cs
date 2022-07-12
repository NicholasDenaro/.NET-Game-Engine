using GameEngine.UI.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.UI.NAudio
{
    public class NAudioMMLTrack : ITrack
    {
        private MML mml;
        private IEnumerable<NAudioSound> channels;

        public NAudioMMLTrack(MML mml)
        {
            this.mml = mml;
            this.channels = mml.Channels.Select(channel => new NAudioSound(channel));
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
