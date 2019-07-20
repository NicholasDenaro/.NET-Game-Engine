using GameEngine.Interfaces;
using System.Drawing;

namespace GameEngine._2D
{
    public class Description2D : IDescription, IFollowable
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public Sprite Sprite { get; private set; }
        public int ImageIndex { get; set; }

        public Point Position => new Point((int)X, (int)Y);

        public Description2D(Sprite sprite, int x, int y)
        {
            this.Sprite = sprite;
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

        public Image Image()
        {
            return Sprite.GetImage(ImageIndex);
        }
    }
}
