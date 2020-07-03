using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine.UI.WinForms
{
    public class GameFrame : Form
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

        public GameFrame(int x, int y, int width, int height, int xScale, int yScale)
        {
            rect = new Rectangle(x, y, width * xScale, height * yScale);
            GamePanel panel = new GamePanel(width, height, xScale, yScale);
            Pane = panel;
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
