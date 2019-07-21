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

        private Drawer2D drawer;
        public override IDrawer Drawer => drawer;

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
            gfx = Graphics.FromImage(buffer);
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            drawer = new Drawer2D(gfx);

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

            gfx.FillRectangle(Brushes.Magenta, 0, 0, Bounds.Width, Bounds.Height);

            gfx.TranslateTransform(-Bounds.X, -Bounds.Y);
            TileMap tileMap = location.Description as TileMap;
            if (tileMap != null)
            {
                gfx.FillRectangle(tileMap.BackgroundColor, 0, 0, tileMap.Width, tileMap.Height);
            }

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
            Follow();

            if (LockViewToLocation)
            {
                LockFollow();
            }
        }

        public void Follow()
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
        }

        public void LockFollow()
        {
            TileMap tileMap = Program.Engine.Location.Description as TileMap;
            if (tileMap != null)
            {
                if (Bounds.X + Bounds.Width > tileMap.Width)
                {
                    Bounds.X = tileMap.Width - Bounds.Width;
                }

                if (Bounds.X < 0)
                {
                    Bounds.X = 0;
                }

                if (Bounds.Y + Bounds.Height > tileMap.Height)
                {
                    Bounds.Y = tileMap.Height - Bounds.Height;
                }

                if (Bounds.Y < 0)
                {
                    Bounds.Y = 0;
                }
            }
        }
    }

    public class Drawer2D : IDrawer
    {
        private Graphics gfx;

        public Drawer2D(Graphics gfx)
        {
            this.gfx = gfx;
        }

        public void Draw(Entity entity)
        {
            Description2D description = entity.Description as Description2D;
            if (description != null)
            {
                gfx.DrawImage(description.Image(), (float)description.X, (float)description.Y);
            }
        }

        public void Draw(Location location)
        {
            TileMap description = location.Description as TileMap;
            if (description.Sprite != null)
            {
                int x = 0;
                int y = 0;
                foreach (byte tile in description.Tiles)
                {
                    gfx.DrawImage(description.Image(tile), x * description.Sprite.Width, y * description.Sprite.Height);
                    x++;
                    if (x >= description.Columns)
                    {
                        x = 0;
                        y++;
                    }
                }
            }
        }
    }
}
