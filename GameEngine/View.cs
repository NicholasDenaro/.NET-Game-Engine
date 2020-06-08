using GameEngine.Interfaces;

namespace GameEngine
{
    public abstract class View : ITicker
    {
        public abstract IDrawer Drawer { get; }

        public View()
        {
        }

        internal abstract void Draw(Location location);

        public abstract void Tick(Location currentLocation);
    }

    public interface IDrawer
    {
        void Draw(object output, IDescription description);
    }
}
