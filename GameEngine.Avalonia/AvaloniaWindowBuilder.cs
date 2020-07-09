using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaWindowBuilder : IGameWindowBuilder
    {
        public (IGameWindow, ISoundPlayer) Run(IGameFrame frame)
        {
            AvaloniaWindow window = null;

            Task.Run(() =>
            {
                int i = AppBuilder
                    .Configure<Application>()
                    .UsePlatformDetect()
                    .UseReactiveUI()
                    .AfterSetup(ab => CreateWindow(ref window, frame))
                    .StartWithClassicDesktopLifetime(new string[] { }, ShutdownMode.OnMainWindowClose);
            });

            while (window == null) { }

            return (window, new AvaloniaSoundPlayer());
        }

        private void CreateWindow(ref AvaloniaWindow window, IGameFrame frame)
        {
            window = new AvaloniaWindow();
            window.Title = "GameWindow";
            GamePanel panel = new GamePanel(window, (int)(frame.Bounds.Width / frame.ScaleX), (int)(frame.Bounds.Height / frame.ScaleY), frame.ScaleX, frame.ScaleY);
            window.Add(panel);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Width = frame.Bounds.Width;
            window.Height = frame.Bounds.Height;
            window.Show();
        }
    }
}
