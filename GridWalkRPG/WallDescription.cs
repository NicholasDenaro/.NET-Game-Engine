using GameEngine._2D;

namespace GridWalkRPG
{
    public class WallDescription : Description2D
    {
        public WallDescription() : base()
        {

        }

        public WallDescription(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override string Serialize()
        {
            return $"{this.GetType().FullName}:{base.Serialize()}";
        }
    }
}
