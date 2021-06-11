using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI
{
    public interface ITrack
    {
        IEnumerable<ISound> Channels();

        ISound GetChannel(int c);

        int Length { get; }
    }
}
