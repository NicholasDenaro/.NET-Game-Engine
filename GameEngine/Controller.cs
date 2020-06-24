using System.Collections.Generic;
using System.Linq;

namespace GameEngine
{
    public class Controller
    {
        internal Dictionary<int, ActionState> Actions { get; private set; } = new Dictionary<int, ActionState>();
        private readonly List<(int, HoldState)>[] actionBuffer = new List<(int, HoldState)>[2];
        private int buff;
        private List<int> keys;
        private object Lock = new object();

        public Controller(IEnumerable<int> keys)
        {
            this.keys = keys.ToList();
            Actions = keys.ToDictionary(key => key, key => new ActionState());
            buff = 1;
            actionBuffer[0] = new List<(int, HoldState)>();
            actionBuffer[1] = new List<(int, HoldState)>();
        }

        public void ActionStart(int actionCode)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock(Lock)
                {
                    actionBuffer[buff % 2].Add((actionCode,HoldState.PRESS));
                }

            }
        }

        public void ActionEnd(int actionCode)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock (Lock)
                {
                    actionBuffer[buff % 2].Add((actionCode, HoldState.RELEASE));
                }
            }
        }

        public ActionState this[int action]
        {
            get
            {
                return Actions[action];
            }
        }

        public void Update()
        {
            lock(Lock)
            {
                buff++;
            }

            ActionState actionstate;
            foreach (int key in this.keys)
            {
                actionstate = Actions[key];
                if (actionstate.State == HoldState.HOLD || actionstate.State == HoldState.UNHELD)
                {
                    actionstate.Duration++;
                }
                else if(actionstate.State == HoldState.PRESS)
                {
                    actionstate.Duration = 0;
                    actionstate.State = HoldState.HOLD;
                }
                else if (actionstate.State == HoldState.RELEASE)
                {
                    actionstate.Duration = 0;
                    actionstate.State = HoldState.UNHELD;
                }
            }

            foreach ((int key, HoldState state) in actionBuffer[(buff - 1) % 2])
            {
                actionstate = Actions[key];
                if (actionstate.State == HoldState.UNHELD && state == HoldState.PRESS)
                {
                    actionstate.State = HoldState.PRESS;
                }
                else if (actionstate.State == HoldState.HOLD && state == HoldState.RELEASE)
                {
                    actionstate.State = HoldState.RELEASE;
                }
            }

            actionBuffer[(buff - 1) % 2].Clear();
        }
    }
    public enum HoldState { PRESS, HOLD, RELEASE, UNHELD }

    public class ActionState
    {
        public HoldState State;
        public int Duration { get; internal set; }

        public ActionState()
        {
            State = HoldState.UNHELD;
        }

        public bool IsDown()
        {
            return State == HoldState.HOLD || State == HoldState.PRESS;
        }
    }
}
