using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class Controller : IDescription
    {
        internal Dictionary<object, ActionState> Actions { get; private set; } = new Dictionary<object, ActionState>();
        private readonly NBuffer<List<PendingAction>> actionBuffer = new NBuffer<List<PendingAction>>(2);
        private List<object> keys;
        private object Lock = new object();

        public bool IsHooked { get; set; }

        public Controller()
        {

        }

        public Controller(IEnumerable<object> keys)
        {
            this.keys = keys.ToList();
            Actions = keys.ToDictionary(key => key, key => new ActionState());
        }

        public void ActionInfo(object actionCode, IControllerActionInfo info)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock (Lock)
                {
                    actionBuffer.Next().Add(new PendingAction(actionCode, HoldState.INFO, info));
                }

            }
        }

        public void ActionStart(object actionCode, IControllerActionInfo info)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock(Lock)
                {
                    actionBuffer.Next().Add(new PendingAction(actionCode, HoldState.PRESS, info));
                }

            }
        }

        public void ActionEnd(object actionCode, IControllerActionInfo info)
        {
            if (Actions.ContainsKey(actionCode))
            {
                lock (Lock)
                {
                    actionBuffer.Next().Add(new PendingAction(actionCode, HoldState.RELEASE, info));
                }
            }
        }

        public ActionState this[object action]
        {
            get
            {
                return Actions[action];
            }
        }

        public void Update()
        {

            ActionState actionstate;
            foreach (object key in this.keys)
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
                List<PendingAction> actions = actionBuffer.MoveNext();
                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                {
                    actions.Reverse();
#if net60
                    List<PendingAction> temp = actions.DistinctBy(action => action.ActionCode).ToList();
#endif
#if net48
                    List<object> actionCodes = new List<object>();
                    List<PendingAction> outActions = new List<PendingAction>();
                    for (int i = 0; i < actions.Count; i++)
                    {
                        if (!actionCodes.Contains(actions[i].ActionCode))
                        {
                            outActions.Add(actions[i]);
                        }
                    }
                    List<PendingAction> temp = outActions;
#endif
                    actions.Clear();
                    actions.AddRange(temp);
                    actions.Reverse();
                }

                next = actions;
            }

            foreach (PendingAction pa in next)
            {
                actionstate = Actions[pa.ActionCode];
                if (pa.State == HoldState.INFO)
                {
                    actionstate.Info = pa.Info;
                }
                else if (actionstate.State == HoldState.UNHELD && pa.State == HoldState.PRESS)
                {
                    actionstate.State = HoldState.PRESS;
                    actionstate.Info = pa.Info;
                }
                else if (actionstate.State == HoldState.HOLD && pa.State == HoldState.RELEASE)
                {
                    actionstate.State = HoldState.RELEASE;
                    actionstate.Info = pa.Info;
                }
            }

            next.Clear();
        }

        public virtual string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(StringConverter.Serialize<object>(keys));
            sb.Append(",");
            sb.Append(StringConverter.Serialize<object, ActionState>(Actions));
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
            keys = StringConverter.Deserialize<object>(tokens[0], str => int.Parse(str));
            Actions = StringConverter.Deserialize<object, ActionState>(tokens[1], str => int.Parse(str), str => { ActionState acts = new ActionState(); acts.Deserialize(str); return acts; });
        }
    }
    public enum HoldState { PRESS, HOLD, RELEASE, UNHELD, INFO }

    public interface IControllerActionInfo
    {

    }

    public class ControllerActionInfoEmpty
    {

    }

    public class PendingAction : IDescription
    {
        public object ActionCode { get; private set; }
        public HoldState State { get; private set; }

        public IControllerActionInfo Info { get; private set; }

        public PendingAction()
        {

        }

        public PendingAction(object actionCode, HoldState state, IControllerActionInfo info)
        {
            this.ActionCode = actionCode;
            this.State = state;
            this.Info = info;
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

        public IControllerActionInfo Info { get; internal set; }

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

        public bool IsPress()
        {
            return this.State == HoldState.PRESS;
        }
    }
}
