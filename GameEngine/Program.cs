using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            GameView2D view = new GameView2D(width, height, 3, 3);
            Engine.View = view;
            KeyController controller = new KeyController(CreateKeyMap());
            controller.Hook(view.Pane);
            Engine.Location = new Location(300, 200);
            Sprite sprite = new Sprite("ent1", @"Sprites\untitled.png", 0, 0, 16, 24);
            Entity player = new Entity(0, 0, sprite);
            view.Follow(player);
            player.TickAction = () =>
            {
                if(controller[(int)KEYS.UP] == KeyState.HOLD)
                {
                    player.ChangeCoordsDelta(0, -1);
                }

                if (controller[(int)KEYS.DOWN] == KeyState.HOLD)
                {
                    player.ChangeCoordsDelta(0, 1);
                }

                if (controller[(int)KEYS.LEFT] == KeyState.HOLD)
                {
                    player.ChangeCoordsDelta(-1, 0);
                }

                if (controller[(int)KEYS.RIGHT] == KeyState.HOLD)
                {
                    player.ChangeCoordsDelta(1, 0);
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
