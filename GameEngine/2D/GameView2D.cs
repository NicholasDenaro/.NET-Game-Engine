using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngine._2D
{
    public class GameView2D : FollowingView
    {
        private Graphics[] gfxs;
        private Bitmap[] buffer;
        private int buf;

        private Drawer2D drawer;
        public override IDrawer Drawer => drawer;

        private Rectangle Bounds;

        public int ScrollTop { get; set; }
        public int ScrollBottom { get; set; }
        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        private SolidBrush backgroundColor;

        public Bitmap Image
        { 
            get
            {
                return buffer[buf % 2];
            }
        }

        public GameView2D(int width, int height, Color bgColor)
        {
            Bounds = new Rectangle(0, 0, width, height);
            buffer = new Bitmap[] { new Bitmap(width, height), new Bitmap(width, height) };
            gfxs = new Graphics[] { Graphics.FromImage(buffer[0]), Graphics.FromImage(buffer[1]) };
            gfxs[0].InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gfxs[1].InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            drawer = new Drawer2D();

            ScrollTop = 20;
            ScrollBottom = 20;
            ScrollLeft = 20;
            ScrollRight = 20;
            LockViewToLocation = true;
            backgroundColor = new SolidBrush(bgColor);
        }

        public void Redraw(Location location)
        {
            Draw(location);
        }

        internal override void Draw(Location location)
        {
            if (location == null)
            {
                return;
            }

            Graphics gfx = gfxs[++buf % 2];

            gfx.FillRectangle(backgroundColor, 0, 0, Bounds.Width, Bounds.Height);

            gfx.TranslateTransform(-Bounds.X, -Bounds.Y);
            TileMap tileMap = location.Description as TileMap;
            if (tileMap != null)
            {
                gfx.FillRectangle(tileMap.BackgroundColor, 0, 0, tileMap.Width, tileMap.Height);
            }

            List<IDescription> descriptions = location.Draw();

            foreach (IDescription description in descriptions)
            {
                drawer.Draw(gfx, description);
            }

            gfx.TranslateTransform(Bounds.X, Bounds.Y);
        }

        public void Tick(object sender, GameState state)
        {
            Follow();

            if (LockViewToLocation && state?.Location != null)
            {
                LockFollow(state.Location);
            }
        }

        public void Follow()
        {
            Description2D entityDescription = Following as Description2D;
            if(entityDescription == null)
            {
                return;
            }

            Rectangle fBounds = new Rectangle(Following.Position, new Size(0, 0));
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

        public void LockFollow(Location currentLocation)
        {
            TileMap tileMap = currentLocation.Description as TileMap;
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
        private Bitmap tiles;

        public Drawer2D()
        {
        }

        public void Draw(object output, IDescription description)
        {
            if (description is TileMap)
            {
                Draw(output as Graphics, description as TileMap);
            }
            else
            {
                Draw(output as Graphics, description as Description2D);
            }
        }

        public void Draw(Graphics gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    gfx.DrawImage(description.Image(), (float)description.X - (description?.Sprite.X ?? 0), (float)description.Y - (description?.Sprite.Y ?? 0));
                }
            }
        }

        public void RedrawTiles()
        {
            tiles = null;
        }

        public void Draw(Graphics gfx, TileMap map)
        {
            if (map.Sprite != null)
            {
                if (tiles == null)
                {
                    tiles = new Bitmap(map.Width, map.Height);
                    Graphics mgfx = Graphics.FromImage(tiles);
                    mgfx.Clip = new Region(new Rectangle(new Point(), new Size(map.Width, map.Height)));
                    int x = 0;
                    int y = 0;
                    foreach (byte tile in map.Tiles)
                    {
                        mgfx.DrawImage(map.Image(tile), x * map.Sprite.Width, y * map.Sprite.Height);
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

                gfx.DrawImage(tiles, 0, 0);
            }
        }
    }
}
