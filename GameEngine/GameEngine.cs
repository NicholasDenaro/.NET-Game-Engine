using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine
{
    public delegate void TickHandler(object sender, EventArgs e);

    public abstract class GameEngine : ITicker
    {
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
                    currentView = nextView;
                    currentView.Open();
                }
            }
        }

        private Controller controller;
        public Controller Controller
        {
            get
            {
                return controller;
            }
            set
            {
                if (Active)
                {
                    throw new Exception("Engine is active, no swapping controllers!");
                }
                controller = value;
            }
        }

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
            Ticker(this, null);
            Location.Tick();
            currentLocation = nextLocation;

            if (currentView != nextView)
            {
                currentView.Close();
                currentView = nextView;
                currentView.Open();
            }
            controller.Input();
        }

        public void Draw()
        {
            View.Draw(Location);
        }
    }
}
