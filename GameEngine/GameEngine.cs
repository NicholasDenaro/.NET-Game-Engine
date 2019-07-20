using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using View = GameEngine.Interfaces.View;

namespace GameEngine
{
    public delegate void TickHandler(object sender, EventArgs e);

    public abstract class GameEngine : ITicker
    {
        public enum QueueAction { Add, Remove }

        public TickHandler Ticker;

        private Location currentLocation;
        private Location nextLocation;
        public Location Location
        {
            get
            {
                return currentLocation;
            }
            set
            {
                nextLocation = value;
                if (!Active)
                {
                    currentLocation = nextLocation;
                }
            }
        }

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
                if (!Active)
                {
                    currentView?.Close();
                    currentView = nextView;
                    currentView.Open();
                }
            }
        }

        private List<Controller> controllers = new List<Controller>();
        private List<(QueueAction, Controller)> controllerQueue = new List<(QueueAction, Controller)>();

        private IFocusable currentFocus;
        private IFocusable nextFocus;
        public IFocusable Focus
        {
            get
            {
                return currentFocus;
            }
            set
            {
                nextFocus = value;
                if (!Active)
                {
                    currentFocus = nextFocus;
                }
            }
        }

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
            foreach (Controller controller in controllers)
            {
                controller.Input();
            }
            Ticker(this, null);
            Location.Tick();
            currentLocation = nextLocation;

            if (currentView != nextView)
            {
                currentView.Close();
                currentView = nextView;
                currentView.Open();
            }

            foreach((QueueAction action, Controller controller) queueAction in controllerQueue)
            {
                switch(queueAction.action)
                {
                    case QueueAction.Add:
                        controllers.Add(queueAction.controller);
                        break;
                    case QueueAction.Remove:
                        controllers.Remove(queueAction.controller);
                        break;
                }
            }
        }

        public void AddController(Controller controller)
        {
            if (Active)
            {
                controllerQueue.Add((QueueAction.Add, controller));
            }
            else
            {
                controllers.Add(controller);
            }
        }

        public void Draw()
        {
            View.Draw(Location);
        }
    }
}
