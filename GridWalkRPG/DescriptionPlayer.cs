using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridWalkRPG
{
    public class DescriptionPlayer : Description2D
    {
        internal int walkDirection;
        internal List<int> controllerWalkChecks;
        internal int walkDuration;
        internal bool run;

        public const int maxWalkDuration = 32;

        public DescriptionPlayer() : base()
        {

        }

        public DescriptionPlayer(Sprite sprite, int x, int y) : base(sprite, x, y)
        {
            walkDirection = 0;
            walkDuration = 0;
            controllerWalkChecks = new List<int>(new int[] { (int)Program.KEYS.UP, (int)Program.KEYS.LEFT, (int)Program.KEYS.DOWN, (int)Program.KEYS.RIGHT });
        }

        public override string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().FullName);
            sb.Append(":");
            sb.Append("{");
            sb.Append(base.Serialize());
            sb.Append(",");
            sb.Append(walkDirection);
            sb.Append(",");
            sb.Append(walkDuration);
            sb.Append(",");
            sb.Append(run);
            sb.Append(",");
            sb.Append(StringConverter.Serialize<int>(controllerWalkChecks));
            sb.Append("}");
            return sb.ToString();
        }

        public override void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);

            base.Deserialize(tokens[0]);

            walkDirection = int.Parse(tokens[1]);
            walkDuration = int.Parse(tokens[2]);
            run = bool.Parse(tokens[3]);
            controllerWalkChecks = StringConverter.Deserialize<int>(tokens[4], str => int.Parse(str));
        }
    }
}
