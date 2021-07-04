using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
#if Avalonia
using GameEngine.UI.AvaloniaUI;
#endif
#if WinForm
using GameEngine.UI.WinForms;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GridWalkRPG
{
    public class Program
    {
        public static GameEngine.GameEngine Engine { get; private set; }
        public static GameFrame Frame { get; private set; }
        public static Queue<string> states = new Queue<string>();

        public static void Main(string[] args)
        {
            Engine = new FixedTickEngine(144);

#if WinForm
            GameView2D view = new GameView2D(new Drawer2DSystemDrawing() ,240, 160, 4, 4, Color.FromArgb(0, Color.Magenta));
#endif
#if Avalonia
            GameView2D view = new GameView2D(new Drawer2DAvalonia() ,240, 160, 4, 4, Color.FromArgb(0, Color.Magenta));
#endif
            view.ScrollTop = view.Height / 2;
            view.ScrollBottom = view.Height / 2 - 16;
            view.ScrollLeft = view.Width / 2;
            view.ScrollRight = view.Width / 2 - 16;
            Engine.TickEnd += view.Tick;
            Engine.View = view;
            Engine.SetLocation(Location.Load("GridWalkRPG.Maps.map.dat"));
#if WinForm
            Frame = new GameFrame(new WinFormWindowBuilder(), 0, 0, 240, 160, 4, 4);
#endif 
#if Avalonia
            Frame = new GameFrame(new AvaloniaWindowBuilder(), 0, 0, 240, 160, 4, 4);
#endif 
            Engine.DrawEnd += Frame.DrawHandle;
            Frame.Start();

            WindowsKeyController controller = new WindowsKeyController(keymap);
            Engine.AddController(controller);
            
            Frame.Window.Hook(controller);

            DescriptionPlayer dp = new DescriptionPlayer(new Sprite("circle", "Sprites.circle.png", 16, 16), 48, 48);
            Entity player = new Entity(dp);
            Guid playerId = player.Id;
            PlayerActions pActions = new PlayerActions(Engine.GetControllerIndex(controller));
            Engine.TickEnd += (s, e) => Entity.Entities[playerId].TickAction = pActions.TickAction;
            player.TickAction = pActions.TickAction;
            Engine.AddEntity(player);

#if Avalonia
            dp.AddMovementListener(d => AvaloniaWindowBuilder.SetWindowRegion(Frame, d.X - (Engine.View as GameView2D).ViewBounds.X, d.Y - (Engine.View as GameView2D).ViewBounds.Y, d.Width, d.Height));
#endif

            view.Follow(player.Description as Description2D);
            Engine.TickEnd += (s, e) => view.Follow(Entity.Entities[playerId].Description as Description2D);

            MML mml = new MML(new string[] {
                ////// Good // https://www.reddit.com/r/archebards/comments/26rjdt/ocarina_of_time/
                ////"r1l8<faaafaaafaaafaaaegggeggcegggeggcfaaafaaafaaafaaaegggeggcegggeggc1",
                ////"r1l8>fab4fab4fab>ed4c-c<bge2&edege2.fab4fab4fab>ed4c-ce<bg2&gbgde1",
                ////"r1l2<ffffccccffffcccc"
                
                // Very good // https://www.gaiaonline.com/guilds/viewtopic.php?page=1&t=23690909#354075091
                "l16o3f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cre8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcrc8o3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drararrrdrararrrcrbrbrrrcrbrbrrrerarrrarerarrrarerg#rg#rg#rg#rrre&er",
                "l16o5frarb4frarb4frarbr>erd4<b8>cr<brgre2&e8drergre2&e4frarb4frarb4frarbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1r1frgra4br>crd4e8frg2&g4r1r1<f8era8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2r4e&e&er",
                "l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8<bre2&e4drerf4grarb4>c8dre2&e4<drerf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8ar>c8<br>d8crefrde1&e2r4"
            });
            //Frame.PlayTrack(new AvaloniaTrack(mml));

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
            Engine.TickEnd += (s, e) =>
            {
                states.Enqueue(Engine.Serialize());
                if (states.Count > 60)
                {
                    states.Dequeue();
                }
            };

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

        public enum KEYS { UP = 0, DOWN = 2, LEFT = 1, RIGHT = 3, A = 4, B = 5, X = 6, Y = 7, RESET = 8 }

        public static Dictionary<int, int> keymap = new Dictionary<int, int>()
        {
#if WinForm
            { (int)System.Windows.Forms.Keys.Up, (int)KEYS.UP},
            { (int)System.Windows.Forms.Keys.Down, (int)KEYS.DOWN },
            { (int)System.Windows.Forms.Keys.Left, (int)KEYS.LEFT },
            { (int)System.Windows.Forms.Keys.Right, (int)KEYS.RIGHT },
            { (int)System.Windows.Forms.Keys.X, (int)KEYS.A },
            { (int)System.Windows.Forms.Keys.Z, (int)KEYS.B },
            { (int)System.Windows.Forms.Keys.A, (int)KEYS.X },
            { (int)System.Windows.Forms.Keys.S, (int)KEYS.Y },
            { (int)System.Windows.Forms.Keys.OemOpenBrackets, (int)KEYS.RESET }
#endif
#if Avalonia
            { (int)Avalonia.Input.Key.Up, (int)KEYS.UP},
            { (int)Avalonia.Input.Key.Down, (int)KEYS.DOWN },
            { (int)Avalonia.Input.Key.Left, (int)KEYS.LEFT },
            { (int)Avalonia.Input.Key.Right, (int)KEYS.RIGHT },
            { (int)Avalonia.Input.Key.X, (int)KEYS.A },
            { (int)Avalonia.Input.Key.Z, (int)KEYS.B },
            { (int)Avalonia.Input.Key.A, (int)KEYS.X },
            { (int)Avalonia.Input.Key.S, (int)KEYS.Y },
            { (int)Avalonia.Input.Key.OemOpenBrackets, (int)KEYS.RESET },
#endif
        };
    }
}
