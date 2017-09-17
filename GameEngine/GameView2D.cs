using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameView2D : View
    {
        private GameFrame frame;
        public GamePanel Pane { get; private set; }
        private Bitmap buffer;

        private int width;
        private int height;

        public GameView2D(int x, int y, int width, int height, float hscale, float vscale)
        {
            this.width = width;
            this.height = height;
            frame = new GameFrame(x, y, (int)(width * hscale), (int)(height * vscale));
            Pane = frame.Pane;

            buffer = new Bitmap(width, height);
        }

        public override void Open()
        {
            frame.Start();
        }

        public override void Close()
        {
            frame.Close();
        }

        public void Redraw(Location location)
        {
            Draw(location);
        }

        internal override void Draw(Location location)
        {
            while (frame.Pane.Drawing) { };
            Graphics gfx = Graphics.FromImage(buffer);

            gfx.FillRectangle(Brushes.Magenta, 0, 0, width, height);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Drawer.Setup((Entity entity) =>
            {
                DrawEntity(entity, gfx);
            });

            location.Draw(Drawer);

            frame.Pane.Draw(buffer);
        }

        private void DrawEntity(Entity entity, Graphics gfx)
        {
            gfx.DrawImage(entity.Image(), (float)entity.X, (float)entity.Y);
        }
    }
}
