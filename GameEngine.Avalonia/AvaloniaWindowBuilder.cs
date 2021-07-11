using Avalonia;
using Avalonia.Controls;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaWindowBuilder : IGameWindowBuilder
    {
        private bool ready;
        Action<IGameFrame> f;

        public (IGameWindow, ISoundPlayer) Run(IGameFrame frame)
        {
            ready = false;
            AvaloniaWindow window = null;

            if (f == null)
            {
                Task.Run(() =>
                  {
                      int i = AppBuilder
                          .Configure<Application>()
                          .UsePlatformDetect()
                          .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                          .AfterSetup(ab =>
                          {
                              CreateWindow(ref window, frame);
                              f = (frame) => CreateWindow(ref window, frame);
                          })
                          .StartWithClassicDesktopLifetime(new string[] { }, ShutdownMode.OnMainWindowClose);
                  });
            }
            else
            {
                f(frame);
            }


            while (!ready) { }

            return (window, new AvaloniaSoundPlayer());
        }

        private void CreateWindow(ref AvaloniaWindow window, IGameFrame frame)
        {
            window = new AvaloniaWindow(frame.Bounds.Width, frame.Bounds.Height);
            
            window.Title = "GameWindow";
            GamePanel panel = new GamePanel(window, (int)(frame.Bounds.Width / frame.ScaleX), (int)(frame.Bounds.Height / frame.ScaleY), frame.ScaleX, frame.ScaleY);
            window.Add(panel);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.TransparencyLevelHint = WindowTransparencyLevel.Transparent;
            window.SystemDecorations = SystemDecorations.None;
            window.Topmost = true;

            window.Show();
            ready = true;
        }

        public static void MakeTransparent(GameFrame frame, bool transparent)
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
                throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
            }
        }

        public static short GetKeyState(int key)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return Win32GetKeyState(key);
            }

            throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
        }

        public static void SetWindowRegion(GameFrame frame, double x, double y, double w, double h)
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

        public static void SetWindowRegion(GameFrame frame, Point[] points, int xScale = 1, int yScale = 1)
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
                    points[i].x *= xScale;
                    points[i].y *= yScale;
                    Marshal.StructureToPtr(points[i], hPoints + i * size, true);
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
                throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
            }
        }

        static IntPtr hPoints;
        static IntPtr hCount;

        public static void SetWindowRegion(GameFrame frame, Point[][] points, int xScale = 1, int yScale = 1)
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
                        points[p][i].x *= xScale;
                        points[p][i].y *= yScale;
                        Marshal.StructureToPtr(points[p][i], hPoints + c++ * size, true);
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
                throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
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
