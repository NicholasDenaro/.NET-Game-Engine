using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    public class FixedTickEngine : GameEngine
    {
        private int ticksPerSecond;
        private long period;

        private long tickTime = 0;
        private long drawTime = 0;
        private long frameTime = 0;
        private bool gcCollect = false;
        private bool longwait = false;
        private int gcCollectTicks = 0;

        private long stopwatchToThreadRatio;

        public FixedTickEngine(int tps)
        {
            this.ticksPerSecond = tps;
            this.period = Stopwatch.Frequency / ticksPerSecond;
            this.gcCollectTicks = ticksPerSecond * 10;
            Console.WriteLine("TPS: {0}", tps);
            Console.WriteLine("period: {0}", this.period);
            Console.WriteLine($"{TimeSpan.FromSeconds(1).Ticks}");
            Console.WriteLine($"{Stopwatch.Frequency}");

            stopwatchToThreadRatio = Stopwatch.Frequency / TimeSpan.FromSeconds(1).Ticks;
        }

        public override void Control()
        {
            new Thread(() => Run()).Start();
        }

        private void Run()
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                long diff;
                while (true)
                {
                    sw.Restart();
                    Tick();
                    this.tickTime = sw.ElapsedTicks;
                    Draw();
                    this.drawTime = sw.ElapsedTicks - tickTime;

                    diff = this.period - sw.ElapsedTicks;
                    this.gcCollect = false;
                    this.longwait = false;
                    if (--this.gcCollectTicks <= 0 && diff > 100000 * stopwatchToThreadRatio)
                    {
                        this.gcCollect = true;
                        this.gcCollectTicks = ticksPerSecond * 10;
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                    }

                    diff = this.period - sw.ElapsedTicks;
                    if (diff > 10000)
                    {
                        this.longwait = true;
                        Thread.Sleep(TimeSpan.FromTicks((int)(diff / stopwatchToThreadRatio * 0.95)));
                    }

                    while (this.period - sw.ElapsedTicks > 200 * stopwatchToThreadRatio)
                    {
                        Thread.Sleep(TimeSpan.FromTicks(100));
                    }

                    while (this.period - sw.ElapsedTicks > 15 * stopwatchToThreadRatio)
                    {
                        Thread.Sleep(TimeSpan.FromTicks(10));
                    }
                    this.frameTime = sw.ElapsedTicks;

                    //Task.Run(Log);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void LogLeft(long ticks)
        {
            Console.WriteLine(ticks);
        }

        private void Log()
        {
            var more = $"({tickTime:d6}+{drawTime:d6})/{this.period:d6}={(tickTime + drawTime) * 100.0 / (this.period):f2}% GC:{this.gcCollect} longwait:{this.longwait}";
            Console.WriteLine($"({this.frameTime:d6})/{this.period:d6}={(this.frameTime) * 100.0 / (this.period):f2}% {(this.frameTime > this.period ? $"\n  {more}" : "")}");
        }
    }
}
