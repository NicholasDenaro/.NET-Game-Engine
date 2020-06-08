using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameEngine
{
    public class FixedTickEngine : GameEngine
    {
        private int ticksPerSecond;
        private long period;

        public FixedTickEngine(int tps)
        {
            this.ticksPerSecond = tps;
            period = (int)TimeSpan.FromSeconds(1).Ticks / ticksPerSecond;
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
                wait = period + overwait;
                while (sw.ElapsedTicks < wait) { };
                overwait = sw.ElapsedTicks - wait;
                sw.Restart();
                Tick();
                Draw();
            }
        }
    }
}
