using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class GameState : IDescription
    {
        public TickHandler TickStart;
        public TickHandler TickEnd;

        public DrawHandler DrawStart;
        public DrawHandler DrawEnd;

        public List<Controller> Controllers { get; internal set; } = new List<Controller>();

        public Location Location { get; internal set; }
        public Location NextLocation { get; internal set; }

        public View View { get; internal set; }
        public View NextView { get; internal set; }

        public bool IsPaused { get; internal set; }

        public void NotifyTickStart(GameEngine engine)
        {
            if (IsPaused)
            {
                return;
            }

            TickStart?.Invoke(engine, this);
        }

        public void Tick()
        {
            if (IsPaused)
            {
                return;
            }

            Location?.Tick(this);
        }

        public void NotifyTickEnd(GameEngine engine)
        {
            if (IsPaused)
            {
                return;
            }

            TickEnd?.Invoke(engine, this);
        }

        public void Update()
        {
            if (IsPaused)
            {
                return;
            }

            Location = NextLocation ?? Location;
            NextLocation = null;

            View = NextView ?? View;
            NextView = null;

            foreach (Controller controller in Controllers)
            {
                controller.Update();
            }
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(StringConverter.Serialize<Controller>(Controllers));
            sb.Append(",");
            sb.Append(Location.Serialize());
            sb.Append(",");
            sb.Append(NextLocation?.Serialize() ?? "<null>");
            sb.Append("}");
            return sb.ToString();
        }

        public void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);

            Controllers = StringConverter.Deserialize<Controller>(tokens[0], str => null);
            Location.Deserialize(tokens[1]);
            if (tokens[2] == "<null>")
            {
                NextLocation = null;
            }
            else if (NextLocation == null)
            {
                NextLocation = new Location();
                NextLocation.Deserialize(tokens[2]);
            }
            else
            {
                NextLocation.Deserialize(tokens[2]);
            }

        }
    }
}