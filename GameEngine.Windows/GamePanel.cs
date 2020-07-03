using GameEngine._2D;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngine.UI.WinForms
{
    public class GamePanel : Panel
    {
        public bool Drawing { get; private set; }
        private Bitmap[] buffers;
        private byte currentBuffer;
        private int xScale;
        private int yScale;

        public GamePanel(int width, int height, int xScale, int yScale)
        {
            Drawing = false;
            this.xScale = xScale;
            this.yScale = yScale;
            this.Width = width * xScale;
            this.Height = height * yScale;
            currentBuffer = 0;
            buffers = new Bitmap[] { BitmapExtensions.CreateBitmap(this.Width, this.Height), BitmapExtensions.CreateBitmap(this.Width, this.Height) };
            this.DoubleBuffered = true;
        }

        public void HookMouse(GameFrame frame, EventHandler<MouseEventArgs> mouseInfo, EventHandler<MouseEventArgs> mouseDown, EventHandler<MouseEventArgs> mouseUp)
        {
            this.MouseMove += (s, e) => mouseInfo(s, ScaleEvent(Convert(e)));
            this.MouseDown += (s, e) => mouseDown(s, ScaleEvent(Convert(e)));
            this.MouseUp += (s, e) => mouseUp(s, ScaleEvent(Convert(e)));
        }

        public MouseEventArgs Convert(System.Windows.Forms.MouseEventArgs mea)
        {
            return new MouseEventArgs((int)mea.Button, mea.Clicks, mea.X, mea.Y, mea.Delta);
        }

        public MouseEventArgs ScaleEvent(MouseEventArgs e)
        {
            return new MouseEventArgs(e.Button, e.Clicks, e.X / xScale, e.Y / yScale, e.Wheel);
        }

        public void Draw(Bitmap img, Bitmap overlay)
        {
            Drawing = true;

            Graphics gfx = Graphics.FromImage(buffers[++currentBuffer % 2]);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            //gfx.DrawImage(img, 0, 0, this.Width + 2, this.Height + 2);
            gfx.DrawImage(img, 1, 1, this.Width, this.Height);
            gfx.DrawImage(overlay, 1, 1, overlay.Width, overlay.Height);
            //gfx.DrawRectangle(Pens.Cyan, 0, 0, width - 1, height - 1);
            Drawing = false;
            Invalidate();
        }

        public void DrawHandle(object sender, View view)
        {
            GameView2D view2D = view as GameView2D;
            if (view2D != null)
            {
                Draw(view2D.Image, view2D.Overlay);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(buffers[currentBuffer % 2], 0, 0, this.Width, this.Height);
        }
    }
}
