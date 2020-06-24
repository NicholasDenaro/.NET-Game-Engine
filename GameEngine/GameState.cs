using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using static GameEngine.GameEngine;

namespace GameEngine
{
    public class GameState
    {
        public List<Controller> Controllers { get; internal set; } = new List<Controller>();

        public Location Location { get; internal set; }
        public Location NextLocation { get; internal set; }

        public IFocusable Focus { get; internal set; }
        public IFocusable NextFocus { get; internal set; }

        public static string Serialize(GameState state)
        {
            return "";
        }

        public static GameState Deserialize(string state)
        {
            return new GameState();
        }
    }
}