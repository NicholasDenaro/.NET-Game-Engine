using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngine._2D
{
    public class GameView2D : FollowingView
    {
        private int buf;
        private int buffers;

        private IDrawer2D<IGameBitmap> drawer;
        public override IDrawer Drawer => drawer;

        private Rectangle Bounds;

        public Rectangle ViewBounds => new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

        public int ScrollTop { get; set; }
        public int ScrollBottom { get; set; }
        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        private Color backgroundColor;

        private int xScale;
        private int yScale;

        public GameView2D(IDrawer2D<IGameBitmap> drawer, int width, int height, int xScale, int yScale, Color bgColor, int buffers = 1)
        {
            Bounds = new Rectangle(0, 0, width, height);
            
            this.drawer = drawer;
            this.xScale = xScale;
            this.yScale = yScale;
            this.drawer.Init(width, height, xScale, yScale);

            ScrollTop = 20;
            ScrollBottom = 20;
            ScrollLeft = 20;
            ScrollRight = 20;
            LockViewToLocation = true;
            backgroundColor = bgColor;
            this.buffers = buffers;
        }

        public void Resize(int width, int height)
        {
            this.Bounds.Width = width;
            this.Bounds.Height = height;
            this.drawer.Init(width, height, this.xScale, this.yScale);
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

            buf++;
            buf = buf % buffers;
            drawer.Clear(buf, Color.Transparent);
            TileMap tileMap = location.Description as TileMap;
            if (tileMap != null)
            {
                drawer.FillRectangle(buf, tileMap.BackgroundColor, 0, 0, tileMap.Width, tileMap.Height);
            }
            else
            {
                drawer.FillRectangle(buf, this.backgroundColor, 0, 0, this.Width, this.Height);
            }

            List<IDescription> descriptions = location.Draw();

            foreach (IDescription description in descriptions)
            {
                drawer.Draw(buf, description);
            }
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

    public interface IDrawer2D<out T> : IDrawer where T: IGameBitmap
    {
        void Init(int width, int height, int xScale, int yScale);
        void Clear(int buffer, System.Drawing.Color color);
        void TranslateTransform(int buffer, int x, int y);
        void FillRectangle(int buffer, System.Drawing.Color color, int x, int y, int w, int h);

        T Image(int buf);
        T Overlay(int buf);
    }

    public class Drawer2DSystemDrawing : IDrawer2D<SystemBitmap>
    {
        private SystemBitmap[] sbuffer;
        private SystemBitmap[] soverlay;

        private Bitmap tiles;

        public SystemBitmap Image(int buf) => sbuffer[buf];
        public SystemBitmap Overlay(int buf) => soverlay[buf];

        public Drawer2DSystemDrawing()
        {
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
            Bitmap[] buffer = new Bitmap[] { BitmapExtensions.CreateBitmap(width, height), BitmapExtensions.CreateBitmap(width, height) };
            buffer[0].MakeTransparent();
            buffer[1].MakeTransparent();
            Bitmap[] overlay = new Bitmap[] { BitmapExtensions.CreateBitmap(width * xScale, height * yScale), BitmapExtensions.CreateBitmap(width * xScale, height * yScale) };
            overlay[0].MakeTransparent();
            overlay[1].MakeTransparent();

            sbuffer = new SystemBitmap[] { new SystemBitmap(buffer[0]), new SystemBitmap(buffer[1]) };
            soverlay = new SystemBitmap[] { new SystemBitmap(overlay[0]), new SystemBitmap(overlay[1]) };
        }

        public void Clear(int buffer, Color color)
        {
            this.sbuffer[buffer].Context.Clear(color);
            this.soverlay[buffer].Context.Clear(Color.Transparent);
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
            this.sbuffer[buffer].Context.TranslateTransform(x, y);
        }
        
        public void FillRectangle(int buf, System.Drawing.Color color, int x, int y, int w, int h)
        {
            this.sbuffer[buf].Context.FillRectangle(new SolidBrush(color), x, y, w, h);
        }

        public void Draw(int buffer, IDescription description)
        {
            if (description is TileMap)
            {
                Draw(this.sbuffer[buffer].Context, description as TileMap);
            }
            else
            {
                Description2D d2d = description as Description2D;
                Graphics gfx = d2d.DrawInOverlay ? this.soverlay[buffer].Context : this.sbuffer[buffer].Context;
                Draw(gfx, d2d);
            }
        }

        public void Draw(Graphics gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rectangle dest = new Rectangle((int)(description.X + description.DrawOffsetX) - (description?.Sprite.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite.Y ?? 0), description.Width, description.Height);
                    gfx.DrawImage(description.Image(), dest, new Rectangle(0, 0, description.Width, description.Height), GraphicsUnit.Pixel);
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
                    tiles = BitmapExtensions.CreateBitmap(map.Width, map.Height);
                    tiles.MakeTransparent();
                    Graphics mgfx = Graphics.FromImage(tiles);
                    mgfx.Clear(Color.Transparent);
                    mgfx.Clip = new Region(new Rectangle(new Point(), new Size(map.Width, map.Height)));
                    int x = 0;
                    int y = 0;
                    foreach (int tile in map.Tiles)
                    {
                        mgfx.DrawImage(map.Image(tile), new RectangleF(x * map.Sprite.Width, y * map.Sprite.Height, map.Sprite.Width, map.Sprite.Height), new RectangleF(0, 0, map.Sprite.Width, map.Sprite.Height), GraphicsUnit.Pixel);
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }

                    mgfx.DrawRectangle(Pens.Black, 0, 0, map.Width - 1, map.Height - 1);
                }

                gfx.DrawImage(tiles, new RectangleF(0, 0, tiles.Width, tiles.Height), new RectangleF(0, 0, tiles.Width, tiles.Height), GraphicsUnit.Pixel);
            }
        }
    }
}
