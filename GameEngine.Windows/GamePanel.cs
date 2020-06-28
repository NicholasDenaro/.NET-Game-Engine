using GameEngine._2D;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngine
{
    public class GamePanel : Panel
    {
        public bool Drawing { get; private set; }
        private Bitmap[] buffers;
        private byte currentBuffer;

        private int width;
        private int height;

        public GamePanel(int width, int height)
        {
            Drawing = false;
            this.width = width;
            this.height = height;
            this.Width = this.width;
            this.Height = this.height;
            currentBuffer = 0;
            buffers = new Bitmap[] { new Bitmap(this.width, this.height), new Bitmap(this.width, this.height) };
            this.DoubleBuffered = true;
        }

        public void Draw(Bitmap img)
        {
            Drawing = true;

            Graphics gfx = Graphics.FromImage(buffers[++currentBuffer % 2]);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            gfx.DrawImage(img, 0, 0, width + 2, height + 2);
            //gfx.DrawRectangle(Pens.Cyan, 0, 0, width - 1, height - 1);
            Drawing = false;
            Invalidate();
        }

        public void DrawHandle(object sender, View view)
        {
            GameView2D view2D = view as GameView2D;
            if (view2D != null)
            {
                Draw(view2D.Image);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(buffers[currentBuffer % 2], 0, 0, width, height);
        }
    }
}
