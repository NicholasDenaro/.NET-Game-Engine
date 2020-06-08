using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using GameEngine.Windows;
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

            GameView2D view = new GameView2D(240, 160);
            Engine.View = view;
            Engine.Location = Location.Load("Maps/map.dat");
            Engine.Start();
            GameFrame frame = new GameFrame(Engine, 0, 0, 240, 160);
            Engine.Drawer += frame.Pane.DrawHandle;
            frame.Start();

            WindowsKeyController controller = new WindowsKeyController(keymap);
            Engine.AddController(controller);
            controller.Hook(frame);

            Entity player = new Entity(new Description2D(new Sprite("circle", "Sprites/circle.png", 16, 16), 48, 48));
            PlayerActions pActions = new PlayerActions(controller);
            player.TickAction = pActions.TickAction;
            Engine.Location.AddEntity(player);
            view.Follow(player.Description as Description2D);

            TileMap map = Engine.Location.Description as TileMap;
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

        public enum KEYS { UP = 0, DOWN = 2, LEFT = 1, RIGHT = 3, A = 4, B = 5 }

        public static Dictionary<int, ControllerAction> keymap = new Dictionary<int, ControllerAction>()
        {
            { (int)KEYS.UP, new KeyAction((int)System.Windows.Forms.Keys.Up) },
            { (int)KEYS.DOWN, new KeyAction((int)System.Windows.Forms.Keys.Down) },
            { (int)KEYS.LEFT, new KeyAction((int)System.Windows.Forms.Keys.Left) },
            { (int)KEYS.RIGHT, new KeyAction((int)System.Windows.Forms.Keys.Right) },
            { (int)KEYS.A, new KeyAction((int)System.Windows.Forms.Keys.X) },
            { (int)KEYS.B, new KeyAction((int)System.Windows.Forms.Keys.Z) },
        };
    }
}
