using GameEngine.Interfaces;

namespace GameEngine
{
    public abstract class View
    {
        public abstract IDrawer Drawer { get; }

        public View()
        {
        }

        internal abstract void Draw(Location location);
    }

    public interface IDrawer
    {
        void Draw(object output, IDescription description);
    }
}
