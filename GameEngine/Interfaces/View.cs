using System;
using System.Drawing;

namespace GameEngine.Interfaces
{
    public abstract class View : ITicker
    {
        internal Drawer Drawer { get; private set; }

        public View()
        {
            Drawer = new Drawer();
        }

        public abstract void Open();

        public abstract void Close();

        internal abstract void Draw(Location location);
        public abstract void Tick();
    }

    public class Drawer
    {
        private Action<Entity> action;

        public void Draw(Entity entity)
        {
            action(entity);
        }

        public void Setup(Action<Entity> action)
        {
            this.action = action;
        }
    }
}
