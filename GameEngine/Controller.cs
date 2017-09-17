using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public abstract class Controller
    {
        internal Dictionary<int, ControllerAction> Actions { get; private set; } = new Dictionary<int, ControllerAction>();
        internal void SetActions(Dictionary<int, ControllerAction> actions)
        {
            Actions = actions;
        }

        public abstract void Input();
    }

    public class ControllerAction
    {
        
    }
}
