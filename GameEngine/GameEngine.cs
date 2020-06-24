using GameEngine.Interfaces;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public delegate void TickHandler(object sender, GameState state);
    public delegate void DrawHandler(object sender, View view);

    public abstract class GameEngine// : ITicker
    {
        public enum QueueAction { Add, Remove }

        private List<(QueueAction, Controller)> controllerQueue = new List<(QueueAction, Controller)>();

        public TickHandler TickStart;
        public TickHandler TickEnd;

        public DrawHandler DrawStart;
        public DrawHandler DrawEnd;

        private View currentView;
        private View nextView;
        public View View
        {
            get
            {
                return currentView;
            }
            set
            {
                nextView = value;
            }
        }

        public Location Location => this.state.Location;

        public void SetLocation(Location location)
        {
            this.state.Location = location;
            this.state.NextLocation = location;
        }

        private GameState state = new GameState();

        public bool Active { get; private set; }

        public void Start()
        {
            Active = true;
            Control();
        }

        public void Stop()
        {
            Active = false;
        }

        public abstract void Control();

        public void Tick()
        {
            // Let all handlers know there is a tick happening
            TickStart(this, this.state);

            foreach ((QueueAction action, Controller controller) queueAction in controllerQueue)
            {
                switch (queueAction.action)
                {
                    case QueueAction.Add:
                        this.state.Controllers.Add(queueAction.controller);
                        break;
                    case QueueAction.Remove:
                        this.state.Controllers.Remove(queueAction.controller);
                        break;
                }
            }
            controllerQueue.Clear();
            currentView = nextView;
            this.state.Location = this.state.NextLocation;

            // Poll the controllers for input this tick
            foreach (Controller controller in state.Controllers)
            {
                controller.Update();
            }

            // Tick all the things in the location
            this.state.Location?.Tick();

            TickEnd(this, this.state);
        }

        public void AddEntity(Entity player)
        {
            this.state.Location.AddEntity(player);
        }

        public void AddController(Controller controller)
        {
            controllerQueue.Add((QueueAction.Add, controller));
        }

        public void Draw()
        {
            DrawStart?.Invoke(this, View);
            View.Draw(state.Location);
            DrawEnd?.Invoke(this, View);
        }
    }
}
