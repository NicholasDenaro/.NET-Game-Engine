using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine
{
    public class GameFrame : Form
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

        public GameFrame(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            GamePanel panel = new GamePanel( width, height);
            Pane = panel;
            //SetProcessDPIAware();
        }

        public void Start()
        {
            if (!started)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                this.FormBorderStyle = FormBorderStyle.None;
                this.AutoScaleMode = AutoScaleMode.Dpi;
                this.Width = rect.Width;
                this.Height = rect.Height;
                this.Controls.Add(this.Pane);
                Task.Run(() => Application.Run(this));
            }
        }
    }
}
