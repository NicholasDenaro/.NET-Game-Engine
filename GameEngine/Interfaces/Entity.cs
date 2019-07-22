using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngine
{
    public class Entity : ITicker, IFocusable
    {
        public static Dictionary<Guid, Entity> Entities { get; private set; } = new Dictionary<Guid, Entity>();

        public Guid Id { get; private set; }

        public IDescription Description { get; private set; }

        public Entity(IDescription description)
        {
            this.Id = Guid.NewGuid();
            Entities.Add(Id, this);
            Description = description;
        }

        public Action<Location, IDescription> TickAction;

        virtual public void Tick(Location currentLocation)
        {
            TickAction?.Invoke(currentLocation, Description);
        }
    }
}
