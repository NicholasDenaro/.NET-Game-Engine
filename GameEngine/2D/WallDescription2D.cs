using System.Drawing;

namespace GameEngine._2D
{
    public class WallDescription2D : Description2D
    {
        public override Rectangle Bounds => new Rectangle();

        public WallDescription2D(int x, int y) : base(null, x, y)
        {
        }

        public override bool IsCollision(Description2D other)
        {
            return base.IsCollision(other);
        }
    }
}
