using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using GameEngine.UI.Audio;
using GameEngine.UI.Controllers;
using GameEngine.UI.NAudio;
#if Avalonia
using GameEngine.UI.AvaloniaUI;
#endif
#if Blazor
using BlazorUI;
using BlazorUI.Client;
#endif
#if WinForm
using GameEngine.UI.WinForms;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static GameEngine.UI.NAudio.SinWaveSound;
using System.Reflection;

namespace GridWalkRPG
{
    public class Program
    {
        public static GameEngine.GameEngine Engine { get; private set; }

        public static int TPS = 60;
        public static bool playmusic = true;
        public static GameUI UI { get; private set; }
        public static Queue<string> states = new Queue<string>();

        private static Bitmap hudBitmap;

        private const int MAIN_CONTROLLER = 0; //0 is keyboard, 1 is xbox controlelr

        public static async Task Main(string[] args)
        {
            (Engine, UI) = new GameBuilder()
                .GameEngine(new FixedTickEngine(TPS))
#if Avalonia
                .GameView(new GameView2D(new Drawer2DAvalonia(), 240, 160, 4, 4, Color.FromArgb(0, Color.Transparent)))
                .GameFrame(new GameUI(
                    new AvaloniaWindowBuilder()
                        .Transparency(Avalonia.Controls.WindowTransparencyLevel.Transparent)
                        .StartupLocation(Avalonia.Controls.WindowStartupLocation.CenterScreen)
                        .CanResize(false)
                        .ShowInTaskBar(true)
                    , 0, 0, 240, 160, 4, 4))
                .SoundPlayer(new NAudioSoundPlayer())
#endif
#if Blazor
                .GameView(new GameView2D(new Drawer2DBlazor(), 240, 160, 4, 4, Color.FromArgb(0, Color.Transparent)))
                .GameFrame(new GameUI(
                    new BlazorUI.Client.BlazorWindowBuilder()
                    , 0, 0, 240, 160, 4, 4))
                .SoundPlayer(new NAudioClipPlayer())
#endif
                .Controller(new WindowsKeyController(keymap))
#if Avalonia
                .Controller(new XBoxController(xboxkeymap))
#endif
                .Build();
#if WinForm
            GameView2D view = new GameView2D(new Drawer2DSystemDrawing(), 240, 160, 4, 4, Color.FromArgb(0, Color.Magenta), 2);
#endif

            GameView2D view = Engine.Views(0) as GameView2D;
            view.ScrollTop = view.Height / 2;
            view.ScrollBottom = view.Height / 2 - 16;
            view.ScrollLeft = view.Width / 2;
            view.ScrollRight = view.Width / 2 - 16;
#if WinForm
            Frame = new GameFrame(new WinFormWindowBuilder(), 0, 0, 240, 160, 4, 4);
#endif

#if Avalonia
            if (false)
            {
                GameUI.OpenNewWindow(
                    Engine,
                    new AvaloniaWindowBuilder()
                        .TopMost(true)
                        .StartupLocation(Avalonia.Controls.WindowStartupLocation.CenterScreen),
                    0, 0, 240, 160, 4, 4,
                    new []{ Engine.Controllers(0)[MAIN_CONTROLLER] });
            }
#endif


            await UI.WaitInitialized();

            Assembly assembly = Assembly.GetAssembly(typeof(Program));
            Sprite.loadingAssembly = assembly;
            Engine.SetLocation(0, await Location.Load(Assembly.GetAssembly(typeof(Program)), "GridWalkRPG.Maps.map.dat"));


            DescriptionPlayer dp = new DescriptionPlayer(await Sprite.Create("circle", "Sprites.circle.png", 16, 16), 48, 48);
            Entity player = new Entity(dp);
            Guid playerId = player.Id;
            PlayerActions pActions = new PlayerActions(Engine.GetControllerIndex(0, Engine.Controllers(0)[MAIN_CONTROLLER]));
            Engine.TickEnd(0) += (s, e) => Entity.Entities[playerId].TickAction = pActions.TickAction;
            player.TickAction = pActions.TickAction;
            Engine.AddEntity(0, player);

            Bitmap circleb = await Bitmap.CreateAsync("circleb", 16, 16);
            hudBitmap = await Bitmap.CreateAsync("hudb", UI.Bounds.Width, UI.Bounds.Height);

            await circleb.GetGraphics().DrawEllipseAsync(new Color(255, 0, 0), 0, 0, 16, 8);
            await hudBitmap.GetGraphics().FillRectangleAsync(new Color(0, 255, 255), 0, 0, 16, 16);

            Sprite circles = await Sprite.Create("circles", circleb, 0, 0);
            Description2D circled = new Description2D(circles, 64, 64, 16, 16);
            Entity circlee = new Entity(circled);
            Engine.AddEntity(0, circlee);

            Sprite huds = await Sprite.Create("hud", hudBitmap, 0, 0);
            Description2D hudd = new HudDescription(huds, 0, 0);
            Entity hude = new Entity(hudd);
            hude.TickAction += (state, entity) =>
            {
                hudBitmap.GetGraphics().Clear(Color.Transparent);
                string info = $"TPS: {hudTicks}" +
                $"\n Timings:" +
                $"\n\tmin: {hudMinTime}" +
                $"\n\tavg: {hudTotalTime / hudTicks}" +
                $"\n\tmax: {hudMaxTime}" +
                $"\nUP: {state.Controllers[MAIN_CONTROLLER][KEYS.UP].IsDown()}" +
                $"\nDOWN: {state.Controllers[MAIN_CONTROLLER][KEYS.DOWN].IsDown()}" +
                $"\nLEFT: {state.Controllers[MAIN_CONTROLLER][KEYS.LEFT].IsDown()}" +
                $"\nRIGHT: {state.Controllers[MAIN_CONTROLLER][KEYS.RIGHT].IsDown()}";
                hudBitmap.GetGraphics().DrawText(info, 0, 0, Color.Black, 20);
            };
            Engine.AddEntity(0, hude);


#if Avalonia
            //AvaloniaWindowBuilder.MakeTransparent(Frame, true);

            //dp.AddMovementListener(d => AvaloniaWindowBuilder.SetWindowRegion(Frame, d.X - (Engine.View as GameView2D).ViewBounds.X, d.Y - (Engine.View as GameView2D).ViewBounds.Y, d.Width, d.Height));

            var points = new AvaloniaWindowBuilder.Point[] {
                new AvaloniaWindowBuilder.Point{},
                new AvaloniaWindowBuilder.Point{},
                new AvaloniaWindowBuilder.Point{},
                new AvaloniaWindowBuilder.Point{},
            };

            //dp.AddMovementListener(d => {
            //    points[0].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X;
            //    points[0].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y;

            //    points[1].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X;
            //    points[1].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y - 10;

            //    points[2].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 10;
            //    points[2].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y - 10;

            //    points[3].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 20;
            //    points[3].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 20;

            //    for (int i = 0; i < points.Length; i++)
            //    {
            //        points[i].x *= 4;
            //        points[i].y *= 4;
            //    }

            //    AvaloniaWindowBuilder.SetWindowRegion(Frame, ref points);
            //});

            var pointarray = new AvaloniaWindowBuilder.Point[][] {
                new AvaloniaWindowBuilder.Point[] {
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                },
                new AvaloniaWindowBuilder.Point[] {
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                    new AvaloniaWindowBuilder.Point(),
                },
            };

            //dp.AddMovementListener(d =>
            //{
            //    pointarray[0][0].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X;
            //    pointarray[0][0].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y;
            //    pointarray[0][1].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X;
            //    pointarray[0][1].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y - 10;
            //    pointarray[0][2].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 10;
            //    pointarray[0][2].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y - 10;
            //    pointarray[0][3].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 20;
            //    pointarray[0][3].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 20;

            //    pointarray[1][0].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 20;
            //    pointarray[1][0].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 20;
            //    pointarray[1][1].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 30;
            //    pointarray[1][1].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 40;
            //    pointarray[1][2].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 30;
            //    pointarray[1][2].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 30;
            //    pointarray[1][3].x = (int)d.X - (Engine.View as GameView2D).ViewBounds.X + 20;
            //    pointarray[1][3].y = (int)d.Y - (Engine.View as GameView2D).ViewBounds.Y + 30;

            //    for (int p = 0; p < pointarray.Length; p++)
            //    {
            //        for (int i = 0; i < pointarray[p].Length; i++)
            //        {
            //            pointarray[p][i].x *= 4;
            //            pointarray[p][i].y *= 4;
            //        }
            //    }

            //    AvaloniaWindowBuilder.SetWindowRegion(Frame, ref pointarray);
            //});

            short prevState = AvaloniaWindowBuilder.GetKeyState(0xA1);
            Engine.TickEnd(0) += (object s, GameState e) =>
            {
                if (e.Controllers[MAIN_CONTROLLER][KEYS.EXIT].IsDown())
                {
                    Environment.Exit(0);
                }

                short state = AvaloniaWindowBuilder.GetKeyState(0xA1);
                if (prevState != state)
                {
                    Console.WriteLine(state);
                    if (state != 0 && state != 1)
                    {
                        AvaloniaWindowBuilder.MakeTransparent(UI, false);
                    }
                    else
                    {
                        AvaloniaWindowBuilder.MakeTransparent(UI, true);
                    }
                }

                prevState = state;
            };
#endif

            view.Follow(player.Description as Description2D);
            Engine.TickEnd(0) += (s, e) => view.Follow(Entity.Entities[playerId].Description as Description2D);


            //MML mml = new MML(new string[] {
            //    ////// Good // https://www.reddit.com/r/archebards/comments/26rjdt/ocarina_of_time/
            //    ////"r1l8<faaafaaafaaafaaaegggeggcegggeggcfaaafaaafaaafaaaegggeggcegggeggc1",
            //    ////"r1l8>fab4fab4fab>ed4c-c<bge2&edege2.fab4fab4fab>ed4c-ce<bg2&gbgde1",
            //    ////"r1l2<ffffccccffffcccc"
                
            //    // Very good // https://www.gaiaonline.com/guilds/viewtopic.php?page=1&t=23690909#354075091
            //    "t140l16o3f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cre8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcrc8o3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drararrrdrararrrcrbrbrrrcrbrbrrrerarrrarerarrrarerg#rg#rg#rg#rrre&er",
            //    "t140l16o5frarb4frarb4frarbr>erd4<b8>cr<brgre2&e8drergre2&e4frarb4frarb4frarbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1r1frgra4br>crd4e8frg2&g4r1r1<f8era8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2r4e&e&er",
            //    "t140l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8<bre2&e4drerf4grarb4>c8dre2&e4<drerf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8ar>c8<br>d8crefrde1&e2r4"
            //});
            MML mml1 = new MML(new string[] {
                "t140l16o3f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cre8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcro3c8f8o4crcrcro3f8o4crcrcro3f8o4crcrcro3f8o4crcro3cro3e8o4crcrcro3e8o4crcrcro3e8o4crcrcro3e8o4crcrc8o3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drardraro2gro3gro2gro3grcro4cro3cro4cro2aro3aro2aro3aro3drararrrdrararrrcrbrbrrrcrbrbrrrerarrrarerarrrarerg#rg#rg#rg#rrre&er",
            });
            MML mml2 = new MML(new string[] {
                "t140l16o5frarb4frarb4frarbr>erd4<b8>cr<brgre2&e8drergre2&e4frarb4frarb4frarbr>erd4<b8>crer<brg2&g8brgrdre2&e4r1r1frgra4br>crd4e8frg2&g4r1r1<f8era8grb8ar>c8<br>d8cre8drf8er<b>cr<ab1&b2r4e&e&er",
            });
            MML mml3 = new MML(new string[] {
                "t140l16r1r1r1r1r1r1r1r1o4drerf4grarb4>c8<bre2&e4drerf4grarb4>c8dre2&e4<drerf4grarb4>c8<bre2&e4d8crf8erg8fra8grb8ar>c8<br>d8crefrde1&e2r4",
            });

            if (playmusic)
            {
                await UI.CacheAduio(new NAudioMMLTrack("mml1", Waves.TRIANGLE, mml1));
                await UI.CacheAduio(new NAudioMMLTrack("mml2", Waves.PIANO, mml2));
                await UI.CacheAduio(new NAudioMMLTrack("mml3", Waves.PLUCKY, mml3));
                bool firstTick = true;
                Engine.TickEnd(0) += (s, e) =>
                {
                    if (firstTick)
                    {
                        UI.PlayTrack(NAudioMMLTrack.Tracks["mml1"]);
                        UI.PlayTrack(NAudioMMLTrack.Tracks["mml2"]);
                        UI.PlayTrack(NAudioMMLTrack.Tracks["mml3"]);
                        firstTick = false;
                    }
                };
            }

            TileMap map = Engine.Location(0).Description as TileMap;

            if (map != null)
            {
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
                                Engine.Location(0).AddEntity(new Entity(new WallDescription(x, y, 16, 16)));
                                break;
                        }
                    }
                }
            }

            watchSecond = new Stopwatch();
            watchSecond.Start();
            watchTickTime = new Stopwatch();

            Engine.TickEnd(0) += TickInfo;
            Engine.TickStart(0) += TickTimer;
            Engine.TickEnd(0) += TickTimer;
            Engine.TickEnd(0) += (s, e) =>
            {
                string state = Engine.Serialize();
                states.Enqueue(state);
                if (states.Count > 60)
                {
                    states.Dequeue();
                }
            };

            try
            {
                await Engine.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static Stopwatch watchSecond;
        private static int ticks;
        private static int hudTicks = 1;
        private static long hudMinTime;
        private static long hudMaxTime;
        private static long hudTotalTime;
        public static void TickInfo(object sender, GameState state)
        {
            ticks++;
            if (watchSecond.ElapsedMilliseconds >= 1000)
            {
                //Console.WriteLine($"TPS: {ticks} | Timings: min: {minTime} avg: {totalTime / ticks} max: {maxTime} avg remaining: {((int)TimeSpan.FromSeconds(1).Ticks - TPS) -  (totalTime / ticks)}");

                hudTicks = ticks;
                hudMinTime = minTime;
                hudMaxTime = maxTime;
                hudTotalTime = totalTime;

                ticks = 0;
                minTime = long.MaxValue;
                maxTime = long.MinValue;
                totalTime = 0;
                watchSecond.Restart();
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

        public enum KEYS { UP = 0, DOWN = 2, LEFT = 1, RIGHT = 3, A = 4, B = 5, X = 6, Y = 7, RESET = 8, EXIT=9 }

        public static Dictionary<int, object> keymap = new Dictionary<int, object>()
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
            { (int)Avalonia.Input.Key.Up, KEYS.UP},
            { (int)Avalonia.Input.Key.Down, KEYS.DOWN },
            { (int)Avalonia.Input.Key.Left, KEYS.LEFT },
            { (int)Avalonia.Input.Key.Right, KEYS.RIGHT },
            { (int)Avalonia.Input.Key.X, KEYS.A },
            { (int)Avalonia.Input.Key.Z, KEYS.B },
            { (int)Avalonia.Input.Key.A, KEYS.X },
            { (int)Avalonia.Input.Key.S, KEYS.Y },
            { (int)Avalonia.Input.Key.OemOpenBrackets, KEYS.RESET },
            { (int)Avalonia.Input.Key.OemCloseBrackets, KEYS.EXIT },
#endif
#if Blazor
            { BlazorWindow.KeyCodes.ArrowUp, KEYS.UP},
            { BlazorWindow.KeyCodes.ArrowDown, KEYS.DOWN },
            { BlazorWindow.KeyCodes.ArrowLeft, KEYS.LEFT },
            { BlazorWindow.KeyCodes.ArrowRight, KEYS.RIGHT },
            { BlazorWindow.KeyCodes.KeyX, KEYS.A },
            { BlazorWindow.KeyCodes.KeyZ, KEYS.B },
            { BlazorWindow.KeyCodes.KeyA, KEYS.X },
            { BlazorWindow.KeyCodes.KeyS, KEYS.Y },
            { BlazorWindow.KeyCodes.OEMBracketOpen, KEYS.RESET },
            { BlazorWindow.KeyCodes.OEMBracketClose, KEYS.EXIT },
#endif
        };
        public static Dictionary<XBoxController.Key, object> xboxkeymap = new Dictionary<XBoxController.Key, object>()
        {
#if Avalonia
            { XBoxController.Key.DPAD_UP, KEYS.UP},
            { XBoxController.Key.DPAD_DOWN, KEYS.DOWN },
            { XBoxController.Key.DPAD_LEFT, KEYS.LEFT },
            { XBoxController.Key.DPAD_RIGHT, KEYS.RIGHT },
            { XBoxController.Key.A, KEYS.A },
            { XBoxController.Key.B, KEYS.B },
            { XBoxController.Key.X, KEYS.X },
            { XBoxController.Key.Y, KEYS.Y },
            { XBoxController.Key.SELECT, KEYS.RESET },
            { XBoxController.Key.START, KEYS.EXIT },
#endif
        };
    }
}
