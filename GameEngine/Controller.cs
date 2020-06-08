using System.Collections.Generic;

namespace GameEngine
{
    public class Controller
    {
        internal Dictionary<int, ActionState> Actions { get; private set; } = new Dictionary<int, ActionState>();
        private Dictionary<int, ActionState> keys = new Dictionary<int, ActionState>();

        public Controller(Dictionary<int, ActionState> keymap)
        {
            Actions = keymap;
            foreach(KeyValuePair<int, ActionState> kvp in keymap)
            {
                keys[((ActionState)kvp.Value).Key] = (ActionState)kvp.Value;
            }
        }

        public void ActionStart(int actionCode)
        {
            if (keys.ContainsKey(actionCode))
            {
                if (keys[actionCode].State != HoldState.HOLD)
                {
                    keys[actionCode].State = HoldState.PRESS;
                }
            }
        }

        public void ActionEnd(int actionCode)
        {
            if (keys.ContainsKey(actionCode))
            {
                if (keys[actionCode].State != HoldState.UNHELD)
                {
                    keys[actionCode].State = HoldState.RELEASE;
                }
            }
        }

        public ActionState this[int action]
        {
            get
            {
                return (ActionState)Actions[action];
            }
        }

        public void Update()
        {
            foreach (KeyValuePair<int, ActionState> kvp in Actions)
            {
                ActionState act = (ActionState)kvp.Value;
                if (act.State == HoldState.PRESS && act.Duration > 1)
                {
                    keys[((ActionState)kvp.Value).Key].State = HoldState.HOLD;
                }
                else if (act.State == HoldState.RELEASE && act.Duration > 1)
                {
                    keys[((ActionState)kvp.Value).Key].State = HoldState.UNHELD;
                }
                else
                {
                    act.Duration++;
                }
            }
        }
    }
    public enum HoldState { PRESS, HOLD, RELEASE, UNHELD }

    public class ActionState
    {

        public HoldState State;
        public int Key { get; private set; }
        public int Duration { get; internal set; }

        public ActionState(int key)
        {
            State = HoldState.UNHELD;
            Key = key;
        }

        public bool IsDown()
        {
            return State == HoldState.HOLD || State == HoldState.PRESS;
        }
    }
}
