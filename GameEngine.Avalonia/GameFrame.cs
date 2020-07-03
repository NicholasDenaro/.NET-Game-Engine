using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using System.Drawing;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class GameFrame
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

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
                Task.Run(() =>
                {
                    int i = AppBuilder.Configure<Application>().UsePlatformDetect().UseReactiveUI().AfterSetup(ab =>
                    {
                        window = new MainWindow(Pane);
                        window.Position = new Avalonia.PixelPoint(rect.X, rect.Y);
                        window.Width = rect.Width;
                        window.Height = rect.Height;
                        //window.Renderer.DrawDirtyRects = true;
                        //window.Renderer.DrawFps = true;
                        window.Show();
                    }).StartWithClassicDesktopLifetime(new string[] { });
                });
            }
        }
    }
}
