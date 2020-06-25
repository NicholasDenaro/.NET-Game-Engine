using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Controller : IDescription
    {
        internal Dictionary<int, ActionState> Actions { get; private set; } = new Dictionary<int, ActionState>();
        private readonly NBuffer<List<PendingAction>> actionBuffer = new NBuffer<List<PendingAction>>(2);
        private List<int> keys;
        private object Lock = new object();

        public Controller()
        {

        }

        public Controller(IEnumerable<int> keys)
        {
            this.keys = keys.ToList();
            Actions = keys.ToDictionary(key => key, key => new ActionState());
        }

        public void ActionStart(int actionCode)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock(Lock)
                {
                    actionBuffer.Next().Add(new PendingAction(actionCode, HoldState.PRESS));
                }

            }
        }

        public void ActionEnd(int actionCode)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock (Lock)
                {
                    actionBuffer.Next().Add(new PendingAction(actionCode, HoldState.RELEASE));
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

            List<PendingAction> next;
            lock (Lock)
            {
                next = actionBuffer.MoveNext();
            }

            foreach (PendingAction pa in next)
            {
                actionstate = Actions[pa.ActionCode];
                if (actionstate.State == HoldState.UNHELD && pa.State == HoldState.PRESS)
                {
                    actionstate.State = HoldState.PRESS;
                }
                else if (actionstate.State == HoldState.HOLD && pa.State == HoldState.RELEASE)
                {
                    actionstate.State = HoldState.RELEASE;
                }
            }

            next.Clear();
        }

        public virtual string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(StringConverter.Serialize<int>(keys));
            sb.Append(",");
            sb.Append(StringConverter.Serialize<int, ActionState>(Actions));
            sb.Append(",");
            sb.Append("[");
            foreach (List<PendingAction> act in actionBuffer.Buffers)
            {
                sb.Append(StringConverter.Serialize<PendingAction>(act));
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        public virtual void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);
            keys = StringConverter.Deserialize<int>(tokens[0], str => int.Parse(str));
            Actions = StringConverter.Deserialize<int, ActionState>(tokens[1], str => int.Parse(str), str => { ActionState acts = new ActionState(); acts.Deserialize(str); return acts; });
        }
    }
    public enum HoldState { PRESS, HOLD, RELEASE, UNHELD }

    public class PendingAction : IDescription
    {
        public int ActionCode { get; private set; }
        public HoldState State { get; private set; }

        public PendingAction()
        {

        }

        public PendingAction(int actionCode, HoldState state)
        {
            this.ActionCode = actionCode;
            this.State = state;
        }

        public string Serialize()
        {
            return $"{{{this.ActionCode},{(int)this.State}}}";
        }

        public void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);
            this.ActionCode = int.Parse(tokens[0]);
            this.State = (HoldState)int.Parse(tokens[0]);
        }
    }

    public class ActionState : IDescription
    {
        public HoldState State;
        public int Duration { get; internal set; }

        public ActionState()
        {
            this.State = HoldState.UNHELD;
        }

        public bool IsDown()
        {
            return this.State == HoldState.HOLD || this.State == HoldState.PRESS;
        }

        public string Serialize()
        {
            return $"{{{(int)this.State},{this.Duration}}}";
        }

        public void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);
            this.State = (HoldState)int.Parse(tokens[0]);
            this.Duration = int.Parse(tokens[1]);
        }
    }
}
