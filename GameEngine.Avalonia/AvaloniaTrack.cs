using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaTrack : ITrack
    {
        private MML mml;
        private IEnumerable<AvaloniaSound> channels;

        public AvaloniaTrack(MML mml)
        {
            this.mml = mml;
            this.channels = mml.Channels.Select(channel => new AvaloniaSound(channel));
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
