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

        private GameEngine engine;
        private int width;
        private int height;

        public GamePanel(GameEngine engine, int width, int height)
        {
            Drawing = false;
            this.width = width * 4;
            this.height = height * 4;
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

            gfx.DrawImage(img, 0, 0, width, height);
            gfx.DrawRectangle(Pens.Cyan, 0, 0, width, height);
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
