using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class View
    {
        internal Drawer Drawer { get; private set; }

        public View()
        {
            Drawer = new Drawer();
        }

        public abstract void Open();

        public abstract void Close();

        internal abstract void Draw(Location location);
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
