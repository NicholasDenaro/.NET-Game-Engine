using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaWindowBuilder : IGameWindowBuilder
    {
        private bool ready;

        public (IGameWindow, ISoundPlayer) Run(IGameFrame frame)
        {
            AvaloniaWindow window = null;
            ready = false;

            Task.Run(() =>
            {
                int i = AppBuilder
                    .Configure<Application>()
                    .UsePlatformDetect()
                    .AfterSetup(ab =>
                    {
                        CreateWindow(ref window, frame);
                    })
                    .StartWithClassicDesktopLifetime(new string[] { }, ShutdownMode.OnMainWindowClose);
            });

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
            ////Type t = window.PlatformImpl.GetType();
            ////System.Reflection.FieldInfo fi = t.GetField("_scaling", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ////fi.SetValue(window.PlatformImpl, 1);
            window.Show();
            ready = true;
        }
    }
}
