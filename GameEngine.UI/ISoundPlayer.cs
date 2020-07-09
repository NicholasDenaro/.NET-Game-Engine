using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEngine.UI
{
    public interface ISoundPlayer
    {
        void Play(Stream stream);
    }
}
