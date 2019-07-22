using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridWalkRPG
{
    public class Program
    {
        public static GameEngine.GameEngine Engine { get; private set; }

        public static void Main(string[] args)
        {
            int ticks = 0;
            Engine = new FixedTickEngine(60);
            Engine.Ticker = (o, e) => ticks++;
            Engine.View = new GameView2D(240, 160, 4, 4);
            Engine.Location = Location.Load("Maps/map.dat");
            Engine.Start();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                if (sw.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine("Ticks per Second: {0}", ticks);
                    ticks = 0;
                    sw.Restart();
                }
            }
        }
    }
}
