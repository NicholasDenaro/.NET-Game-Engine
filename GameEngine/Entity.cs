using GameEngine.Interfaces;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class Entity : ITicker, IFocusable
    {
        public static Dictionary<Guid, Entity> Entities { get; private set; } = new Dictionary<Guid, Entity>();

        public Guid Id { get; private set; }

        public IDescription Description { get; private set; }

        internal Entity(Guid guid, IDescription description)
        {
            this.Id = guid;
            if(Entities.ContainsKey(Id))
            {
                Entities.Remove(Id);
            }
            Entities.Add(Id, this);
            Description = description;
        }

        public Entity(IDescription description)
        {
            this.Id = Guid.NewGuid();
            Entities.Add(Id, this);
            Description = description;
        }

        public Action<GameState, Entity> TickAction { get; set; }

        virtual public void Tick(GameState currentLocation)
        {
            TickAction?.Invoke(currentLocation, this);
        }
    }
}
