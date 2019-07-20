using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine
{
    public class GamePanel : Form
    {
        public bool Drawing { get; private set; }
        private Bitmap buffer;

        private int width;
        private int height;

        public GamePanel(int width, int height)
        {
            Drawing = false;
            this.width = width;
            this.height = height;
            DoubleBuffered = true;
            Focus();
        }

        public void Draw(Bitmap buffer)
        {
            this.buffer = buffer;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (buffer == null)
            {
                return;
            }

            //if (Draw)
            {
                //Draw = false;
                Drawing = true;
                base.OnPaint(e);

                Graphics gfx = e.Graphics;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                gfx.DrawImage(buffer, 0, 0, width, height);
                
                Drawing = false;
            }
            //Invalidate();
        }
    }
}
