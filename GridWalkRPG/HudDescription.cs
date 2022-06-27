using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridWalkRPG
{
    internal class HudDescription : Description2D
    {
        public HudDescription(Sprite sprite, int x, int y) : base(sprite, x, y)
        {
            this.DrawInOverlay = true;
        }
    }
}
