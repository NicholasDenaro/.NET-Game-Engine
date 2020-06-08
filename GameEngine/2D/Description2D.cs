using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;
using System.Drawing;

namespace GameEngine._2D
{
    public class Description2D : IDescription, IFollowable
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public Sprite Sprite { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ImageIndex { get; set; }
        virtual public Rectangle Bounds => new Rectangle(new Point((int)X, (int)Y), new Size(Width, Height));

        public Point Position => new Point((int)X, (int)Y);

        public Description2D(Sprite sprite, int x, int y)
        {
            this.Sprite = sprite;
            Width = this.Sprite?.Width ?? 0;
            Height = this.Sprite?.Height ?? 0;
            SetCoords(x, y);
        }

        public Description2D(Sprite sprite, int x, int y, int width, int height)
        {
            this.Sprite = sprite;
            this.Width = width;
            this.Height = height;
            SetCoords(x, y);
        }

        public Description2D(int x, int y, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            SetCoords(x, y);
        }

        public void ChangeCoordsDelta(double dx, double dy)
        {
            SetCoords(X + dx, Y + dy);
        }

        public void SetCoords(double x, double y)
        {
            // Listener here?
            this.X = x;
            this.Y = y;
        }

        virtual public bool IsCollision(Description2D other)
        {
            return this.Bounds.IntersectsWith(other.Bounds);
        }

        public bool HasImage()
        {
            return Sprite != null;
        }

        public Image Image()
        {
            return Sprite?.GetImage(ImageIndex);
        }
    }
}
