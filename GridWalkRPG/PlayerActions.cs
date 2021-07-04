using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace GridWalkRPG
{
    public class PlayerActions
    {
        private int controllerIndex;

        public PlayerActions(int controllerIndex)
        {
            this.controllerIndex = controllerIndex;
        }

        public void TickAction(Location location, Entity entity)
        {
            WindowsKeyController controller = Program.Engine.Controllers[controllerIndex] as WindowsKeyController;

            DescriptionPlayer descr = entity.Description as DescriptionPlayer;
            if (descr == null)
            {
                return;
            }

            List<WallDescription> walls = location.GetEntities<WallDescription>().ToList();

            for (int i = descr.controllerWalkChecks.Count - 1; i >= 0; i--)
            {
                int key = descr.controllerWalkChecks[i];
                if (descr.walkDuration <= 0 && controller[key].IsDown() && controller[key].Duration > 5)
                {
                    descr.walkDirection = key;
                    descr.walkDuration = DescriptionPlayer.maxWalkDuration;
                    descr.run = controller[(int)Program.KEYS.B].IsDown();
                    descr.controllerWalkChecks.RemoveAt(i);
                    descr.controllerWalkChecks.Insert(0, key);
                }
            }

            if (descr.walkDuration > 0)
            {
                int dist = descr.run ? 2 : 1;
                descr.ImageIndex = descr.walkDirection * 4 + descr.walkDuration / 8 % 4;

                if(descr.walkDuration % 2 == 1)
                {
                }
                else if (descr.walkDirection == (int)Program.KEYS.UP)
                {
                    descr.ChangeCoordsDelta(0, -1);
                    if (IsCollision(walls, descr))
                    {
                        descr.walkDuration = 0;
                        descr.ChangeCoordsDelta(0, 1);
                    }
                }
                else if (descr.walkDirection == (int)Program.KEYS.LEFT)
                {
                    descr.ChangeCoordsDelta(-1, 0);
                    if (IsCollision(walls, descr))
                    {
                        descr.walkDuration = 0;
                        descr.ChangeCoordsDelta(1, 0);
                    }
                }
                else if (descr.walkDirection == (int)Program.KEYS.DOWN)
                {
                    descr.ChangeCoordsDelta(0, 1);
                    if (IsCollision(walls, descr))
                    {
                        descr.walkDuration = 0;
                        descr.ChangeCoordsDelta(0, -1);
                    }
                }
                else if (descr.walkDirection == (int)Program.KEYS.RIGHT)
                {
                    descr.ChangeCoordsDelta(1, 0);
                    if (IsCollision(walls, descr))
                    {
                        descr.walkDuration = 0;
                        descr.ChangeCoordsDelta(-1, 0);
                    }
                }
                descr.walkDuration -= dist;
            }

            if (controller[(int)Program.KEYS.X].State == HoldState.PRESS)
            {
                oldState = descr.Serialize();
            }

            if (controller[(int)Program.KEYS.Y].State == HoldState.PRESS)
            {
                descr.Deserialize(oldState);
            }

            if (controller[(int)Program.KEYS.RESET].State == HoldState.PRESS)
            {
                Program.Engine.Deserialize(Program.states.Peek());
            }
        }
        private string oldState;

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
