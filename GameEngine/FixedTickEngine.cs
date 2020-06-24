using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameEngine
{
    public class FixedTickEngine : GameEngine
    {
        private int ticksPerSecond;
        private long period;

        private long tickTime = 0;
        private long drawTime = 0;

        public FixedTickEngine(int tps)
        {
            this.ticksPerSecond = tps;
            this.period = (int)TimeSpan.FromSeconds(1).Ticks / ticksPerSecond;
            Console.WriteLine("period: {0}", this.period);
        }

        public override void Control()
        {
            Task.Run(Run);
        }

        private void Run()
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                while (sw.ElapsedTicks < this.period) { };
                sw.Restart();
                Tick();
                this.tickTime = sw.ElapsedTicks;
                Draw();
                this.drawTime = sw.ElapsedTicks - tickTime;
                //Task.Run(Log);
            }
        }

        private void Log()
        {
            Console.WriteLine($"({tickTime:d6}+{drawTime:d6})/{this.period:d6}={(tickTime + drawTime) * 100.0 / (this.period):f2}%");
        }
    }
}
