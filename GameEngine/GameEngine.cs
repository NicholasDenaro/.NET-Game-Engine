using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameEngine
{
    public delegate void TickHandler(object sender, GameState state);
    public delegate void DrawHandler(object sender, View view);

    public abstract class GameEngine
    {
        public enum QueueAction { Add, Remove }

        private List<(int stateKey, QueueAction action, Controller controller)> controllerQueue = new List<(int, QueueAction, Controller)>();

        public ref TickHandler TickStart(int stateKey) => ref this.states[stateKey].TickStart;
        public ref TickHandler TickEnd(int stateKey) => ref this.states[stateKey].TickEnd;

        public ref DrawHandler DrawStart(int stateKey) => ref this.states[stateKey].DrawStart;
        public ref DrawHandler Draw(int stateKey) => ref this.states[stateKey].Draw;
        public ref DrawHandler DrawEnd(int stateKey) => ref this.states[stateKey].DrawEnd;

        public Location Location(int stateKey) => this.states[stateKey].Location ?? this.states[stateKey].NextLocation;

        public List<Controller> Controllers(int stateKey) => this.states[stateKey].Controllers;
        public View Views(int stateKey) => this.states[stateKey].View;

        public int GetControllerIndex(int stateKey, Controller controller)
        {
            return this.states[stateKey].Controllers.IndexOf(controller);
        }

        public void SetLocation(int stateKey, Location location)
        {
            this.states[stateKey].NextLocation = location;
        }

        public void SetView(int stateKey, View view)
        {
            this.states[stateKey].View = view;
        }

        private Dictionary<int, GameState> states = new Dictionary<int, GameState>() { { 0, new GameState() } };

        public bool HasState(int key)
        {
            return states.ContainsKey(key);
        }

        public void AddNewState(int key)
        {
            states.Add(key, new GameState());
        }

        public bool Active { get; private set; }

        public async Task Start()
        {
            Active = true;
            Control();

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            };
        }
        
        public void Pause(int stateKey)
        {
            this.states[stateKey].IsPaused = true;
        }
        

        public void Resume(int stateKey)
        {
            this.states[stateKey].IsPaused = false;
        }

        public void Stop()
        {
            Active = false;
        }

        public abstract Task Control();

        public void Tick()
        {
            foreach (var state in this.states.Values)
            {
                state.NotifyTickStart(this);
            }

            foreach ((int stateKey, QueueAction action, Controller controller) queueAction in controllerQueue)
            {
                switch (queueAction.action)
                {
                    case QueueAction.Add:
                        states[queueAction.stateKey].Controllers.Add(queueAction.controller);
                        break;
                    case QueueAction.Remove:
                        states[queueAction.stateKey].Controllers.Remove(queueAction.controller);
                        break;
                }
            }

            controllerQueue.Clear();

            foreach (var state in this.states.Values)
            {
                state.Update();
            }

            foreach (var state in this.states.Values)
            {
                state.Tick();
            }

            foreach (var state in this.states.Values)
            {
                state.NotifyTickEnd(this);
            }
        }

        public void AddEntity(int stateKey, Entity entity)
        {
            if (this.states[stateKey].Location != null)
            {
                this.states[stateKey].Location.AddEntity(entity);
            }
            else
            {
                this.states[stateKey].NextLocation.AddEntity(entity);
            }
        }

        public void AddController(int stateKey, Controller controller)
        {
            if (!Active)
            {
                this.states[stateKey].Controllers.Add(controller);
            }
            else
            {
                controllerQueue.Add((stateKey, QueueAction.Add, controller));
            }
        }

        public void Draw()
        {
            foreach (var state in states.Values)
            {
                state.DrawStart?.Invoke(this, state.View);
            }

            foreach (var state in states.Values)
            {
                if (state.Location != null)
                {
                    state?.View.Draw(state.Location);
                }
            }

            foreach (var state in states.Values)
            {
                state.Draw?.Invoke(this, state.View);
            }


            foreach (var state in states.Values)
            {
                state.DrawEnd?.Invoke(this, state.View);
            }
        }

        public string Serialize()
        {
            return "";
        }

        public void Deserialize(string state)
        {

        }

        //public string Serialize()
        //{
        //    Stopwatch sw = Stopwatch.StartNew();
        //    string str = states.Serialize();
        //    sw.Stop();
        //    //Console.WriteLine($"{sw.ElapsedTicks} ticks");
        //    return str;
        //}

        //public void Deserialize(string state)
        //{
        //    Stopwatch sw = Stopwatch.StartNew();
        //    this.states.Deserialize(state);
        //    sw.Stop();
        //    //Console.WriteLine($"{sw.ElapsedTicks} ticks");
        //}
    }
}
