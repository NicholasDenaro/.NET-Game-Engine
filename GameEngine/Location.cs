using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Location : ITicker
    {
        private Dictionary<Guid,Entity> entities = new Dictionary<Guid, Entity>();

        public void AddEntity(Entity entity)
        {
            entities.Add(entity.Id, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            RemoveEntity(entity.Id);
        }

        public void RemoveEntity(Guid id)
        {
            entities.Remove(id);
        }

        public void Tick()
        {
            foreach(Entity entity in entities.Values)
            {
                entity.Tick();
            }
        }

        public void Draw(Drawer drawer)
        {
            foreach (Entity entity in entities.Values)
            {
                drawer.Draw(entity);
            }
        }
    }
}
