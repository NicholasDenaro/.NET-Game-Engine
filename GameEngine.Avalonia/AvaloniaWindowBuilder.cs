using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaWindowBuilder : IGameWindowBuilder
    {
        public IGameWindow Run(IGameUI frame)
        {
            AvaloniaWindow window = null;

            if (Application.Current == null)
            {
                _2D.Bitmap.SetBitmapImpl(new AvaloniaBitmapCreator());
                ClassicDesktopStyleApplicationLifetime lifetime = new ClassicDesktopStyleApplicationLifetime { };
                var builder = AppBuilder.Configure<Application>()
                    .UsePlatformDetect()
                    .With(new AvaloniaNativePlatformOptions
                    {
                        UseGpu = true,
                        UseDeferredRendering = true,
                    });

                Task.Run(() =>
                {
                    builder.Start((app, args) =>
                    {
                        app.ApplicationLifetime = lifetime;
                        window = CreateWindow(frame);
                        window.Show();
                        window.Closed += (o, e) => Environment.Exit(0);

                        app.Run(CancellationToken.None);
                    }, null);
                });

                while (window == null)
                {
                    Thread.Yield();
                }
            }
            else
            {
                AvaloniaWindow win = null;
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    win = CreateWindow(frame);
                    win.Show();
                });

                while (win == null)
                {
                    Thread.Yield();
                }

                return win;
            }

            return window;
        }

        private string title = "Game Window";
        private WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen;
        private WindowTransparencyLevel transparency = WindowTransparencyLevel.None;
        private SystemDecorations decorations = SystemDecorations.Full;
        private bool topMost = false;
        private bool canResize = false;
        private bool showInTaskBar = true;

        public AvaloniaWindowBuilder Title(string title) { this.title = title; return this; }
        public AvaloniaWindowBuilder StartupLocation(WindowStartupLocation location) { this.startupLocation = location; return this; }
        public AvaloniaWindowBuilder Transparency(WindowTransparencyLevel transparency) { this.transparency = transparency; return this; }
        public AvaloniaWindowBuilder Decorations(SystemDecorations decorations) { this.decorations = decorations; return this; }
        public AvaloniaWindowBuilder TopMost(bool topmost) { this.topMost = topmost; return this; }
        public AvaloniaWindowBuilder CanResize(bool canResize) { this.canResize = canResize; return this; }
        public AvaloniaWindowBuilder ShowInTaskBar(bool showInTaskBar) { this.showInTaskBar = showInTaskBar; return this; }

        private AvaloniaWindow CreateWindow(IGameUI frame)
        {
            AvaloniaWindow window = new AvaloniaWindow(frame.Bounds.Width, frame.Bounds.Height);
            
            window.Title = title;
            GamePanel panel = new GamePanel(window, (int)(frame.Bounds.Width / frame.ScaleX), (int)(frame.Bounds.Height / frame.ScaleY), frame.ScaleX, frame.ScaleY);
            window.Add(panel);
            window.WindowStartupLocation = startupLocation;

            window.TransparencyLevelHint = transparency;
            window.SystemDecorations = decorations;
            window.Topmost = topMost;

            window.ShowInTaskbar = showInTaskBar;
            window.CanResize = canResize;

            return window;
        }

        public static void MakeTransparent(GameUI frame, bool transparent)
        {
            var win = frame.Window as AvaloniaWindow;
            IntPtr winHandle = win.PlatformImpl.Handle.Handle;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                uint initialStyle = Win32GetWindowLong(winHandle.ToInt32(), -20);

                if (transparent)
                {
                    Win32SetWindowLong(winHandle.ToInt32(), -20, initialStyle | 0x80000 | 0x20);
                }
                else
                {
                    Win32SetWindowLong(winHandle.ToInt32(), -20, initialStyle & ~0x80000 & ~0x20);
                }
            }
            else
            {
                return;
                //throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
            }
        }

        public static short GetKeyState(int key)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return Win32GetKeyState(key);
            }

            return 0;
            //throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
        }

        public static void SetWindowRegion(GameUI frame, double x, double y, double w, double h)
        {
            var win = frame.Window as AvaloniaWindow;
            IntPtr winHandle = win.PlatformImpl.Handle.Handle;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                uint hRegion = Win32CreateRectRgn(
                        (int)(x * frame.ScaleX),
                        (int)(y * frame.ScaleY),
                        (int)((x + w) * frame.ScaleX),
                        (int)((y + h) * frame.ScaleY));
                Win32SetWindowRgn(winHandle.ToInt32(), hRegion, 0);

                Win32DeleteObject(hRegion);
            }
        }

        public static void SetWindowRegion(GameUI frame, ref Point[] points)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                int size = Marshal.SizeOf(new Point());
                if (hPoints == IntPtr.Zero)
                {
                    hPoints = Marshal.AllocCoTaskMem(points.Length * size);
                }

                for (int i = 0; i < points.Length; i++)
                {
                    Marshal.StructureToPtr(points[i], hPoints + i * size, false);
                }


                long hRegion = Win32CreatePolygonRgn(hPoints.ToInt64(), points.Length, 1);

                var win = frame.Window as AvaloniaWindow;
                IntPtr winHandle = win.PlatformImpl.Handle.Handle;
                Win32SetWindowRgn(winHandle.ToInt32(),
                    hRegion,
                    0);

                Win32DeleteObject(hRegion);
            }
            else
            {
                return;
                //throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
            }
        }

        static IntPtr hPoints;
        static IntPtr hCount;

        public static void SetWindowRegion(GameUI frame, ref Point[][] points)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                int size = Marshal.SizeOf(new Point());

                int totalPoints = points.Aggregate(0, (v, p) => v + p.Length);

                if (hPoints == IntPtr.Zero)
                {
                    hPoints = Marshal.AllocCoTaskMem(totalPoints * size);
                }

                if (hCount == IntPtr.Zero)
                {
                    hCount = Marshal.AllocCoTaskMem(points.Length * sizeof(int));
                }

                int c = 0;
                for (int p = 0; p < points.Length; p++)
                {
                    Marshal.WriteInt32(hCount + p * sizeof(int), points[p].Length);

                    for (int i = 0; i < points[p].Length; i++)
                    {
                        Marshal.StructureToPtr(points[p][i], hPoints + c++ * size, false);
                    }
                }

                long hRegion = Win32CreatePolyPolygonRgn(hPoints.ToInt64(), hCount.ToInt64(), points.Length, 1);

                var win = frame.Window as AvaloniaWindow;
                IntPtr winHandle = win.PlatformImpl.Handle.Handle;
                Win32SetWindowRgn(winHandle.ToInt32(),
                    hRegion,
                    0);

                Win32DeleteObject(hRegion);
            }
            else
            {
                return;
                //throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
            }
        }

        [DllImport("user32.DLL", EntryPoint = "SetWindowRgn")]
        private static extern int Win32SetWindowRgn(int hWnd, long hRgn, int bRedraw);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRectRgn")]
        private static extern uint Win32CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("Gdi32.dll", EntryPoint = "CreatePolygonRgn")]
        private static extern long Win32CreatePolygonRgn(long hPtr, int count, int fillMode);

        [DllImport("Gdi32.dll", EntryPoint = "CreatePolyPolygonRgn")]
        private static extern long Win32CreatePolyPolygonRgn(long hPtr, long hCount, int total, int fillMode);

        [DllImport("Gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern int Win32DeleteObject(long hPtr);

        [DllImport("user32.DLL", EntryPoint = "GetWindowLong")]
        private static extern uint Win32GetWindowLong(int hWnd, int index);

        [DllImport("user32.DLL", EntryPoint = "SetWindowLong")]
        private static extern int Win32SetWindowLong(int hWnd, int index, long newLong);

        [DllImport("user32.dll", EntryPoint = "GetKeyState")]
        private static extern short Win32GetKeyState(int key);

        public struct Point
        {
            public Int32 x;
            public Int32 y;
        }
    }
}
