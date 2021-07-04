using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
//using Avalonia.ReactiveUI;
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

        // Import the SetWindowRgn function from the user32.DLL
        // From the Unmanaged Code
        [DllImport("user32.DLL", EntryPoint = "SetWindowRgn")]
        public static extern int SetWindowRgn(int hWnd, int hRgn, int bRedraw);

        // Import the SetWindowRgn function from the user32.DLL
        // From the Unmanaged Code
        [DllImport("Gdi32.dll", EntryPoint = "CreateRectRgn")]
        public static extern int CreateRectRgn(int x1, int y1, int x2, int y2);

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
            window = new AvaloniaWindow(frame.Bounds.Width - (int)frame.ScaleX, frame.Bounds.Height - (int)frame.ScaleY);
            window.Title = "GameWindow";
            GamePanel panel = new GamePanel(window, (int)(frame.Bounds.Width / frame.ScaleX), (int)(frame.Bounds.Height / frame.ScaleY), frame.ScaleX, frame.ScaleY);
            window.Add(panel);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.TransparencyLevelHint = WindowTransparencyLevel.Transparent;
            window.SystemDecorations = SystemDecorations.None;


            window.Show();
            ready = true;
        }

        public static void SetWindowRegion(GameFrame frame, double x, double y, double w, double h)
        {
            var win = frame.Window as AvaloniaWindow;
            IntPtr winHandle = win.PlatformImpl.Handle.Handle;
            SetWindowRgn(winHandle.ToInt32(), CreateRectRgn((int)(x * frame.ScaleX), (int)(y * frame.ScaleY), (int)((x + w) * frame.ScaleX), (int)((y + h) * frame.ScaleY)), 1);
        }
    }
}
