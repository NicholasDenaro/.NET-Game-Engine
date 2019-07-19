using GameEngine.Interfaces;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngine
{
    public class GameView2D : FollowingView
    {
        private GameFrame frame;
        public GamePanel Pane { get; private set; }
        private Bitmap buffer;

        private Rectangle Bounds;

        public int ScrollTop { get; set; }
        public int ScrollBottom { get; set; }
        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public bool LockViewToLocation { get; set; }

        public GameView2D(int width, int height, float hscale, float vscale)
        {
            Bounds = new Rectangle(0, 0, width, height);
            frame = new GameFrame(0, 0, (int)(width * hscale), (int)(height * vscale));
            Pane = frame.Pane;

            buffer = new Bitmap(width, height);
            ScrollTop = 20;
            ScrollBottom = 20;
            ScrollLeft = 20;
            ScrollRight = 20;
            LockViewToLocation = true;
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
            gfx.FillRectangle(Brushes.Magenta, 0, 0, Bounds.Width, Bounds.Height);

            gfx.TranslateTransform(-Bounds.X, -Bounds.Y);
            gfx.FillRectangle(location.BackgroundColor, 0, 0, location.Width, location.Height);

            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Drawer.Setup((Entity entity) =>
            {
                DrawEntity(entity, gfx);
            });

            location.Draw(Drawer);

            gfx.TranslateTransform(Bounds.X, Bounds.Y);

            frame.Pane.Draw(buffer);
        }

        private void DrawEntity(Entity entity, Graphics gfx)
        {
            Image img = entity.Image();
            gfx.DrawImage(img, (float)entity.X, (float)entity.Y);
        }

        public override void Tick()
        {
            Rectangle fBounds = new Rectangle(Following.Position, new Size(0, 0));
            Entity entity = Following as Entity;
            if (entity != null)
            {
                fBounds.Width = entity.Sprite?.Width ?? 0;
                fBounds.Height = entity.Sprite?.Height ?? 0;
            }

            if (fBounds.X < Bounds.X + ScrollLeft)
            {
                Bounds.X = fBounds.X - ScrollLeft;
            }
            else if (fBounds.X + fBounds.Width > Bounds.X + Bounds.Width - ScrollRight)
            {
                Bounds.X = fBounds.X + fBounds.Width + ScrollRight - Bounds.Width;
            }

            if (fBounds.Y < Bounds.Y + ScrollTop)
            {
                Bounds.Y = fBounds.Y - ScrollTop;
            }
            else if (fBounds.Y + fBounds.Height > Bounds.Y + Bounds.Height - ScrollBottom)
            {
                Bounds.Y = fBounds.Y + fBounds.Height + ScrollBottom - Bounds.Height;
            }

            if (LockViewToLocation)
            {
                if (Bounds.X + Bounds.Width > Program.Engine.Location.Width)
                {
                    Bounds.X = Program.Engine.Location.Width - Bounds.Width;
                }

                if (Bounds.X < 0)
                {
                    Bounds.X = 0;
                }

                if (Bounds.Y + Bounds.Height > Program.Engine.Location.Height)
                {
                    Bounds.Y = Program.Engine.Location.Height - Bounds.Height;
                }

                if (Bounds.Y < 0)
                {
                    Bounds.Y = 0;
                }
            }
        }
    }
}
