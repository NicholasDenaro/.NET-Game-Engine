using Avalonia;
using Avalonia.Controls;
using System;
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

        public static void SetWindowRegion(GameFrame frame, double x, double y, double w, double h)
        {
            var win = frame.Window as AvaloniaWindow;
            IntPtr winHandle = win.PlatformImpl.Handle.Handle;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Win32SetWindowRgn(winHandle.ToInt32(), Win32CreateRectRgn((int)(x * frame.ScaleX), (int)(y * frame.ScaleY), (int)((x + w) * frame.ScaleX), (int)((y + h) * frame.ScaleY)), 0);
            }
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
        }

        public static short GetKeyState(int key)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return Win32GetKeyState(key);
            }

            throw new NotImplementedException($"Platform '{Environment.OSVersion.Platform}' not supported.");
        }

        [DllImport("user32.DLL", EntryPoint = "SetWindowRgn")]
        private static extern int Win32SetWindowRgn(int hWnd, int hRgn, int bRedraw);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRectRgn")]
        private static extern int Win32CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("user32.DLL", EntryPoint = "GetWindowLong")]
        private static extern uint Win32GetWindowLong(int hWnd, int index);

        [DllImport("user32.DLL", EntryPoint = "SetWindowLong")]
        private static extern int Win32SetWindowLong(int hWnd, int index, long newLong);

        [DllImport("user32.dll", EntryPoint = "GetKeyState")]
        private static extern short Win32GetKeyState(int key);
    }
}
