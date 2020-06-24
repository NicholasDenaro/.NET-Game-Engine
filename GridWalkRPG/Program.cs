using GameEngine;
using GameEngine._2D;
using GameEngine.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GridWalkRPG
{
    public class Program
    {
        public static GameEngine.GameEngine Engine { get; private set; }

        public static void Main(string[] args)
        {
            Engine = new FixedTickEngine(60);

            GameView2D view = new GameView2D(240, 160);
            view.ScrollTop = view.Height / 2;
            view.ScrollBottom = view.Height / 2 - 16;
            view.ScrollLeft = view.Width / 2;
            view.ScrollRight = view.Width / 2 - 16;
            Engine.TickEnd += view.Tick;
            Engine.View = view;
            Engine.SetLocation(Location.Load("GridWalkRPG.Maps.map.dat"));
            GameFrame frame = new GameFrame(0, 0, 240, 160);
            Engine.DrawEnd += frame.Pane.DrawHandle;
            frame.Start();

            WindowsKeyController controller = new WindowsKeyController(keymap);
            Engine.AddController(controller);
            controller.Hook(frame);

            Entity player = new Entity(new Description2D(new Sprite("circle", "Sprites.circle.png", 16, 16), 48, 48));
            PlayerActions pActions = new PlayerActions(controller);
            player.TickAction = pActions.TickAction;
            Engine.AddEntity(player);
            view.Follow(player.Description as Description2D);

            TileMap map = Engine.Location.Description as TileMap;

            // This is a hack to make the walls spawn where tree tiles are.
            for (int x = 0; x < map.Width; x += 16)
            {
                for (int y = 0; y < map.Height; y += 16)
                {
                    switch (map[x / map.Sprite.Width, y / map.Sprite.Height])
                    {
                        case 3:
                        case 4:
                        case 19:
                        case 20:
                            Engine.Location.AddEntity(new Entity(new WallDescription(x, y, 16, 16)));
                            break;
                    }
                }
            }

            watchSecond = new Stopwatch();
            watchSecond.Start();
            watchTickTime = new Stopwatch();

            Engine.TickEnd += TickInfo;
            Engine.TickStart += TickTimer;
            Engine.TickEnd += TickTimer;

            Engine.Start();

            while (true)
            {
            }
        }

        private static Stopwatch watchSecond;
        private static int ticks;
        public static void TickInfo(object sender, GameState state)
        {
            ticks++;
            if (watchSecond.ElapsedMilliseconds >= 1000)
            {
                Console.WriteLine($"TPS: {ticks} | Timings: min: {minTime} avg: {totalTime / ticks} max: {maxTime}");
                ticks = 0;
                watchSecond.Restart();
                minTime = long.MaxValue;
                maxTime = long.MinValue;
                totalTime = 0;
            }
        }

        private static Stopwatch watchTickTime;
        private static long minTime;
        private static long maxTime;
        private static long totalTime;
        public static void TickTimer(object sender, GameState state)
        {
            if (watchTickTime.IsRunning)
            {
                long time = watchTickTime.ElapsedTicks;
                totalTime += time;
                if (time < minTime)
                {
                    minTime = time;
                }
                if (time > maxTime)
                {
                    maxTime = time;
                }
                watchTickTime.Stop();
            }
            else
            {
                watchTickTime.Restart();
            }
        }

        public enum KEYS { UP = 0, DOWN = 2, LEFT = 1, RIGHT = 3, A = 4, B = 5 }

        public static Dictionary<int, int> keymap = new Dictionary<int, int>()
        {
            { (int)System.Windows.Forms.Keys.Up, (int)KEYS.UP},
            { (int)System.Windows.Forms.Keys.Down, (int)KEYS.DOWN },
            { (int)System.Windows.Forms.Keys.Left, (int)KEYS.LEFT },
            { (int)System.Windows.Forms.Keys.Right, (int)KEYS.RIGHT },
            { (int)System.Windows.Forms.Keys.X, (int)KEYS.A },
            { (int)System.Windows.Forms.Keys.Z, (int)KEYS.B },
        };
    }
}
