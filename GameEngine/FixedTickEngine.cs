using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    internal class FixedTickEngine : GameEngine
    {
        private int ticksPerSecond;
        private long period;

        public FixedTickEngine(int tps)
        {
            this.ticksPerSecond = tps;
            period = (int)TimeSpan.FromSeconds(1).TotalMilliseconds / ticksPerSecond;
            Console.WriteLine("period: {0}", period);
        }

        public override void Control()
        {
            Task.Run(() => Run());
        }

        private void Run()
        {
            Stopwatch sw = Stopwatch.StartNew();
            long overwait = 0;
            long wait;
            while (true)
            {
                wait = period - overwait;
                while (sw.ElapsedMilliseconds < wait) { };
                overwait = wait - sw.ElapsedMilliseconds;
                sw.Restart();
                Tick();
                Draw();
            }
        }
    }
}
