using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System.Drawing;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class GameFrame
    {
        private bool started = false;
        private Rectangle rect;

        private GamePanel panel;
        public GamePanel Pane {
            get { return panel; }
            set
            {
                if(!started)
                {
                    panel = value;
                }
            }
        }

        internal Window window;

        public GameFrame(int x, int y, int width, int height, int xScale, int yScale)
        {
            rect = new Rectangle(x, y, width * xScale, height * yScale);
            GamePanel panel = new GamePanel(this, width, height, xScale, yScale);
            Pane = panel;

        }

        public void Start()
        {
            if (!started)
            {
                started = true;
                Task.Run(() =>
                {
                    int i = AppBuilder.Configure<Application>().UsePlatformDetect().UseReactiveUI().AfterSetup(ab =>
                    {
                        window = new MainWindow(Pane);
                        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        window.Position = new Avalonia.PixelPoint(rect.X, rect.Y);
                        window.Width = rect.Width;
                        window.Height = rect.Height;
                        window.Show();
                    }).StartWithClassicDesktopLifetime(new string[] { });
                });
            }
        }
    }
}
