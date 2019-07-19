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
                this.Pane.DpiChanged += (o, e) => Console.WriteLine("{0} -> {1}", e.DeviceDpiOld, e.DeviceDpiNew);
                this.Pane.StartPosition = FormStartPosition.CenterScreen;
                this.Pane.FormBorderStyle = FormBorderStyle.None;
                this.Pane.AutoScaleMode = AutoScaleMode.None;
                this.Pane.AutoSize = false;
                this.Pane.AutoScaleDimensions = new SizeF(240, 160);
                this.Pane.SetBounds(0, 0, rect.Width, rect.Height, BoundsSpecified.Size);
                Task.Run(() => Application.Run(this.Pane));
                Task.Delay(2000).ContinueWith(t => Console.WriteLine(this.Pane.Parent));
            }
        }

        public void Close()
        {
            Pane.Close();
        }
    }
}
