using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine._2D
{
    public class Description2D : IDescription, IFollowable
    {
        public delegate void MovementTrigger(Description2D d2d);

        public double X { get; private set; }
        public double Y { get; private set; }

        public int ZIndex { get; set; }

        public double DrawOffsetX { get; protected set; }
        public double DrawOffsetY { get; protected set; }

        public Sprite Sprite { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ImageIndex { get; set; }
        virtual public Rectangle Bounds => new Rectangle((int)X, (int)Y, Width, Height);

        protected MovementTrigger onMove;

        public Point Position => new Point((int)X, (int)Y);

        public bool DrawInOverlay { get; protected set; }

        public delegate Bitmap DirectDraw();

        public DirectDraw DrawAction;

        public Description2D()
        {

        }

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

        public void AddMovementListener(MovementTrigger listener)
        {
            onMove += listener;
        }

        public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void SetCoords(double x, double y)
        {
            this.X = x;
            this.Y = y;
            onMove?.Invoke(this);
        }

        public void SetOffset(double xOffset, double yOffset)
        {
            this.DrawOffsetX = xOffset;
            this.DrawOffsetY = yOffset;
        }

        virtual public bool IsCollision(Description2D other)
        {
            return this.Bounds.IntersectsWith(other.Bounds);
        }

        public bool HasImage()
        {
            return Sprite != null || DrawAction != null;
        }

        public BitmapSection Image()
        {
            Bitmap bmp = DrawAction?.Invoke();

            if (bmp != null)
            {
                return new BitmapSection(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            else
            {
                return Sprite?.GetImage(ImageIndex);
            }
        }

        public double Distance(Description2D other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
        }

        public double Distance(Point p)
        {
            return Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y));
        }

        public double Distance(double x, double y)
        {
            return Math.Sqrt((X - x) * (X - x) + (Y - y) * (Y - y));
        }

        public double Direction(Description2D other)
        {
            return Math.Atan2(other.Y - Y, other.X - X);
        }

        public double Direction(Point point)
        {
            return Math.Atan2(point.Y - Y, point.X - X);
        }

        virtual public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(X);
            sb.Append(",");
            sb.Append(Y);
            sb.Append(",");
            sb.Append(Sprite?.Name ?? "<null>");
            sb.Append(",");
            sb.Append(Width);
            sb.Append(",");
            sb.Append(Height);
            sb.Append(",");
            sb.Append(ImageIndex);
            sb.Append("}");
            return sb.ToString();
        }

        virtual public void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);

            this.X = int.Parse(tokens[0]);
            this.Y = int.Parse(tokens[1]);
            if (tokens[2] != "<null>")
            {
                this.Sprite = Sprite.Sprites[tokens[2]];
            }
            else
            {
                this.Sprite = null;
            }
            this.Width = int.Parse(tokens[3]);
            this.Height = int.Parse(tokens[4]);
            this.ImageIndex = int.Parse(tokens[5]);
        }
    }
}
