using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngine
{
    public class Program
    {
        public static GameEngine Engine { get; private set; }

        static void Main(string[] args)
        {
            int width = 240;
            int height = 160;
            Engine = new FixedTickEngine(60);
            int ticks = 0;
            Engine.Ticker += (e, o) => ticks++;
            GameView2D view = new GameView2D(width, height, 4, 4);
            Engine.View = view;
            KeyController controller = new KeyController(CreateKeyMap());
            controller.Hook(view.Pane);
            Engine.Location = Location.Load(@"Maps\map.dat");
            Sprite sprite = new Sprite("ent1", @"Sprites\circle.png", 0, 0, 16, 16);
            Description2D playerDescription = new Description2D(sprite, 0, 0);
            Entity player = new Entity(playerDescription);
            view.Follow(playerDescription);
            player.TickAction = (currentLocation, descr) =>
            {
                Description2D description = descr as Description2D;
                if(controller[(int)KEYS.UP] == KeyState.HOLD)
                {
                    description.ChangeCoordsDelta(0, -1);
                }

                if (controller[(int)KEYS.DOWN] == KeyState.HOLD)
                {
                    description.ChangeCoordsDelta(0, 1);
                }

                if (controller[(int)KEYS.LEFT] == KeyState.HOLD)
                {
                    description.ChangeCoordsDelta(-1, 0);
                }

                if (controller[(int)KEYS.RIGHT] == KeyState.HOLD)
                {
                    description.ChangeCoordsDelta(1, 0);
                }
            };
            Engine.Focus = player;
            Engine.Location.AddEntity(player);
            Engine.AddController(controller);
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

        public enum KEYS { UP = 0, DOWN = 2, LEFT = 1, RIGHT = 3, A = 4, B = 5 }

        private static Dictionary<int, ControllerAction> CreateKeyMap()
        {
            Dictionary<int, ControllerAction> dict = new Dictionary<int, ControllerAction>();

            dict[(int)KEYS.UP] = new KeyAction(Keys.Up);
            dict[(int)KEYS.DOWN] = new KeyAction(Keys.Down);
            dict[(int)KEYS.LEFT] = new KeyAction(Keys.Left);
            dict[(int)KEYS.RIGHT] = new KeyAction(Keys.Right);
            dict[(int)KEYS.A] = new KeyAction(Keys.X);
            dict[(int)KEYS.B] = new KeyAction(Keys.Z);

            return dict;
        }
    }
}
