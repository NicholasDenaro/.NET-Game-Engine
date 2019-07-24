using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GridWalkRPG
{
    public class PlayerActions
    {
        private KeyController controller;

        public PlayerActions(KeyController controller)
        {
            this.controller = controller;
        }

        public void TickAction(Location location, IDescription description)
        {
            Description2D descr = description as Description2D;
            if (descr == null)
            {
                return;
            }

            List<WallDescription> walls = location.GetEntities<WallDescription>().ToList();

            if (this.controller[(int)Program.KEYS.UP].IsDown())
            {
                descr.ChangeCoordsDelta(0, -1);
                if (IsCollision(walls, descr))
                {
                    descr.ChangeCoordsDelta(0, 1);
                }
            }

            if (this.controller[(int)Program.KEYS.DOWN].IsDown())
            {
                descr.ChangeCoordsDelta(0, 1);
                if (IsCollision(walls, descr))
                {
                    descr.ChangeCoordsDelta(0, -1);
                }
            }

            if (this.controller[(int)Program.KEYS.LEFT].IsDown())
            {
                descr.ChangeCoordsDelta(-1, 0);
                if (IsCollision(walls, descr))
                {
                    descr.ChangeCoordsDelta(1, 0);
                }
            }

            if (this.controller[(int)Program.KEYS.RIGHT].IsDown())
            {
                descr.ChangeCoordsDelta(1, 0);
                if (IsCollision(walls, descr))
                {
                    descr.ChangeCoordsDelta(-1, 0);
                }
            }
        }

        private bool IsCollision(List<WallDescription> walls, Description2D description)
        {
            foreach (WallDescription wall in walls)
            {
                if (description.IsCollision(wall))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
