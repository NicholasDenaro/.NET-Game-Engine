using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;
using System.Collections.Generic;

namespace GameEngine._2D
{
    public class GameView2D : FollowingView, ITicker
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


        public void Tick(GameState state)
        {
            Tick(null, state);
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
        void Clear(int buffer, Color color);
        void TranslateTransform(int buffer, int x, int y);
        void FillRectangle(int buffer, Color color, int x, int y, int w, int h);

        T Image(int buf);
        T Overlay(int buf);
    }
}
