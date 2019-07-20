using GameEngine.Interfaces;
using System.Drawing;

namespace GameEngine._2D
{
    public class GameView2D : FollowingView
    {
        private GameFrame frame;
        public GamePanel Pane { get; private set; }
        private Graphics gfx;
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

            if (!Drawer.IsSetup)
            {
                gfx = Graphics.FromImage(buffer);
                Drawer.Setup(
                    (Entity entity) =>
                    {
                        DrawEntity(entity, gfx);
                    },
                    (Sprite sprite, int index, int x, int y) =>
                    {
                        DrawSprite(sprite, index, x, y, gfx);
                    },
                    (Image image, int x, int y) =>
                    {
                        DrawImage(image, x, y, gfx);
                    });
            }

            gfx.FillRectangle(Brushes.Magenta, 0, 0, Bounds.Width, Bounds.Height);

            gfx.TranslateTransform(-Bounds.X, -Bounds.Y);
            gfx.FillRectangle(location.BackgroundColor, 0, 0, location.Width, location.Height);

            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            location.Draw(Drawer);

            gfx.TranslateTransform(Bounds.X, Bounds.Y);

            frame.Pane.Draw(buffer);
        }

        private void DrawEntity(Entity entity, Graphics gfx)
        {
            Description2D description = entity.Description as Description2D;
            if (description != null)
            {
                Image img = description.Image();
                gfx.DrawImage(img, (float)description.X, (float)description.Y);
            }
        }

        private void DrawSprite(Sprite sprite, int index, int x, int y, Graphics gfx)
        {
            Image img = sprite.GetImage(index);
            gfx.DrawImage(img, x, y);
        }

        private void DrawImage(Image image, int x, int y, Graphics gfx)
        {
            gfx.DrawImage(image, x, y);
        }

        public override void Tick()
        {
            Rectangle fBounds = new Rectangle(Following.Position, new Size(0, 0));
            Description2D entityDescription = Following as Description2D;
            if (entityDescription != null)
            {
                fBounds.Width = entityDescription.Sprite?.Width ?? 0;
                fBounds.Height = entityDescription.Sprite?.Height ?? 0;
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
