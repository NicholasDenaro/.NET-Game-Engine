using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine
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

        public GameFrame(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
            GamePanel panel = new GamePanel(width, height);
            Pane = panel;
        }

        public void Start()
        {
            if (!started)
            {
                this.Pane.StartPosition = FormStartPosition.Manual;
                this.Pane.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
                Task.Run(() => Application.Run(this.Pane));
            }
        }

        public void Close()
        {
            Pane.Close();
        }
    }
}
